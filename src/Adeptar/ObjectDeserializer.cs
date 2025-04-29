using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using FastMember;
using static Adeptar.AdeptarDebugger;
using static Adeptar.DeserializationHelpers;

namespace Adeptar
{
    internal static class ObjectDeserializer
    {
        private static readonly ConcurrentDictionary<Type, TypeAccessor> _accessorCache = new();
        private static readonly ConcurrentDictionary<Type, Dictionary<string, Member>> _memberMapCache = new();
        private static readonly ConcurrentDictionary<Type, Func<object>> _defaultConstructorCache = new();

        private delegate void KeyValueSpanAction( ReadOnlySpan<char> key, ReadOnlySpan<char> value );

        /// <summary>
        /// Deserializes the Adeptar representation of an object.
        /// </summary>
        internal static object DeserializeObjectInstance( ReadOnlySpan<char> sourceSpan, Type targetType, Dictionary<string, string> memberNameMap = null )
        {
            ArgumentNullException.ThrowIfNull( targetType );
            ValidateSurroundingBraces( sourceSpan );

            object targetInstance = CreateInstance(targetType);
            TypeAccessor accessor = GetOrCreateAccessor(targetType);
            Dictionary<string, Member> members = GetOrCreateMemberMap(targetType);

            ReadOnlySpan<char> contentSpan = sourceSpan.Slice(1, sourceSpan.Length - 2);
            if ( contentSpan.IsEmpty ) return targetInstance;

            ParseKeyValuePairs( contentSpan, ( keySpan, valueSpan ) =>
            {
                string sourceKey = keySpan.ToString();
                string targetMemberName = sourceKey;

                if ( memberNameMap != null && memberNameMap.TryGetValue( sourceKey, out string mappedName ) )
                {
                    targetMemberName = mappedName;
                }

                if ( members.TryGetValue( targetMemberName, out Member member ) )
                {
                    object deserializedValue;
                    try
                    {
                        deserializedValue = AdeptarDeserializer.DeserializeObject( member.Type, valueSpan );
                    }
                    catch ( AdeptarException ex ) { throw new AdeptarException( $"Failed to deserialize value for member '{targetMemberName}' (source key '{sourceKey}'). Input: '{PreviewSpan( valueSpan )}'. Reason: {ex.Message}", ex ); }
                    catch ( Exception ex ) { throw new AdeptarException( $"Failed to deserialize value for member '{targetMemberName}' (source key '{sourceKey}'). Input: '{PreviewSpan( valueSpan )}'.", ex ); }

                    try
                    {
                        accessor[targetInstance, targetMemberName] = deserializedValue;
                    }
                    catch ( Exception ex ) { throw new AdeptarException( $"Failed to set member '{targetMemberName}' (source key '{sourceKey}') on type '{targetType.Name}'. Value type: '{deserializedValue?.GetType().Name ?? "null"}'.", ex ); }
                }
            } );

            return targetInstance;
        }

        /// <summary>
        /// Parses key-value pairs from object content span and invokes action for each.
        /// </summary>
        /// <exception cref="AdeptarException">For format errors.</exception>
        private static void ParseKeyValuePairs( ReadOnlySpan<char> objectContentSpan, KeyValueSpanAction processPairAction )
        {
            ArgumentNullException.ThrowIfNull( processPairAction );
            int currentPos = 0;
            bool expectingValue = false;

            while ( currentPos < objectContentSpan.Length )
            {
                int colonPos = FindNextTopLevelDelimiter(objectContentSpan, currentPos, out _, ':', ',');
                if ( colonPos == -1 && !expectingValue )
                {
                    if ( !objectContentSpan.Slice( currentPos ).Trim().IsEmpty )
                        throw new AdeptarException( $"Invalid object format. Unexpected content '{PreviewSpan( objectContentSpan.Slice( currentPos ) )}' found." );
                    break;
                }
                if ( colonPos == -1 && expectingValue )
                {
                    throw new AdeptarException( "Invalid object format. Dangling key without colon found at end." );
                }

                ReadOnlySpan<char> keySpan = objectContentSpan.Slice(currentPos, colonPos - currentPos).Trim();
                if ( keySpan.IsEmpty ) throw new AdeptarException( $"Invalid object format. Missing property name before ':' near position {colonPos}." );

                currentPos = colonPos + 1;

                int commaPos = FindNextTopLevelDelimiter(objectContentSpan, currentPos, out _, ',');
                int valueEndPos = (commaPos == -1) ? objectContentSpan.Length : commaPos;
                ReadOnlySpan<char> valueSpan = objectContentSpan.Slice(currentPos, valueEndPos - currentPos).Trim();

                processPairAction( keySpan, valueSpan );

                if ( commaPos == -1 ) break;
                currentPos = commaPos + 1;
                expectingValue = false;
            }
        }

