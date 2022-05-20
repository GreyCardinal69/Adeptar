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
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using static Adeptar.AdeptarReader;
using static Adeptar.DeserializationHelpers;

namespace Adeptar
{
    /// <summary>
    ///
    /// </summary>
    public class AdeptarDynamic
    {
        /// <summary>
        ///
        /// </summary>
        private AdeptarDynamic ()
        {
            _keyMaps = new();
        }

        /// <summary>
        ///
        /// </summary>
        private Dictionary<string, string> _keyMaps;

        /// <summary>
        ///
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

        private bool _isClass;

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
                throw new AdeptarException($"Invalid variable name {key}.");
            }

            return ( T ) DeserializeObject( typeof( T ), _keyMaps[key] );
        }

        /// <summary>
        ///
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T Deserialize<T> () where T : class
        {
            StringBuilder str = new( _textLength );

            if (_isClass)
            {
                str.Append( '{' );
            }

            for (int i = 0; i < _keyMaps.Count; i++)
            {
                var value = KeyMaps.ElementAt( i );

                str.Append( value.Key );
                if (_isClass)
                {
                    str.Append( ':' );
                }
                str.Append( CleanText( value.Value ) );

                if (i != _keyMaps.Count - 1)
                {
                    str.Append( ',' );
                }
            }

            if (_isClass)
            {
                str.Append( '}' );
            }

            return ( T ) DeserializeObject( typeof( T ), str.ToString() );
        }

        /// <summary>
        ///
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="map"></param>
        /// <returns></returns>
        public dynamic DeserializeWithMap<T>( AdeptarMap map )
        {
            return null;
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
            ReadOnlySpan<char> str = ( CleanText( File.ReadAllText( path ) ) ).Slice( 1 );

            AdeptarDynamic result = new();

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
            ReadOnlySpan<char> clean = ( CleanText( str ) ).Slice( 1 );

            AdeptarDynamic result = new();

            result._isClass = clean[clean.Length - 1] == '}';

            int level = 0;
            int i = 0;
            int j = 0;
            int w = 0;

            bool nested = false;
            bool inString = false;
            bool falseEnd = false;

            string name = "";

            foreach (var item in clean)
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
                            result._keyMaps.Add( name, clean.Slice( j, w - j ).ToString() );
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
                            name = clean.Slice( j, w - j ).ToString();
                            j = w + 1;
                        }
                        break;
                }
                if (level - 1 == -2 && !inString){
                    if (w == clean.Length - 1)
                        result._keyMaps.Add( name, clean.Slice( j, w - j ).ToString() );
                }
                w++;
            }

            return result;
        }
    }
}