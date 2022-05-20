﻿#region License
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
    /// <summary>
    /// Internal class containing method(s) for deserialization of class or struct objects.
    /// and two or more dimensional arrays.
    /// </summary>
    internal class ClassReader
    {
        /// <summary>
        /// Deserializes the Adeptar string of a class or a struct to a .NET object.
        /// </summary>
        /// <param name="text">The Adeptar string representation of the object.</param>
        /// <param name="type">The type of the class or struct.</param>
        /// <returns>The .NET version of the class or struct.</returns>
        internal static object DeserializeClassStruct ( ReadOnlySpan<char> text, Type type )
        {
            int level = 0;
            int i = 0;
            int w = 0;
            int j = 0;

            var target = Activator.CreateInstance( type );

            var accessor = TypeAccessor.Create( type, true );
            var members = accessor.GetMembers();

            List<String> ids = new(members.Count);

            foreach (var item in members)
            {
                ids.Add( item.Name );
            }

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
                        if (falseEnd && inString){
                            falseEnd = false;
                        }
                        else if (!falseEnd)
                            inString = !inString;
                        break;
                    case '[':
                        if (!inString){
                            level++; nested = true;
                        }
                        else if (!inString)
                            level++;
                        break;
                    case ']':
                        if (level - 1 == 0 && !inString){
                            nested = false;
                        }
                        level--;
                        break;
                    case '\\':
                        if (inString){
                            falseEnd = true;
                        }else{
                            throw new AdeptarException("Invalid character '\\', such a character can appear only inside a string.");
                        }
                        break;
                    case ',':
                        if (!nested && !inString){
                            if (ids.Contains( name )){
                                accessor[target, name] = DeserializeObject( members[i].Type, text.Slice( j, w - j ) );
                                i++;
                            }
                            j = w + 1;
                        }
                        break;
                    case '{':
                        if (!inString){
                            level++;
                            nested = true;
                        }
                        break;
                    case '}':
                        if (level - 1 == 0 && !inString){
                            nested = false;
                        }
                        else if (level - 1 == -1 && !inString && w == text.Length - 1 && members.Count > 0 && ids.Contains( name )){
                            accessor[target, name] = DeserializeObject( members[i].Type, text.Slice( j, w - j ) );
                        }
                        level--;
                        break;
                    case '(':
                        if (!inString){
                            level++;
                            nested = true;
                        }
                        break;
                    case ')':
                        if (level - 1 == 0 && !inString){
                            nested = false;
                            level--;
                        }
                        break;
                    case ':':
                        if (!nested && !inString){
                            name = text.Slice( j, w - j ).ToString();
                            j = w + 1;
                        }
                        break;
                }
                w++;
            }

            return target;
        }

        /// <summary>
        /// Deserializes the Adeptar string of a class or a struct to a .NET object. Accepts a <see cref="Dictionary{TKey, TValue}"/> map
        /// that is used for name mapping.
        /// </summary>
        /// <param name="text">The Adeptar string representation of the object.</param>
        /// <param name="type">The type of the class or struct.</param>
        /// <param name="map">The map to use for names.</param>
        /// <returns>The .NET version of the class or struct.</returns>
        internal static object DeserializeClassStructWithMap ( ReadOnlySpan<char> text, Type type, Dictionary<string, string> map )
        {
            int level = 0;
            int i = 0;
            int w = 0;
            int j = 0;

            var target = Activator.CreateInstance( type );

            var accessor = TypeAccessor.Create( type, true );
            var members = accessor.GetMembers();

            List<String> ids = new( members.Count );

            foreach (var item in members)
            {
                ids.Add( item.Name );
            }

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
                        if (falseEnd && inString){
                            falseEnd = false;
                        }
                        else if (!falseEnd)
                            inString = !inString;
                        break;
                    case '[':
                        if (!inString){
                            level++; nested = true;
                        }
                        else if (!inString)
                            level++;
                        break;
                    case ']':
                        if (level - 1 == 0 && !inString){
                            nested = false;
                        }
                        level--;
                        break;
                    case '\\':
                        if (inString){
                            falseEnd = true;
                        }else{
                            throw new AdeptarException( "Invalid character '\\', such a character can appear only inside a string." );
                        }
                        break;
                    case ',':
                        if (!nested && !inString){
                            if (ids.Contains( map[name] )){
                                accessor[target, map[name]] = DeserializeObject( members[ids.IndexOf( map[name] )].Type, text.Slice( j, w - j ) );
                                i++;
                            }
                            j = w + 1;
                        }
                        break;
                    case '{':
                        if (!inString){
                            level++;
                            nested = true;
                        }
                        break;
                    case '}':
                        if (level - 1 == 0 && !inString){
                            nested = false;
                        }
                        else if (level - 1 == -1 && !inString && w == text.Length - 1 && members.Count > 0 && ids.Contains( map[name] )){
                            accessor[target, map[name]] = DeserializeObject( members[ids.IndexOf( map[name] )].Type, text.Slice( j, w - j ) );
                        }
                        level--;
                        break;
                    case '(':
                        if (!inString){
                            level++;
                            nested = true;
                        }
                        break;
                    case ')':
                        if (level - 1 == 0 && !inString){
                            nested = false;
                            level--;
                        }
                        break;
                    case ':':
                        if (!nested && !inString){
                            name = text.Slice( j, w - j ).ToString();
                            j = w + 1;
                        }
                        break;
                }
                w++;
            }

            return target;
        }
    }
}