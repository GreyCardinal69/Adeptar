using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

using static Adeptar.AdeptarReader;
using static Adeptar.DeserializationHelpers;
using static Adeptar.ClassReader;

namespace Adeptar
{
    /// <summary>
    /// A class for deserializing .Adeptar objects without knowing their types at first.
    /// Such as when multiple different .Adeptar objects are serialized in the the same directory.
    /// Allows to deserialize them all, then later determine which object belongs to which .NET type.
    /// Class is restricted for objects of type class or struct.
    /// </summary>
    public class AdeptarDynamic
    {
        /// <summary>
        /// Creates a new instance of <see cref="AdeptarDynamic"/> with no mappings.
        /// </summary>
        public AdeptarDynamic ()
        {
            _keyMaps = new();
        }

        /// <summary>
        /// A private field of a <see cref="Dictionary{TKey, TValue}"/>. Keys are the
        /// property/field names, and the values are their .Adeptar strings.
        /// </summary>
        private Dictionary<string, string> _keyMaps;

        /// <summary>
        /// Fetches the <see cref="Dictionary{TKey, TValue}"/> where the keys are the
        /// property/field names, and the values are their .Adeptar strings.
        /// </summary>
        public Dictionary<string, string> KeyMaps => _keyMaps;

        /// <summary>
        /// A property that gets the total string length of all the key and values of the cleaned .Adeptar string.
        /// </summary>
        private int _textLength
        {
            get
            {
                int len = 0;
                foreach (var item in _keyMaps)
                {
                    len += item.Key.Length + item.Value.Length;
                }
                return len;
            }
        }

        /// <summary>
        /// Clears the <see cref="AdeptarDynamic"/> object's key and value maps.
        /// </summary>
        public void Clear () => _keyMaps.Clear();

        /// <summary>
        /// Checks if the <see cref="AdeptarDynamic"/> object contains the field/property with the given name.
        /// </summary>
        /// <param name="key">The key name to check for.</param>
        /// <returns>True if the <see cref="AdeptarDynamic"/> object contains a field/property with the provided name.</returns>
        public bool ContainsKey ( string key ) => _keyMaps.ContainsKey( key );

        /// <summary>
        /// Takes a key name of a field/property. If the key is not found throws an exception.
        /// If the key is found deserializes its .Adeptar string to the provided type.
        /// </summary>
        /// <typeparam name="T">The type to deserialize to.</typeparam>
        /// <param name="key">The key to deserialize for.</param>
        /// <returns>
        /// The deserialized .NET object.
        /// </returns>
        public T GetValue<T>( string key )
        {
            if (!_keyMaps.ContainsKey(key)){
                throw new AdeptarException($"Invalid variable name {key}, no property or field with such a name exists.");
            }

            return ( T ) DeserializeObject( typeof( T ), _keyMaps[key] );
        }

        /// <summary>
        /// Deserializes the <see cref="AdeptarDynamic"/> object to a .Net object.
        /// The type constraint is a class.
        /// </summary>
        /// <typeparam name="T">The type of the class to deserialize to.</typeparam>
        /// <returns>
        /// The deserialized .Net object.
        /// </returns>
        public T Deserialize<T> () where T : class
        {
            StringBuilder str = new( _textLength );
            int i = 0;

            str.Append( '{' );
            foreach (var value in _keyMaps)
            {
                str.Append( value.Key );
                str.Append( ':' );
                str.Append( CleanText( value.Value ) );
                if (i != _keyMaps.Count - 1){
                    str.Append( ',' );
                    i++;
                }
            }
            str.Append( '}' );

            return ( T ) DeserializeObject( typeof( T ), str.ToString() );
        }

        /// <summary>
        /// Deserializes the <see cref="AdeptarDynamic"/> object to a .Net object.
        /// The type constraint is a struct.
        /// </summary>
        /// <typeparam name="T">The type of the struct to deserialize to.</typeparam>
        /// <returns>
        /// The deserialized .Net object.
        /// </returns>
        public T Deserialize<T> ( object ignore = null ) where T : struct
        {
            StringBuilder str = new( _textLength );
            int i = 0;

            str.Append( '{' );
            foreach (var value in _keyMaps)
            {
                str.Append( value.Key );
                str.Append( ':' );
                str.Append( CleanText( value.Value ) );
                if (i != _keyMaps.Count - 1){
                    str.Append( ',' );
                    i++;
                }
            }
            str.Append( '}' );

            return ( T ) DeserializeObject( typeof( T ), str.ToString() );
        }

