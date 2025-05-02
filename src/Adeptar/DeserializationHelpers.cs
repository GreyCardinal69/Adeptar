using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using static Adeptar.AdeptarDebugger;

namespace Adeptar
{
    /// <summary>
    /// A class that contains methods for the deserialization of .Adeptar objects.
    /// </summary>
    internal sealed class DeserializationHelpers
    {  /// <summary>
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
        internal static Type _intType = typeof( int );

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
        /// Cached type for <see cref="IList"/>.
        /// </summary>
        internal static readonly Type _IListType = typeof(IList);

        /// <summary>
        /// Cached type for <see cref="object"/>.
        /// </summary>
        internal static readonly Type _objectType = typeof(object);

        /// <summary>
        /// Cached type for <see cref="IEnumerable{T}"/>.
        /// </summary>
        internal static readonly Type _IEnumerableType = typeof(IEnumerable<>);

        /// <summary>
        /// Cached type for <see cref="List{T}"/>.
        /// </summary>
        internal static readonly Type _listType = typeof(List<>);

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
        /// Removes insignificant whitespace (tabs, spaces, newlines) from Adeptar text
        /// that is *outside* of double-quoted string literals.
        /// </summary>
        /// <param name="str">The Adeptar text span to clean.</param>
        /// <returns>A new string with insignificant whitespace removed.</returns>
        internal static ReadOnlySpan<char> CleanText( ReadOnlySpan<char> str )
        {
            StringBuilder sb = new( str.Length );

            bool inString = false;
            bool escapeNext = false;

            foreach ( char c in str )
            {
                if ( escapeNext )
                {
                    sb.Append( c );
                    escapeNext = false;
                    continue;
                }

                if ( c == '\\' )
                {
                    sb.Append( c );
                    escapeNext = true;
                    continue;
                }

                if ( c == '"' )
                {
                    inString = !inString;
                    sb.Append( c );
                    continue;
                }

                if ( inString )
                {
                    sb.Append( c );
                }
                else
                {
                    switch ( c )
                    {
                        case '\t':
                        case ' ':
                        case '\n':
                        case '\r':
                            break;
                        default:
                            sb.Append( c );
                            break;
                    }
                }
            }

            return sb.ToString();
        }

        /// <summary>
        /// Validates that the provided span represents an Adeptar object structure,
        /// meaning it is not null, has at least two characters, starts with '{', and ends with '}'.
        /// </summary>
        /// <param name="span">The span representing the potential object data.</param>
        /// <exception cref="AdeptarException">Thrown if the span does not meet the required object format criteria (minimum length, start '{', end '}').</exception>
        internal static void ValidateSurroundingBraces( ReadOnlySpan<char> span )
        {
            if ( span.Length < 2 || span[0] != '{' || span[span.Length - 1] != '}' )
                throw new AdeptarException( $"Invalid object format. Expected content enclosed in braces '{{}}'. Received start: '{AdeptarDebugger.PreviewSpan( span, 50 )}'" );
        }

        /// <summary>
        /// Validates that the provided span represents an Adeptar array or list structure,
        /// meaning it is not null, has at least two characters, starts with '[', and ends with ']'.
        /// </summary>
        /// <param name="span">The span representing the potential array or list data.</param>
        /// <exception cref="AdeptarException">Thrown if the span does not meet the required array/list format criteria (minimum length, start '[', end ']').</exception>
        internal static void ValidateSurroundingBrackets( ReadOnlySpan<char> span )
        {
            if ( span.Length < 2 || span[0] != '[' || span[span.Length - 1] != ']' )
            {
                throw new AdeptarException( $"Invalid array/list format. Expected content enclosed in square brackets '[]'. Received start: '{AdeptarDebugger.PreviewSpan( span, 50 )}'" );
            }
        }

        /// <summary>
        /// Finds the index of the next top-level delimiter (defaulting to ',') not nested within strings or structures.
        /// Can optionally stop at a specific closing character.
        /// </summary>
        internal static int FindNextTopLevelDelimiter( ReadOnlySpan<char> span, int startIndex, out char delimiterFound, char primaryDelimiter = ',', char stopChar = '\0' )
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
        /// Resolves objects considered numeric, such as: <see cref="int"/>, <see cref="long"/>,
        /// <see cref="short"/>, <see cref="byte"/> and others..
        /// </summary>
        /// <param name="typeOf">The type to convert to.</param>
        /// <param name="value">The string representation of the number.</param>
        /// <returns>The adeptar string converted to a .NET object.</returns>
        internal static object ConvertToNumeric( Type typeOf, string value ) => GetNumericType( typeOf ) switch
        {
            NumericType.Byte => Convert.ToByte( value, CultureInfo.InvariantCulture ),
            NumericType.Sbyte => Convert.ToSByte( value, CultureInfo.InvariantCulture ),
            NumericType.Short => Convert.ToInt16( value, CultureInfo.InvariantCulture ),
            NumericType.Ushort => Convert.ToUInt16( value, CultureInfo.InvariantCulture ),
            NumericType.Long => Convert.ToInt64( value, CultureInfo.InvariantCulture ),
            NumericType.Ulong => Convert.ToUInt64( value, CultureInfo.InvariantCulture ),
            NumericType.Single => Convert.ToSingle( value, CultureInfo.InvariantCulture ),
            NumericType.Decimal => Convert.ToDecimal( value, CultureInfo.InvariantCulture ),
            NumericType.Double => Convert.ToDouble( value, CultureInfo.InvariantCulture ),
            NumericType.Uint => Convert.ToUInt64( value, CultureInfo.InvariantCulture ),
            NumericType.Int => Convert.ToInt32( value, CultureInfo.InvariantCulture ),
            NumericType.NotNumeric => null
        };

