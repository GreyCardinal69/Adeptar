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

using static Adeptar.TypeGetters;
using static Adeptar.DeserializationHelpers;
using static Adeptar.DictionaryReader;
using static Adeptar.TupleReader;
using static Adeptar.ClassReader;
using static Adeptar.ArrayReader;

namespace Adeptar
{
    /// <summary>
    ///
    /// </summary>
    internal class AdeptarReader
    {
        /// <summary>
        ///
        /// </summary>
        /// <param name="type"></param>
        /// <param name="text"></param>
        /// <returns></returns>
        internal static object DeserializeByChar ( Type type, ReadOnlySpan<char> text )
        {
            DeserializableType deserializableType = GetDeserializableType( type );

            return deserializableType switch
            {
                DeserializableType.Numeric => NumericResolver( type, text.ToString() ),
                DeserializableType.String => text.ToString(),
                DeserializableType.Boolean => Convert.ToBoolean( text.ToString() ),
                DeserializableType.Char => Convert.ToChar( text.Slice(1,1).ToString() ),
                DeserializableType.Enum => ParseToEnumNonGeneric( text.ToString(), type ),
                DeserializableType.NULL => null,
                DeserializableType.DateTime => DateTime.Parse( text.ToString() ),
            //    DeserializableType.Class => ByCharClassStruct( text, type ),
                DeserializableType.Array => DeserializeArray( text, type ),
            //    DeserializableType.List => ByCharList( text, type ),
            //    DeserializableType.Dictionary => ByCharDictionary( text, type ),
           ///     DeserializableType.Tuple => ByCharTuple( text, type ),
           ////     DeserializableType.DimensionalArray2D => ByCharTwoDimensional( text, type ),
           //     DeserializableType.DimensionalArray3D => ByCharThreeDimensional( text, type ),
                _ => throw new NotImplementedException(),
            };





        }
    }
}