        /// <summary>
        /// Deserializes the <see cref="AdeptarDynamic"/> object to a .Net object.
        /// Accepts a <see cref="Dictionary{TKey, TValue}"/> map for field/property mapping.
        /// </summary>
        /// <typeparam name="T">The type of the struct to deserialize to.</typeparam>
        /// <returns>
        /// The deserialized .Net object.
        /// </returns>
        public T DeserializeWithMap<T>( Dictionary<string, string> map )
        {
            StringBuilder str = new( _textLength );
            int i = 0;

            str.Append( '{' );
            foreach (var value in _keyMaps)
            {
                str.Append( value.Key );
                str.Append( ':' );
                str.Append( CleanText( value.Value ) );
                if (i != _keyMaps.Count - 1)
                {
                    str.Append( ',' );
                    i++;
                }
            }
            str.Append( '}' );

            return ( T ) DeserializeClassStructWithMap( str.ToString(), typeof( T ), map );
        }

        /// <summary>
        /// Creates a <see cref="AdeptarDynamic"/> object from a file that contains
        /// a .Adeptar object.
        /// </summary>
        /// <param name="path">The path of the file.</param>
        /// <returns>
        /// A <see cref="AdeptarDynamic"/> object that contains the data of the given .Adeptar string.
        /// </returns>
        public static AdeptarDynamic FromFile ( string path )
        {
            AdeptarDynamic result = new();
            PopulateMaps( ( CleanText( File.ReadAllText( path ) ) ).Slice( 1 ), ref result );
            return result;
        }

        /// <summary>
        /// Creates a <see cref="AdeptarDynamic"/> object from the .Adeptar string.
        /// </summary>
        /// <param name="str">The .Adeptar string to convert to a <see cref="AdeptarDynamic"/> object.</param>
        /// <returns>
        /// A <see cref="AdeptarDynamic"/> object that contains the data of the given .Adeptar string.
        /// </returns>
        public static AdeptarDynamic FromString ( string str )
        {
            AdeptarDynamic result = new();
            PopulateMaps( ( CleanText( str ) ).Slice( 1 ), ref result );
            return result;
        }

        /// <summary>
        /// Handles the process of populating the <see cref="AdeptarDynamic.KeyMaps"/>.
        /// </summary>
        /// <param name="str">The <see cref="ReadOnlySpan{T}"/> text.</param>
        /// <param name="result">The <see cref="AdeptarDynamic"/> to populate.</param>
        private static void PopulateMaps( ReadOnlySpan<char> str, ref AdeptarDynamic result )
        {
            int level = 0;
            int i = 0;
            int j = 0;
            int w = 0;

            bool nested = false;
            bool inString = false;
            bool falseEnd = false;

            string name = "";

            foreach (var item in str)
            {
                switch (item)
                {
                    case '"':
                        if (falseEnd && inString){
                            falseEnd = false;
                        }
                        else if (!falseEnd)
                            inString = !inString;
                        break;
                    case '[':
                        if (!inString){
                            level++; nested = true;
                        }
                        else if (!inString)
                            level++;
                        break;
                    case ']':
                        if (level - 1 == 0 && !inString){
                            nested = false;
                        }
                        level--;
                        break;
                    case '\\':
                        if (inString){
                            falseEnd = true;
                        }else{
                            throw new AdeptarException( "Invalid character '\\', such a character can appear only inside a string." );
                        }
                        break;
                    case ',':
                        if (!nested && !inString){
                            result._keyMaps.Add( name, str.Slice( j, w - j ).ToString() );
                            j = w + 1;
                            i++;
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
                            nested = false;
                        }
                        level--;
                        break;
                    case '(':
                        if (!inString){
                            level++;
                            nested = true;
                        }
                        break;
                    case ')':
                        if (level - 1 == 0 && !inString){
                            nested = false;
                            level--;
                        }
                        break;
                    case ':':
                        if (!nested && !inString){
                            name = str.Slice( j, w - j ).ToString();
                            j = w + 1;
                        }
                        break;
                }
                if (level - 1 == -2 && !inString){
                    if (w == str.Length - 1)
                        result._keyMaps.Add( name, str.Slice( j, w - j ).ToString() );
                }
                w++;
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