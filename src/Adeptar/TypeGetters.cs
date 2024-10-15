﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Adeptar
{
    /// <summary>
    /// A class providing methods for determining object types.
    /// </summary>
    public static class TypeGetters
    {
        /// <summary>
        /// Checks if an object is of type <see cref="ValueTuple"/>, such as (<see cref="int"/>, <see cref="int"/>).
        /// </summary>
        /// <param name="tuple">The type to check for.</param>
        /// <returns>True if the object is a <see cref="ValueTuple"/>.</returns>
        public static bool IsTuple( Type tuple )
        {
            if ( !tuple.IsGenericType )
            {
                return false;
            }
            Type openType = tuple.GetGenericTypeDefinition();
            return openType == typeof( ValueTuple<> )
                || openType == typeof( ValueTuple<,> )
                || openType == typeof( ValueTuple<,,> )
                || openType == typeof( ValueTuple<,,,> )
                || openType == typeof( ValueTuple<,,,,> )
                || openType == typeof( ValueTuple<,,,,,> )
                || openType == typeof( ValueTuple<,,,,,,> )
                || openType == typeof( ValueTuple<,,,,,,,> ) && IsTuple( tuple.GetGenericArguments()[7] );
        }

        public static bool IsTuple2( Type tuple )
        {
            if ( !tuple.IsGenericType )
                return false;

            // Get the open generic type definition
            Type openType = tuple.GetGenericTypeDefinition();

            // Check if it's a known ValueTuple type from the cached types
            if ( openType == _cachedTypes[3]  // ValueTuple<>
                || openType == _cachedTypes[4]  // ValueTuple<,>
                || openType == _cachedTypes[5]  // ValueTuple<,,>
                || openType == _cachedTypes[6]  // ValueTuple<,,,>
                || openType == _cachedTypes[7]  // ValueTuple<,,,,>
                || openType == _cachedTypes[8]  // ValueTuple<,,,,,>
                || openType == _cachedTypes[9]  // ValueTuple<,,,,,,>
                || openType == _cachedTypes[10] ) // ValueTuple<,,,,,,,>
            {
                // Check for nested ValueTuples in the 8th generic argument
                if ( openType == _cachedTypes[10] )  // ValueTuple<,,,,,,,>
                {
                    Type[] genericArgs = tuple.GetGenericArguments();
                    return IsTuple2( genericArgs[7] );
                }

                return true;
            }

            return false;
        }


        /// <summary>
        /// Cached types for <see cref="TypeGetters"/> methods.
        /// </summary>
        private static Type[] _cachedTypes = new Type[]
        {
            typeof( Dictionary<,> ),
            typeof( List<> ),
            typeof( IList<> ),
            typeof( ValueTuple<> ),
            typeof( ValueTuple<,> ),
            typeof( ValueTuple<,,> ),
            typeof( ValueTuple<,,,> ),
            typeof( ValueTuple<,,,,> ),
            typeof( ValueTuple<,,,,,> ),
            typeof( ValueTuple<,,,,,,> ),
            typeof( ValueTuple<,,,,,,,> ),
            typeof( char ),
            typeof( string ),
            typeof( DateTime ),
            typeof( bool ),
            typeof( decimal )
        };

        /// <summary>
        /// Gets the <see cref="SerializableType"/> of the provided <see cref="Type"/>.
        /// </summary>
        /// <param name="fInfo">The <see cref="Type"/> to check.</param>
        /// <returns>The <see cref="SerializableType"/> of the provided <see cref="Type"/>.</returns>
        public static SerializableType GetSerializableType( Type fInfo )
        {
            if ( fInfo == _cachedTypes[12] ) return SerializableType.String;
            if ( fInfo == _cachedTypes[11] ) return SerializableType.Char;
            if ( fInfo == _cachedTypes[13] ) return SerializableType.DateTime;
            if ( fInfo == _cachedTypes[2] ) return SerializableType.Array;

            if ( fInfo.IsPrimitive ) return SerializableType.Simple;

            if ( fInfo.IsGenericType )
            {
                Type genericTypeDef = fInfo.GetGenericTypeDefinition();
                if ( IsTupleGenericKnown( genericTypeDef ) )
                    return SerializableType.Tuple;
                if ( genericTypeDef == _cachedTypes[0] )
                    return SerializableType.Dictionary;
            }

            if ( fInfo.IsArray ) return SerializableType.DimensionalArray;

            return SerializableType.Class;
        }

        /// <summary>
        /// Gets the Type's <see cref="DeserializableType"/>.
        /// </summary>
        /// <param name="fInfo">The Type's field type.</param>
        /// <returns>
        /// The object's <see cref="DeserializableType"/>. Returns <see cref="DeserializableType.Class"/> if the type
        /// cant be determined.
        /// </returns>
        internal static DeserializableType GetDeserializableType( Type fInfo )
        {
            if ( fInfo == _cachedTypes[12] )
                return DeserializableType.String;
            if ( fInfo == _cachedTypes[11] )
                return DeserializableType.Char;
            if ( fInfo == _cachedTypes[13] )
                return DeserializableType.DateTime;
            if ( fInfo == _cachedTypes[14] )
                return DeserializableType.Boolean;
            if ( fInfo.IsPrimitive || fInfo == _cachedTypes[15] )
                return DeserializableType.Numeric;
            if ( fInfo.IsGenericType )
            {
                Type genericTypeDef = fInfo.GetGenericTypeDefinition();
                if ( IsTupleGenericKnown( genericTypeDef ) )
                    return DeserializableType.Tuple;
                if ( genericTypeDef == _cachedTypes[0] )
                    return DeserializableType.Dictionary;
                if ( genericTypeDef == _cachedTypes[1] || genericTypeDef == _cachedTypes[2] )
                    return DeserializableType.List;
            }
            if ( fInfo.IsArray )
                return fInfo.GetArrayRank() > 1 ? DeserializableType.DimensionalArray : DeserializableType.Array;
            if ( fInfo.IsEnum )
                return DeserializableType.Enum;

            return DeserializableType.Class;
        }

        /// <summary>
        /// Checks if the provided object is a <see cref="Dictionary{TKey, TValue}"/>.
        /// </summary>
        /// <param name="obj">The object to check.</param>
        /// <returns>
        /// True/False if the object is of type <see cref="Dictionary{TKey, TValue}"/>.
        /// </returns>
        public static bool IsDictionary( object obj ) => obj is IDictionary;

        /// <summary>
        /// Checks if an object is of type <see cref="ValueTuple"/>, such as (<see cref="int"/>, <see cref="int"/>). Omits the .IsGeneric check.
        /// </summary>
        /// <param name="tuple">The type to check for.</param>
        /// <returns>True if the object is a <see cref="ValueTuple"/>.</returns>
        public static bool IsTupleGenericKnown( Type tuple )
        {
            return tuple == _cachedTypes[3]
                || tuple == _cachedTypes[4]
                || tuple == _cachedTypes[5]
                || tuple == _cachedTypes[6]
                || tuple == _cachedTypes[7]
                || tuple == _cachedTypes[8]
                || tuple == _cachedTypes[9]
                || tuple == _cachedTypes[10];
        }

        /// <summary>
        /// Checks if the object is an array with two or more dimensions.
        /// </summary>
        /// <param name="received">The object to check.</param>
        /// <returns>True if the object has two or three dimensions.</returns>
        public static bool IsMultiDimensionalArray( object received ) => ( received as Array ).Rank > 1;

        /// <summary>
        /// Gets the object's <see cref="SerializableType"/>.
        /// </summary>
        /// <param name="received">The received object.</param>
        /// <returns>
        /// The provided object's <see cref="SerializableType"/>. Returns <see cref="SerializableType.Class"/> if the
        /// provided object is null or if its type can't be determined.
        /// </returns>
        public static SerializableType FetchType( object received )
        {
            if ( received is string )
                return SerializableType.String;

            if ( received is DateTime || received is DateTimeOffset )
                return SerializableType.DateTime;

            if ( received is char )
                return SerializableType.Char;

            if ( received is Enum || received is bool || received is IConvertible )
                return SerializableType.Simple;

            if ( received is Array array )
                return IsMultiDimensionalArray( array ) ? SerializableType.DimensionalArray : SerializableType.Array;

            if ( received is ITuple )
                return SerializableType.Tuple;

            if ( received is IDictionary )
                return SerializableType.Dictionary;

            if ( received is IList )
                return SerializableType.Array;

            return SerializableType.Class;
        }

        /// <summary>
        /// Checks if the provided object is a numeric value of types: <see cref="sbyte"/>, <see cref="short"/>,
        /// <see cref="ushort"/>, <see cref="int"/>, <see cref="uint"/>, <see cref="long"/>,
        /// <see cref="ulong"/>, <see cref="float"/>, <see cref="double"/> or <see cref="decimal"/>.
        /// </summary>
        /// <param name="value">The object to check.</param>
        /// <returns>True if an object is a number.</returns>
        public static bool IsNumber( object value )
        {
            return value is sbyte
                    || value is byte
                    || value is short
                    || value is ushort
                    || value is int
                    || value is uint
                    || value is long
                    || value is ulong
                    || value is float
                    || value is double
                    || value is decimal;
        }

        /// <summary>
        /// Checks if the provided object is a numeric value of types: <see cref="sbyte"/>, <see cref="short"/>,
        /// <see cref="ushort"/>, <see cref="int"/>, <see cref="uint"/>, <see cref="long"/>,
        /// <see cref="ulong"/>, <see cref="float"/>, <see cref="double"/> or <see cref="decimal"/> using <see cref="Type"/>
        /// </summary>
        /// <param name="type">The Type to check.</param>
        /// <returns>True if an object is a number.</returns>
        public static bool IsNumericType( Type type ) => Type.GetTypeCode( type ) switch
        {
            TypeCode.Byte => true,
            TypeCode.SByte => true,
            TypeCode.UInt16 => true,
            TypeCode.UInt32 => true,
            TypeCode.UInt64 => true,
            TypeCode.Int16 => true,
            TypeCode.Int32 => true,
            TypeCode.Int64 => true,
            TypeCode.Decimal => true,
            TypeCode.Double => true,
            TypeCode.Single => true,
            _ => false
        };

        /// <summary>
        /// Parses an object into an enum using a generic T type.
        /// </summary>
        /// <typeparam name="T">The type to parse to.</typeparam>
        /// <param name="obj">The object to parse.</param>
        /// <returns>The converted enum.</returns>
        public static T ParseToEnum<T>( object obj ) => ( T ) Enum.Parse( typeof( T ), obj.ToString() );

        /// <summary>
        /// Parses an object into an enum without a generic T type.
        /// </summary>
        /// <param name="obj">The object that is the enum.</param>
        /// <param name="enumType">The type of the enum to parse to.</param>
        /// <returns>Returns an object casted to the provided enum type.</returns>
        public static object ParseToEnumNonGeneric( ReadOnlySpan<char> obj, Type enumType ) => Enum.Parse( enumType, obj.ToString() );
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