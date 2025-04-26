using System;
using System.IO;
using System.Text;

namespace Adeptar
{
    /// <summary>
    /// A class containing helper methods for different use cases.
    /// </summary>
    public class AdeptarHelpers
    {
        /// <summary>
        /// Checks if the specified file contains an Adeptar object marker with the given ID (e.g., "~ObjectID~").
        /// Reads the file in chunks for efficiency, suitable for files with or without newlines.
        /// </summary>
        /// <param name="path">The path to the file to check.</param>
        /// <param name="id">The ID string to search for. Must contain only letters and digits.</param>
        /// <returns><c>true</c> if the pattern "~id~" is found; otherwise, <c>false</c>.</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="path"/> or <paramref name="id"/> is null.</exception>
        /// <exception cref="ArgumentException">Thrown if <paramref name="path"/> is empty or whitespace, or if <paramref name="id"/> is empty, whitespace, or contains characters other than letters or digits.</exception>
        /// <exception cref="AdeptarException">Thrown if an error occurs during file access (e.g., file not found, permissions error), wrapping the original IO exception.</exception>
        private static bool ContainsId( string path, string id )
        {
            if ( path == null ) throw new ArgumentNullException( nameof( path ) );
            if ( id == null ) throw new ArgumentNullException( nameof( id ) );
            if ( string.IsNullOrWhiteSpace( path ) ) throw new ArgumentException( "File path cannot be empty or whitespace.", nameof( path ) );
            if ( string.IsNullOrWhiteSpace( id ) ) throw new ArgumentException( "ID cannot be empty or whitespace.", nameof( id ) );

            for ( int i = 0; i < id.Length; i++ )
            {
                if ( !char.IsLetterOrDigit( id[i] ) )
                {
                    throw new ArgumentException( $"ID '{id}' contains invalid character '{id[i]}' at index {i}. Only letters and digits are allowed.", nameof( id ) );
                }
            }

            const int bufferSize = 4096;
            char[] buffer = new char[bufferSize];
            int charsRead;
            int idLength = id.Length;
            int matchIndex = 0;

            using ( FileStream fs = new FileStream( path, FileMode.Open, FileAccess.Read, FileShare.Read ) )
            using ( StreamReader reader = new StreamReader( fs, Encoding.UTF8, true, bufferSize ) )
            {
                while ( ( charsRead = reader.ReadBlock( buffer, 0, bufferSize ) ) > 0 )
                {
                    for ( int i = 0; i < charsRead; i++ )
                    {
                        char currentChar = buffer[i];

                        if ( matchIndex == 0 )
                        {
                            if ( currentChar == '~' )
                            {
                                matchIndex = 1;
                            }
                        }
                        else if ( matchIndex <= idLength )
                        {
                            if ( currentChar == id[matchIndex - 1] )
                            {
                                matchIndex++;
                            }
                            else
                            {
                                matchIndex = ( currentChar == '~' ) ? 1 : 0;
                            }
                        }
                        else
                        {
                            if ( currentChar == '~' )
                            {
                                // Found the complete pattern ~id~
                                return true;
                            }
                            else // Character after ID wasn't '~'
                            {
                                matchIndex = ( currentChar == '~' ) ? 1 : 0;
                            }
                        }
                    }
                }
            }

            return false;
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