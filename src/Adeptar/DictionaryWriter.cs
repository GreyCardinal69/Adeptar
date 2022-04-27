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

using System.Collections;
using System.Collections.Generic;
using System.Text;

using static Adeptar.AdeptarWriter;
using static Adeptar.TypeGetters;

namespace Adeptar
{
    internal static class DictionaryWriter
    {
        /// <summary>
        /// Serializes <see cref="Dictionary{TKey, TValue}"/>s to.Adeptar strings.
        /// </summary>
        /// <param name="data">The <see cref="Dictionary{TKey, TValue}"/>.</param>
        /// <param name="Indent">The indentation amount.</param>
        /// <param name="Builder">Main instance of <see cref="StringBuilder"/> to append to.</param>
        /// <param name="isIndended">Is the dictionary nested?</param>
        private static void WriteDictionaryInternal( object data, int Indent, ref StringBuilder Builder, bool isIndended = false )
        {
            var dict = data as IDictionary;
            List<(object, object)> x = new();

            int count = 0;

            foreach (var item in dict.Keys){
                x.Add( (item, null) );
            }
            foreach (var item in dict.Values){
                x[count] = (x[count].Item1, item);
                count++;
            }
            count = 0;

            if (DoesntUseIndentation){
                for (int i = 0; i < x.Count; i++)
                {
                    if (IsDictionary( x[i].Item2 )){
                        Write( x[i].Item1, FetchType( x[i].Item1 ), ref Builder, null, Indent, true, false, true );
                        Builder.Append( ':' );
                        if (Indent >= 1){
                            Builder.Append( '[' );
                        }
                        isIndended = true;
                        WriteDictionary( x[i].Item2, Indent + 1, ref Builder );
                        Builder.Append( ']' );
                        if (count != x.Count - 1){
                            Builder.Append( ',' );
                        }
                    }else{
                        SerializableType RootType = FetchType( x[i].Item2 );
                        if (RootType == SerializableType.Array){
                            Write( x[i].Item1, FetchType( x[i].Item1 ), ref Builder, null, Indent, true, false, true );
                            Builder.Append( ':' );
                            Write( x[i].Item2, FetchType( x[i].Item2 ), ref Builder, null, Indent, true, false, true );
                            if (count != x.Count - 1){
                                Builder.Append( ',' );
                            }
                        }else{
                            Write( x[i].Item1, FetchType( x[i].Item1 ), ref Builder, null, isIndended ? Indent : 0, true, false, true );
                            Builder.Append( ':' );
                            Write( x[i].Item2, RootType, ref Builder, null, 0, true, false, count == dict.Count - 1 );
                        }
                    }
                    count++;
                }
            }else{
                for (int i = 0; i < x.Count; i++)
                {
                    if (IsDictionary( x[i].Item2 )){
                        Write( x[i].Item1, FetchType( x[i].Item1 ), ref Builder, null, Indent, true, false, true );
                        Builder.Append( ':' ).Append( '\n' );
                        if (Indent >= 1){
                            for (int w = 0; w < Indent; w++)
                            {
                                Builder.Append( '\t' );
                            }
                            Builder.Append( '[' ).Append( '\n' );
                        }
                        isIndended = true;
                        WriteDictionary( x[i].Item2, Indent + 1, ref Builder );
                        for (int w = 0; w < Indent; w++)
                        {
                            Builder.Append( '\t' );
                        }
                        Builder.Append( ']' );
                        if (count != x.Count - 1){
                            Builder.Append( ',' );
                        }
                        Builder.Append( '\n' );
                    }else{
                        SerializableType RootType = FetchType( x[i].Item2 );
                        if (RootType == SerializableType.Array){
                            Write( x[i].Item1, FetchType( x[i].Item1 ), ref Builder, null, Indent, true, false, true );
                            Builder.Append( ':' );
                            Builder.Append( '\n' );
                            Write( x[i].Item2, FetchType( x[i].Item2 ), ref Builder, null, Indent, true, false, true );
                            if (count != x.Count - 1){
                                Builder.Append( ',' );
                            }
                        }else{
                            Write( x[i].Item1, FetchType( x[i].Item1 ), ref Builder, null, isIndended ? Indent : 0, true, false, true );
                            Builder.Append( ':' );
                            Write( x[i].Item2, RootType, ref Builder, null, 0, true, false, count == dict.Count - 1 );
                        }
                        if (isIndended){
                            Builder.Append( '\n' );
                        }
                    }
                    count++;
                }
            }
        }

        /// <summary>
        /// Wrapper for <see cref="WriteDictionaryInternal(object, int, ref StringBuilder, bool)"/>.
        /// </summary>
        /// <param name="Dict">The received <see cref="Dictionary{TKey, TValue}"/></param>
        /// <param name="Indent">The indentation amount.</param>
        /// <param name="MainBuilder">The main instance of a <see cref="StringBuilder"/> to append to.</param>
        internal static void WriteDictionary ( object Dict, int Indent, ref StringBuilder MainBuilder )
        {
            bool IsIntended = Indent > 0;
            WriteDictionaryInternal( Dict, Indent, ref MainBuilder, IsIntended );
        }
    }
}