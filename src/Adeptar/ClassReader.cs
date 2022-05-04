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
using FastMember;

using static Adeptar.AdeptarReader;

namespace Adeptar
{
    /// <summary>
    ///
    /// </summary>
    internal class ClassReader
    {
        /// <summary>
        ///
        /// </summary>
        /// <param name="text"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        internal static object DeserializeClassStruct ( ReadOnlySpan<char> text, Type type )
        {
            int level = 0;
            int i = 0;

            var target = Activator.CreateInstance( type );
            var accessor = TypeAccessor.Create( type );

            bool nested = false;
            bool inString = false;
            bool falseEnd = false;

            StringBuilder reader = new();

            text = text.Slice( 1, text.Length - 1 );
            string name = "";

            foreach (var item in text)
            {
                switch (item)
                {
                    case '"':
                        if (falseEnd && inString)
                        {
                            falseEnd = false;
                            reader.Append( '\\' );
                        }
                        else if (!falseEnd)
                        {
                            inString = !inString;
                        }
                        if (nested)
                        {
                            reader.Append( item );
                        }
                        break;
                    case '[':
                        if (!inString)
                        {
                            level++; nested = true;
                        }
                        else if (!inString)
                        {
                            level++;
                        }
                        reader.Append( item );
                        break;
                    case ']':
                        if (level - 1 == 0 && !inString)
                        {
                            nested = false;
                        }
                        else if (level - 1 == -1 && !inString)
                        {
                            if (i == text.Length - 1)
                            {
                                accessor[target, name] = DeserializeObject( accessor[target, name].GetType(), reader.ToString() );
                                name = "";
                                reader.Clear();
                            }
                        }
                        level--;
                        reader.Append( item );
                        break;
                    case '\\':
                        if (inString)
                        {
                            falseEnd = true;
                        }
                        break;
                    case ',':
                        if (!nested && !inString)
                        {
                            accessor[target, name] = DeserializeObject( accessor[target, name].GetType(), reader.ToString() );
                            name = "";
                            reader.Clear();
                        }
                        else if (nested)
                        {
                            reader.Append( item );
                        }
                        break;
                    case '{':
                        if (!inString)
                        {
                            level++; nested = true;
                        }
                        reader.Append( item );
                        break;
                    case '}':
                        if (level - 1 == 0 && !inString)
                        {
                            nested = false;
                            level--;
                        }
                        reader.Append( item );
                        break;
                    case '(':
                        if (!inString)
                        {
                            level++; nested = true;
                        }
                        reader.Append( item );
                        break;
                    case ')':
                        if (level - 1 == 0 && !inString)
                        {
                            nested = false;
                            level--;
                        }
                        reader.Append( item );
                        break;
                    case ':':
                        if (!nested && !inString)
                        {
                            name = reader.ToString();
                            reader.Clear();
                        }
                        else
                        {
                            reader.Append( item );
                        }
                        break;
                    default:
                        reader.Append( item );
                        break;
                }
                i++;
            }

            return target;
        }
    }
}