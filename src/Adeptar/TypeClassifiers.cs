using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Adeptar
{
    /// <summary>
    /// A class providing methods for determining object types.
    /// </summary>
    internal static class TypeClassifiers
    {
        /// <summary>
        /// Cached <see cref="System.Type"/> object for the open generic <see cref="System.Collections.Generic.Dictionary{TKey, TValue}"/>.
        /// </summary>
        private static readonly Type _openGenericDictionaryType = typeof(Dictionary<,>);

        /// <summary>
        /// Cached <see cref="System.Type"/> object for the open generic <see cref="System.Collections.Generic.List{T}"/> type definition (<c>typeof(List)</c>).
        /// </summary>
        private static readonly Type _openGenericListType = typeof(List<>);

        /// <summary>
        /// Cached <see cref="System.Type"/> object for the open generic <see cref="System.Collections.Generic.IList{T}"/> type definition (<c>typeof(IList)</c>).
        /// </summary>
        private static readonly Type _openGenericIListType = typeof(IList<>);

        /// <summary>
        /// Cached <see cref="System.Type"/> object for the <see cref="System.Char"/> type.
        /// </summary>
        private static readonly Type _charType = typeof(char);

        /// <summary>
        /// Cached <see cref="System.Type"/> object for the <see cref="System.String"/> type.
        /// </summary>
        private static readonly Type _stringType = typeof(string);

        /// <summary>
        /// Cached <see cref="System.Type"/> object for the <see cref="System.DateTime"/> type.
        /// </summary>
        private static readonly Type _dateTimeType = typeof(DateTime);

        /// <summary>
        /// Cached <see cref="System.Type"/> object for the <see cref="System.Boolean"/> type.
        /// </summary>
        private static readonly Type _boolType = typeof(bool);

        /// <summary>
        /// Cached <see cref="System.Type"/> object for the <see cref="System.Decimal"/> type.
        /// </summary>
        private static readonly Type _decimalType = typeof(decimal);

        /// <summary>
        /// Cache for GetSerializableType results, mapping Type to its SerializableType.
        /// Uses ConcurrentDictionary for thread safety.
        /// </summary>
        private static readonly ConcurrentDictionary<Type, SerializableType> _serializableTypeCache =
            new ConcurrentDictionary<Type, SerializableType>();

        /// <summary>
        /// Gets the <see cref="SerializableType"/> classification for a given <see cref="Type"/>.
        /// </summary>
        /// <param name="type">The <see cref="Type"/> instance to classify, typically representing a field or property type during serialization.</param>
        /// <returns>
        /// The cached or calculated <see cref="SerializableType"/> corresponding to the input <paramref name="type"/>.
        /// Returns <see cref="SerializableType.Class"/> as a default if the type cannot be determined by the internal logic.
        /// </returns>
        /// <remarks>
        /// This method acts as the public accessor for serialization type classification.
        /// It first checks a static, thread-safe cache (<c>_serializableTypeCache</c>) for a previously computed result for the given <paramref name="type"/>.
        /// If the type is found in the cache, the cached value is returned immediately.
        /// If the type is not found (cache miss), it calls the internal <see cref="SerializableTypeOfInternal"/> method to perform the actual classification logic.
        /// The result from the internal method is then added to the cache before being returned.
        /// </remarks>
        /// <seealso cref="SerializableTypeOfInternal(Type)"/>
        internal static SerializableType SerializableTypeOf( Type type )
        {
            if ( _serializableTypeCache.TryGetValue( type, out SerializableType cachedResult ) )
            {
                return cachedResult;
            }

            SerializableType value = SerializableTypeOfInternal( type );
            _serializableTypeCache.TryAdd( type, value );

            return value;
        }

        /// <summary>
        /// Performs the core logic to determine the <see cref="SerializableType"/> classification for a given <see cref="Type"/>.
        /// </summary>
        /// <param name="type">The <see cref="Type"/> instance to classify.</param>
        /// <returns>
        /// The calculated <see cref="SerializableType"/> based on the type's characteristics.
        /// Returns <see cref="SerializableType.Class"/> as a fallback if the type doesn't match any specific classification rules.
        /// </returns>
        /// <remarks>
        /// This private helper method contains the actual, non-cached classification logic. It is called by <see cref="SerializableTypeOf(Type)"/> only when a cache miss occurs.
        /// The sequence of checks within this method (<c>if</c> statements) is intentionally ordered based on performance benchmarks for common types.
        /// </remarks>
        /// <seealso cref="SerializableTypeOf(Type)"/>
        private static SerializableType SerializableTypeOfInternal( Type type )
        {
            if ( type == _stringType )
                return SerializableType.String;
            if ( type == _charType )
                return SerializableType.Char;
            if ( type == _dateTimeType )
                return SerializableType.DateTime;
            if ( type == _openGenericIListType )
                return SerializableType.Array;
            if ( type.IsPrimitive )
                return SerializableType.Simple;
            if ( type.IsGenericType )
            {
                Type genericTypeDef = type.GetGenericTypeDefinition();
                if ( IsKnownTupleGenericDefinition( genericTypeDef ) )
                    return SerializableType.Tuple;
                if ( genericTypeDef == _openGenericDictionaryType )
                    return SerializableType.Dictionary;
            }
            if ( type.IsArray )
                return SerializableType.DimensionalArray;

            return SerializableType.Class;
        }

        /// <summary>
        /// Cache for GetDeserializableType results, mapping Type to its DeserializableType.
        /// Uses ConcurrentDictionary for thread safety.
        /// </summary>
        private static readonly ConcurrentDictionary<Type, DeserializableType> _deserializableTypeCache =
            new ConcurrentDictionary<Type, DeserializableType>();

        /// <summary>
        /// Gets the <see cref="DeserializableType"/> classification for a given <see cref="Type"/>.
        /// </summary>
        /// <param name="type">The <see cref="Type"/> instance to classify, typically representing a field or property type during deserialization.</param>
        /// <returns>
        /// The cached or calculated <see cref="DeserializableType"/> corresponding to the input <paramref name="type"/>.
        /// Returns <see cref="DeserializableType.Class"/> as a default if the type cannot be determined by the internal logic.
        /// </returns>
        /// <remarks>
        /// This method acts as the public accessor for deserialization type classification.
        /// It first checks a static, thread-safe cache (<c>_deserializableTypeCache</c>) for a previously computed result for the given <paramref name="type"/>.
        /// If the type is found in the cache, the cached value is returned immediately.
        /// If the type is not found (cache miss), it calls the internal <see cref="DeserializableTypeOfInternal"/> method to perform the actual classification logic.
        /// The result from the internal method is then added to the cache before being returned.
        /// This caching strategy significantly improves performance by ensuring the classification logic runs only once per unique <see cref="Type"/>.
        /// </remarks>
        /// <seealso cref="DeserializableTypeOfInternal(Type)"/>
        internal static DeserializableType DeserializableTypeOf( Type type )
        {
            if ( _deserializableTypeCache.TryGetValue( type, out DeserializableType cachedResult ) )
            {
                return cachedResult;
            }

            DeserializableType value = DeserializableTypeOfInternal( type );
            _deserializableTypeCache.TryAdd( type, value );

            return value;
        }

        /// <summary>
        /// Performs the core logic to determine the <see cref="DeserializableType"/> classification for a given <see cref="Type"/>.
        /// </summary>
        /// <param name="type">The <see cref="Type"/> instance to classify.</param>
        /// <returns>
        /// The calculated <see cref="DeserializableType"/> based on the type's characteristics.
        /// Returns <see cref="DeserializableType.Class"/> as a fallback if the type doesn't match any specific classification rules.
        /// </returns>
        /// <remarks>
        /// This private helper method contains the actual, non-cached classification logic. It is called by <see cref="DeserializableTypeOf(Type)"/> only when a cache miss occurs.
        /// The sequence of checks within this method (<c>if</c> statements) is intentionally ordered based on performance benchmarks for common types.
        /// </remarks>
        /// <seealso cref="DeserializableTypeOf(Type)"/>
        private static DeserializableType DeserializableTypeOfInternal( Type type )
        {
            if ( type == _stringType )
                return DeserializableType.String;
            if ( type == _charType )
                return DeserializableType.Char;
            if ( type == _dateTimeType )
                return DeserializableType.DateTime;
            if ( type == _boolType )
                return DeserializableType.Boolean;
            if ( type.IsPrimitive || type == _decimalType )
                return DeserializableType.Numeric;
            if ( type.IsGenericType )
            {
                Type genericTypeDef = type.GetGenericTypeDefinition();
                if ( IsKnownTupleGenericDefinition( genericTypeDef ) )
                    return DeserializableType.Tuple;
                if ( genericTypeDef == _openGenericDictionaryType )
                    return DeserializableType.Dictionary;
                if ( genericTypeDef == _openGenericIListType || genericTypeDef == _openGenericListType )
                    return DeserializableType.List;
            }
            if ( type.IsArray )
                return type.GetArrayRank() > 1 ? DeserializableType.DimensionalArray : DeserializableType.Array;
            if ( type.IsEnum )
                return DeserializableType.Enum;

            return DeserializableType.Class;
        }

        /// <summary>
        /// Contains the open generic type definitions for standard <see cref="System.ValueTuple"/> types (up to 8 type arguments).
        /// </summary>
        /// <remarks>
        /// This set is used by the <see cref="IsKnownTupleGenericDefinition"/> method to efficiently determine
        /// if a given open generic type definition corresponds to one of the standard ValueTuple types.
        /// This field is initialized once during static type initialization and is read-only thereafter.
        /// </remarks>
        private static readonly HashSet<Type> _valueTupleGenericTypes = new HashSet<Type>
        {
            typeof(ValueTuple<>), typeof(ValueTuple<,>), typeof(ValueTuple<,,>),
            typeof(ValueTuple<,,,>), typeof(ValueTuple<,,,,>), typeof(ValueTuple<,,,,,>),
            typeof(ValueTuple<,,,,,,>), typeof(ValueTuple<,,,,,,,>)
        };

        /// <summary>
        /// Checks if the provided open generic type definition is one of the standard <c>System.ValueTuple</c> generic types.
        /// </summary>
        /// <param name="openGenericTupleType">The open generic type definition to check (e.g., obtained via <c>type.GetGenericTypeDefinition()</c>).</param>
        /// <returns><c>true</c> if the specified type is found within the <see cref="_valueTupleGenericTypes"/> set; otherwise, <c>false</c>.</returns>
        private static bool IsKnownTupleGenericDefinition( Type openGenericTupleType ) => _valueTupleGenericTypes.Contains( openGenericTupleType );

        /// <summary>
        /// Gets the object's <see cref="SerializableType"/>.
        /// </summary>
        /// <param name="received">The received object.</param>
        /// <returns>
        /// The provided object's <see cref="SerializableType"/>. Returns <see cref="SerializableType.Class"/> if the
        /// provided object is null or if its type can't be determined.
        /// </returns>
        public static SerializableType FetchType( object received ) =>
        received switch
        {
            string => SerializableType.String,
            DateTime or DateTimeOffset => SerializableType.DateTime,
            char => SerializableType.Char,
            Enum or bool or IConvertible => SerializableType.Simple,
            IList array => IsMultiDimensionalArray( array ) ? SerializableType.DimensionalArray : SerializableType.Array,
            ITuple => SerializableType.Tuple,
            IDictionary => SerializableType.Dictionary,
            _ => SerializableType.Class
        };

        /// <summary>
        /// Checks if the object is an array with two or more dimensions.
        /// </summary>
        /// <param name="received">The object to check.</param>
        /// <returns>True if the object is an array and has rank > 1.</returns>
        public static bool IsMultiDimensionalArray( object received )
        {
            if ( received is Array array ) return array.Rank > 1;
            return false;
        }

        /// <summary>
        /// Parses a span containing an enum member name into the specified enum type (non-generic).
        /// </summary>
        /// <param name="enumType">The enum type to parse into.</param>
        /// <param name="value">The span containing the member name to parse.</param>
        /// <param name="ignoreCase">Whether to ignore case during parsing.</param>
        /// <returns>The parsed enum value as an object.</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="enumType"/> is null.</exception>
        /// <exception cref="ArgumentException">Thrown if <paramref name="enumType"/> is not an enum type, or if <paramref name="value"/> cannot be parsed.</exception>
        public static object ParseEnum( Type enumType, ReadOnlySpan<char> value, bool ignoreCase = false )
        {
            ArgumentNullException.ThrowIfNull( enumType );
            return Enum.Parse( enumType, value.ToString(), ignoreCase );
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