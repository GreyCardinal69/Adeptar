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
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using static Adeptar.AdeptarReader;

namespace Adeptar
{
    /// <summary>
    ///
    /// </summary>
    internal class DictionaryReader
    {
        /// <summary>
        ///
        /// </summary>
        /// <param name="text"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        internal static IDictionary DeserializeDictionary( ReadOnlySpan<char> text, Type type )
        {
            int level = 0;
            int i = 0;

            IDictionary motherDictionary = ( IDictionary ) Activator.CreateInstance( type );

            bool nested = false;
            bool inString = false;
            bool falseEnd = false;

            StringBuilder reader = new();

            Type[] generics = type.GetGenericArguments();
            Type keyType = generics[0];
            Type valueType = generics[1];

            Object key = null;
            Object value = null;

            text = text.Slice( 1, text.Length - 1 );

            foreach (var item in text)
            {
                switch (item)
                {
                    case '"':
                        if (falseEnd && inString){
                            falseEnd = false;
                            reader.Append( '\\' );
                        }
                        else if (!falseEnd){
                            inString = !inString;
                        }
                        if (nested){
                            reader.Append( item );
                        }
                        break;
                    case '[':
                        if (!inString){
                            level++; nested = true;
                        }
                        reader.Append( item );
                        break;
                    case ']':
                        if (level - 1 == 0 && !inString){
                            nested = false;
                        }
                        else if (level - 1 == -1 && !inString){
                            if (i == text.Length - 1){
                                value = DeserializeByChar( valueType, reader.ToString() );
                                motherDictionary.Add( key, value );
                                reader.Clear();
                            }
                        }
                        level--;
                        reader.Append( item );
                        break;
                    case '\\':
                        if (inString){
                            falseEnd = true;
                        }
                        break;
                    case ',':
                        if (!nested && !inString){
                            value = DeserializeByChar( valueType, reader.ToString() );
                            motherDictionary.Add( key, value );
                            reader.Clear();
                        }
                        else if (nested){
                            reader.Append( item );
                        }
                        break;
                    case '{':
                        if (!inString){
                            level++; nested = true;
                        }
                        reader.Append( item );
                        break;
                    case '}':
                        if (level - 1 == 0 && !inString){
                            nested = false;
                            level--;
                        }
                        reader.Append( item );
                        break;
                    case '(':
                        if (!inString){
                            level++; nested = true;
                        }
                        reader.Append( item );
                        break;
                    case ')':
                        if (level - 1 == 0 && !inString){
                            nested = false;
                            level--;
                        }
                        reader.Append( item );
                        break;
                    case ':':
                        if (!nested && !inString){
                            key = DeserializeByChar( keyType, reader.ToString() );
                            reader.Clear();
                        }
                        break;
                    default:
                        reader.Append( item );
                        break;
                }
                i++;
            }

            return motherDictionary;
        }
    }
}