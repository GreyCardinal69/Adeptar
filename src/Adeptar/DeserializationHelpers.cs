using System;
using System.Collections.Generic;
using System.Text;

namespace Adeptar
{
    /// <summary>
    /// A class that contains methods for deserialization of .Adeptar objects.
    /// </summary>
    public class DeserializationHelpers
    {
        /// <summary>
        /// Cached type for <see cref="sbyte"/>.
        /// </summary>
        private static Type _sbyteType = typeof( sbyte );

        /// <summary>
        /// Cached type for <see cref="byte"/>.
        /// </summary>
        private static Type _byteType = typeof( byte );

        /// <summary>
        /// Cached type for <see cref="short"/>.
        /// </summary>
        private static Type _shortType = typeof( short );

        /// <summary>
        /// Cached type for <see cref="ushort"/>.
        /// </summary>
        private static Type _ushortType = typeof( ushort );

        /// <summary>
        /// Cached type for <see cref="int"/>.
        /// </summary>
        internal static Type IntType = typeof( int );

        /// <summary>
        /// Cached type for <see cref="uint"/>.
        /// </summary>
        private static Type _uintType = typeof( uint );

        /// <summary>
        /// Cached type for <see cref="long"/>.
        /// </summary>
        private static Type _longType = typeof( long );

        /// <summary>
        /// Cached type for <see cref="ulong"/>.
        /// </summary>
        private static Type _ulongType = typeof( ulong );

        /// <summary>
        /// Cached type for <see cref="Single"/>.
        /// </summary>
        private static Type _floatType = typeof( Single );

        /// <summary>
        /// Cached type for <see cref="decimal"/>.
        /// </summary>
        private static Type _decimalType = typeof( decimal );

        /// <summary>
        /// Cached type for <see cref="double"/>.
        /// </summary>
        private static Type _doubleType = typeof( double );

        /// <summary>
        /// Private enumeration for determining the type of a number.
        /// </summary>
        private enum NumericType
        {
            Sbyte,
            Byte,
            Short,
            Ushort,
            Int,
            Uint,
            Long,
            Ulong,
            Decimal,
            Double,
            Single,
            NotNumeric,
        }

        /// <summary>
        /// Removes all indentation from a .Adeptar string.
        /// </summary>
        /// <param name="str">The .Adeptar string to clean.</param>
        /// <returns>The .Adeptar string with all the indentation removed.</returns>
        public static ReadOnlySpan<char> CleanText ( ReadOnlySpan<char> str )
        {
            StringBuilder sb = new(str.Length);

            bool inStr = false;
            bool falseMark = false;

            foreach (var item in str)
            {
                switch (item)
                {
                    case '"':
                        if (falseMark){
                            inStr = true;
                            falseMark = false;
                            sb.Append( item );
                            continue;
                        }else{
                            inStr = !inStr;
                            sb.Append( item );
                        }
                        break;
                    case '\\':
                        if (inStr){
                            sb.Append( item );
                            falseMark = true;
                        }
                        break;
                    default:
                        switch (item)
                        {
                            case '\t':
                                if (inStr){
                                    sb.Append( item );
                                }else{
                                    continue;
                                }
                                break;
                            case ' ':
                                if (inStr){
                                    sb.Append( item );
                                }else{
                                    continue;
                                }
                                break;
                            case '\n':
                                if (inStr){
                                    sb.Append( item );
                                }else{
                                    continue;
                                }
                                break;
                            default:
                                sb.Append( item );
                                break;
                        }
                        break;
                }
            }
            return sb.ToString();
        }

        /// <summary>
        /// Resolves objects considered numeric, such as: <see cref="int"/>, <see cref="long"/>,
        /// <see cref="short"/>, <see cref="byte"/> and others..
        /// </summary>
        /// <param name="typeOf">The type to convert to.</param>
        /// <param name="value">The string representation of the number.</param>
        /// <returns>The adeptar string converted to a .NET object.</returns>
        internal static object NumericResolver ( Type typeOf, string value )
        {
            return GetNumericType( typeOf ) switch
            {
                NumericType.Byte => Convert.ToByte( value ),
                NumericType.Sbyte => Convert.ToSByte( value ),
                NumericType.Short => Convert.ToInt16( value ),
                NumericType.Ushort => Convert.ToUInt16( value ),
                NumericType.Long => Convert.ToInt64( value ),
                NumericType.Ulong => Convert.ToUInt64( value ),
                NumericType.Single => Convert.ToSingle( value ),
                NumericType.Decimal => Convert.ToDecimal( value ),
                NumericType.Double => Convert.ToDouble( value ),
                NumericType.Uint => Convert.ToUInt64( value ),
                NumericType.Int => Convert.ToInt32( value ),
                NumericType.NotNumeric => null
            };
        }

