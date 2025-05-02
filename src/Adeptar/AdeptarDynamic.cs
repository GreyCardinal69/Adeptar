using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Adeptar.Unity;
using static Adeptar.AdeptarDebugger;
using static Adeptar.AdeptarDeserializer;
using static Adeptar.DeserializationHelpers;
using static Adeptar.ObjectDeserializer;

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
        public AdeptarDynamic()
        {
            _keyValueMap = new();
        }

        /// <summary>
        /// Creates a new instance of <see cref="AdeptarDynamic"/> with no mappings.
        /// </summary>
        public AdeptarDynamic( string adeptarString )
        {
            _keyValueMap = new();
            _cleanedAdeptarObjectString = adeptarString;
        }

        /// <summary>
        /// A private field of a <see cref="Dictionary{TKey, TValue}"/>. Keys are the
        /// property/field names, and the values are their .Adeptar strings.
        /// </summary>
        private readonly Dictionary<string, string> _keyValueMap = new(StringComparer.Ordinal);

        /// <summary>
        /// The original full .Adeptar cleaned string.
        /// </summary>
        private readonly string _cleanedAdeptarObjectString;

        /// <summary>
        /// Returns a <see cref="IReadOnlyDictionary{TKey, TValue}"/> where the keys are the
        /// property/field names, and the values are their .Adeptar strings.
        /// </summary>
        public IReadOnlyDictionary<string, string> KeyValueMap => _keyValueMap;

        /// <summary>
        /// A property that gets the total string length of all the key and values of the cleaned .Adeptar string.
        /// </summary>
        private int _textLength
        {
            get
            {
                int len = 0;
                foreach ( KeyValuePair<string, string> item in _keyValueMap )
                {
                    len += item.Key.Length + item.Value.Length;
                }
                return len;
            }
        }

        /// <summary>
        /// Creates and populates a new <see cref="AdeptarDynamic"/> object by parsing Adeptar object data from a file path.
        /// </summary>
        /// <param name="path">The path to the file containing Adeptar object data (must represent an object starting with '{').</param>
        /// <returns>A populated <see cref="AdeptarDynamic"/> instance.</returns>
        /// <exception cref="ArgumentNullException">If <paramref name="path"/> is null.</exception>
        /// <exception cref="ArgumentException">If <paramref name="path"/> is empty or whitespace.</exception>
        /// <exception cref="AdeptarException">For file IO errors or format errors during parsing.</exception>
        public static AdeptarDynamic FromFile( string path )
        {
            ArgumentNullException.ThrowIfNull( path );
            if ( string.IsNullOrWhiteSpace( path ) ) throw new ArgumentException( "File path cannot be empty.", nameof( path ) );

            string fileContent;
            try
            {
                fileContent = File.ReadAllText( path, Encoding.UTF8 );
            }
            catch ( Exception ex ) when ( ex is IOException || ex is UnauthorizedAccessException || ex is DirectoryNotFoundException || ex is FileNotFoundException || ex is System.Security.SecurityException )
            { throw new AdeptarException( $"Failed to read file '{path}'. See inner exception.", ex ); }
            catch ( Exception ex ) { throw new AdeptarException( $"An unexpected error occurred reading file '{path}'. See inner exception.", ex ); }

            return FromString( fileContent, false );
        }

        /// <summary>
        /// Creates and populates a new <see cref="AdeptarDynamic"/> object by parsing Adeptar object data from a string.
        /// </summary>
        /// <param name="adeptarObjectString">The Adeptar string content representing an object (must start with '{' and end with '}').</param>
        /// <param name="textIsClean">Indicates whether the string is already free of insignificant whitespace and other indentation.</param>
        /// <returns>A populated <see cref="AdeptarDynamic"/> instance.</returns>
        /// <exception cref="ArgumentNullException">If <paramref name="adeptarObjectString"/> is null.</exception>
        /// <exception cref="AdeptarException">For format errors during parsing.</exception>
        public static AdeptarDynamic FromString( string adeptarObjectString, bool textIsClean )
        {
            ArgumentNullException.ThrowIfNull( adeptarObjectString );

            ReadOnlySpan<char> cleanedSpan;
            AdeptarDynamic result;

            if ( textIsClean )
            {
                result = new AdeptarDynamic( adeptarObjectString );
                cleanedSpan = adeptarObjectString;
            }
            else
            {
                var cleanText =  DeserializationHelpers.CleanText( adeptarObjectString.AsSpan() );
                result = new AdeptarDynamic( cleanText.ToString() );
                cleanedSpan = cleanText;
            }

            if ( cleanedSpan.Length < 1 || cleanedSpan[0] != '{' )
            {
                throw new AdeptarException( $"Invalid dynamic object format after cleaning. Expected content starting with '{{'. Preview: '{PreviewSpan( cleanedSpan )}'" );
            }

            PopulateMapFromSpan( cleanedSpan.Slice( 1 ), result._keyValueMap );
            return result;
        }

        /// <summary>
        /// Clears the <see cref="AdeptarDynamic"/> object's key and value maps.
        /// </summary>
        public void Clear() => _keyValueMap.Clear();

        /// <summary>
        /// Checks if the dynamic object contains data for the specified key (property/field name). Comparison is case-sensitive (Ordinal).
        /// </summary>
        /// <param name="key">The key name to check for.</param>
        /// <returns><c>true</c> if the key exists; otherwise, <c>false</c>.</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="key"/> is null.</exception>
        public bool ContainsKey( string key )
        {
            ArgumentNullException.ThrowIfNull( key );
            return _keyValueMap.ContainsKey( key );
        }


        /// <summary>
        /// Gets the raw Adeptar string value associated with the specified key.
        /// </summary>
        /// <param name="key">The key name.</param>
        /// <returns>The raw string value.</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="key"/> is null.</exception>
        /// <exception cref="KeyNotFoundException">Thrown if the key is not found.</exception>
        public string GetRawValue( string key )
        {
            ArgumentNullException.ThrowIfNull( key );
            if ( !_keyValueMap.TryGetValue( key, out string value ) )
            {
                throw new KeyNotFoundException( $"The key '{key}' was not found in the AdeptarDynamic object." );
            }
            return value;
        }

        /// <summary>
        /// Deserializes the raw string value associated with the specified key into the requested type <typeparamref name="T"/>.
        /// </summary>
        /// <typeparam name="T">The target .NET type to deserialize the value into.</typeparam>
        /// <param name="key">The key (property/field name) whose value should be deserialized.</param>
        /// <returns>The deserialized .NET object.</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="key"/> is null.</exception>
        /// <exception cref="KeyNotFoundException">Thrown if the key is not found.</exception>
        /// <exception cref="AdeptarException">Thrown if the raw string value cannot be deserialized into type <typeparamref name="T"/>.</exception>
        public T GetValue<T>( string key )
        {
            string rawValue = GetRawValue(key);
            ReadOnlySpan<char> valueSpan = rawValue.AsSpan();

            try
            {
                return (T)AdeptarReader.DeserializeObject( typeof( T ), valueSpan );
            }
            catch ( AdeptarException ) { throw; }
            catch ( Exception ex )
            {
                throw new AdeptarException( $"Failed to deserialize value for key '{key}' into type '{typeof( T ).Name}'. Raw value: '{PreviewSpan( valueSpan )}'.", ex );
            }
        }

        /// <summary>
        /// Populates the key-value map by parsing Adeptar object data.
        /// Assumes input span represents content *immediately after* opening '{', but includes closing '}'.
        /// </summary>
        /// <param name="contentSpanAfterOpeningBrace">Span starting after '{', ending with '}'.</param>
        /// <param name="targetMap">The dictionary to populate.</param>
        private static void PopulateMapFromSpan( ReadOnlySpan<char> contentSpanAfterOpeningBrace, Dictionary<string, string> targetMap )
        {
            if ( contentSpanAfterOpeningBrace.IsEmpty || contentSpanAfterOpeningBrace[contentSpanAfterOpeningBrace.Length - 1] != '}' )
            {
                throw new AdeptarException( $"Invalid dynamic object format. Input to PopulateMapFromSpan should end with '}}'. Preview: '{{{PreviewSpan( contentSpanAfterOpeningBrace )}'." );
            }
            ReadOnlySpan<char> contentSpan = contentSpanAfterOpeningBrace.Slice(0, contentSpanAfterOpeningBrace.Length - 1);

            if ( contentSpan.IsEmpty ) return;

            ParseKeyValuePairs( contentSpan, ( keySpan, valueSpan ) =>
            {
                string key = keySpan.ToString();
                if ( targetMap.ContainsKey( key ) ) { /* Overwrite duplicate silently */ }
                targetMap[key] = valueSpan.ToString();
            } );
        }

        private delegate void KeyValueSpanAction( ReadOnlySpan<char> key, ReadOnlySpan<char> value );

        /// <summary>
        /// Parses key-value pairs from object content span and invokes action for each.
        /// </summary>
        /// <exception cref="AdeptarException">For format errors.</exception>
        private static void ParseKeyValuePairs( ReadOnlySpan<char> objectContentSpan, KeyValueSpanAction processPairAction )
        {
            ArgumentNullException.ThrowIfNull( processPairAction );
            int currentPos = 0;

            while ( currentPos < objectContentSpan.Length )
            {
                int colonPos = FindNextTopLevelDelimiter(objectContentSpan, currentPos, out _, primaryDelimiter: ':');
                if ( colonPos == -1 )
                {
                    if ( !objectContentSpan.Slice( currentPos ).Trim().IsEmpty )
                        throw new AdeptarException( $"Invalid object format. Unexpected content '{PreviewSpan( objectContentSpan.Slice( currentPos ) )}' found. Expected 'Key : Value' pairs." );
                    break;
                }

                ReadOnlySpan<char> keySpan = objectContentSpan.Slice(currentPos, colonPos - currentPos).Trim();
                if ( keySpan.IsEmpty ) throw new AdeptarException( $"Invalid object format. Missing property name before ':' near position {colonPos}. Input: '{{{PreviewSpan( objectContentSpan )}}}'." );

                currentPos = colonPos + 1;

                int commaPos = FindNextTopLevelDelimiter(objectContentSpan, currentPos, out _, primaryDelimiter: ',');
                int valueEndPos = (commaPos == -1) ? objectContentSpan.Length : commaPos;
                ReadOnlySpan<char> valueSpan = objectContentSpan.Slice(currentPos, valueEndPos - currentPos).Trim();

                processPairAction( keySpan, valueSpan );

                if ( commaPos == -1 ) break;
                currentPos = commaPos + 1;
            }
        }

        /// <summary>
        /// Deserializes the <see cref="AdeptarDynamic"/> object to a .Net object.
        /// The type constraint is a class.
        /// </summary>
        /// <typeparam name="T">The type of the class to deserialize to.</typeparam>
        /// <returns>
        /// The deserialized .Net object.
        /// </returns>
        public T Deserialize<T>() where T : class => (T)DeserializeObject( typeof( T ), _cleanedAdeptarObjectString );

        /// <summary>
        /// Deserializes the <see cref="AdeptarDynamic"/> object to a .Net object.
        /// The type constraint is a struct.
        /// </summary>
        /// <typeparam name="T">The type of the struct to deserialize to.</typeparam>
        /// <returns>
        /// The deserialized .Net object.
        /// </returns>
        public T Deserialize<T>( object ignore = null ) where T : struct => (T)DeserializeObject( typeof( T ), _cleanedAdeptarObjectString );

        /// <summary>
        /// Deserializes the <see cref="AdeptarDynamic"/> object to a .Net object.
        /// Accepts a <see cref="Dictionary{TKey, TValue}"/> map for field/property mapping.
        /// </summary>
        /// <typeparam name="T">The type of the struct to deserialize to.</typeparam>
        /// <returns>
        /// The deserialized .Net object.
        /// </returns>
        public T DeserializeWithMap<T>( Dictionary<string, string> map ) => (T)DeserializeObjectInstance( _cleanedAdeptarObjectString, typeof( T ), map );
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