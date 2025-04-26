using System;
using System.IO;
using static Adeptar.AdeptarReader;
using static Adeptar.DeserializationHelpers;
using static Adeptar.TypeGetters;

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
        public static T Deserialize<T>( string path ) => (T)DeserializeObject( typeof( T ), CleanText( File.ReadAllText( path ) ) );

        /// <summary>
        /// Deserializes the Adeptar string from the file at the specified path to the .NET <see cref="Type"/>.
        /// </summary>
        /// <param name="path">The path to the object.</param>
        /// <param name="type">The <see cref="Type"/> of the .NET object.</param>
        public static object Deserialize( string path, Type type ) => DeserializeObject( type, CleanText( File.ReadAllText( path ) ) );

        /// <summary>
        /// Deserializes the Adeptar string to the specified .NET type.
        /// </summary>
        /// <param name="content">The Adeptar string.</param>
        public static T DeserializeString<T>( string content ) => (T)DeserializeObject( typeof( T ), CleanText( content ) );

        /// <summary>
        /// Deserializes the Adeptar string to the specified .NET <see cref="Type"/>.
        /// </summary>
        /// <param name="content">The Adeptar string.</param>
        /// <param name="type">The <see cref="Type"/> of the .NET object.</param>
        /// <returns></returns>
        public static object DeserializeString( string content, Type type ) => DeserializeObject( type, CleanText( content ) );

        /// <summary>
        /// Deserializes an object serialized with the ID feature. Accepts a generic <typeparamref name="T"/> type.
        /// </summary>
        /// <typeparam name="T">The generic type to deserialize to.</typeparam>
        /// <param name="path">The path to the file where the object is serialized.</param>
        /// <param name="id">The id used to serialize the object with.</param>
        /// <returns>The deserialized .NET object.</returns>
        public static T DeserializeAppended<T>( string path, string id ) => (T)DeserializeObject( typeof( T ), FetchAppendedSegment( File.ReadAllText( path ), id, 0 ) );

        /// <summary>
        /// Deserializes an object serialized with the ID feature. Accepts <see cref="Type"/>.
        /// </summary>
        /// <param name="path">The path to the file where the object is serialized.</param>
        /// <param name="type">The type of the object.</param>
        /// <param name="id">The id used to serialize the object with.</param>
        /// <returns>The deserialized .NET object.</returns>
        public static object DeserializeAppended( string path, Type type, string id ) => DeserializeObject( type, FetchAppendedSegment( File.ReadAllText( path ), id, 0 ) );

        /// <summary>
        /// Deserializes an object serialized with the ID feature using shared data of the object pool to override
        /// field and property values on deserialization.
        /// </summary>
        /// <param name="path">The path to the file where the object is serialized.</param>
        /// <param name="id">The id used to serialize the object with.</param>
        /// <returns>The deserialized .NET object.</returns>
        public static T DeserializeAppendedWithSharedData<T>( string path, string id )
        {
            ReadOnlySpan<char> text = CleanText(File.ReadAllText( path ));

            bool inString = false;
            bool falseEnd = false;
            bool inSharedData = false;
            bool exit = false;
            int i = 0;
            int index = -1;

            foreach ( char item in text )
            {
                index++;
                if ( exit )
                {
                    break;
                }
                switch ( item )
                {
                    case '"':
                        if ( falseEnd && inString )
                            falseEnd = false;
                        else if ( !falseEnd )
                            inString = !inString;
                        break;
                    case '\\':
                        if ( inString )
                            falseEnd = true;
                        break;
                    case '&':
                        if ( !inString )
                        {
                            inSharedData = !inSharedData;
                            if ( !inSharedData )
                            {
                                i = index;
                                exit = true;
                                break;
                            }
                        }
                        break;
                    default:
                        break;
                }
            }

            return (T)DeserializeObject( typeof( T ), string.Concat(
                // takes the string data of the required object and removes the last comma and '}' from it.
                string.Concat( FetchAppendedSegment( text, id, 2 ), "," ),
                // appends the shared data at the end of the string above, when deserializing the ClassReader will read the field or property twice
                // the individual value of the field/property will be overriden by the shared data value, since it is deserialized later.
                // This is faster than deserializing the shared data as an individual object, deserializing the needed object, and then iterating
                // through fields/properties of both and setting values.
                string.Concat( text.Slice( 3, i - 5 ), "}" ) ) );
        }

        /// <summary>
        /// Serializes the object to a .Adeptar string and writes it to a file, the file is overwritten.
        /// Accepts an <see cref="AdeptarSettings"/> object to set serialization rules.
        /// </summary>
        /// <param name="path">The path of the file.</param>
        /// <param name="toSerialize">The object to serialize.</param>
        /// <param name="settings">The user provided serialization settings.</param>
        public static void SerializeWrite( string path, object toSerialize, AdeptarSettings settings )
        {
            AdeptarWriter.AssignSettings( settings );
            AdeptarWriter.SerializeWrite( path, toSerialize, FetchType( toSerialize ), SerializationMode.Default );
        }

        /// <summary>
        /// Serializes the object to a .Adeptar string. Accepts an <see cref="AdeptarSettings"/> object to set serialization rules.
        /// </summary>
        /// <param name="toSerialize">The object to serialize.</param>
        /// <param name="settings">The user provided serialization settings.</param>
        /// <returns>A .Adeptar representation of the object.</returns>
        public static string Serialize( object toSerialize, AdeptarSettings settings )
        {
            AdeptarWriter.AssignSettings( settings );
            return AdeptarWriter.Serialize( toSerialize, FetchType( toSerialize ) );
        }

        /// <summary>
        /// Serializes the object to a .Adeptar string using specified formatting style, and appends it to a file.
        /// If an object with the provided id already exists will throw an exception.
        /// The id is limited to numbers and letters. Accepts an <see cref="AdeptarSettings"/> object to set serialization rules.
        /// </summary>
        /// <param name="path">The path to the file to append the object to.</param>
        /// <param name="toSerialize">The object to serialize.</param>
        /// <param name="id">The id of the object used to append it.</param>
        /// <param name="settings">The user provided serialization settings.</param>
        public static void SerializeAppend( string path, object toSerialize, string id, AdeptarSettings settings )
        {
            AdeptarWriter.AssignSettings( settings );
            AdeptarWriter.SerializeWrite( path, toSerialize, FetchType( toSerialize ), SerializationMode.Append, id );
        }

        /// <summary>
        /// Serializes the object to a .Adeptar string using specified formatting style, and appends it to a file.
        /// The current object with the provided id is rewritten in the file.
        /// The id is limited to numbers and letters. Accepts an <see cref="AdeptarSettings"/> object to set serialization rules.
        /// </summary>
        /// <param name="path">The path of the file.</param>
        /// <param name="toSerialize">The object to serialize.</param>
        /// <param name="id">The id of the object used to append it.</param>
        /// <param name="settings">The user provided serialization settings.</param>
        public static void SerializeRewriteAppended( string path, object toSerialize, string id, AdeptarSettings settings )
        {
            AdeptarWriter.AssignSettings( settings );
            AdeptarWriter.SerializeWrite( path, toSerialize, FetchType( toSerialize ), SerializationMode.ChangeAppended, id );
        }

        /// <summary>
        /// Serializes the object to a .Adeptar string and writes it to a file, the file is overwritten.
        /// </summary>
        /// <param name="path">The path of the file.</param>
        /// <param name="toSerialize">The object to serialize.</param>
        public static void SerializeWrite( string path, object toSerialize )
        {
            AdeptarWriter.AssignSettings( AdeptarWriter.DefaultSettings );
            AdeptarWriter.SerializeWrite( path, toSerialize, FetchType( toSerialize ), SerializationMode.Default );
        }

        /// <summary>
        /// Serializes the object to a .Adeptar string using the provided formatting, and writes it to a file, the file is overwritten.
        /// </summary>
        /// <param name="path">The path of the file.</param>
        /// <param name="toSerialize">The object to serialize.</param>
        /// <param name="formatting">The formatting style.</param>
        public static void SerializeWrite( string path, object toSerialize, Formatting formatting = Formatting.Indented )
        {
            AdeptarSettings settings = new()
            {
                CheckClassAttributes = false,
                UseIndentation = formatting == Formatting.Indented,
            };
            AdeptarWriter.AssignSettings( settings );
            AdeptarWriter.SerializeWrite( path, toSerialize, FetchType( toSerialize ), SerializationMode.Default );
        }

        /// <summary>
        /// Serializes the object to a .Adeptar string.
        /// </summary>
        /// <param name="toSerialize">The object to serialize.</param>
        /// <returns>A .Adeptar representation of the object.</returns>
        public static string Serialize( object toSerialize )
        {
            AdeptarWriter.AssignSettings( AdeptarWriter.DefaultSettings );
            return AdeptarWriter.Serialize( toSerialize, FetchType( toSerialize ) );
        }

        /// <summary>
        /// Serializes the object to a .Adeptar string with specified formatting.
        /// </summary>
        /// <param name="toSerialize">The object to serialize.</param>
        /// <param name="formatting">The formatting style.</param>
        /// <returns>A .Adeptar representation of the object with the provided formatting style.</returns>
        public static string Serialize( object toSerialize, Formatting formatting = Formatting.Indented )
        {
            AdeptarSettings settings = new()
            {
                CheckClassAttributes = false,
                UseIndentation = formatting == Formatting.Indented,
            };
            AdeptarWriter.AssignSettings( settings );
            return AdeptarWriter.Serialize( toSerialize, FetchType( toSerialize ) );
        }

        /// <summary>
        /// Sets or rewrites the shared data of an Index feature object collection using specified formatting style.
        /// Fields or properties with null or default values are not serialized.
        /// </summary>
        /// <param name="path">The path to the file to append the object to.</param>
        /// <param name="toSerialize">The object to serialize as the shared data.</param>
        /// <param name="formatting">The formatting style.</param>
        public static void SerializeSetShared( string path, object toSerialize, Formatting formatting = Formatting.Indented )
        {
            AdeptarSettings settings = new()
            {
                CheckClassAttributes = false,
                UseIndentation = formatting == Formatting.Indented,
                IgnoreDefaultValues = true,
                IgnoreNullValues = true
            };
            AdeptarWriter.AssignSettings( settings );
            AdeptarWriter.SerializeWrite( path, toSerialize, FetchType( toSerialize ), SerializationMode.SetShared );
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
        public static void SerializeAppend( string path, object toSerialize, string id, Formatting formatting = Formatting.Indented )
        {
            AdeptarSettings settings = new()
            {
                CheckClassAttributes = false,
                UseIndentation = formatting == Formatting.Indented,
            };
            AdeptarWriter.AssignSettings( settings );
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
        public static void SerializeRewriteAppended( string path, object toSerialize, string id, Formatting formatting = Formatting.Indented )
        {
            AdeptarSettings settings = new()
            {
                CheckClassAttributes = false,
                UseIndentation = formatting == Formatting.Indented,
            };
            AdeptarWriter.AssignSettings( settings );
            AdeptarWriter.SerializeWrite( path, toSerialize, FetchType( toSerialize ), SerializationMode.ChangeAppended, id );
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