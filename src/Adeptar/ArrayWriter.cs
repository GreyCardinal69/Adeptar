using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using static Adeptar.AdeptarWriter;
using static Adeptar.TypeClassifiers;

namespace Adeptar
{
    /// <summary>
    /// Contains methods for serializing arrays, lists and 2 or more dimensional arrays into .Adeptar strings.
    /// </summary>
    internal sealed class ArrayWriter
    {
        /// <summary>
        /// Writes arrays and lists into a .Adeptar string, appends to the builder instance.
        /// </summary>
        /// <param name="target">The array of list to serialize.</param>
        /// <param name="indent">The amount of indentation.</param>
        /// <param name="builder">The <see cref="StringBuilder"/> instance to append to.</param>
        internal static void WriteArray( object target, int indent, StringBuilder builder )
        {
            if ( target is null )
            {
                return;
            }

            IList tempList = target as IList;
            SerializableType type = FetchSerializableTypeOf( tempList[0] );
            int count = tempList.Count;

            if ( !CurrentSettings.UseIndentation )
            {
                for ( int i = 0; i < count; i++ )
                {
                    if ( tempList[i] is IList )
                    {
                        if ( indent >= 1 )
                        {
                            builder.Append( '[' );
                        }
                        WriteArray( tempList[i], 0, builder );
                        builder.Append( ']' );
                        if ( i != count - 1 )
                        {
                            builder.Append( ',' );
                        }
                    }
                    else
                    {
                        WriteNoIndentation( tempList[i], type, builder, i == count - 1, false );
                    }
                }
            }
            else
            {
                bool isIntended = indent > 0;
                for ( int i = 0; i < count; i++ )
                {
                    if ( tempList[i] is IList )
                    {
                        for ( int w = 1; w <= indent; w++ )
                        {
                            builder.Append( '\t' );
                        }
                        if ( indent >= 1 )
                        {
                            builder.Append( '[' ).Append( '\n' );
                        }
                        WriteArray( tempList[i], indent + 2, builder );
                        for ( int w = 1; w <= indent; w++ )
                        {
                            builder.Append( '\t' );
                        }
                        builder.Append( ']' );
                        if ( i != count - 1 )
                        {
                            builder.Append( ',' );
                        }
                        builder.Append( '\n' );
                    }
                    else
                    {
                        Write( tempList[i], type, builder, isIntended ? indent - 1 : indent, i == count - 1, false );
                        builder.Append( '\n' );
                    }
                }
            }
        }

        /// <summary>
        /// Serializes 2 or more dimensional arrays by flattening them into a one dimensional array.
        /// </summary>
        /// <param name="target">The object to serialize.</param>
        /// <param name="indent">The amount of indentation.</param>
        /// <param name="builder">The <see cref="StringBuilder"/> to append to.</param>
        internal static void WriteDimensionalArray( object target, int indent, StringBuilder builder )
        {
            if ( target is null )
            {
                return;
            }

            Array array = target as Array;
            Stack<IEnumerator> stack = new();
            stack.Push( array.GetEnumerator() );
            int count = 0;
            int len = array.Length;

            if ( CurrentSettings.UseIndentation )
            {
                builder.Append( '\t' );
            }

            builder.Append( '<' );

            for ( int i = 0; i < array.Rank; i++ )
            {
                builder.Append( array.GetLength( i ) );
                if ( i < array.Rank - 1 )
                {
                    builder.Append( ',' );
                }
            }

            builder.Append( '>' );

            if ( CurrentSettings.UseIndentation )
            {
                for ( int i = 0; i < indent; i++ )
                {
                    builder.Append( '\n' );
                }
                do
                {
                    for ( IEnumerator iterator = stack.Pop(); iterator.MoveNext(); )
                    {
                        Write( iterator.Current, FetchSerializableTypeOf( iterator.Current ), builder, indent, count == len - 1, false );
                        for ( int i = 0; i < indent; i++ )
                        {
                            builder.Append( '\n' );
                        }
                        count++;
                    }
                }
                while ( stack.Count > 0 );
            }
            else
            {
                do
                {
                    for ( IEnumerator iterator = stack.Pop(); iterator.MoveNext(); )
                    {
                        WriteNoIndentation( iterator.Current, FetchSerializableTypeOf( iterator.Current ), builder, count == len - 1, false );
                        count++;
                    }
                }
                while ( stack.Count > 0 );
            }
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