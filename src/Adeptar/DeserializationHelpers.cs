using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

using static Adeptar.AdeptarDebugger;

namespace Adeptar
{
    /// <summary>
    /// A class that contains methods for the deserialization of .Adeptar objects.
    /// </summary>
    internal class DeserializationHelpers
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
        /// Parses a string segment representing a numeric value into the specified target numeric type.
        /// Uses culture-invariant parsing and handles potential format or overflow errors.
        /// </summary>
        /// <param name="targetType">The target numeric <see cref="Type"/> (e.g., typeof(int), typeof(double)).</param>
        /// <param name="valueSpan">The <see cref="ReadOnlySpan{Char}"/> containing the numeric string to parse.</param>
        /// <returns>The parsed numeric value as an <see cref="object"/>.</returns>
        /// <exception cref="AdeptarException">Thrown if the value cannot be parsed as the target type due to format or overflow issues.</exception>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="targetType"/> is null.</exception>
        /// <exception cref="NotSupportedException">Thrown if the <paramref name="targetType"/> is not a supported numeric type.</exception>
        internal static object ParseNumeric( Type targetType, ReadOnlySpan<char> valueSpan )
        {
            if ( targetType == null ) throw new ArgumentNullException( nameof( targetType ) );

            const NumberStyles styles = NumberStyles.Any;
            var culture = CultureInfo.InvariantCulture;

            try
            {
                if ( targetType == _intType ) { if ( int.TryParse( valueSpan, styles, culture, out int result ) ) return result; }
                else if ( targetType == _doubleType ) { if ( double.TryParse( valueSpan, styles, culture, out double result ) ) return result; }
                else if ( targetType == _floatType ) { if ( float.TryParse( valueSpan, styles, culture, out float result ) ) return result; }
                else if ( targetType == _longType ) { if ( long.TryParse( valueSpan, styles, culture, out long result ) ) return result; }
                else if ( targetType == _decimalType ) { if ( decimal.TryParse( valueSpan, styles, culture, out decimal result ) ) return result; }
                else if ( targetType == _shortType ) { if ( short.TryParse( valueSpan, styles, culture, out short result ) ) return result; }
                else if ( targetType == _byteType ) { if ( byte.TryParse( valueSpan, styles, culture, out byte result ) ) return result; }
                else if ( targetType == _sbyteType ) { if ( sbyte.TryParse( valueSpan, styles, culture, out sbyte result ) ) return result; }
                else if ( targetType == _ushortType ) { if ( ushort.TryParse( valueSpan, styles, culture, out ushort result ) ) return result; }
                else if ( targetType == _uintType ) { if ( uint.TryParse( valueSpan, styles, culture, out uint result ) ) return result; }
                else if ( targetType == _ulongType ) { if ( ulong.TryParse( valueSpan, styles, culture, out ulong result ) ) return result; }
                else { throw new NotSupportedException( $"Numeric parsing not supported for type: {targetType.FullName}" ); }

                throw new FormatException( $"Input string '{valueSpan.ToString()}' was not in a correct format for type {targetType.Name}." );
            }
            catch ( Exception ex ) when ( ex is FormatException || ex is OverflowException )
            {
                throw new AdeptarException( $"Failed to parse numeric value '{PreviewSpan( valueSpan )}' as type {targetType.Name}. Reason: {ex.Message}", ex );
            }
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
                        // Found the marker ~id~
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
                            // This suggests additionalTakeAway is too large for the found segment
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
        /// Resolves escape sequences within a string literal read from Adeptar format.
        /// Assumes input includes the surrounding double quotes.
        /// </summary>
        /// <param name="text">The string literal span (including quotes) to unescape.</param>
        /// <returns>The unescaped string content.</returns>
        /// <exception cref="AdeptarException">Thrown for invalid escape sequences or unterminated strings.</exception>
        /// <remarks>
        /// Handles standard escape sequences: \", \\, \n, \r, \t, \b, \f.
        /// Basic support for \uXXXX Unicode escapes.
        /// </remarks>
        internal static string StringResolver( ReadOnlySpan<char> text )
        {
            if ( text.Length < 2 || text[0] != '"' || text[text.Length - 1] != '"' )
            {
                throw new AdeptarException( $"Invalid string literal format. Expected content enclosed in double quotes. Received: '{PreviewSpan( text )}'" );
            }

            ReadOnlySpan<char> innerSpan = text.Slice(1, text.Length - 2);
            StringBuilder sb = new StringBuilder(innerSpan.Length);
            bool escapeActive = false;

            for ( int i = 0; i < innerSpan.Length; i++ )
            {
                char c = innerSpan[i];

                if ( escapeActive )
                {
                    switch ( c )
                    {
                        case '"': sb.Append( '"' ); break;
                        case '\\': sb.Append( '\\' ); break;
                        case 'n': sb.Append( '\n' ); break;
                        case 'r': sb.Append( '\r' ); break;
                        case 't': sb.Append( '\t' ); break;
                        case 'b': sb.Append( '\b' ); break;
                        case 'f': sb.Append( '\f' ); break;
                        case 'u':
                            // Unicode escape \uXXXX
                            if ( i + 4 < innerSpan.Length )
                            {
                                ReadOnlySpan<char> hexSpan = innerSpan.Slice(i + 1, 4);
                                if ( ushort.TryParse( hexSpan, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out ushort unicodeChar ) )
                                {
                                    sb.Append( (char)unicodeChar );
                                    i += 4; // Skip the 4 hex digits
                                }
                                else
                                {
                                    throw new AdeptarException( $"Invalid Unicode escape sequence '\\u{hexSpan.ToString()}' found in string literal." );
                                }
                            }
                            else
                            {
                                throw new AdeptarException( "Incomplete Unicode escape sequence '\\u' found at end of string literal." );
                            }
                            break;
                        default:
                            throw new AdeptarException( $"Invalid escape sequence '\\{c}' found in string literal." );
                    }
                    escapeActive = false;
                }
                else if ( c == '\\' )
                {
                    escapeActive = true;
                }
                else if ( c == '"' )
                {
                    throw new AdeptarException( "Unescaped double quote found inside string literal." );
                }
                else
                {
                    sb.Append( c );
                }
            }

            if ( escapeActive )
            {
                throw new AdeptarException( "Unterminated escape sequence at end of string literal." );
            }

            return sb.ToString();
        }

        /// <summary>
        /// Increments an array with a binary style.
        /// Starting from the end and traversing towards the start.
        /// The array serves as an index for a dimensional array.
        /// Accepts a <see cref="List{T}"/> of ints that serves as an upper bound.
        /// </summary>
        /// <param name="dimensionSizes">The sizes of the dimensional array.</param>
        /// <param name="currentIndex">The current index of the dimensional array to use.</param>
        /// <returns>The index array incremented with binary style.</returns>
        public static void BinaryStyleIndexArrayByRefIncrement( in List<int> dimensionSizes, ref int[] currentIndex )
        {
            if ( dimensionSizes == null ) throw new ArgumentNullException( nameof( dimensionSizes ) );
            if ( currentIndex == null ) throw new ArgumentNullException( nameof( currentIndex ) );
            if ( dimensionSizes.Count != currentIndex.Length )
                throw new ArgumentException( $"Dimension mismatch: sizes has {dimensionSizes.Count} elements, index has {currentIndex.Length} elements." );

            for ( int i = dimensionSizes.Count - 1; i >= 0; i-- )
            {
                if ( dimensionSizes[i] > currentIndex[i] )
                {
                    currentIndex[i]++;
                    return;
                }
                else
                {
                    currentIndex[i] = 0;
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