        /// <summary>
        /// Gets the <see cref="NumericType"/> of the specified <see cref="Type"/>
        /// which is presumably a numeric one.
        /// </summary>
        /// <param name="type">The type to check.</param>
        /// <returns>The <see cref="NumericType"/> of the provided <see cref="Type"/>.</returns>
        private static NumericType GetNumericType ( Type type )
        {
            if (type == _sbyteType)
                return NumericType.Sbyte;
            if (type == _byteType)
                return NumericType.Byte;
            if (type == _shortType)
                return NumericType.Short;
            if (type == _ushortType)
                return NumericType.Ushort;
            if (type == IntType)
                return NumericType.Int;
            if (type == _uintType)
                return NumericType.Uint;
            if (type == _longType)
                return NumericType.Long;
            if (type == _ulongType)
                return NumericType.Ulong;
            if (type == _floatType)
                return NumericType.Single;
            if (type == _decimalType)
                return NumericType.Decimal;
            if (type == _doubleType)
                return NumericType.Double;
            return NumericType.NotNumeric;
        }

        /// <summary>
        /// Extracts the string data of an object with the provided id from the ID feature object collection.
        /// </summary>
        /// <param name="text">The object collection string to extract from.</param>
        /// <param name="id">The id of the object to extract.</param>
        /// <param name="additionalTakeAway">Amount of additional characters taken away from the end of the extracted string.</param>
        /// <returns></returns>
        internal static ReadOnlySpan<char> FetchAppendedSegment(  ReadOnlySpan<char> text, string id, int additionalTakeAway = 0 )
        {
            StringBuilder name = new();

            bool inString = false;
            bool falseEnd = false;
            bool inId = false;
            bool exit = false;

            int i = 0, j = 0, w = 0;
            int index = -1;

            foreach (var item in text)
            {
                index++;
                if (exit)
                {
                    break;
                }
                switch (item)
                {
                    case '"':
                        if (falseEnd && inString)
                            falseEnd = false;
                        else if (!falseEnd)
                            inString = !inString;
                        break;
                    case '\\':
                        if (inString)
                            falseEnd = true;
                        break;
                    case '~':
                        if (!inString)
                        {
                            inId = !inId;
                            if (inId)
                            {
                                w = index;
                            }
                            if (!inId && name.ToString() == id)
                            {
                                i = w + 1;
                            }
                            if (inId && i != 0)
                            {
                                j = index;
                                exit = true;
                                break;
                            }
                            if (inId && i == 0)
                            {
                                name.Clear();
                            }
                        }
                        break;
                    default:
                        if (inId)
                        {
                            name.Append( item );
                        }
                        break;
                }
                w++;
            }

            return CleanText( text.Slice( i, j - i - additionalTakeAway ) );
        }

        /// <summary>
        /// Removes first and last quatation marks of the string as well as
        /// removes extra backslashes.
        /// </summary>
        /// <param name="text">The string to resolve.</param>
        /// <returns>The string with first and last quotation marks as well as extra backslashes removed.</returns>
        internal static string StringResolver ( ReadOnlySpan<char> text ) => text.Slice( 1, text.Length - 2 ).ToString().Replace( "\\\"", "\"" );

        /// <summary>
        /// Increments an array with a binary style.
        /// Starting from the end and traversing towards the start.
        /// The array serves as an index for a dimensional array.
        /// Accepts a <see cref="List{T}"/> of ints that server as an upper bound.
        /// </summary>
        /// <param name="sizes">The sizes of the dimensional array.</param>
        /// <param name="index">The current index of the dimensional array to use.</param>
        /// <returns>The index array incremented with binary style.</returns>
        public static void BinaryStyleIndexArrayByRefIncrement ( in List<int> sizes, ref int[] index )
        {
            for (int i = sizes.Count - 1; i >= 0; i--)
            {
                if (sizes[i] > index[i]){
                    index[i]++;
                    return;
                }else{
                    index[i] = 0;
                }
            }
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