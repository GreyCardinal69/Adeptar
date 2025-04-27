using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Reflection;

using static Adeptar.AdeptarDebugger;

namespace Adeptar
{
    /// <summary>
    /// Internal class containing methods for the deserialization of tuples.
    /// </summary>
    internal static class TupleDeserializer
    {
        // --- Caches ---
        /// <summary>
        /// Cache for tuple FieldInfo objects keyed by tuple Type and then by field name ("Item1"..."Rest").
        /// </summary>
        private static readonly ConcurrentDictionary<Type, Dictionary<string, FieldInfo>> _tupleFieldInfoCache =
            new ConcurrentDictionary<Type, Dictionary<string, FieldInfo>>();

        /// <summary>
        /// Cache for delegates that set a specific field on a boxed tuple instance. Keyed by FieldInfo.
        /// </summary>
        /// <remarks>
        /// Uses a lambda calling FieldInfo.SetValue due to complexities of efficiently setting fields on boxed value types via Expression Trees.
        /// </remarks>
        private static readonly ConcurrentDictionary<FieldInfo, Action<object, object>> _tupleFieldSetterCache =
            new ConcurrentDictionary<FieldInfo, Action<object, object>>();

        /// <summary>
        /// Binding flags used to retrieve public instance fields (like Item1, Rest) from ValueTuples.
        /// </summary>
        private const BindingFlags _fieldBindingFlags = BindingFlags.Public | BindingFlags.Instance;

        /// <summary>
        /// Deserializes the Adeptar string representation of a tuple into a .NET ValueTuple object.
        /// </summary>
        /// <param name="sourceText">The ReadOnlySpan containing the cleaned tuple string representation (including parentheses).</param>
        /// <param name="tupleType">The target <see cref="System.ValueTuple"/> type.</param>
        /// <returns>The deserialized tuple object.</returns>
        /// <exception cref="AdeptarException">Thrown if the format is invalid (e.g., missing '()', ':', ','), field names mismatch, element count mismatches, element deserialization fails, or type mismatches occur during setting.</exception>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="tupleType"/> is null (though typically checked before calling).</exception>
        internal static object DeserializeTuple( ReadOnlySpan<char> sourceText, Type tupleType )
        {
            if ( tupleType == null ) throw new ArgumentNullException( nameof( tupleType ) ); // Added explicit null check

            if ( sourceText.Length < 2 || sourceText[0] != '(' || sourceText[sourceText.Length - 1] != ')' )
            {
                throw new AdeptarException( $"Invalid tuple format. Expected cleaned text enclosed in parentheses '()'. Received start: '{PreviewSpan( sourceText, 50 )}'" );
            }

            ReadOnlySpan<char> innerSpan = sourceText.Slice(1, sourceText.Length - 2);

            Dictionary<string, FieldInfo> fields = GetOrCreateTupleFieldMap(tupleType);

            object target = Activator.CreateInstance(tupleType);
            int elementsFound = 0;

            if ( innerSpan.IsEmpty )
            {
                if ( fields.Count != 0 )
                {
                    throw new AdeptarException( $"Invalid tuple format. Received empty content '()' but expected fields for type {tupleType.Name}." );
                }
                return target;
            }

            int currentPosition = 0;
            bool expectingName = true;
            string currentName = null;
            int valueStartPosition = 0;

            while ( currentPosition < innerSpan.Length )
            {
                char delimiterFound;
                int delimiterIndex = FindNextTopLevelDelimiter(innerSpan, currentPosition, out delimiterFound);

                int endPosition = (delimiterIndex == -1) ? innerSpan.Length : delimiterIndex;
                ReadOnlySpan<char> segment = innerSpan.Slice(currentPosition, endPosition - currentPosition);

                if ( expectingName )
                {
                    if ( delimiterFound != ':' )
                    {
                        throw new AdeptarException( $"Invalid tuple format. Expected ':' after field name but found '{delimiterFound}' or end of content near position {currentPosition}. Input: '{PreviewSpan( innerSpan )}'. Segment: '{segment.ToString()}'" );
                    }
                    if ( segment.IsEmpty ) // Check if name itself is empty
                    {
                        throw new AdeptarException( $"Invalid tuple format. Missing field name before ':' near position {currentPosition}. Input: '{PreviewSpan( innerSpan )}'." );
                    }
                    currentName = segment.ToString();
                    expectingName = false;
                    valueStartPosition = endPosition + 1; // Value starts after the colon
                }
                else
                {
                    if ( delimiterFound != ',' && delimiterIndex != -1 ) // Must be comma or end
                    {
                        throw new AdeptarException( $"Invalid tuple format. Expected ',' or end of tuple after value for field '{currentName}' but found '{delimiterFound}' near position {endPosition}. Input: '{PreviewSpan( innerSpan )}'." );
                    }

                    // Validate field name and get FieldInfo
                    if ( !fields.TryGetValue( currentName, out FieldInfo currentField ) )
                    {
                        throw new AdeptarException( $"Invalid field name '{currentName}' found in input for tuple type {tupleType.Name} near position {valueStartPosition - currentName.Length - 1}. Input: '{PreviewSpan( innerSpan )}'. Expected one of: {string.Join( ", ", fields.Keys )}." );
                    }

                    ReadOnlySpan<char> valueSpan = innerSpan.Slice(valueStartPosition, endPosition - valueStartPosition);
                    object elementValue;

                    try
                    {
                        // Deserialize the value: Special handling for 'Rest' field
                        if ( currentName == "Rest" )
                        {
                            elementValue = DeserializeTuple( valueSpan, currentField.FieldType );
                        }
                        else
                        {
                            elementValue = AdeptarReader.DeserializeObject( currentField.FieldType, valueSpan );
                        }
                    }
                    catch ( AdeptarException ex )
                    {
                        throw new AdeptarException( $"Failed to deserialize value for tuple field '{currentName}'. Input: '{PreviewSpan( valueSpan )}'. Reason: {ex.Message}", ex );
                    }
                    catch ( Exception ex )
                    {
                        throw new AdeptarException( $"Failed to deserialize value for tuple field '{currentName}'. Input: '{PreviewSpan( valueSpan )}'. See inner exception.", ex );
                    }

                    try
                    {
                        Action<object, object> setter = GetOrCreateTupleFieldSetter(currentField);
                        setter( target, elementValue );
                        elementsFound++;
                    }
                    catch ( Exception ex )
                    {
                        throw new AdeptarException( $"Failed to set value for tuple field '{currentName}'. Type mismatch or other error. Value was '{elementValue ?? "null"}'. See inner exception.", ex );
                    }

                    currentName = null;
                    expectingName = true;
                }

                currentPosition = ( delimiterIndex == -1 ) ? innerSpan.Length : endPosition + 1;
            }

