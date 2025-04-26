using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Reflection;

using static Adeptar.AdeptarReader;

namespace Adeptar
{
    /// <summary>
    /// Internal class containing method(s) for deserialization of tuples
    /// </summary>
    internal class TupleReader
    {
        private static readonly ConcurrentDictionary<Type, Dictionary<string, FieldInfo>> _tupleFieldInfoCache =
            new ConcurrentDictionary<Type, Dictionary<string, FieldInfo>>();
        private static readonly ConcurrentDictionary<FieldInfo, Action<object, object>> _tupleFieldSetterCache =
            new ConcurrentDictionary<FieldInfo, Action<object, object>>();

        private const BindingFlags FieldBindingFlags = BindingFlags.Public | BindingFlags.Instance;

        /// <summary>
        /// Deserializes the Adeptar string representation of a tuple with named fields
        /// e.g., "(Item1:1,Item2:"abc",Rest:(Item1:true))" into a .NET ValueTuple object.
        /// Assumes input string has already been cleaned of extraneous whitespace/indentation.
        /// </summary>
        /// <param name="cleanedText">The ReadOnlySpan<char> containing the cleaned tuple string representation (including parentheses).</param>
        /// <param name="tupleType">The target <see cref="System.ValueTuple"/> type.</param>
        /// <returns>The deserialized tuple object (boxed).</returns>
        /// <exception cref="AdeptarSerializationException">Thrown if the format is invalid, field names mismatch, element count mismatches, or element deserialization fails.</exception>
        internal static object DeserializeTuple( ReadOnlySpan<char> cleanedText, Type tupleType )
        {
            // Basic Validation
            if ( cleanedText.Length < 2 || cleanedText[0] != '(' || cleanedText[cleanedText.Length - 1] != ')' )
            {
                // Use a helper to show limited portion of text in exceptions for long inputs
                throw new AdeptarException( $"Invalid tuple format. Expected cleaned text enclosed in parentheses '()'. Received start: '{PreviewSpan( cleanedText, 50 )}'" );
            }
            if ( !tupleType.IsValueType || !IsValueTupleType( tupleType ) )
            {
                throw new AdeptarException( $"Invalid target type. Expected a ValueTuple type, but received {tupleType.FullName}." );
            }

            // Slice off parentheses. No Trim needed as input is pre-cleaned.
            ReadOnlySpan<char> innerSpan = cleanedText.Slice(1, cleanedText.Length - 2);

            // Get expected fields (Item1, Item2, ..., Rest) and cache them
            Dictionary<string, FieldInfo> fields = GetOrCreateTupleFieldMap(tupleType);

            // Create boxed instance
            object target = Activator.CreateInstance(tupleType);
            int elementsFound = 0;

            if ( innerSpan.IsEmpty )
            {
                if ( fields.Count != 0 )
                {
                    throw new AdeptarException( $"Invalid tuple format. Received empty content '()' but expected fields for type {tupleType.Name}." );
                }
                return target; // Return default empty tuple
            }

            // --- Parsing State ---
            int currentPosition = 0;
            bool expectingName = true;
            string currentName = null;
            int valueStartPosition = 0;

            while ( currentPosition < innerSpan.Length )
            {
                char delimiterFound;
                // Find delimiter in the already cleaned span
                int delimiterIndex = FindNextTopLevelDelimiter(innerSpan, currentPosition, out delimiterFound);

                int endPosition = (delimiterIndex == -1) ? innerSpan.Length : delimiterIndex;
                // No Trim() needed on the segment as innerSpan is clean
                ReadOnlySpan<char> segment = innerSpan.Slice(currentPosition, endPosition - currentPosition);

                if ( expectingName )
                {
                    // Segment should be the name (e.g., "Item1")
                    if ( delimiterFound != ':' )
                    {
                        throw new AdeptarException( $"Invalid tuple format. Expected ':' after field name but found '{delimiterFound}' or end of content near position {currentPosition}. Input: '{PreviewSpan( innerSpan )}'. Segment: '{segment.ToString()}'" );
                    }
                    if ( segment.IsEmpty ) // Check if name itself is empty
                    {
                        throw new AdeptarException( $"Invalid tuple format. Missing field name before ':' near position {currentPosition}. Input: '{PreviewSpan( innerSpan )}'." );
                    }
                    // Convert name segment to string *once*
                    currentName = segment.ToString();
                    expectingName = false;
                    valueStartPosition = endPosition + 1; // Value starts after the colon
                }
                else // Expecting Value
                {
                    // Segment processing for value is done below after validation

                    if ( delimiterFound != ',' && delimiterIndex != -1 ) // Must be comma or end
                    {
                        throw new AdeptarException( $"Invalid tuple format. Expected ',' or end of tuple after value for field '{currentName}' but found '{delimiterFound}' near position {endPosition}. Input: '{PreviewSpan( innerSpan )}'." );
                    }

                    // Validate field name and get FieldInfo
                    if ( !fields.TryGetValue( currentName, out FieldInfo currentField ) )
                    {
                        throw new AdeptarException( $"Invalid field name '{currentName}' found in input for tuple type {tupleType.Name} near position {valueStartPosition - currentName.Length - 1}. Input: '{PreviewSpan( innerSpan )}'. Expected one of: {string.Join( ", ", fields.Keys )}." );
                    }

                    // Get the value slice. No Trim() needed.
                    ReadOnlySpan<char> valueSpan = innerSpan.Slice(valueStartPosition, endPosition - valueStartPosition);
                    object elementValue;

                    try
                    {
                        // Deserialize the value: Special handling for 'Rest' field
                        if ( currentName == "Rest" )
                        {
                            elementValue = DeserializeTuple( valueSpan, currentField.FieldType );
                        }
                        else
                        {
                            elementValue = AdeptarReader.DeserializeObject( currentField.FieldType, valueSpan );
                        }
                    }
                    catch ( Exception ex ) when ( !( ex is Exception ) )
                    {
                        throw new Exception( $"Failed to deserialize value for tuple field '{currentName}'. Input: '{PreviewSpan( valueSpan )}'. See inner exception.", ex );
                    }
                    catch ( Exception ex )
                    {
                        throw new Exception( $"Failed to deserialize value for tuple field '{currentName}'. Input: '{PreviewSpan( valueSpan )}'. Reason: {ex.Message}", ex.InnerException );
                    }

                    // Set the value using cached setter
                    try
                    {
                        Action<object, object> setter = GetOrCreateTupleFieldSetter(currentField);
                        setter( target, elementValue );
                        elementsFound++;
                    }
                    catch ( Exception ex )
                    {
                        throw new Exception( $"Failed to set value for tuple field '{currentName}'. Type mismatch or other error. See inner exception.", ex );
                    }

                    // Reset for next element
                    currentName = null;
                    expectingName = true;
                }

                // Move position past the delimiter (or to the end)
                currentPosition = ( delimiterIndex == -1 ) ? innerSpan.Length : endPosition + 1;
            }

