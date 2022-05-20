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
        ///
        /// </summary>
        private int _textLength;

        /// <summary>
        ///
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public bool ContainsKey ( string key ) => _keyMaps.ContainsKey( key );

        /// <summary>
        ///
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        public object GetValue<T>( string key )
        {
            if (!_keyMaps.ContainsKey(key)){
                throw new AdeptarException($"Invalid variable name {key}.");
            }

            return DeserializeObject( typeof( T ), _keyMaps[key] );
        }

        /// <summary>
        ///
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public dynamic DeserializeToWhole<T> ()
        {
            StringBuilder str = new( _textLength );

            foreach (var item in _keyMaps.Keys){
                str.Append( item );
            }

            return DeserializeObject( typeof( T ), str.ToString() );
        }

        /// <summary>
        ///
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="map"></param>
        /// <returns></returns>
        public dynamic DeserializeToMap<T>( AdeptarMap map )
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
                        if (falseEnd && inString)
                        {
                            falseEnd = false;
                        }
                        else if (!falseEnd)
                            inString = !inString;
                        break;
                    case '[':
                        if (!inString)
                        {
                            level++; nested = true;
                        }
                        else if (!inString)
                            level++;
                        break;
                    case ']':
                        if (level - 1 == 0 && !inString)
                        {
                            nested = false;
                        }
                        level--;
                        break;
                    case '\\':
                        if (inString)
                        {
                            falseEnd = true;
                        }
                        else
                        {
                            throw new AdeptarException( "Invalid character '\\', such a character can appear only inside a string." );
                        }
                        break;
                    case ',':
                        if (!nested && !inString)
                        {
                            result._keyMaps.Add( name, clean.Slice( j, w - j ).ToString() );
                            j = w + 1;
                            i++;
                        }
                        break;
                    case '{':
                        if (!inString)
                        {
                            level++;
                            nested = true;
                        }
                        break;
                    case '}':
                        if (level - 1 == 0 && !inString)
                        {
                            nested = false;
                        }
                        level--;
                        break;
                    case '(':
                        if (!inString)
                        {
                            level++;
                            nested = true;
                        }
                        break;
                    case ')':
                        if (level - 1 == 0 && !inString)
                        {
                            nested = false;
                            level--;
                        }
                        break;
                    case ':':
                        if (!nested && !inString)
                        {
                            name = clean.Slice( j, w - j ).ToString();
                            j = w + 1;
                        }
                        break;
                }
                if (level - 1 == -2 && !inString)
                {
                    if (w == clean.Length - 1)
                        result._keyMaps.Add( name, clean.Slice( j, w - j ).ToString() );
                }
                w++;
            }

            return result;
        }
    }
}