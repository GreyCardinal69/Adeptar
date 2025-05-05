using System;
using System.Collections;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Text;
using static Adeptar.ArrayWriter;
using static Adeptar.ClassWriter;
using static Adeptar.DictionaryWriter;
using static Adeptar.TypeClassifiers;

namespace Adeptar
{
    /// <summary>
    /// Core class that coordinates serialization via recursion.
    /// </summary>
    internal sealed class AdeptarSerializer
    {
        /// <summary>
        /// Serializes the target object into an Adeptar string representation using specified settings.
        /// </summary>
        /// <param name="target">The object to serialize.</param>
        /// <param name="settings">The serialization settings to use.</param>
        /// <returns>The serialized Adeptar string.</returns>
        /// <exception cref="AdeptarException">Thrown if serialization fails.</exception>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="settings"/> is null.</exception>
        internal static string Serialize( object target, AdeptarSettings settings )
        {
            ArgumentNullException.ThrowIfNull( settings );

            SerializableType type = FetchSerializableTypeOf( target );

            StringBuilder builder = new StringBuilder(512);
            var context = new SerializationContext(builder, settings);

            Write( target, type, null, ref context, true, false );

            return builder.ToString();
        }

        /// <summary>
        /// Core recursive method that writes an object's Adeptar representation.
        /// </summary>
        /// <param name="target">Object/value to serialize.</param>
        /// <param name="type">Pre-determined SerializableType.</param>
        /// <param name="propertyName">Name if serializing a member, else null.</param>
        /// <param name="context">Serialization context.</param>
        /// <param name="isLastElement">True if last element in collection/object.</param>
        /// <param name="isAComplexDictionaryKey">True if serializing with a complex dictionary key.</param>
        internal static void Write(
            object target,
            SerializableType type,
            string propertyName,
            ref SerializationContext context,
            bool isLastElement,
            bool isAComplexDictionaryKey )
        {
            if ( target == null && !context.Settings.IgnoreNullValues )
            {
                ApplyIndentationAndName( propertyName, ref context );

                switch ( type )
                {
                    case SerializableType.Simple:
                        context.Builder.Append( target );
                        break;
                    case SerializableType.String:
                        context.Builder.Append( '"' ).Append( '"' );
                        break;
                    case SerializableType.Char:
                        context.Builder.Append( '\'' ).Append( '\'' );
                        break;
                    case SerializableType.Class:
                        context.Builder.Append( '{' ).Append( '}' );
                        break;
                    case SerializableType.Tuple:
                        context.Builder.Append( '(' ).Append( ')' );
                        break;
                    case SerializableType.DateTime:
                        context.Builder.Append( '"' ).Append( '"' );
                        break;
                    case SerializableType.Boolean:
                        context.Builder.Append( "False" );
                        break;
                    default:
                        context.Builder.Append( '[' ).Append( ']' );
                        break;
                }

                AppendCommaAndNewline( ref context, isLastElement );
                return;
            }

            if ( context.Settings.IgnoreDefaultValues && propertyName != null && IsDefaultValue( target, type ) )
            {
                return;
            }

            if ( context.SuppressFullIndentationOnce )
            {
                context.SuppressFullIndentationOnce = false;
            }
            else
            {
                ApplyIndentationAndName( propertyName, ref context );
            }

            try
            {
                switch ( type )
                {
                    case SerializableType.Simple: AppendFormattedSimpleValue( context.Builder, target ); break;
                    case SerializableType.String: AppendEscapedString( context.Builder, Convert.ToString( target, CultureInfo.InvariantCulture ) ); break;
                    case SerializableType.Char: AppendEscapedChar( context.Builder, Convert.ToChar( target, CultureInfo.InvariantCulture ) ); break;
                    case SerializableType.DateTime: context.Builder.Append( '"' ).Append( target ).Append( '"' ); break;
                    case SerializableType.Class: WriteClassInternal( target, ref context ); break;
                    case SerializableType.Array: WriteArrayInternal( target, ref context ); break;
                    case SerializableType.Dictionary: WriteDictionaryInternal( target, isAComplexDictionaryKey, ref context ); break;
                    case SerializableType.Tuple: WriteTupleInternal( target, ref context ); break;
                    case SerializableType.DimensionalArray: WriteDimensionalArrayInternal( target, ref context ); break;
                    default: throw new AdeptarException( "Unknown type for serialization in AdepterSerializer." );
                }
            }
            catch ( AdeptarException ) { throw; }
            catch ( Exception ex )
            {
                throw new AdeptarException( $"Error serializing value{( propertyName == null ? "" : $" for property '{propertyName}'" )}. Type: {target?.GetType()?.FullName ?? "null"}.", ex );
            }

            AppendCommaAndNewline( ref context, isLastElement );
        }