            // Final Validation
            if ( !expectingName ) // Ended while expecting a value - dangling name:
            {
                throw new AdeptarException( $"Invalid tuple format. Dangling field name '{currentName}' without a value at the end of input: '{PreviewSpan( innerSpan )}'." );
            }
            // Optional check: Ensure all fields defined in the type were present in the input
            // if (elementsFound < fields.Count && fields.Any(kvp => kvp.Key != "Rest")) { // Allow omitting 'Rest' if it's default? Depends on requirements.
            //     throw new AdeptarSerializationException($"Missing fields in input for tuple type {tupleType.Name}. Found {elementsFound}, expected {fields.Count}. Input: '{PreviewSpan(innerSpan)}'.");
            // }

            return target; // Return the boxed tuple instance
        }

        // (Keep FindNextTopLevelDelimiter helper - it doesn't rely on trimming)
        private static int FindNextTopLevelDelimiter( ReadOnlySpan<char> span, int startIndex, out char delimiterFound )
        {
            delimiterFound = '\0';
            int level = 0;
            bool inString = false;
            bool escapeNext = false;

            for ( int i = startIndex; i < span.Length; i++ )
            {
                char c = span[i];
                if ( escapeNext ) { escapeNext = false; continue; }
                if ( c == '\\' ) { escapeNext = true; continue; }
                if ( c == '"' ) { inString = !inString; continue; }
                if ( inString ) continue;

                switch ( c )
                {
                    case '(': case '[': case '{': level++; break;
                    case ')': case ']': case '}': level--; break;
                    case ':': case ',': if ( level == 0 ) { delimiterFound = c; return i; } break;
                }
                if ( level < 0 ) throw new AdeptarException( $"Mismatched nesting level (negative) near index {i}. Input: '{PreviewSpan( span )}'." );
            }
            return -1;
        }

        // (Keep GetOrCreateTupleFieldMap - unchanged)
        private static Dictionary<string, FieldInfo> GetOrCreateTupleFieldMap( Type tupleType )
        {
            return _tupleFieldInfoCache.GetOrAdd( tupleType, type =>
            {
                var fieldMap = new Dictionary<string, FieldInfo>();
                foreach ( var field in type.GetFields( FieldBindingFlags ) )
                {
                    if ( field.Name.StartsWith( "Item" ) || field.Name == "Rest" ) fieldMap[field.Name] = field;
                }
                if ( fieldMap.Count == 0 && type.GetFields( FieldBindingFlags ).Length > 0 )
                {
                    if ( type.FullName?.StartsWith( "System.ValueTuple`" ) != true || type.GenericTypeArguments.Length > 0 )
                    { // Check if not ValueTuple<>
                        throw new InvalidOperationException( $"Could not find expected 'ItemX' or 'Rest' fields for type {type.Name}." );
                    }
                }
                return fieldMap;
            } );
        }

        // (Keep GetOrCreateTupleFieldSetter - unchanged)
        private static Action<object, object> GetOrCreateTupleFieldSetter( FieldInfo field )
        {
            return _tupleFieldSetterCache.GetOrAdd( field, f => ( targetObj, valueObj ) => f.SetValue( targetObj, valueObj ) );
        }

        // (Keep IsValueTupleType - unchanged)
        private static bool IsValueTupleType( Type type ) => type != null && type.IsValueType && type.Namespace == "System" && type.Name.StartsWith( "ValueTuple`" );

        // Helper to preview span content in exceptions
        private static string PreviewSpan( ReadOnlySpan<char> span, int maxLength = 100 )
        {
            if ( span.Length <= maxLength ) return span.ToString();
            return span.Slice( 0, maxLength ).ToString() + "...";
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