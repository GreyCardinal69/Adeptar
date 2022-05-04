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

using static Adeptar.AdeptarWriter;
using static Adeptar.TypeGetters;
using static Adeptar.AdeptarReader;
using static Adeptar.DeserializationHelpers;
using System.IO;

namespace Adeptar
{
    /// <summary>
    /// Provides methods for converting between .NET and .Adeptar types
    /// </summary>
    public class AdeptarConverter
    {
        /// <summary>
        /// Deserializes the Adeptar string from the file at the specified path to the .NET <see cref="Type"/>.
        /// </summary>
        /// <param name="path">The path to the object.</param>
        public static T Deserialize<T> ( string path )
        {
            return ( T ) DeserializeObject( typeof( T ), File.ReadAllText( path ) );
        }

        /// <summary>
        /// Deserializes the Adeptar string from the file at the specified path to the .NET <see cref="Type"/>.
        /// </summary>
        /// <param name="path">The path to the object.</param>
        /// <param name="type">The <see cref="Type"/> of the .NET object.</param>
        public static object Deserialize ( string path, Type type )
        {
            return DeserializeObject( type, File.ReadAllText( path ) );
        }

        /// <summary>
        /// Deserializes the Adeptar string to the specified .NET type.
        /// </summary>
        /// <param name="content">The Adeptar string.</param>
        public static T DeserializeString<T> ( string content )
        {
            return ( T ) DeserializeObject( typeof( T ), CleanText( content ) );
        }

        /// <summary>
        /// Deserializes the Adeptar string to the specified .NET <see cref="Type"/>.
        /// </summary>
        /// <param name="content">The Adeptar string.</param>
        /// <param name="type">The <see cref="Type"/> of the .NET object.</param>
        /// <returns></returns>
        public static object DeserializeString ( string content, Type type )
        {
            return DeserializeObject( type, CleanText( content ) );
        }

        /// <summary>
        ///
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="path"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public static T DeserializeAppended<T> ( string path, string id )
        {
            throw new NotImplementedException();
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="content"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public static object DeserializeAppended ( string content, Type type )
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Serializes the object to a .Adeptar string and writes it to a file, the file is overwritten.
        /// </summary>
        /// <param name="path">The path of the file.</param>
        /// <param name="toSerialize">The object to serialize.</param>
        public static void SerializeWrite ( string path, object toSerialize )
        {
            DoesntUseIndentation = false;
            AdeptarWriter.SerializeWrite( path, toSerialize, FetchType(toSerialize), SerializationMode.Default );
        }

        /// <summary>
        /// Serializes the object to a .Adeptar string using the provided formatting, and writes it to a file, the file is overwritten.
        /// </summary>
        /// <param name="path">The path of the file.</param>
        /// <param name="toSerialize">The object to serialize.</param>
        /// <param name="formatting">The formatting style.</param>
        public static void SerializeWrite ( string path, object toSerialize, Formatting formatting = Formatting.Indented )
        {
            if (formatting == Formatting.NoIndentation){
                DoesntUseIndentation = true;
            }
            AdeptarWriter.SerializeWrite( path, toSerialize, FetchType( toSerialize ), SerializationMode.Default );
        }

        /// <summary>
        /// Serializes the object to a .Adeptar string.
        /// </summary>
        /// <param name="toSerialize">The object to serialize.</param>
        /// <returns>A .Adeptar representation of the object.</returns>
        public static string Serialize ( object toSerialize )
        {
            DoesntUseIndentation = false;
            return AdeptarWriter.Serialize( toSerialize, FetchType( toSerialize ) );
        }

        /// <summary>
        /// Serializes the object to a .Adeptar string with specified formatting.
        /// </summary>
        /// <param name="toSerialize">The object to serialize.</param>
        /// <param name="formatting">The formatting style.</param>
        /// <returns>A .Adeptar representation of the object with the provided formatting style.</returns>
        public static string Serialize ( object toSerialize, Formatting formatting = Formatting.Indented )
        {
            if (formatting == Formatting.NoIndentation){
                DoesntUseIndentation = true;
            }
            return AdeptarWriter.Serialize( toSerialize, FetchType( toSerialize ) );
        }

        /// <summary>
        /// Serializes the object to a .Adeptar string using specified formatting style, and appends it to a file.
        /// If an object with the provided id already exists will throw an exception.
        /// The id is limited to numbers and letters.
        /// </summary>
        /// <param name="path">The path to the file to append the object to.</param>
        /// <param name="toSerialize">The object to serialize.</param>
        /// <param name="id">The id of the object used to append it.</param>
        /// <param name="formatting">The formatting style.</param>
        public static void SerializeAppend ( string path, object toSerialize, string id, Formatting formatting = Formatting.Indented )
        {
            if (formatting == Formatting.NoIndentation){
                DoesntUseIndentation = true;
            }
            AdeptarWriter.SerializeWrite( path, toSerialize, FetchType( toSerialize ), SerializationMode.Append, id );
        }

        /// <summary>
        /// Serializes the object to a .Adeptar string using specified formatting style, and appends it to a file.
        /// The current object with the provided id is rewritten in the file.
        /// The id is limited to numbers and letters.
        /// </summary>
        /// <param name="path">The path of the file.</param>
        /// <param name="toSerialize">The object to serialize.</param>
        /// <param name="id">The id of the object used to append it.</param>
        /// <param name="formatting">The formatting style.</param>
        public static void SerializeRewriteAppended ( string path, object toSerialize, string id, Formatting formatting = Formatting.Indented )
        {
            if (formatting == Formatting.NoIndentation){
                DoesntUseIndentation = true;
            }
            AdeptarWriter.SerializeWrite( path, toSerialize, FetchType( toSerialize ), SerializationMode.ChangeAppended, id );
        }
    }
}