        private static void WriteClassInternal( object target, ref SerializationContext context )
        {
            context.Builder.Append( '{' );
            if ( context.Settings.UseIndentation ) context.Builder.Append( '\n' );
            context.IndentLevel++;
            ClassWriter.WriteClassStruct( target, ref context );
            context.IndentLevel--;
            if ( context.Settings.UseIndentation ) { context.Builder.Append( '\n' ); AppendIndent( context.Builder, context.IndentLevel ); }
            context.Builder.Append( '}' );
        }

        private static void WriteArrayInternal( object target, ref SerializationContext context )
        {
            context.Builder.Append( '[' );
            if ( target is IList list && list.Count > 0 )
            {
                if ( context.Settings.UseIndentation ) context.Builder.Append( '\n' );
                context.IndentLevel++;
                WriteListElements( list, ref context );
                context.IndentLevel--;
                if ( context.Settings.UseIndentation ) AppendIndent( context.Builder, context.IndentLevel );
            }
            context.Builder.Append( ']' );
        }

        private static void WriteDictionaryInternal( object target, bool isComplexDictionaryKey, ref SerializationContext context )
        {
            if ( isComplexDictionaryKey ) context.Builder.Append( '@' );
            context.Builder.Append( '[' );
            if ( target is IDictionary dict && dict.Count > 0 )
            {
                if ( context.Settings.UseIndentation ) context.Builder.Append( '\n' );
                context.IndentLevel++;
                WriteDictionaryElements( dict, ref context );
                context.IndentLevel--;
                if ( context.Settings.UseIndentation ) AppendIndent( context.Builder, context.IndentLevel );
            }
            context.Builder.Append( ']' );
        }

        private static void WriteTupleInternal( object target, ref SerializationContext context )
        {
            context.Builder.Append( '(' );
            if ( target is ITuple tuple && tuple.Length > 0 )
            {
                if ( context.Settings.UseIndentation ) context.Builder.Append( '\n' );
                context.IndentLevel++;
                WriteTuple( target, ref context );
                context.IndentLevel--;
                if ( context.Settings.UseIndentation ) { context.Builder.Append( '\n' ); AppendIndent( context.Builder, context.IndentLevel ); }
            }
            context.Builder.Append( ')' );
        }

        private static void WriteDimensionalArrayInternal( object target, ref SerializationContext context )
        {
            context.Builder.Append( '[' );
            context.IndentLevel++;
            WriteDimensionalArray( target, ref context );
            context.IndentLevel--;
            context.Builder.Append( ']' );
        }

        /// <summary>Applies indentation (if enabled) and writes property name: (if provided).</summary>
        private static void ApplyIndentationAndName( string propertyName, ref SerializationContext context )
        {
            bool needsPropertyIndent = context.Settings.UseIndentation && context.IndentLevel > 0;

            if ( propertyName != null )
            {
                if ( needsPropertyIndent ) AppendIndent( context.Builder, context.IndentLevel );
                context.Builder.Append( propertyName ).Append( ':' );
                if ( context.Settings.UseIndentation ) context.Builder.Append( ' ' );
            }
            else if ( needsPropertyIndent )
            {
                AppendIndent( context.Builder, context.IndentLevel );
            }
        }

        /// <summary>Appends trailing comma (if not last) and newline (if indenting).</summary>
        private static void AppendCommaAndNewline( ref SerializationContext context, bool isLastElement )
        {
            if ( !isLastElement )
            {
                context.Builder.Append( ',' );
            }
            if ( context.Settings.UseIndentation && !context.SuppressNewLineOnce )
            {
                context.Builder.Append( '\n' );
            }
            context.SuppressNewLineOnce = false;
        }

