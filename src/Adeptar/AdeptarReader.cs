using System;
using static Adeptar.ArrayReader;
using static Adeptar.ClassReader;
using static Adeptar.DeserializationHelpers;
using static Adeptar.DictionaryReader;
using static Adeptar.TupleReader;
using static Adeptar.TypeGetters;

namespace Adeptar
{
    /// <summary>
    /// Internal central dispatcher for deserializing Adeptar objects from ReadOnlySpan.
    /// </summary>
    internal class AdeptarReader
    {
        /// <summary>
        /// Deserializes a ReadOnlySpan containing Adeptar data into an object of the specified type.
        /// Acts as the central dispatcher for deserialization logic based on the target type.
        /// </summary>
        /// <param name="type">The target <see cref="Type"/> to deserialize into.</param>
        /// <param name="text">The <see cref="ReadOnlySpan{Char}"/> containing the Adeptar representation of the object. Assumed to be the exact segment for the object (e.g., including surrounding quotes for strings, braces for objects, etc.).</param>
        /// <returns>The deserialized .NET object.</returns>
        /// <exception cref="AdeptarException">Thrown for format errors, type mismatches, or other deserialization issues.</exception>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="type"/> is null.</exception>
        internal static object DeserializeObject( Type type, ReadOnlySpan<char> text )
        {
            DeserializableType deserializableType = GetDeserializableType( type );

            return deserializableType switch
            {
                DeserializableType.Numeric => ParseNumeric( type, text.ToString() ),
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
                _ => throw new AdeptarException( $"Internal Error: Deserialization not implemented or type '{type.FullName}'." )
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