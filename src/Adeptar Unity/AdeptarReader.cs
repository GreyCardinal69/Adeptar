using System;
using static Adeptar.Unity.ArrayReader;
using static Adeptar.Unity.ClassReader;
using static Adeptar.Unity.DeserializationHelpers;
using static Adeptar.Unity.DictionaryReader;
using static Adeptar.Unity.TupleReader;
using static Adeptar.Unity.TypeGetters;

namespace Adeptar.Unity
{
    /// <summary>
    /// A class that contains method(s) for deserializing .Adeptar objects.
    /// </summary>
    internal class AdeptarReader
    {
        /// <summary>
        /// Serves as a "main" node that coordinates deserialization of elements.
        /// </summary>
        /// <param name="type">The type of the object.</param>
        /// <param name="text">The .Adeptar string representation of the object.</param>
        /// <returns>The deserialized .NET object.</returns>
        internal static object DeserializeObject( Type type, ReadOnlySpan<char> text )
        {
            DeserializableType deserializableType = GetDeserializableType( type );

            return deserializableType switch
            {
                DeserializableType.Numeric => NumericResolver( type, text.ToString() ),
                DeserializableType.String => StringResolver( text ),
                DeserializableType.Boolean => Convert.ToBoolean( text.ToString() ),
                DeserializableType.Char => Convert.ToChar( text.Slice( 1, 1 ).ToString() ),
                DeserializableType.Enum => ParseEnum( type, text ),
                DeserializableType.NULL => null,
                DeserializableType.DateTime => DateTime.Parse( StringResolver( text ) ),
                DeserializableType.Class => DeserializeClassStruct( text, type ),
                DeserializableType.Array => DeserializeArray( text, type ),
                DeserializableType.List => DeserializeList( text, type ),
                DeserializableType.Dictionary => DeserializeDictionary( text, type ),
                DeserializableType.DimensionalArray => DeserializeDimensionalArray( text, type ),
                DeserializableType.Tuple => DeserializeTuple( text, type ),
                _ => throw new NotImplementedException(),
            };
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