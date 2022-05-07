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

using static Adeptar.TypeGetters;
using static Adeptar.AdeptarWriter;

using FastMember;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

namespace Adeptar
{
    /// <summary>
    /// Class that handles serialization of class and struct objects.
    /// </summary>
    internal class ClassWriter
    {
        /// <summary>
        /// Cached type for <see cref="AdeptarIgnore"/>.
        /// </summary>
        private static readonly Type _ignoreAttribute = typeof( AdeptarIgnore );

        /// <summary>
        /// Serializes the class or struct object to a .Adeptar string.
        /// </summary>
        /// <param name="target">The object to serialize.</param>
        /// <param name="indent">Indentation amount.</param>
        /// <param name="builder"><see cref="StringBuilder"/> instance to append text to.</param>
        internal static void WriteClassStruct ( object target, int indent, ref StringBuilder builder )
        {
            var accessor = TypeAccessor.Create( target.GetType());
            int count = 0;
            MemberSet vals = accessor.GetMembers();

            foreach (var item in vals)
            {
                object value = accessor[target, item.Name];
                var type = GetSerializableType( item.Type );

                if (item.IsDefined(_ignoreAttribute)){
                    count++;
                    continue;
                }

                switch (type)
                {
                    case SerializableType.DateTime:
                        Write( value, SerializableType.DateTime, ref builder, item.Name, indent, false, true, count == vals.Count - 1 );
                        if (!DoesntUseIndentation){
                            builder.Append( '\n' );
                        }
                        break;
                    case SerializableType.Class:
                        Write( value, type, ref builder, item.Name, indent, false, true, count == vals.Count - 1 );
                        if (!DoesntUseIndentation){
                            builder.Append( '\n' );
                        }
                        break;
                    default:
                        Write( value, type, ref builder, item.Name, indent, false, true, count == vals.Count - 1 );
                        if (!DoesntUseIndentation){
                            builder.Append( '\n' );
                        }
                        break;
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
                        if (!DoesntUseIndentation){
                            builder.Append( '\n' );
                        }
                        break;
                    case SerializableType.Class:
                        Write( param.GetValue( target ), Type, ref builder, param.Name, indent, false, true, count == FieldTypes.Length - 1 );
                        if (!DoesntUseIndentation){
                            builder.Append( '\n' );
                        }
                        break;
                    default:
                        Write( param.GetValue( target ), Type, ref builder, param.Name, indent, false, true, count == FieldTypes.Length - 1 );
                        if (!DoesntUseIndentation){
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
                        if (!DoesntUseIndentation){
                            builder.Append( '\n' );
                        }
                        break;
                    case SerializableType.Class:
                        Write( param.GetValue( target ), Type, ref builder, param.Name, indent, false, true, count == PropertyTypes.Length - 1 );
                        if (!DoesntUseIndentation){
                            builder.Append( '\n' );
                        }
                        break;
                    default:
                        Write( param.GetValue( target ), Type, ref builder, param.Name, indent, false, true, count == PropertyTypes.Length - 1 );
                        if (!DoesntUseIndentation){
                            builder.Append( '\n' );
                        }
                        break;
                }
                count++;
            }
        }
    }
}