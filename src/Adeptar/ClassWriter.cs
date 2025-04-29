using System;
using System.Reflection;
using System.Text;
using FastMember;
using static Adeptar.AdeptarWriter;
using static Adeptar.TypeClassifiers;

namespace Adeptar
{
    /// <summary>
    /// Class that handles serialization of class and struct objects.
    /// </summary>
    internal class ClassWriter
    {
        /// <summary>
        /// Cached <see cref="Type"/> for <see cref="AdeptarConfiguration"/> class.
        /// </summary>
        private static Type _adeptarConfiguration = typeof( AdeptarConfiguration );

        /// <summary>
        /// Default empty instance of an <see cref="AdeptarConfiguration"/> class.
        /// </summary>
        private static AdeptarConfiguration _defaultConfig = new() { ToIgnore = Array.Empty<string>() };

        /// <summary>
        /// Cached type for <see cref="AdeptarIgnoreAttribute"/>.
        /// </summary>
        private static Type _ignoreAttribute = typeof( AdeptarIgnoreAttribute );

        /// <summary>
        /// Serializes the class or struct object to a .Adeptar string.
        /// </summary>
        /// <param name="target">The object to serialize.</param>
        /// <param name="indent">Indentation amount.</param>
        /// <param name="builder"><see cref="StringBuilder"/> instance to append text to.</param>
        internal static void WriteClassStruct( object target, int indent, StringBuilder builder )
        {
            TypeAccessor accessor = TypeAccessor.Create( target.GetType() );

            int count = 0;
            MemberSet vals = accessor.GetMembers();

            int arrayLen = vals.Count;

            if ( CurrentSettings.CheckClassAttributes )
            {
                int last = vals.Count - 1;

                for ( int e = 0; e < arrayLen; e++ )
                {
                    Member item = vals[e];

                    if ( item.IsDefined( _ignoreAttribute ) )
                    {
                        count++;
                        continue;
                    }

                    Type itemType = item.Type;
                    string name = item.Name;
                    object value = accessor[target, name];

                    if ( CurrentSettings.IgnoreNullValues && value is null )
                    {
                        count++;
                        continue;
                    }

                    if ( CurrentSettings.IgnoreDefaultValues )
                    {
                        if ( value is string )
                        {
                            if ( value as string == "" )
                            {
                                count++;
                                continue;
                            }
                            count++;
                        }
                        else if ( Activator.CreateInstance( itemType ).Equals( value ) )
                        {
                            count++;
                            continue;
                        }
                    }

                    FieldPropertyName = name;

                    if ( value is null )
                    {
                        WriteRaw( value, SerializableTypeOf( itemType ), builder, indent, count == last );
                    }
                    else if ( CurrentSettings.UseIndentation )
                    {
                        Write( value, FetchType( value ), builder, indent, count == last, false );
                        builder.Append( '\n' );
                    }
                    else
                    {
                        WriteNoIndentation( value, FetchType( value ), builder, count == last, false );
                    }

                    count++;
                }
            }
            else
            {
                AdeptarConfiguration config = _defaultConfig;

                for ( int i = 0; i < arrayLen; i++ )
                {
                    if ( vals[i].Type == _adeptarConfiguration )
                    {
                        object value = accessor[target, vals[i].Name];
                        config = value is null ? config : (AdeptarConfiguration)value;
                        config.MustBeUsed = true;
                        break;
                    }
                }

                int last = config.ToIgnore.Length > 0 ? vals.Count - 1 + config.ToIgnore.Length : vals.Count - 1;

                for ( int i = 0; i < arrayLen; i++ )
                {
                    Member item = vals[i];

                    Type itemType = item.Type;
                    string name = item.Name;

                    if ( config.MustBeUsed )
                    {
                        if ( itemType == _adeptarConfiguration )
                        {
                            count++;
                            continue;
                        }
                        if ( config.ToIgnore is not null )
                        {
                            bool exit = false;
                            for ( int e = 0; e < config.ToIgnore.Length; e++ )
                            {
                                if ( name == config.ToIgnore[e] )
                                {
                                    count++;
                                    exit = true;
                                    break;
                                }
                            }
                            if ( exit )
                            {
                                count++;
                                continue;
                            }
                        }
                    }

                    object value = accessor[target, name];

                    if ( CurrentSettings.IgnoreNullValues && value is null )
                    {
                        count++;
                        continue;
                    }

                    if ( CurrentSettings.IgnoreDefaultValues )
                    {
                        if ( value is string )
                        {
                            if ( value as string == "" )
                            {
                                count++;
                                continue;
                            }
                            count++;
                        }
                        else if ( Activator.CreateInstance( itemType ).Equals( value ) )
                        {
                            count++;
                            continue;
                        }
                    }

                    FieldPropertyName = name;

                    if ( value is null )
                    {
                        WriteRaw( value, SerializableTypeOf( itemType ), builder, indent, count == last );
                    }
                    else if ( CurrentSettings.UseIndentation )
                    {
                        Write( value, FetchType( value ), builder, indent, count == last, false );
                        builder.Append( '\n' );
                    }
                    else
                    {
                        WriteNoIndentation( value, FetchType( value ), builder, count == last, false );
                    }

                    count++;
                }
            }
        }

        /// <summary>
        /// Serializes the value tuple to a .Adeptar string.
        /// </summary>
        /// <param name="target">The object to serialize.</param>
        /// <param name="indent">Indentation amount.</param>
        /// <param name="builder"><see cref="StringBuilder"/> instance to append text to.</param>
        internal static void WriteTuple( object target, int indent, StringBuilder builder )
        {
            if ( target is null )
            {
                return;
            }

            Type type = target.GetType();
            int count = 0;

            FieldInfo[] FieldTypes = type.GetFields( BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly );

            for ( int i = 0; i < FieldTypes.Length; i++ )
            {
                FieldPropertyName = FieldTypes[i].Name;
                object value = FieldTypes[i].GetValue( target );

                if ( CurrentSettings.UseIndentation )
                {
                    Write( value, FetchType( value ), builder, indent, count == FieldTypes.Length - 1, false );
                    builder.Append( '\n' );
                }
                else
                {
                    WriteNoIndentation( value, FetchType( value ), builder, count == FieldTypes.Length - 1, false );
                }

                count++;
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