using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

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
            if (text.Length == 2)
                return ( IList ) Array.CreateInstance( type.GetElementType(), 0 );

            bool inString = false;
            int length = 0;
            int index = 0;

            double firstCase = 0;
            bool nested = false;
            bool falseEnd = false;

            int j = 0;
            int i = 0;
            bool start = false;

            foreach (char Char in text)
            {
                switch (Char)
                {
                    case '\\':
                        if (inString){
                            falseEnd = true;
                        }else{
                            throw new AdeptarException("Invalid character '\\', such a character can appear only inside a string.");
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
                    case '{':
                        if (inString)
                            continue;
                        firstCase++;
                        nested = true;
                        break;
                    case '}':
                        if (inString)
                            continue;
                        firstCase--;
                        if (firstCase == 0){
                            nested = false;
                        }
                        break;
                    case '[':
                        if (inString)
                            continue;
                        if (firstCase == 0 && !start){
                            start = true;
                            continue;
                        }
                        firstCase++;
                        nested = true;
                        break;
                    case ']':
                        if (inString)
                            continue;
                        if (firstCase - 1 == -1){
                            nested = false;
                            length++;
                        }else{
                            firstCase--;
                        }
                        break;
                    case '(':
                        if (inString)
                            continue;
                        firstCase++;
                        nested = true;
                        break;
                    case ')':
                        if (inString)
                            continue;
                        firstCase--;
                        if (firstCase == 0){
                            nested = false;
                        }
                        break;
                    case ',':
                        if (inString)
                            continue;
                        if (!nested){
                            length++;
                        }
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
                    case ':':
                        break;
                    case '\'':
                        break;
                    case '\\':
                        if (inString){
                            falseEnd = true;
                        }else{
                            throw new AdeptarException( "Invalid character '\\', such a character can appear only inside a string." );
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
                            firstCase++;
                            nested = true;
                        }
                        break;
                    case ']':
                        if (firstCase - 1 == 0 && !inString){
                            firstCase--;
                        }
                        else if (firstCase - 1 == -1 && !inString){
                            firstCase--;
                            if (i == text.Length - 1){
                                main[index] = DeserializeObject( childType, text.Slice( j, i - j ) );
                            }
                        }else{
                            firstCase--;
                        }
                        break;
                    case ',':
                        if (!inString && !nested){
                            main[index] = DeserializeObject( childType, text.Slice( j, i - j ) );
                            j = i + 1;
                            index++;
                        }
                        break;
                    case '{':
                        if (!inString){
                            firstCase++;
                            nested = true;
                        }
                        break;
                    case '}':
                        if (firstCase - 1 == 0 && !inString){
                            firstCase--;
                            nested = false;
                        }else{
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
            if (text.Length == 2)
                return ( IList ) Array.CreateInstance( type.GetElementType(), 0 );

            bool inString = false;

            double level = 0;
            bool nested = false;
            bool falseEnd = false;

            int i = 0;
            int j = 0;

            Type childType = type.GetGenericArguments()[0];
            var main = ( IList ) Activator.CreateInstance( type );

            text = text.Slice( 1, text.Length - 1 );

            foreach (var Char in text)
            {
                switch (Char)
                {
                    case ':':
                        break;
                    case '\'':
                        break;
                    case '\\':
                        if (inString)
                            falseEnd = true;
                        else
                            throw new AdeptarException( "Invalid character '\\', such a character can appear only inside a string." );
                        break;
                    case '"':
                        if (falseEnd && !nested){
                            falseEnd = false;
                            continue;
                        }
                        if (!nested)
                            inString = !inString;
                        break;
                    case '[':
                        if (!inString){
                            level++;
                            nested = true;
                        }
                        break;
                    case ']':
                        if (level - 1 == 0 && !inString){
                            level--;
                            nested = false;
                        }
                        else if (level - 1 == - 1 && !inString){
                            level--;
                            if (i == text.Length - 1){
                                main.Add( DeserializeObject( childType, text.Slice( j, i - j ) ) );
                            }
                        }else{
                            level--;
                        }
                        break;
                    case ',':
                        if (!inString && !nested){
                            main.Add( DeserializeObject( childType, text.Slice( j, i - j ) ) );
                            j = i + 1;
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
                            level--;
                            nested = false;
                        }else{
                            level--;
                        }
                        break;
                    case '(':
                        if (!inString){
                            level++; nested = true;
                        }
                        break;
                    case ')':
                        if (level - 1 == 0 && !inString){
                            nested = false;
                            level--;
                        }
                        break;
                }
                i++;
            }

            return main;
        }

        /// <summary>
        /// Deserializes the Adeptar string of two or more dimensional arrays to a .NET object.
        /// </summary>
        /// <param name="text">The Adeptar string representation of the object.</param>
        /// <param name="type">The type of the dimensional array.</param>
        /// <returns>The .NET version of the dimensional array.</returns>
        internal static object DeserializeDimensionalArray( ReadOnlySpan<char> text, Type type )
        {
            List<int> sizes = new();
            bool inSizes = false;
            bool exit = false;

            int i = 0, j = 0;

            foreach (var item in text)
            {
                if (exit)
                    break;
                switch (item)
                {
                    case '<':
                        inSizes = true;
                        j = i + 1;
                        break;
                    case '>':
                        if (inSizes){
                            sizes.Add( ( int ) NumericResolver( IntType, text.Slice( j, i - j ).ToString() ) );
                            j = i + 1;
                            inSizes = false;
                            exit = true;
                        }
                        break;
                    case ',':
                        sizes.Add( ( int ) NumericResolver( IntType, text.Slice( j, i - j ).ToString() ) );
                        j = i + 1;
                        break;
                }
                i++;
            }

            text = text.Slice( j );

            var elementType = type.GetElementType();

            IList flat =  DeserializeArray( $"[{text.ToString()}", elementType.MakeArrayType() );
            Array main = Array.CreateInstance( elementType, sizes.ToArray() );

            int[] index = new int[sizes.Count];

            for (int e = 0; e < sizes.Count; e++)
            {
                sizes[e] = sizes[e] - 1;
            }

            for (int w = 0; w < flat.Count; w++)
            {
                main.SetValue( flat[w], index );
                BinaryStyleIndexArrayByRefIncrement( in sizes, ref index );
            }

            return main;
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