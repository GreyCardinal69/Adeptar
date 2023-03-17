using System;

namespace Adeptar
{
    /// <summary>
    /// A class containing custom <see cref="DateTime"/> formatting functions.
    /// </summary>
    internal static class DateTimeWriterHelper
    {
        /// <summary>
        /// A faster <see cref="DateTime"/> string formatter.
        /// </summary>
        /// <param name="dateTime">The date to format.</param>
        /// <returns>The formatted <see cref="DateTime"/></returns>
        internal static string FormatDateTime( DateTime dateTime )
        {
            return string.Create( 21, dateTime, ( chars, dt ) =>
            {
                Write2Chars( chars, 0, dt.Day );
                chars[2] = '.';
                Write2Chars( chars, 3, dt.Month );
                chars[5] = '.';
                Write2Chars( chars, 6, dt.Year % 100 );
                chars[8] = ' ';
                Write2Chars( chars, 9, dt.Hour );
                chars[11] = ' ';
                Write2Chars( chars, 12, dt.Minute );
                chars[14] = ' ';
                Write2Chars( chars, 15, dt.Second );
                chars[17] = ' ';
                Write2Chars( chars, 18, dt.Millisecond / 10 );
                chars[20] = Digit( dt.Millisecond % 10 );
            } );
        }

        private static void Write2Chars( in Span<char> chars, int offset, int value )
        {
            chars[offset] = Digit( value / 10 );
            chars[offset + 1] = Digit( value % 10 );
        }

        private static char Digit( int value )
        {
            return ( char ) ( value + '0' );
        }
    }
}