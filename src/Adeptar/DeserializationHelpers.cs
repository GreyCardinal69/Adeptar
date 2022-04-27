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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Adeptar
{
    internal class DeserializationHelpers
    {
        internal enum NumericType
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
            NotNumeric
        }

        /// <summary>
        /// Resolves objects considered numeric, such as: <see cref="int"/>, <see cref="long"/>,
        /// <see cref="short"/>, <see cref="byte"/> and others..
        /// </summary>
        /// <param name="typeOf">The type to convert to.</param>
        /// <param name="value">The string representation of the number.</param>
        /// <returns>The converted number.</returns>
        internal static object NumericResolver ( Type typeOf, string value )
        {
            value = value.Replace( ",", "" );
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
            };
        }

        internal static NumericType GetNumericType ( Type type )
        {
            if (type == typeof( sbyte ))
                return NumericType.Sbyte;
            if (type == typeof( byte ))
                return NumericType.Byte;
            if (type == typeof( short ))
                return NumericType.Short;
            if (type == typeof( ushort ))
                return NumericType.Ushort;
            if (type == typeof( int ))
                return NumericType.Int;
            if (type == typeof( uint ))
                return NumericType.Uint;
            if (type == typeof( long ))
                return NumericType.Long;
            if (type == typeof( ulong ))
                return NumericType.Ulong;
            if (type == typeof( Single ))
                return NumericType.Single;
            if (type == typeof( decimal ))
                return NumericType.Decimal;
            if (type == typeof( double ))
                return NumericType.Double;
            return NumericType.NotNumeric;
        }

        /// <summary>
        /// Resolves objects of type <see cref="string"/>, takes care of quotation marks.
        /// </summary>
        /// <param name="line">The line to resolve.</param>
        /// <returns>The correctly trimmed and cuտ string.</returns>
        internal static string StringResolver ( ReadOnlySpan<char> line )
        {
            var trim = line.TrimStart( ' ' ).TrimStart( '\t' );
            trim = trim.EndsWith( "," )
                ? trim.Slice( 1, trim.Length - 3 )
                : trim.Slice( 1, trim.Length - 2 );
            return trim.ToString();
        }
    }
}