            if ( !expectingName )
            {
                throw new AdeptarException( $"Invalid tuple format. Dangling field name '{currentName}' without a value at the end of input: '{PreviewSpan( innerSpan )}'." );
            }

            return target;
        }

        /// <summary>
        /// Finds the index of the next top-level delimiter (':' or ',') within the given span,
        /// starting from the specified index, that is not nested within strings ('"'),
        /// parentheses ('()'), square brackets ('[]'), or curly braces ('{}').
        /// </summary>
        /// <param name="span">The span of characters to search within (assumed pre-cleaned).</param>
        /// <param name="startIndex">The zero-based starting position for the search.</param>
        /// <param name="delimiterFound">When this method returns, contains the delimiter character (':' or ',') found at the top level, or the null character ('\0') if no top-level delimiter was found before the end of the span.</param>
        /// <returns>The zero-based index position of the first top-level delimiter found; otherwise, -1 if no such delimiter is found.</returns>
        /// <exception cref="AdeptarException">Thrown if mismatched nesting characters (e.g., extra closing brace) are detected.</exception>
        private static int FindNextTopLevelDelimiter( ReadOnlySpan<char> span, int startIndex, out char delimiterFound )
        {
            delimiterFound = '\0';
            int level = 0;      // Tracks nesting depth for (), [], {}
            bool inString = false; // Tracks whether currently inside double quotes
            bool escapeNext = false; // Tracks if the next character is escaped by a backslash

            for ( int i = startIndex; i < span.Length; i++ )
            {
                char c = span[i];

                if ( escapeNext )
                {
                    escapeNext = false; // Consume the escape, ignore the current character's normal meaning
                    continue;
                }
                if ( c == '\\' )
                {
                    escapeNext = true;
                    continue;
                }

                if ( c == '"' )
                {
                    inString = !inString;
                    continue;
                }
                if ( inString )
                {
                    continue;
                }

                switch ( c )
                {
                    case '(':
                    case '[':
                    case '{':
                        level++;
                        break;
                    case ')':
                    case ']':
                    case '}':
                        level--;
                        break;
                    case ':':
                    case ',':
                        if ( level == 0 ) // Check if at the top level (not nested)
                        {
                            delimiterFound = c;
                            return i;
                        }
                        break;
                }

                // Check for invalid nesting (e.g., closing more levels than opened)
                if ( level < 0 )
                {
                    throw new AdeptarException( $"Mismatched nesting level (negative) detected near index {i}. Check for unbalanced parentheses, brackets, or braces. Input: '{PreviewSpan( span )}'." );
                }
            }

            return -1;
        }

        /// <summary>
        /// Retrieves or creates and caches a dictionary mapping the standard ValueTuple field names ("Item1", "Item2", ..., "Rest")
        /// to their corresponding <see cref="FieldInfo"/> objects for a given ValueTuple type.
        /// </summary>
        /// <param name="tupleType">The <see cref="System.ValueTuple"/> type for which to get the field map.</param>
        /// <returns>A dictionary containing the relevant <see cref="FieldInfo"/> objects keyed by their standard names.</returns>
        /// <exception cref="InvalidOperationException">Thrown if the given <paramref name="tupleType"/> is not a recognized ValueTuple type or if expected fields are missing.</exception>
        private static Dictionary<string, FieldInfo> GetOrCreateTupleFieldMap( Type tupleType )
        {
            return _tupleFieldInfoCache.GetOrAdd( tupleType, type =>
            {
                var fieldMap = new Dictionary<string, FieldInfo>();
                foreach ( var field in type.GetFields( _fieldBindingFlags ) )
                {
                    // Only map fields named "ItemX" or "Rest"
                    if ( field.Name.StartsWith( "Item" ) || field.Name == "Rest" )
                    {
                        fieldMap[field.Name] = field;
                    }
                }
                return fieldMap;
            } );
        }

        /// <summary>
        /// Retrieves or creates and caches a compiled delegate (<see cref="Action{T1, T2}"/>)
        /// that efficiently sets the value of a specific field on a (boxed) ValueTuple object.
        /// </summary>
        /// <param name="field">The <see cref="FieldInfo"/> representing the ValueTuple field (e.g., Item1, Rest) to be set.</param>
        /// <returns>An <see cref="Action{Object, Object}"/> delegate that takes the boxed tuple instance and the value to set.</returns>
        private static Action<object, object> GetOrCreateTupleFieldSetter( FieldInfo field )
        {
            return _tupleFieldSetterCache.GetOrAdd( field, f =>
            {
                return ( targetObj, valueObj ) => f.SetValue( targetObj, valueObj );
            } );
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