        /// <summary>
        /// Retrieves a cached <see cref="TypeAccessor"/> for the specified type, creating it if necessary.
        /// Uses FastMember library for reflection optimization.
        /// </summary>
        /// <param name="type">The target <see cref="Type"/>.</param>
        /// <returns>A <see cref="TypeAccessor"/> for the given type.</returns>
        private static TypeAccessor GetOrCreateAccessor( Type type ) => _accessorCache.GetOrAdd( type, t => TypeAccessor.Create( t, true ) );

        /// <summary>
        /// Retrieves a cached map of member names to <see cref="Member"/> objects for the specified type, creating it if necessary.
        /// Uses FastMember to get members and stores them in a dictionary for quick lookups.
        /// </summary>
        /// <param name="type">The target <see cref="Type"/>.</param>
        /// <returns>A dictionary mapping member names (case-sensitive, ordinal) to their corresponding <see cref="Member"/> objects.</returns>
        private static Dictionary<string, Member> GetOrCreateMemberMap( Type type )
        {
            return _memberMapCache.GetOrAdd( type, t => TypeAccessor.Create( t, true ).GetMembers().ToDictionary( m => m.Name, m => m, StringComparer.Ordinal ) );
        }

        /// <summary>
        /// Retrieves or compiles and caches a delegate (<see cref="Func{Object}"/>) that invokes the parameterless constructor for the specified type.
        /// </summary>
        /// <param name="type">The target <see cref="Type"/>.</param>
        /// <returns>A delegate that, when invoked, creates and returns a new instance of the specified type (boxed if it's a value type).</returns>
        /// <exception cref="MissingMethodException">Thrown via the factory delegate if no accessible parameterless constructor is found for a non-struct type.</exception>
        private static Func<object> GetOrCreateDefaultConstructor( Type type )
        {
            return _defaultConstructorCache.GetOrAdd( type, t =>
                {
                    ConstructorInfo ctor = t.GetConstructor(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance, null, Type.EmptyTypes, null);
                    if ( ctor == null && t.IsValueType ) return () => Activator.CreateInstance( t )!;
                    if ( ctor == null ) throw new MissingMethodException( $"No parameterless constructor for {t.FullName}." );
                    NewExpression ne = Expression.New(ctor);
                    Expression body = t.IsValueType ? Expression.Convert(ne, typeof(object)) : (Expression)ne;
                    return Expression.Lambda<Func<object>>( body ).Compile();
                }
            );
        }

        /// <summary>
        /// Creates a new instance of the specified type using its cached parameterless constructor delegate.
        /// </summary>
        /// <param name="type">The <see cref="Type"/> to instantiate.</param>
        /// <returns>A new object instance.</returns>
        private static object CreateInstance( Type type )
        {
            try
            {
                return GetOrCreateDefaultConstructor( type )();
            }
            catch ( Exception ex )
            {
                throw new AdeptarException( $"Failed to create instance of type '{type.FullName}'.", ex );
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