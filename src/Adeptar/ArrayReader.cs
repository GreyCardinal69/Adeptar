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

using static Adeptar.TypeGetters;
using static Adeptar.DeserializationHelpers;
using static Adeptar.AdeptarReader;

namespace Adeptar
{
    /// <summary>
    /// Internal class containing method(s) for deserialization of <see cref="List{T}"/>, <see cref="Array"/>
    /// and two or more dimensional arrays.
    /// </summary>
    internal class ArrayReader
    {
        /// <summary>
        /// Deserializes the Adeptar string of type <see cref="Array"/> to a .NET object.
        /// </summary>
        /// <param name="text">The Adeptar string representation of the object.</param>
        /// <param name="type">The type of the <see cref="Array"/>.</param>
        /// <returns>The .NET version of the <see cref="Array"/>.</returns>
        internal static IList DeserializeArray( ReadOnlySpan<char> text, Type type )
        {
            bool inString = false;
            double length = 0;
            int index = 0;

            double firstCase = 0;
            bool nested = false;
            bool falseEnd = false;

            int j = 0;
            int i = 0;

            foreach (char Char in text)
            {
                switch (Char)
                {
                    case '\\':
                        if (inString){
                            falseEnd = true;
                        }
                        break;
                    case '"':
                        if (falseEnd && !nested){
                            falseEnd = false;
                            break;
                        }
                        if (!nested){
                            inString = !inString;
                        }
                        break;
                    case ',':
                        if (!nested && !inString){
                            length++;
                        }
                        break;
                    case '[':
                        if (firstCase == 0){
                            firstCase++;
                            continue;
                        }
                        else if (!nested){
                            firstCase++;
                            nested = true;
                        }
                        break;
                    case ']':
                        if (firstCase - 1 == 1){
                            nested = false;
                            firstCase--;
                        }
                        else if (firstCase - 1 == 0){
                            length++;
                        }
                        break;
                    case '{':
                        if (!nested){
                            firstCase++;
                            nested = true;
                        }
                        break;
                    case '}':
                        if (firstCase - 1 == 1){
                            nested = false;
                            firstCase--;
                        }
                        break;
                    case '(':
                        firstCase++;
                        nested = true;
                        break;
                    case ')':
                        if (firstCase - 1 == 1){
                            nested = false;
                            firstCase--;
                        }
                        break;
                    default:
                        break;
                }
            }

            Type childType = type.GetElementType();
            var main = ( IList ) Array.CreateInstance( childType, ( int ) length );

            inString = false;
            nested = false;
            firstCase = 0;

            text = text.Slice(  1, text.Length - 1 );

            foreach (var Char in text)
            {
                switch (Char)
                {
                    case '\\':
                        if (inString){
                            falseEnd = true;
                        }
                        break;
                    case '"':
                        if (falseEnd && !nested){
                            falseEnd = false;
                            break;
                        }
                        if (!nested){
                            inString = !inString;
                        }
                        break;
                    case '[':
                        if (!inString){
                            firstCase++; nested = true;
                        }
                        break;
                    case ']':
                        if (firstCase - 1 == 0 && !inString){
                            nested = false;
                            firstCase--;
                        }
                        else if (firstCase - 1 == -1 && !inString){
                            firstCase--;
                            if (i == text.Length - 1){
                                main[index] = DeserializeObject( childType, text.Slice( j, i - j ) );
                            }
                        }
                        break;
                    case ',':
                        if (!inString && !nested){
                            main[index] = DeserializeObject( childType, text.Slice(j, i - j ) );
                            j = i+1;
                            index++;
                        }
                        break;
                    case '{':
                        if (!inString){
                            firstCase++; nested = true;
                        }
                        break;
                    case '}':
                        if (firstCase - 1 == 0 && !inString){
                            nested = false;
                            firstCase--;
                        }
                        break;
                    case '(':
                        if (!inString){
                            firstCase++; nested = true;
                        }
                        break;
                    case ')':
                        if (firstCase - 1 == 0 && !inString){
                            nested = false;
                            firstCase--;
                        }
                        break;
                    default:
                        break;
                }
                i++;
            }

            return main;
        }

        /// <summary>
        /// Deserializes the Adeptar string of type <see cref="List{T}"/> to a .NET object.
        /// </summary>
        /// <param name="text">The Adeptar string representation of the object.</param>
        /// <param name="type">The type of the <see cref="List{T}"/>.</param>
        /// <returns>The .NET version of the <see cref="List{T}"/>.</returns>
        internal static IList DeserializeList ( ReadOnlySpan<char> text, Type type )
        {
            bool inString = false;

            double firstCase = 0;
            bool nested = false;
            bool falseEnd = false;

            int i = 0;

            Type childType = type.GetGenericArguments()[0];
            var main = ( IList ) Activator.CreateInstance( type );

            StringBuilder value = new();

            text = text.Slice( 1, text.Length - 1 );

            foreach (var Char in text)
            {
                switch (Char)
                {
                    case '\\':
                        if (inString)
                        {
                            falseEnd = true;
                        }
                        break;
                    case '"':
                        if (falseEnd && !nested)
                        {
                            value.Append( Char );
                            falseEnd = false;
                            break;
                        }
                        if (!nested)
                        {
                            inString = !inString;
                        }
                        break;
                    case '[':
                        if (!inString)
                        {
                            firstCase++; nested = true;
                        }
                        value.Append( Char );
                        break;
                    case ']':
                        if (firstCase - 1 == 0 && !inString)
                        {
                            nested = false;
                            firstCase--;
                        }
                        else if (firstCase - 1 == -1 && !inString)
                        {
                            firstCase--;
                            if (i == text.Length - 1)
                            {
                                main.Add( DeserializeObject( childType, value.ToString() ) );
                                value.Clear();
                            }
                        }
                        value.Append( Char );
                        break;
                    case ',':
                        if (!inString && !nested)
                        {
                            main.Add( DeserializeObject( childType, value.ToString() ) );
                            value.Clear();
                        }
                        else
                        {
                            value.Append( Char );
                        }
                        break;
                    case '{':
                        if (!inString)
                        {
                            firstCase++; nested = true;
                        }
                        value.Append( Char );
                        break;
                    case '}':
                        if (firstCase - 1 == 0 && !inString)
                        {
                            nested = false;
                            firstCase--;
                        }
                        value.Append( Char );
                        break;
                    case '(':
                        if (!inString)
                        {
                            firstCase++; nested = true;
                        }
                        value.Append( Char );
                        break;
                    case ')':
                        if (firstCase - 1 == 0 && !inString)
                        {
                            nested = false;
                            firstCase--;
                        }
                        value.Append( Char );
                        break;
                    default:
                        value.Append( Char );
                        break;
                }
                i++;
            }

            return main;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="target"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        internal static object DeserializeDimensionalArray( ReadOnlySpan<char> text, Type type )
        {
            return null;
        }
    }
}