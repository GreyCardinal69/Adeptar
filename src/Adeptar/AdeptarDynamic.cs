using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using static Adeptar.AdeptarReader;

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
        public AdeptarDynamic () { }

        /// <summary>
        ///
        /// </summary>
        private Dictionary<string, string> _keyMaps;

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
        ///
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static AdeptarDynamic FromFile ( string path )
        {
            return null;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static AdeptarDynamic FromString ( string str )
        {
            return null;
        }
    }
}