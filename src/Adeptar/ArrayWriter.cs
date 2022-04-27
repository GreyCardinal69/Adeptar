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
using System.Text;
using System.Collections.Generic;

using static Adeptar.TypeGetters;
using static Adeptar.AdeptarWriter;

namespace Adeptar
{
    /// <summary>
    /// Contains methods for serializing arrays, lists and 2 or more dimensional arrays into .Adeptar strings.
    /// </summary>
    internal class ArrayWriter
    {
        /// <summary>
        /// Writes arrays and lists into a .Adeptar string, appends to the builder instance.
        /// </summary>
        /// <param name="target">The array of list to serialize.</param>
        /// <param name="indent">The amount of indentation.</param>
        /// <param name="builder">The <see cref="StringBuilder"/> instance to append to.</param>
        internal static void WriteArray ( object target, int indent, ref StringBuilder builder )
        {
            if (DoesntUseIndentation){
                int count = 0;
                bool isIntended = indent > 0;
                IList tempList = target as IList;
                for (int i = 0; i < tempList.Count; i++)
                {
                    if ( tempList[i] is IList ){
                        if (indent >= 1){
                            builder.Append( '[' );
                        }
                        WriteArray( tempList[i], indent + 1, ref builder );
                        builder.Append( ']' );
                        if (count != tempList.Count - 1){
                            builder.Append( ',' );
                        }
                    }else{
                        Write( tempList[i], FetchType( tempList[i] ), ref builder, null, isIntended ? indent - 1 : indent, false, false, count == tempList.Count - 1 );
                    }
                    count++;
                }
            }else{
                int count = 0;
                bool isIntended = indent > 0;
                IList tempList = target as IList;
                for (int i = 0; i < tempList.Count; i++)
                {
                    if (tempList[i] is IList){
                        for (int w = 1; w <= indent; w++)
                        {
                            builder.Append( '\t' );
                        }
                        if (indent >= 1){
                            builder.Append( '[' ).Append( '\n' );
                        }
                        WriteArray( tempList[i], indent + 2, ref builder );
                        for (int w = 1; w <= indent; w++)
                        {
                            builder.Append( '\t' );
                        }
                        builder.Append( ']' );
                        if (count != tempList.Count - 1){
                            builder.Append( ',' );
                        }
                        builder.Append( '\n' );
                    }else{
                        Write( tempList[i], FetchType( tempList[i] ), ref builder, null, isIntended ? indent - 1 : indent, false, false, count == tempList.Count - 1 );
                        builder.Append( '\n' );
                    }
                    count++;
                }
            }
        }

        /// <summary>
        /// Serializes 2 or more dimensional arrays by flattening them into a one dimensional array.
        /// </summary>
        /// <param name="target">The object to serialize.</param>
        /// <param name="indent">The amount of indentation.</param>
        /// <param name="builder">The <see cref="StringBuilder"/> to append to.</param>
        internal static void WriteDimensionalArray ( object target, int indent, ref StringBuilder builder )
        {
            Array array = target as Array;
            Stack<IEnumerator> stack = new();
            stack.Push( array.GetEnumerator() );
            int count = 0;
            int len = array.Length;
            if (!DoesntUseIndentation){
                builder.Append( '\t' );
            }
            builder.Append( '<' );
            for (int i = 0; i < array.Rank; i ++)
            {
                builder.Append( array.GetLength( i ) );
                if (i < array.Rank - 1){
                    builder.Append ( ',' );
                }
            }
            builder.Append( '>' );
            if (!DoesntUseIndentation){
                builder.Append( ' ' );
            }
            do
            {
                for (var iterator = stack.Pop(); iterator.MoveNext();)
                {
                    Write( iterator.Current, FetchType( iterator.Current ),
                        ref builder, null, DoesntUseIndentation ? 0 : indent, false, false, count == len - 1 );
                    count++;
                }
            }
            while (stack.Count > 0);
        }
    }
}