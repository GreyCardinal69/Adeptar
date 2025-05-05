using System;
using System.Collections;
using static Adeptar.AdeptarSerializer;
using static Adeptar.TypeClassifiers;

namespace Adeptar
{
    /// <summary>
    /// Serializes the elements of an IDictionary into the Adeptar format Key:Value,...
    /// Called by AdeptarWriter when processing dictionaries.
    /// </summary>
    internal static class DictionaryWriter
    {
        /// <summary>
        /// Writes the key-value pairs of a dictionary to the StringBuilder using the provided context.
        /// </summary>
        /// <param name="dict">The <see cref="IDictionary"/> instance to serialize elements from.</param>
        /// <param name="context">The current serialization context (passed by ref).</param>
        /// <exception cref="AdeptarException">Wraps exceptions from underlying serialization calls.</exception>
        internal static void WriteDictionaryElements( IDictionary dict, ref SerializationContext context )
        {
            if ( dict == null || dict.Count == 0 )
            {
                return;
            }

            int elementIndex = 0;
            int totalCount = dict.Count;

            foreach ( DictionaryEntry entry in dict )
            {
                object key = entry.Key;
                object value = entry.Value;
                bool isLastElement = (elementIndex == totalCount - 1);

                SerializableType keyType = FetchSerializableTypeOf( key );
                SerializableType valueType = FetchSerializableTypeOf( value );

                try
                {
                    bool isComplexKey = keyType != SerializableType.Simple &&
                                        keyType != SerializableType.String &&
                                        keyType != SerializableType.Char &&
                                        keyType != SerializableType.DateTime;

                    context.SurpressNewLineOnce = true;
                    Write( key, keyType, null, ref context, true, isComplexKey );
                }
                catch ( AdeptarException ) { throw; }
                catch ( Exception ex ) { throw new AdeptarException( $"Failed to serialize dictionary key. Key: '{key ?? "null"}'.", ex ); }

                context.Builder.Append( ':' );
                if ( context.Settings.UseIndentation )
                {
                    context.Builder.Append( ' ' );
                }

                // at this point we have "...key:"
                // we dont want full indentation applied here, only a single space.
                context.SurpessFullIndentationOnce = true;

                try
                {
                    Write( value, valueType, null, ref context, isLastElement, false );
                }
                catch ( AdeptarException ) { throw; }
                catch ( Exception ex ) { throw new AdeptarException( $"Failed to serialize value for dictionary key '{key ?? "null"}'. Value Type: '{value?.GetType().Name ?? "null"}'.", ex ); }

                elementIndex++;
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