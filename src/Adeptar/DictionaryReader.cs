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
    /// Internal class containing method(s) for deserialization of <see cref="Dictionary{TKey, TValue}"/>
    /// </summary>
    internal class DictionaryReader
    {
        /// <summary>
        /// Deserializes the Adeptar string of type <see cref="Dictionary{TKey, TValue}"/> to a .NET object.
        /// </summary>
        /// <param name="text">The Adeptar string representation of the object.</param>
        /// <param name="type">The type of the <see cref="Dictionary{TKey, TValue}"/>.</param>
        /// <returns>The .NET version of the <see cref="Dictionary{TKey, TValue}"/>.</returns>
        internal static IDictionary DeserializeDictionary( ReadOnlySpan<char> text, Type type )
        {
            int level = 0;
            int i = 0;

            IDictionary motherDictionary = ( IDictionary ) Activator.CreateInstance( type );

            bool nested = false;
            bool inString = false;
            bool falseEnd = false;
            bool complexKey = false;

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
                    case '@':
                        if (!inString && !nested){
                            complexKey = true;
                        }
                        break;
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
                        if (!inString && !complexKey){
                            level++; nested = true;
                        }
                        else if (complexKey && !inString){
                            level++;
                        }
                        reader.Append( item );
                        break;
                    case ']':
                        if (level - 1 == 0 && !inString && complexKey)
                        {
                            complexKey = false;
                        }
                        else if (level - 1 == 0 && !inString){
                            nested = false;
                        }
                        else if (level - 1 == -1 && !inString && !complexKey){
                            if (i == text.Length - 1){
                                value = DeserializeObject( valueType, reader.ToString() );
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
                        if (!complexKey){
                            if (!nested && !inString){
                                value = DeserializeObject( valueType, reader.ToString() );
                                motherDictionary.Add( key, value );
                                reader.Clear();
                            }
                            else if (nested){
                                reader.Append( item );
                            }
                        }else{
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
                        if (!nested && !inString && !complexKey){
                            key = DeserializeObject( keyType, reader.ToString() );
                            reader.Clear();
                        }else{
                            reader.Append( item );
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