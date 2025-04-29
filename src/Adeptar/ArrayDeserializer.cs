using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Adeptar.Unity;
using static Adeptar.AdeptarDebugger;
using static Adeptar.DeserializationHelpers;

namespace Adeptar
{
    /// <summary>
    /// Deserializes Adeptar array/list representations (`[...]`) into .NET Array, List{T}, or multi-dimensional arrays.
    /// Internal class used by the core Adeptar deserialization process.
    /// </summary>
    internal static class ArrayDeserializer
    {
        // Cache for List<T> default constructors
        private static readonly ConcurrentDictionary<Type, Func<object>> _listConstructorCache = new();

        // Cached types used for reflection checks
        private static readonly Type _IListType = typeof(IList);
        private static readonly Type _objectType = typeof(object);
        private static readonly Type _IEnumerableType = typeof(IEnumerable<>); // Cache generic definition
        private static readonly Type _listType = typeof(List<>); // Cache generic definition

        // Binding flags for constructor lookup
        private const BindingFlags _constructorBindingFlags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;

        /// <summary>
        /// Deserializes the Adeptar representation of an array (`[...]`) into a .NET Array (`T[]`).
        /// </summary>
        /// <param name="sourceSpan">The span containing the cleaned array representation (must include surrounding brackets).</param>
        /// <param name="arrayType">The target array <see cref="Type"/> (e.g., typeof(int[])).</param>
        /// <returns>The deserialized array object as <see cref="IList"/>.</returns>
        /// <exception cref="AdeptarException">Thrown for format errors, element deserialization failures, or type mismatches during population.</exception>
        /// <exception cref="ArgumentNullException">If <paramref name="arrayType"/> is null.</exception>
        /// <exception cref="ArgumentException">If <paramref name="arrayType"/> is not a single-dimensional array type.</exception>
        internal static IList DeserializeArray( ReadOnlySpan<char> sourceSpan, Type arrayType )
        {
            if ( sourceSpan.Length < 2 || sourceSpan[0] != '[' || sourceSpan[sourceSpan.Length - 1] != ']' )
            {
                throw new AdeptarException( $"Invalid array format. Expected at least '[]'. Received: '{PreviewSpan( sourceSpan )}'" );
            }

            Type elementType = arrayType.GetElementType();
            if ( elementType == null ) throw new AdeptarException( $"Cannot determine element type for array type {arrayType.FullName}." );

            ReadOnlySpan<char> contentSpan = sourceSpan.Slice(1, sourceSpan.Length - 2);
            if ( contentSpan.IsEmpty ) return Array.CreateInstance( elementType, 0 );

            List<object> tempList = new List<object>();
            ParseArrayOrListElements( contentSpan, elementType, tempList.Add );

            Array finalArray = Array.CreateInstance(elementType, tempList.Count);
            for ( int i = 0; i < tempList.Count; i++ )
            {
                try { finalArray.SetValue( tempList[i], i ); }
                catch ( Exception ex ) { throw new AdeptarException( $"Error setting array element at index {i}", ex ); }
            }
            return finalArray;
        }

