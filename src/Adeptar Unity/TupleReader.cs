using System;
using System.Reflection;

using static Adeptar.Unity.AdeptarReader;

namespace Adeptar.Unity
{
    /// <summary>
    /// Internal class containing method(s) for deserialization of tuples
    /// </summary>
    internal class TupleReader
    {
        /// <summary>
        /// Deserializes the Adeptar string of a tuple to a .NET object.
        /// </summary>
        /// <param name="text">The Adeptar string representation of the object.</param>
        /// <param name="type">The type of the tuple.</param>
        /// <returns>The .NET version of the tuple.</returns>
        internal static object DeserializeTuple( ReadOnlySpan<char> text, Type type )
        {
            int level = 0;
            int i = 0;
            int w = 0;
            int j = 0;

            object target = Activator.CreateInstance( type );

            bool nested = false;
            bool inString = false;
            bool falseEnd = false;

            text = text.Slice( 1 );
            string name = "";

            foreach ( char item in text )
            {
                switch ( item )
                {
                    case '"':
                        if ( falseEnd && inString )
                        {
                            falseEnd = false;
                        }
                        else if ( !falseEnd )
                            inString = !inString;
                        break;
                    case '[':
                        if ( !inString )
                        {
                            level++; nested = true;
                        }
                        else if ( !inString )
                            level++;
                        break;
                    case ']':
                        if ( level - 1 == 0 && !inString )
                        {
                            nested = false;
                        }
                        level--;
                        break;
                    case '\'':
                        break;
                    case '\\':
                        if ( inString )
                        {
                            falseEnd = true;
                        }
                        else
                        {
                            throw new AdeptarException( "Invalid character '\\', such a character can appear only inside a string." );
                        }
                        break;
                    case ',':
                        if ( !nested && !inString )
                        {
                            FieldInfo field = type.GetField( name );
                            field.SetValue( target, DeserializeObject( field.FieldType, text.Slice( j, w - j ) ) );
                            j = w + 1;
                            i++;
                        }
                        break;
                    case '{':
                        if ( !inString )
                        {
                            level++;
                            nested = true;
                        }
                        break;
                    case '}':
                        if ( level - 1 == 0 && !inString )
                        {
                            nested = false;
                        }
                        level--;
                        break;
                    case '(':
                        if ( !inString )
                        {
                            level++;
                            nested = true;
                        }
                        break;
                    case ')':
                        if ( level - 1 == 0 && !inString )
                        {
                            nested = false;
                            level--;
                        }
                        else if ( level - 1 == -1 && !inString )
                        {
                            level--;
                            if ( w == text.Length - 1 )
                            {
                                FieldInfo field = type.GetField( name );
                                field.SetValue( target, DeserializeObject( field.FieldType, text.Slice( j, w - j ) ) );
                                j = w + 1;
                                i++;
                            }
                        }
                        break;
                    case ':':
                        if ( !nested && !inString )
                        {
                            name = text.Slice( j, w - j ).ToString();
                            j = w + 1;
                        }
                        break;
                    default:
                        if ( !inString &&
                            item != '_' &&
                            item != '-' &&
                            !char.IsLetter( item ) &&
                            !char.IsDigit( item ) )
                        {
                            throw new AdeptarException( $"Invalid character \"{item}\" outside of string at position {i} ( indentation removed )." );
                        }
                        break;
                }
                w++;
            }

            return target;
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