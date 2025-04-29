using System;
using System.Collections;
using System.Collections.Generic;
using static Adeptar.AdeptarDebugger;
using static Adeptar.AdeptarDeserializer;

namespace Adeptar
{
    /// <summary>
    /// Internal class containing method(s) for deserialization of <see cref="Dictionary{TKey, TValue}"/>
    /// </summary>
    internal sealed class DictionaryDeserializer
    {
        /// <summary>
        /// Deserializes the Adeptar string of type <see cref="Dictionary{TKey, TValue}"/> to a .NET object.
        /// </summary>
        /// <param name="sourceSpan">The Adeptar string representation of the object.</param>
        /// <param name="dictionaryType">The type of the <see cref="Dictionary{TKey, TValue}"/>.</param>
        /// <returns>The .NET version of the <see cref="Dictionary{TKey, TValue}"/>.</returns>
        internal static IDictionary DeserializeDictionary( ReadOnlySpan<char> sourceSpan, Type dictionaryType )
        {
            if ( sourceSpan.Length < 2 || sourceSpan[0] != '[' || sourceSpan[sourceSpan.Length - 1] != ']' )
            {
                throw new AdeptarException( $"Invalid dictionary format. Expected cleaned text enclosed in square brackets '[]'. Received start: '{PreviewSpan( sourceSpan, 50 )}'" );
            }

            if ( sourceSpan.Length == 2 )
            {
                return (IDictionary)Activator.CreateInstance( dictionaryType );
            }

            Type[] genericArgs = dictionaryType.GetGenericArguments();
            Type keyType = genericArgs[0];
            Type valueType = genericArgs[1];

            IDictionary targetDictionary = (IDictionary)Activator.CreateInstance( dictionaryType );

            int nestingLevel = 0;
            int segmentStartIndex = 0;
            int currentReadIndex = 0;
            int spanPosition = 0;

            bool isInsideNestedStructure = false;
            bool isInsideStringLiteral = false;
            bool escapeNextChar = false;
            bool readingComplexKey = false;

            object currentKey = null;
            object currentValue = null;

            ReadOnlySpan<char> contentSpan = sourceSpan.Slice( 1, sourceSpan.Length - 2 );

            foreach ( char currentChar in contentSpan )
            {
                if ( escapeNextChar )
                {
                    escapeNextChar = false;
                }
                else switch ( currentChar )
                    {
                        case '@':
                            if ( !isInsideStringLiteral && !isInsideNestedStructure && nestingLevel == 0 )
                            {
                                readingComplexKey = true;
                                segmentStartIndex = spanPosition + 1;
                            }
                            break;
                        case '"':
                            isInsideStringLiteral = !isInsideStringLiteral;
                            break;
                        case '[':
                        case '{':
                        case '(':
                            if ( !isInsideStringLiteral )
                            {
                                nestingLevel++;
                                isInsideNestedStructure = true;
                            }
                            break;
                        case ']':
                        case '}':
                        case ')':
                            if ( !isInsideStringLiteral )
                            {
                                if ( nestingLevel == 1 )
                                {
                                    if ( readingComplexKey && currentChar == ']' )
                                    {
                                        readingComplexKey = false;
                                    }
                                    isInsideNestedStructure = false;
                                }
                                nestingLevel--;
                                if ( nestingLevel < 0 ) throw new AdeptarException( $"Mismatched closing bracket '{currentChar}' near position {spanPosition}. Input: '[{PreviewSpan( contentSpan )}]'" );
                            }
                            break;
                        case '\\':
                            if ( isInsideStringLiteral )
                            {
                                escapeNextChar = true;
                            }
                            else
                            {
                                throw new AdeptarException( $"Invalid character '\\' outside string literal near position {spanPosition}. Input: '[{PreviewSpan( contentSpan )}]'." );
                            }
                            break;
                        case ',':
                            if ( nestingLevel == 0 && !isInsideNestedStructure && !isInsideStringLiteral && !readingComplexKey )
                            {
                                ReadOnlySpan<char> valueSegment = contentSpan.Slice( segmentStartIndex, spanPosition - segmentStartIndex );
                                try
                                {
                                    currentValue = DeserializeObject( valueType, valueSegment );
                                }
                                catch ( AdeptarException ex ) { throw new AdeptarException( $"Failed to deserialize value for dictionary key '{currentKey ?? "??"}'. Input: '{valueSegment.ToString()}'. Reason: {ex.Message}", ex ); }
                                catch ( Exception ex ) { throw new AdeptarException( $"Failed to deserialize value for dictionary key '{currentKey ?? "??"}'. Input: '{valueSegment.ToString()}'. See inner exception.", ex ); }

                                try { targetDictionary.Add( currentKey, currentValue ); }
                                catch ( Exception ex ) { throw new AdeptarException( $"Failed to add key/value pair. Key: '{currentKey ?? "??"}'. See inner exception.", ex ); }

                                segmentStartIndex = spanPosition + 1;
                                currentKey = null;
                                currentValue = null;
                            }
                            break;
                        case ':':
                            if ( nestingLevel == 0 && !isInsideNestedStructure && !isInsideStringLiteral && !readingComplexKey )
                            {
                                ReadOnlySpan<char> keySegment = contentSpan.Slice( segmentStartIndex, spanPosition - segmentStartIndex );
                                try
                                {
                                    currentKey = DeserializeObject( keyType, keySegment );
                                }
                                catch ( AdeptarException ex ) { throw new AdeptarException( $"Failed to deserialize dictionary key. Input: '{keySegment.ToString()}'. Reason: {ex.Message}", ex ); }
                                catch ( Exception ex ) { throw new AdeptarException( $"Failed to deserialize dictionary key. Input: '{keySegment.ToString()}'. See inner exception.", ex ); }

                                segmentStartIndex = spanPosition + 1;
                            }
                            break;
                        default:
                            break;
                    }
                spanPosition++;
                currentReadIndex++;
            }

            if ( currentKey != null )
            {
                if ( segmentStartIndex >= contentSpan.Length )
                {
                    throw new AdeptarException( $"Invalid dictionary format. Dangling key '{currentKey}' without a value at the end. Input: '[{PreviewSpan( contentSpan )}]'." );
                }

                ReadOnlySpan<char> lastValueSegment = contentSpan.Slice( segmentStartIndex );
                try
                {
                    currentValue = DeserializeObject( valueType, lastValueSegment );
                }
                catch ( AdeptarException ex ) { throw new AdeptarException( $"Failed to deserialize last value for dictionary key '{currentKey}'. Input: '{lastValueSegment.ToString()}'. Reason: {ex.Message}", ex ); }
                catch ( Exception ex ) { throw new AdeptarException( $"Failed to deserialize last value for dictionary key '{currentKey}'. Input: '{lastValueSegment.ToString()}'. See inner exception.", ex ); }

                try { targetDictionary.Add( currentKey, currentValue ); }
                catch ( Exception ex ) { throw new AdeptarException( $"Failed to add last key/value pair. Key: '{currentKey}'. See inner exception.", ex ); }
            }

            if ( isInsideStringLiteral ) throw new AdeptarException( "Unterminated string literal at end of dictionary content." );
            if ( nestingLevel != 0 ) throw new AdeptarException( $"Mismatched nesting level ({nestingLevel}) at end of dictionary content." );

            return targetDictionary;
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