        /// <summary>
        /// Shared helper to parse comma-separated elements within '[' and ']'.
        /// Elements are deserialized using AdeptarDeserializer.DeserializeObject and added via the provided action.
        /// </summary>
        /// <param name="contentSpan">The span containing elements, without the surrounding brackets.</param>
        /// <param name="elementType">The expected Type of each element.</param>
        /// <param name="addAction">Action delegate (e.g., list.Add) to call with each deserialized element.</param>
        private static void ParseArrayOrListElements( ReadOnlySpan<char> contentSpan, Type elementType, Action<object> addAction )
        {
            int currentPosition = 0;
            while ( currentPosition < contentSpan.Length )
            {
                char delimiterFound;
                int delimiterIndex = FindNextTopLevelDelimiter(contentSpan, currentPosition, out delimiterFound, stopChar: ']');

                int endPosition = (delimiterIndex == -1) ? contentSpan.Length : delimiterIndex;
                ReadOnlySpan<char> elementSpan = contentSpan.Slice(currentPosition, endPosition - currentPosition).Trim();

                if ( elementSpan.IsEmpty && delimiterIndex != -1 )
                {
                    throw new AdeptarException( $"Invalid array/list format. Empty element found near position {currentPosition}. Input: '[{PreviewSpan( contentSpan )}]'" );
                }
                if ( !elementSpan.IsEmpty || delimiterIndex == -1 )
                {
                    object elementValue;
                    try
                    {
                        elementValue = AdeptarReader.DeserializeObject( elementType, elementSpan );
                    }
                    catch ( AdeptarException ex ) { throw new AdeptarException( $"Failed to deserialize array/list element. Input: '{elementSpan.ToString()}'. Reason: {ex.Message}", ex ); }
                    catch ( Exception ex ) { throw new AdeptarException( $"Failed to deserialize array/list element. Input: '{elementSpan.ToString()}'. See inner exception.", ex ); }

                    try
                    {
                        addAction( elementValue );
                    }
                    catch ( Exception ex ) { throw new AdeptarException( $"Failed to add element to list/array. Element: '{elementValue ?? "null"}'. See inner exception.", ex ); }
                }


                if ( delimiterIndex == -1 ) break; // Reached end
                currentPosition = endPosition + 1;

                if ( delimiterFound != ',' && delimiterIndex != -1 )
                {
                    throw new AdeptarException( $"Invalid array/list format. Expected ',' between elements near position {endPosition}. Input: '[{PreviewSpan( contentSpan )}]'" );
                }
            }

        }

        /// <summary>
        /// Finds the index of the next top-level delimiter (defaulting to ',') not nested within strings or structures.
        /// Can optionally stop at a specific closing character.
        /// </summary>
        private static int FindNextTopLevelDelimiter( ReadOnlySpan<char> span, int startIndex, out char delimiterFound, char primaryDelimiter = ',', char stopChar = '\0' )
        {
            delimiterFound = '\0';
            int level = 0;
            bool inString = false;
            bool escapeNext = false;

            for ( int i = startIndex; i < span.Length; i++ )
            {
                char c = span[i];
                if ( escapeNext ) { escapeNext = false; continue; }
                if ( c == '\\' ) { escapeNext = true; continue; }
                if ( c == '"' ) { inString = !inString; continue; }
                if ( inString ) continue;

                if ( c == stopChar && level == 0 ) return -1;

                switch ( c )
                {
                    case '(': case '[': case '{': level++; break;
                    case ')': case ']': case '}': level--; break;
                    case var del when del == primaryDelimiter:
                        if ( level == 0 ) { delimiterFound = c; return i; }
                        break;
                }
                if ( level < 0 ) throw new AdeptarException( $"Mismatched nesting level near index {i} in '{PreviewSpan( span )}'." );
            }
            return -1;
        }

        /// <summary>
        /// Deserializes the Adeptar string representation of a list (`[...]`) into a .NET <see cref="List{T}"/> or compatible <see cref="IList"/>.
        /// </summary>
        /// <param name="sourceSpan">The ReadOnlySpan containing the cleaned list string representation (must include surrounding brackets).</param>
        /// <param name="listType">The target list <see cref="Type"/> (e.g., typeof(List)). Must have a parameterless constructor and implement IList.</param>
        /// <returns>The deserialized list object as <see cref="IList"/>.</returns>
        /// <exception cref="AdeptarException">Thrown for format errors, element deserialization failures, instance creation errors, or Add failures.</exception>
        /// <exception cref="ArgumentNullException">If <paramref name="listType"/> is null.</exception>
        /// <exception cref="ArgumentException">If <paramref name="listType"/> does not implement IList or cannot be instantiated.</exception>
        internal static IList DeserializeList( ReadOnlySpan<char> sourceSpan, Type listType )
        {
            ArgumentNullException.ThrowIfNull( listType );
            if ( !_IListType.IsAssignableFrom( listType ) )
            {
                throw new ArgumentException( $"Type '{listType.FullName}' does not implement IList.", nameof( listType ) );
            }

            ValidateSurroundingBrackets( sourceSpan );
            ReadOnlySpan<char> contentSpan = sourceSpan.Slice(1, sourceSpan.Length - 2);

            Type elementType =_objectType;
            if ( listType.IsGenericType && listType.GetGenericTypeDefinition() == _listType )
            {
                elementType = listType.GetGenericArguments()[0];
            }
            else
            {
                var enumerableInterface = listType.GetInterfaces().FirstOrDefault(
                    i => i.IsGenericType && i.GetGenericTypeDefinition() == _IEnumerableType);
                if ( enumerableInterface != null )
                {
                    elementType = enumerableInterface.GetGenericArguments()[0];
                }
            }

            IList targetList = CreateListInstance(listType);
            if ( contentSpan.IsEmpty ) return targetList;

            ParseArrayOrListElements( contentSpan, elementType, element => targetList.Add( element ) );
            return targetList;
        }