        /// <summary>
        /// Gets the <see cref="NumericType"/> of the specified <see cref="Type"/>
        /// which is presumably a numeric one.
        /// </summary>
        /// <param name="type">The type to check.</param>
        /// <returns>The <see cref="NumericType"/> of the provided <see cref="Type"/>.</returns>
        private static NumericType GetNumericType( Type type )
        {
            if ( type == _sbyteType )
                return NumericType.Sbyte;
            if ( type == _byteType )
                return NumericType.Byte;
            if ( type == _shortType )
                return NumericType.Short;
            if ( type == _ushortType )
                return NumericType.Ushort;
            if ( type == _intType )
                return NumericType.Int;
            if ( type == _uintType )
                return NumericType.Uint;
            if ( type == _longType )
                return NumericType.Long;
            if ( type == _ulongType )
                return NumericType.Ulong;
            if ( type == _floatType )
                return NumericType.Single;
            if ( type == _decimalType )
                return NumericType.Decimal;
            if ( type == _doubleType )
                return NumericType.Double;
            return NumericType.NotNumeric;
        }

        /// <summary>
        /// Extracts the string data of an object with the provided id from the ID feature object collection.
        /// </summary>
        /// <param name="textSpan">The object collection string to extract from.</param>
        /// <param name="id">The id of the object to extract.</param>
        /// <param name="additionalTakeAway">Amount of additional characters taken away from the end of the extracted string.</param>
        /// <returns></returns>
        internal static ReadOnlySpan<char> FetchAppendedSegment( ReadOnlySpan<char> textSpan, string id, int additionalTakeAway = 0 )
        {
            int idLength = id.Length;
            int searchPos = 0;

            while ( searchPos < textSpan.Length )
            {
                int openTildePos = textSpan.Slice(searchPos).IndexOf('~');
                if ( openTildePos == -1 ) break;
                openTildePos += searchPos;

                if ( openTildePos + 1 + idLength + 1 > textSpan.Length ) break;

                ReadOnlySpan<char> idCandidateSpan = textSpan.Slice(openTildePos + 1, idLength);

                if ( idCandidateSpan.SequenceEqual( id.AsSpan() ) )
                {
                    if ( textSpan[openTildePos + 1 + idLength] == '~' )
                    {
                        int segmentStartPos = openTildePos + 1 + idLength + 1;
                        int nextTildePos = textSpan.Slice(segmentStartPos).IndexOf('~');
                        int segmentEndPos;

                        if ( nextTildePos == -1 )
                        {
                            segmentEndPos = textSpan.Length;
                        }
                        else
                        {
                            segmentEndPos = segmentStartPos + nextTildePos;
                        }

                        int rawLength = segmentEndPos - segmentStartPos;

                        int finalLength = rawLength - additionalTakeAway;
                        if ( finalLength < 0 )
                        {
                            throw new ArgumentOutOfRangeException( nameof( additionalTakeAway ), $"Calculated segment length ({rawLength}) is less than additionalTakeAway ({additionalTakeAway}) for ID '{id}'." );
                        }

                        return textSpan.Slice( segmentStartPos, finalLength );
                    }
                }
                searchPos = openTildePos + 1;
            }

            throw new AdeptarException( $"Could not find appended segment with ID: '{id}'." );
        }

        /// <summary>
        /// Removes first and last quotation marks of the string.
        /// </summary>
        /// <param name="text">The string to resolve.</param>
        /// <returns>The string with first and last quotation marks removed.</returns>
        internal static string RemoveQuotationMarksFromString( ReadOnlySpan<char> text ) => text.Slice( 1, text.Length - 2 ).ToString();

        /// <summary>
        /// Parses a span containing an enum member name into the specified enum type (non-generic).
        /// </summary>
        /// <param name="enumType">The enum type to parse into.</param>
        /// <param name="value">The span containing the member name to parse.</param>
        /// <param name="ignoreCase">Whether to ignore case during parsing.</param>
        /// <returns>The parsed enum value as an object.</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="enumType"/> is null.</exception>
        /// <exception cref="ArgumentException">Thrown if <paramref name="enumType"/> is not an enum type, or if <paramref name="value"/> cannot be parsed.</exception>
        public static object ParseToEnum( Type enumType, ReadOnlySpan<char> value, bool ignoreCase = false )
        {
            ArgumentNullException.ThrowIfNull( enumType );
            return Enum.Parse( enumType, value.ToString(), ignoreCase );
        }

        /// <summary>
        /// Increments an array with a binary style.
        /// Starting from the end and traversing towards the start.
        /// The array serves as an index for a dimensional array.
        /// Accepts a <see cref="List{T}"/> of ints that serves as an upper bound.
        /// </summary>
        /// <param name="sizes">The sizes of the dimensional array.</param>
        /// <param name="index">The current index of the dimensional array to use.</param>
        /// <returns>The index array incremented with binary style.</returns>
        public static void BinaryStyleIndexArrayByRefIncrement( in List<int> sizes, ref int[] index )
        {
            for ( int i = sizes.Count - 1; i >= 0; i-- )
            {
                if ( sizes[i] > index[i] )
                {
                    index[i]++;
                    return;
                }
                else
                {
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