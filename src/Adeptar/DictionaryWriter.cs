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
        /// <param name="data">The <see cref="Dictionary{TKey, TValue}"/> to serialize.</param>
        /// <param name="Indent">The indentation amount.</param>
        /// <param name="Builder">Main instance of <see cref="StringBuilder"/> to append to.</param>
        internal static void WriteDictionary( object data, int Indent, StringBuilder Builder )
        {
            if (data is null){
                return;
            }

            bool isIndended = Indent > 1;
            IDictionary dict = data as IDictionary;

            (object, object)[] keyVals = new (object, object)[dict.Count];

            int count = 0;

            foreach (object item in dict.Keys){
                keyVals[count] = (item, dict[item]);
                count++;
            }
            count = 0;

            if (!AdeptarWriter.CurrentSettings.UseIndentation){
                for (int i = 0; i < dict.Count; i++)
                {
                    if (IsDictionary( dict[i] )){
                        WriteNoIndentation( keyVals[i].Item1, FetchType( keyVals[i].Item1 ), Builder, true, true );
                        Builder.Append( ':' );
                        if (Indent >= 1){
                            Builder.Append( '[' );
                        }
                        WriteDictionary( keyVals[i].Item2, 0, Builder );
                        Builder.Append( ']' );
                        if (count != keyVals.Length - 1){
                            Builder.Append( ',' );
                        }
                    }else{
                        SerializableType RootType = FetchType( keyVals[i].Item2 );
                        if (RootType == SerializableType.Array){
                            WriteNoIndentation( keyVals[i].Item1, FetchType( keyVals[i].Item1 ), Builder, true, true );
                            Builder.Append( ':' );
                            WriteNoIndentation( keyVals[i].Item2, RootType, Builder, true, false );
                            if (count != keyVals.Length - 1){
                                Builder.Append( ',' );
                            }
                        }else{
                            WriteNoIndentation( keyVals[i].Item1, FetchType( keyVals[i].Item1 ), Builder, true, true );
                            Builder.Append( ':' );
                            WriteNoIndentation( keyVals[i].Item2, RootType, Builder, count == dict.Count - 1, false );
                        }
                    }
                    count++;
                }
            }else{
                for (int i = 0; i < keyVals.Length; i++)
                {
                    if (IsDictionary( keyVals[i].Item2 )){
                        Write( keyVals[i].Item1, FetchType( keyVals[i].Item1 ), Builder, Indent, true, true );
                        Builder.Append( ':' ).Append( '\n' );
                        if (Indent >= 1){
                            for (int w = 0; w < Indent; w++)
                            {
                                Builder.Append( '\t' );
                            }
                            Builder.Append( '[' ).Append( '\n' );
                        }
                        isIndended = true;
                        WriteDictionary( keyVals[i].Item2, Indent + 1, Builder );
                        for (int w = 0; w < Indent; w++)
                        {
                            Builder.Append( '\t' );
                        }
                        Builder.Append( ']' );
                        if (count != keyVals.Length - 1){
                            Builder.Append( ',' );
                        }
                        Builder.Append( '\n' );
                    }else{
                        SerializableType RootType = FetchType( keyVals[i].Item2 );
                        if (RootType == SerializableType.Array){
                            Write( keyVals[i].Item1, FetchType( keyVals[i].Item1 ), Builder, Indent, true, false );
                            Builder.Append( ':' );
                            Builder.Append( '\n' );
                            Write( keyVals[i].Item2, RootType, Builder, Indent, true, false );
                            if (count != keyVals.Length - 1){
                                Builder.Append( ',' );
                            }
                        }else{
                            Write( keyVals[i].Item1, FetchType( keyVals[i].Item1 ), Builder, isIndended ? Indent : 0, true, false );
                            Builder.Append( ':' );
                            Write( keyVals[i].Item2, RootType, Builder, 0, count == dict.Count - 1, false );
                        }
                            Builder.Append( '\n' );
                        
                    }
                    count++;
                }
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