        /// <summary>Creates an instance of a List type using a cached compiled constructor.</summary>
        private static IList CreateListInstance( Type listType )
        {
            try
            {
                var constructor = _listConstructorCache.GetOrAdd(listType, type =>
                {
                    ConstructorInfo ctor = type.GetConstructor(_constructorBindingFlags, null, Type.EmptyTypes, null);
                    if (ctor == null) throw new MissingMethodException($"No parameterless constructor found for list type {type.FullName}.");
                    NewExpression newExpr = Expression.New(ctor);
                    LambdaExpression lambda = Expression.Lambda<Func<object>>(newExpr);
                    return (Func<object>)lambda.Compile();
                });
                return (IList)constructor();
            }
            catch ( Exception ex )
            {
                throw new AdeptarException( $"Failed to create instance of list type '{listType.FullName}'. See inner exception.", ex );
            }
        }

        /// <summary>Checks for surrounding brackets and throws if invalid.</summary>
        private static void ValidateSurroundingBrackets( ReadOnlySpan<char> span )
        {
            if ( span.Length < 2 || span[0] != '[' || span[span.Length - 1] != ']' )
            {
                throw new AdeptarException( $"Invalid list format. Expected content enclosed in square brackets '[]'. Received start: '{PreviewSpan( span, 50 )}'" );
            }
        }

        /// <summary>
        /// Deserializes the Adeptar string of two or more dimensional arrays to a .NET object.
        /// </summary>
        /// <param name="text">The Adeptar string representation of the object.</param>
        /// <param name="type">The type of the dimensional array.</param>
        /// <returns>The .NET version of the dimensional array.</returns>
        internal static object DeserializeDimensionalArray( ReadOnlySpan<char> text, Type type )
        {
            List<int> sizes = new();
            bool inSizes = false;
            bool exit = false;

            int i = 0, j = 0;

            foreach ( char item in text )
            {
                if ( exit )
                    break;
                switch ( item )
                {
                    case '<':
                        inSizes = true;
                        j = i + 1;
                        break;
                    case '>':
                        if ( inSizes )
                        {
                            sizes.Add( (int)NumericResolver( _intType, text.Slice( j, i - j ).ToString() ) );
                            j = i + 1;
                            inSizes = false;
                            exit = true;
                        }
                        break;
                    case ',':
                        sizes.Add( (int)NumericResolver( _intType, text.Slice( j, i - j ).ToString() ) );
                        j = i + 1;
                        break;
                }
                i++;
            }

            text = text.Slice( j );

            Type elementType = type.GetElementType();

            IList flat = DeserializeArray( $"[{text.ToString()}", elementType.MakeArrayType() );
            Array main = Array.CreateInstance( elementType, sizes.ToArray() );

            int[] index = new int[sizes.Count];

            for ( int e = 0; e < sizes.Count; e++ )
            {
                sizes[e] = sizes[e] - 1;
            }

            for ( int w = 0; w < flat.Count; w++ )
            {
                main.SetValue( flat[w], index );
                BinaryStyleIndexArrayByRefIncrement( in sizes, ref index );
            }

            return main;
        }
    }
}

#region License
// Copyright (c) 2022 The Grey Cardinal ( Michael Kananov )
//
// Permission is hereby granted, free of charge, to any person
// obtaining a copy of this software and associated documentation
// files (the "Software"), to deal in the Software without
// restriction, including without limitation the rights to use,
// copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the
// Software is furnished to do so, subject to the following
// conditions:
//
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES
// OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT
// HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY,
// WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING
// FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR
// OTHER DEALINGS IN THE SOFTWARE.
#endregion