        /// <summary>Appends indentation (tabs) to the StringBuilder.</summary>
        private static void AppendIndent( StringBuilder builder, int level )
        {
            if ( level <= 0 ) return;
            builder.Append( '\t', level );
        }

        /// <summary>Appends a culture-invariant string representation of simple types.</summary>
        private static void AppendFormattedSimpleValue( StringBuilder builder, object value )
        {
            if ( value is Enum ) { builder.Append( value.ToString() ); }
            else if ( value is bool b ) { builder.Append( b ? "True" : "False" ); }
            else if ( value is IFormattable formattable ) { builder.Append( formattable.ToString( null, CultureInfo.InvariantCulture ) ); }
            else { builder.Append( value?.ToString() ?? "null" ); }
        }

        private static void AppendEscapedString( StringBuilder builder, string? value ) // Nullable string
        {
            builder.Append( '"' );

            if ( string.IsNullOrEmpty( value ) )
            {
                builder.Append( '"' );
                return;
            }

            foreach ( char c in value )
            {
                switch ( c )
                {
                    case '"': builder.Append( "\\\"" ); break;
                    case '\\': builder.Append( "\\\\" ); break;
                    case '\n': builder.Append( "\\n" ); break;
                    case '\r': builder.Append( "\\r" ); break;
                    case '\t': builder.Append( "\\t" ); break;
                    case '\b': builder.Append( "\\b" ); break;
                    case '\f': builder.Append( "\\f" ); break;
                    default: builder.Append( c ); break;
                }
            }
            builder.Append( '"' );
        }

        /// <summary>Appends an escaped char literal (including surrounding single quotes).</summary>
        private static void AppendEscapedChar( StringBuilder builder, char value )
        {
            builder.Append( '\'' );
            switch ( value )
            {
                case '\'': builder.Append( "\\'" ); break;
                case '\\': builder.Append( "\\\\" ); break;
                case '\n': builder.Append( "\\n" ); break;
                case '\r': builder.Append( "\\r" ); break;
                case '\t': builder.Append( "\\t" ); break;
                case '\b': builder.Append( "\\b" ); break;
                case '\f': builder.Append( "\\f" ); break;
                default: builder.Append( value ); break;
            }
            builder.Append( '\'' );
        }

        /// <summary>Checks if an object holds the default value for its type.</summary>
        private static bool IsDefaultValue( object value, SerializableType type )
        {
            if ( value == null ) return true;

            switch ( type )
            {
                case SerializableType.Simple:
                    if ( value is int i ) return i == 0;
                    if ( value is bool b ) return b == false;
                    if ( value is double d ) return d == 0.0;
                    if ( value is float f ) return f == 0.0f;
                    if ( value is long l ) return l == 0L;
                    if ( value is decimal m ) return m == 0m;
                    if ( value is DateTime dt ) return dt == default; // DateTime.MinValue
                    if ( value is byte by ) return by == 0;
                    if ( value is sbyte sb ) return sb == 0;
                    if ( value is short sh ) return sh == 0;
                    if ( value is ushort us ) return us == 0;
                    if ( value is uint ui ) return ui == 0u;
                    if ( value is ulong ul ) return ul == 0ul;
                    if ( value is Enum e ) return Convert.ToInt64( e, CultureInfo.InvariantCulture ) == 0;
                    break;
                case SerializableType.Char: return (char)value == '\0';
                case SerializableType.DateTime:
                    if ( value is DateTime dtt ) return dtt == default;
                    if ( value is DateTimeOffset dto ) return dto == default;
                    break;
            }

            Type valueType = value.GetType();
            if ( valueType.IsValueType && !valueType.IsPrimitive && !valueType.IsEnum && valueType != typeof( decimal ) && valueType != typeof( DateTime ) && valueType != typeof( DateTimeOffset ) )
            {
                try { return value.Equals( Activator.CreateInstance( valueType ) ); }
                catch { return false; }
            }

            return false;
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