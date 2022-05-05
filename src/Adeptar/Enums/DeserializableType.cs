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

using System.Collections.Generic;

namespace Adeptar
{
    /// <summary>
    /// Keeps track of object types, used in deserialization code.
    /// </summary>
    internal enum DeserializableType
    {
        /// <summary>
        /// True for numerical objects.
        /// </summary>
        Numeric,
        /// <summary>
        /// True for objects of type <see cref="System.String"/>.
        /// </summary>
        String,
        /// <summary>
        /// True for objects of type <see cref="System.Boolean"/>.
        /// </summary>
        Boolean,
        /// <summary>
        /// True for objects of type <see cref="System.Char"/>.
        /// </summary>
        Char,
        /// <summary>
        /// True for objects of type class and struct.
        /// </summary>
        Class,
        /// <summary>
        /// True for objects of type<see cref="System.Array"/>.
        /// </summary>
        Array,
        /// <summary>
        /// True for objects of type <see cref="List{T}"/>.
        /// </summary>
        List,
        /// <summary>
        /// True for objects of type <see cref="Dictionary{TKey, TValue}"/>.
        /// </summary>
        Dictionary,
        /// <summary>
        /// True for objects of type <see cref="System.ValueTuple"/>.
        /// </summary>
        Tuple,
        /// <summary>
        /// True for <see cref="System.Enum"/>s.
        /// </summary>
        Enum,
        /// <summary>
        /// Represents an unknown type.
        /// </summary>
        NULL,
        /// <summary>
        /// True for two or more dimensional arrays.
        /// </summary>
        DimensionalArray,
        /// <summary>
        /// True for objects of type <see cref="System.DateTime"/>.
        /// </summary>
        DateTime
    }
}