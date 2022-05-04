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
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using FastMember;

using static Adeptar.AdeptarReader;

namespace Adeptar
{
    public static class TypeAccessorExtensions
    {
        public static void AssignValue ( this TypeAccessor accessor, object t, MemberSet members, string fieldName, object fieldValue )
        {
            foreach (var item in members)
            {
                if (string.Equals( item.Name, fieldName, StringComparison.OrdinalIgnoreCase ))
                {
                    accessor[t, fieldName] = fieldValue;
                }
            }
        }
    }
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
            int w = 0;

            int j = 0;

            var target = Activator.CreateInstance( type );

            var accessor = TypeAccessor.Create( type, true );
            var members = accessor.GetMembers();

            bool nested = false;
            bool inString = false;
            bool falseEnd = false;

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
                        }
                        else if (!falseEnd)
                        {
                            inString = !inString;
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
                        break;
                    case ']':
                        if (level - 1 == 0 && !inString)
                        {
                            nested = false;
                        }
                        level--;
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
                            accessor[target, name] = DeserializeObject( members[i].Type, text.Slice(j,w-j) );
                            j = w + 1;
                            i++;
                        }
                        break;
                    case '{':
                        if (!inString)
                        {
                            level++; nested = true;
                        }
                        break;
                    case '}':
                        if (level - 1 == 0 && !inString)
                        {
                            nested = false;
                        }
                        else if (level - 1 == -1 && !inString)
                        {
                            if (w == text.Length - 1)
                            {
                                accessor[target, name] = DeserializeObject( members[i].Type, text.Slice( j, w - j ) );
                            }
                        }
                        level--;
                        break;
                    case '(':
                        if (!inString)
                        {
                            level++; nested = true;
                        }
                        break;
                    case ')':
                        if (level - 1 == 0 && !inString)
                        {
                            nested = false;
                            level--;
                        }
                        break;
                    case ':':
                        if (!nested && !inString)
                        {
                            name = text.Slice(j,w-j).ToString();
                            j = w + 1;
                        }
                        break;
                    default:
                        break;
                }
                w++;
            }

            return target;
        }
    }
}