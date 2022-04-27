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

using System;
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
        /// Checks if the provided object is a <see cref="List{T}"/>.
        /// </summary>
        /// <param name="obj">The object to check.</param>
        /// <returns>
        /// True if the object is a <see cref="List{T}"/>.
        /// </returns>
        public static bool IsList ( object obj )
        {
            return obj is IList &&
                   obj.GetType().IsGenericType &&
                   obj.GetType().GetGenericTypeDefinition().IsAssignableFrom( typeof( List<> ) );
        }

        /// <summary>
        /// Checks if the provided object is a list, accepts a <see cref="Type"/> instead.
        /// </summary>
        /// <param name="type">The type to check for.</param>
        /// <returns>
        /// True if the type is a list.
        /// </returns>
        public static bool IsList ( Type type )
        {
            return type.IsGenericType &&
                 ( type.GetGenericTypeDefinition() == typeof( List<> ) || type.GetGenericTypeDefinition() == typeof( IList<> ) );
        }

        /// <summary>
        /// Checks if an object is of type <see cref="ValueTuple"/>, such as (<see cref="int"/>, <see cref="int"/>).
        /// </summary>
        /// <param name="tuple">The type to check for.</param>
        /// <returns>True if the object is a <see cref="ValueTuple"/>.</returns>
        public static bool IsTuple ( Type tuple )
        {
            if (!tuple.IsGenericType)
                return false;
            var openType = tuple.GetGenericTypeDefinition();
            return openType == typeof( ValueTuple<> )
                || openType == typeof( ValueTuple<,> )
                || openType == typeof( ValueTuple<,,> )
                || openType == typeof( ValueTuple<,,,> )
                || openType == typeof( ValueTuple<,,,,> )
                || openType == typeof( ValueTuple<,,,,,> )
                || openType == typeof( ValueTuple<,,,,,,> )
                || openType == typeof( ValueTuple<,,,,,,,> ) && IsTuple( tuple.GetGenericArguments()[7] );
        }

        /// <summary>
        /// Checks if the provided object is a <see cref="Dictionary{TKey, TValue}"/>.
        /// </summary>
        /// <param name="obj">The object to check.</param>
        /// <returns>
        /// True/False if the object is of type <see cref="Dictionary{TKey, TValue}"/>.
        /// </returns>
        public static bool IsDictionary ( object obj )
        {
            if (obj == null) return false;
            return obj is IDictionary;
        }

        /// <summary>
        /// Checks if the provided object is a <see cref="Dictionary{TKey, TValue}"/>, uses a <see cref="Type"/>.
        /// </summary>
        /// <param name="type">The type to check for.</param>
        /// <returns>
        /// True if the type is a <see cref="Dictionary{TKey, TValue}"/>.
        /// </returns>
        public static bool IsDictionary ( Type type )
        {
            if (type == null) return false;
            return type.IsGenericType && type.GetGenericTypeDefinition() == typeof( Dictionary<,> );
        }

        /// <summary>
        /// Checks if the provided object has an empty constructor defined.
        /// </summary>
        /// <param name="type">The type to check</param>
        public static bool HasDefaultConstructor ( Type type )
        {
            return type.IsValueType || type.GetConstructor( Type.EmptyTypes ) != null;
        }

        /// <summary>
        /// Checks if the provided object is a class or a struct.
        /// </summary>
        /// <param name="obj">The provided object.</param>
        /// <returns>
        /// True if the object is a class or a struct. Returns false if the object is null.
        /// </returns>
        public static bool IsClassStruct ( object obj )
        {
            if (obj is null)
                return false;

            var type = obj.GetType();

            if (type.IsClass || ( type.IsValueType && !type.IsEnum ))
                return true;

            if (!IsList( obj ) && !IsDictionary( obj ) && ( type != typeof( string ) ))
                return true;

            return false;
        }

        /// <summary>
        /// Gets the Type's <see cref="DeserializableType"/>.
        /// </summary>
        /// <param name="fInfo">The Type's field type.</param>
        /// <returns>
        /// The object's <see cref="DeserializableType"/>. Returns <see cref="DeserializableType.Class"/> if the type
        /// cant be determined.
        /// </returns>
        internal static DeserializableType GetDeserializableType ( Type fInfo )
        {
            if (fInfo.IsEnum)
                return DeserializableType.Enum;
            if (IsNumericType( fInfo ))
                return DeserializableType.Numeric;
            if (fInfo == typeof( string ))
                return DeserializableType.String;
            if (fInfo == typeof( char ))
                return DeserializableType.Char;
            if (fInfo == typeof( bool ))
                return DeserializableType.Boolean;
            if (IsList( fInfo ))
                return DeserializableType.List;
            if (IsDictionary( fInfo ))
                return DeserializableType.Dictionary;
            if (fInfo == typeof( IDictionary ))
                return DeserializableType.Dictionary;
            if (fInfo == typeof( DateTime ))
                return DeserializableType.DateTime;
            if (fInfo == typeof( object ))
                return DeserializableType.Object;
            if (IsTuple( fInfo ))
                return DeserializableType.Tuple;
            if (fInfo.GetInterface( typeof( ICollection<> ).FullName ) != null)
                return DeserializableType.Array;

            if (fInfo.IsArray)
            {
                int rank = fInfo.GetArrayRank();
                if (rank == 2)
                    return DeserializableType.DimensionalArray2D;
                if (rank == 3)
                    return DeserializableType.DimensionalArray3D;
            }

            return DeserializableType.Class;
        }

        /// <summary>
        /// Checks if the object is an array with two or more dimensions.
        /// </summary>
        /// <param name="received">The object to check.</param>
        /// <returns>True if the object has two or three dimensions.</returns>
        public static bool IsMultiDimensionalArray ( object received )
        {
            if (received is Array array)
                return array.Rank > 1;
            return false;
        }

        /// <summary>
        /// Checks if the <see cref="SerializableType"/> is considered simple.
        /// </summary>
        /// <param name="type">The <see cref="SerializableType"/> to check.</param>
        /// <returns>
        /// True if the type is <see cref="bool"/>, <see cref="string"/>, <see cref="char"/>, <see cref="Enum"/>, <see cref="DateTime"/> or a number type.
        /// </returns>
        internal static bool IsSimpleSerializableType ( SerializableType type )
        {
            return type == SerializableType.Boolean
                || type == SerializableType.String
                || type == SerializableType.Char
                || type == SerializableType.Enum
                || type == SerializableType.DateTime
                || type == SerializableType.Numeric;
        }

        /// <summary>
        /// Checks if the <see cref="DeserializableType"/> is considered simple, such are
        /// <see cref="string"/>, <see cref="bool"/>, <see cref="char"/>, <see cref="Enum"/> and numbers.
        /// </summary>
        /// <param name="type"></param>
        /// <returns>
        /// True if the type is <see cref="string"/>, <see cref="bool"/>, <see cref="char"/>, <see cref="Enum"/> or a number type.
        /// </returns>
        internal static bool IsSimpleDeserializableType ( DeserializableType type )
        {
            return    type == DeserializableType.Boolean
                   || type == DeserializableType.String
                   || type == DeserializableType.Char
                   || type == DeserializableType.Enum
                   || type == DeserializableType.Numeric
                   || type == DeserializableType.DateTime;
        }

        /// <summary>
        /// Gets the object's <see cref="SerializableType"/>.
        /// </summary>
        /// <param name="received">The received object.</param>
        /// <returns>
        /// The provided object's <see cref="SerializableType"/>. Returns <see cref="SerializableType.NULL"/> if the
        /// provided object is null or if its type can't be determined.
        /// </returns>
        public static SerializableType FetchType ( object received )
        {
            if (received is null)
                return SerializableType.NULL;
            if (received is bool)
                return SerializableType.Boolean;
            if (received is char)
                return SerializableType.Char;
            if (received is string)
                return SerializableType.String;
            if (IsNumber( received ))
                return SerializableType.Numeric;
            if (received is DateTime || received is DateTimeOffset)
                return SerializableType.DateTime;
            if (received is Array){
                if (IsMultiDimensionalArray( received )){
                    return SerializableType.DimensionalArray;
                }else{
                    return SerializableType.Array;
                }
            }
            if (received is Enum)
                return SerializableType.Enum;
            if (received is ITuple)
                return SerializableType.Tuple;
            if (IsDictionary( received ))
                return SerializableType.Dictionary;
            if (received.GetType().IsGenericType)
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
        public static bool IsNumber ( object value )
        {
            return     value is sbyte
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
        public static bool IsNumericType ( Type type )
        {
            return Type.GetTypeCode( type ) switch
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
        }

        /// <summary>
        /// Parses an object into an enum using a generic T type.
        /// </summary>
        /// <typeparam name="T">The type to parse to.</typeparam>
        /// <param name="obj">The object to parse.</param>
        /// <returns>The converted enum.</returns>
        public static T ParseToEnum<T> ( object obj )
        {
            return ( T ) Enum.Parse( typeof( T ), obj.ToString() );
        }

        /// <summary>
        /// Parses an object into an enum without a generic T type.
        /// </summary>
        /// <param name="obj">The object that is the enum.</param>
        /// <param name="enumType">The type of the enum to parse to.</param>
        /// <returns>Returns an object casted to the provided enum type.</returns>
        public static object ParseToEnumNonGeneric ( object obj, Type enumType )
        {
            return Enum.Parse( enumType, obj.ToString() );
        }
    }
}