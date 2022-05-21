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
using System.Text;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

using FastMember;

using static Adeptar.TypeGetters;
using static Adeptar.AdeptarWriter;
using System.Linq;

namespace Adeptar
{
    /// <summary>
    /// Class that handles serialization of class and struct objects.
    /// </summary>
    internal class ClassWriter
    {
        internal static Type _adeptarConfiguration = typeof( AdeptarConfiguration );

        /// <summary>
        /// Default empty instance of an <see cref="AdeptarConfiguration"/> class.
        /// </summary>
        internal static AdeptarConfiguration defaultConfig = new();

        /// <summary>
        /// Serializes the class or struct object to a .Adeptar string.
        /// </summary>
        /// <param name="target">The object to serialize.</param>
        /// <param name="indent">Indentation amount.</param>
        /// <param name="builder"><see cref="StringBuilder"/> instance to append text to.</param>
        internal static void WriteClassStruct ( object target, int indent, ref StringBuilder builder )
        {
            var accessor = TypeAccessor.Create( target.GetType() );
            int count = 0;
            MemberSet vals = accessor.GetMembers();

            AdeptarConfiguration config = defaultConfig;

            foreach (var item in vals)
            {
                if (item.Type == _adeptarConfiguration){
                    var value = accessor[target, item.Name];
                    config = value == null ? config : ( AdeptarConfiguration ) value;
                    config.SetName( item.Name );
                    break;
                }
            }

            foreach (var item in vals)
            {
                var itemType = item.Type;

                if (itemType == _adeptarConfiguration){
                    count++;
                    continue;
                }

                string name = item.Name;

                if (config.ToIgnore is not null){
                    bool exit = false;
                    for (int i = 0; i < config.ToIgnore.Length; i++)
                    {
                        if (name == config.ToIgnore[i]){
                            count++;
                            exit = true;
                            break;
                        }
                    }
                    if (exit){
                        count++;
                        continue;
                    }
                }

                var type = GetSerializableType( itemType );

                object value = accessor[target, name];
                Write( value, type, ref builder, name, indent, true, count == vals.Count - 1, false );

                if (AdeptarWriter.CurrentSettings.UseIndentation){
                    builder.Append( '\n' );
                }
                count++;
            }
        }

        /// <summary>
        /// Serializes the value tuple to a .Adeptar string.
        /// </summary>
        /// <param name="target">The object to serialize.</param>
        /// <param name="indent">Indentation amount.</param>
        /// <param name="builder"><see cref="StringBuilder"/> instance to append text to.</param>
        internal static void WriteTuple ( object target, int indent, ref StringBuilder builder )
        {
            var type = target.GetType();
            int count = 0;

            FieldInfo[] FieldTypes = type.GetFields( BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly );

            foreach (var param in FieldTypes)
            {
                var Type = GetSerializableType( param.FieldType );

                switch (Type)
                {
                    case SerializableType.DateTime:
                        Write( param.GetValue( target ), SerializableType.DateTime, ref builder, param.Name, indent, false, true, count == FieldTypes.Length - 1 );
                        if (AdeptarWriter.CurrentSettings.UseIndentation){
                            builder.Append( '\n' );
                        }
                        break;
                    case SerializableType.Class:
                        Write( param.GetValue( target ), Type, ref builder, param.Name, indent, false, true, count == FieldTypes.Length - 1 );
                        if (AdeptarWriter.CurrentSettings.UseIndentation){
                            builder.Append( '\n' );
                        }
                        break;
                    default:
                        Write( param.GetValue( target ), Type, ref builder, param.Name, indent, false, true, count == FieldTypes.Length - 1 );
                        if (AdeptarWriter.CurrentSettings.UseIndentation){
                            builder.Append( '\n' );
                        }
                        break;
                }
                count++;
            }

            PropertyInfo[] PropertyTypes = type.GetProperties( BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly );

            foreach (var param in PropertyTypes)
            {
                var Type = GetSerializableType( param.PropertyType );

                switch (Type)
                {
                    case SerializableType.DateTime:
                        Write( param.GetValue( target ), SerializableType.DateTime, ref builder, param.Name, indent, false, true, count == PropertyTypes.Length - 1 );
                        if (AdeptarWriter.CurrentSettings.UseIndentation){
                            builder.Append( '\n' );
                        }
                        break;
                    case SerializableType.Class:
                        Write( param.GetValue( target ), Type, ref builder, param.Name, indent, false, true, count == PropertyTypes.Length - 1 );
                        if (AdeptarWriter.CurrentSettings.UseIndentation){
                            builder.Append( '\n' );
                        }
                        break;
                    default:
                        Write( param.GetValue( target ), Type, ref builder, param.Name, indent, false, true, count == PropertyTypes.Length - 1 );
                        if (AdeptarWriter.CurrentSettings.UseIndentation){
                            builder.Append( '\n' );
                        }
                        break;
                }
                count++;
            }
        }
    }
}