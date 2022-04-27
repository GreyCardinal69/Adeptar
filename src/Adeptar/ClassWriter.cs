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

namespace Adeptar
{
    /// <summary>
    /// Class that handles serialization of class and struct objects.
    /// Does not handle <see cref="SerializableType.NULL"/> case.
    /// </summary>
    internal class ClassWriter
    {
        /// <summary>
        /// Cached type for <see cref="AdeptarIgnore"/>.
        /// </summary>
        private static readonly Type _ignoreAttribute = typeof( AdeptarIgnore );

        /// <summary>
        /// Serializes the class or struct object to a .Adeptarstring.
        /// </summary>
        /// <param name="target">The object to serialize.</param>
        /// <param name="indent">Indentation amount.</param>
        /// <param name="builder"><see cref="StringBuilder"/> instance to append text to.</param>
        internal static void WriteClassStruct ( object target, int indent, ref StringBuilder builder )
        {
            var accessor = TypeAccessor.Create( target.GetType() );
            int count = 0;
            MemberSet vals = accessor.GetMembers();

            foreach (var item in vals)
            {
                object value = accessor[target, item.Name];
                var type = FetchType( value );

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
                        // Checks if the object is not a literal new object(), this caused issues.
                        if (!string.Equals( item.Name, "Object", System.StringComparison.OrdinalIgnoreCase )){
                            Write( value, type, ref builder, item.Name, indent, false, true, count == vals.Count - 1 );
                            // Apply a new line if indentation is enabled.
                            if (!DoesntUseIndentation){
                                builder.Append( '\n' );
                            }
                        }
                        break;
                    default:
                        if (type != SerializableType.NULL){
                            Write( value, type, ref builder, item.Name, indent, false, true, count == vals.Count - 1 );
                            if (!DoesntUseIndentation){
                                builder.Append( '\n' );
                            }
                        }
                        break;
                }
                count++;
            }
        }
    }
}