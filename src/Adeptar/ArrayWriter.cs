using System;
using System.Collections;
using System.Globalization;
using static Adeptar.AdeptarSerializer;
using static Adeptar.TypeClassifiers;

namespace Adeptar
{
    /// <summary>
    /// Serializes IList (arrays, lists) and multi-dimensional Array instances
    /// into their Adeptar string representations.
    /// </summary>
    internal static class ArrayWriter
    {
        /// <summary>
        /// Writes the elements of an IList (like T[] or ListT).
        /// </summary>
        /// <param name="list">The IList containing the elements to serialize.</param>
        /// <param name="context">The current serialization context.</param>
        internal static void WriteListElements( IList list, ref SerializationContext context )
        {
            if ( list == null || list.Count == 0 ) return;

            int count = list.Count;
            for ( int i = 0; i < count; i++ )
            {
                object element = list[i];
                bool isLastElement = (i == count - 1);

                SerializableType elementType = FetchSerializableTypeOf(element);
                Write( element, elementType, null, ref context, isLastElement, false );
            }
        }

        /// <summary>
        /// Writes a multi-dimensional array..
        /// Handles the dimensions prefix and flattening the elements.
        /// </summary>
        /// <param name="array">The multi-dimensional <see cref="Array"/> to serialize.</param>
        /// <param name="context">The current serialization context.</param>
        internal static void WriteDimensionalArray( object array, ref SerializationContext context )
        {
            Array mdArray = array as Array;

            if ( mdArray == null || mdArray.Length == 0 )
            {
                return;
            }

            int rank = mdArray.Rank;
            // format
            // <i1, i2....in>element1, element2....
            context.Builder.Append( '<' );
            for ( int i = 0; i < rank; i++ )
            {
                context.Builder.Append( mdArray.GetLength( i ).ToString( CultureInfo.InvariantCulture ) );
                if ( i < rank - 1 )
                {
                    context.Builder.Append( ',' );
                }
            }
            context.Builder.Append( '>' );

            if ( context.Settings.UseIndentation ) context.Builder.Append( '\n' );

            int elementCount = mdArray.Length;
            int currentElementIndex = 0;

            foreach ( var element in mdArray )
            {
                currentElementIndex++;
                bool isLastElement = (currentElementIndex == elementCount);
                SerializableType elementType = FetchSerializableTypeOf(element);
                Write( element, elementType, null, ref context, isLastElement, false );
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