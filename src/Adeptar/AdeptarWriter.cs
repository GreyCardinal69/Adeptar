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
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Linq;

using static Adeptar.ClassWriter;
using static Adeptar.ArrayWriter;
using static Adeptar.DictionaryWriter;
using static Adeptar.TypeGetters;

namespace Adeptar
{
    /// <summary>
    /// Core class than coordinates serialization via recursion.
    /// </summary>
    internal class AdeptarWriter
    {
        /// <summary>
        /// The main instance of a <see cref="StringBuilder"/> the text is appended to.
        /// </summary>
        private static StringBuilder _result = new(32);

        /// <summary>
        /// A static bool used across Adeptar serialization files to control indentation.
        /// </summary>
        internal static bool DoesntUseIndentation;

        /// <summary>
        /// Serializes the object to a .Adeptar string representation and writes it to a file. Used in the ID feature.
        /// </summary>
        /// <param name="path">The path of the file.</param>
        /// <param name="target">The object to serialize.</param>
        /// <param name="type">The <see cref="SerializableType"/> of the object.</param>
        /// <param name="mode">The serialization mode.</param>
        /// <param name="id">The optional id provided used in the id feature.</param>
        internal static void SerializeWrite ( string path, object target, SerializableType type, SerializationMode mode, string id = null )
        {
            bool simple = false;

            if (mode == SerializationMode.Append){
                List<string> ids = new();
                foreach (var line in File.ReadLines( path ))
                {
                    if (line.StartsWith( '~' ) && line.Length > 1){
                        ids.Add( line );
                    }
                }
                if (ids.Contains( $"~{id}" )){
                    throw new AdeptarException( "Can not append the object, an object with the same id already exists." );
                }
            }

            Write( target, type, ref _result, null, 0, false, true, false );

            switch (mode)
            {
                case SerializationMode.Append:
                    bool noText = File.ReadAllLines( path ).Length < 2;
                    if (simple){
                        File.AppendAllText( path, noText
                            ? $"~{id}~\n" + _result.Append( '\n' ).ToString()
                            : $"\n~{id}~\n" + _result.Append( '\n' ).ToString() );
                    }else{
                        File.AppendAllText( path, noText
                            ? $"~{id}~\n" + _result.ToString()
                            : $"\n~{id}~\n" + _result.ToString() );
                    }
                    break;
                case SerializationMode.ChangeAppended:
                    StringBuilder final = new();
                    StringBuilder name = new();
                    bool inString = false;
                    bool falseEnd = false;
                    bool inId = false;
                    bool exit = false;
                    int i = 0, j = 0, w = 0;
                    ReadOnlySpan<char> text = File.ReadAllText( path );
                    foreach (var item in text)
                    {
                        if (exit){
                            break;
                        }
                        switch (item)
                        {
                            case '"':
                                if (falseEnd && inString)
                                    falseEnd = false;
                                else if (!falseEnd)
                                    inString = !inString;
                                break;
                            case '\\':
                                if (inString)
                                    falseEnd = true;
                                break;
                            case '~':
                                if (!inString){
                                    inId = !inId;
                                    if (inId){
                                        if (i == 0 && j == 0){
                                            i = w;
                                        }
                                        else if ( j != 0){
                                            i = w;
                                        }else{
                                            j = w;
                                        }
                                    }else{
                                        if (name.ToString() == id){
                                            for (int e = 0; e < i; e++)
                                            {
                                                final.Append( text[e] );
                                            }
                                            final
                                                .Append( '~' )
                                                .Append( name )
                                                .Append( '~' )
                                                .Append('\n')
                                                .Append( _result )
                                                .Append('\n');
                                            for (int e = text.Length - i; e < text.Length; e++)
                                            {
                                                final.Append( text[e] );
                                            }
                                            File.WriteAllText( path, final.ToString() );
                                            exit = true;
                                            break;
                                        }else{
                                            name.Clear();
                                        }
                                    }
                                }
                                break;
                            default:
                                if (inId){
                                    name.Append( item );
                                }
                                break;
                        }
                        w++;
                    }
                    break;
                case SerializationMode.Default:
                    File.WriteAllText( path, _result.ToString() );
                    break;
            }
            _result.Clear();
            DoesntUseIndentation = false;
        }

        /// <summary>
        /// Serializes the object to a .Adeptar string representation.
        /// </summary>
        /// <param name="target">The object to serialize.</param>
        /// <param name="type">The <see cref="SerializableType"/> of the object.</param>
        /// <returns></returns>
        internal static string Serialize ( object target, SerializableType type )
        {
            Write( target, type, ref _result, null, 0, false, true, false );
            string final = _result.ToString();
            _result.Clear();
            return final;
        }

        /// <summary>
        /// The core function that handles serialization of object via recursion.
        /// </summary>
        /// <param name="toSerialize">The object to serialize.</param>
        /// <param name="type">The <see cref="SerializableType"/> of the object.</param>
        /// <param name="mainBuilder">The <see cref="StringBuilder"/> instance text is appended to.</param>
        /// <param name="name">The name of the field or property of a class.</param>
        /// <param name="indent">The amount of indentation.</param>
        /// <param name="calledByClassWriter">Tells the writer that the function was class from <see cref="ClassWriter"/>.</param>
        /// <param name="last">If true tells the writer to not append a comma.</param>
        /// <param name="addAtSign">True if the object is a complex key of a dictionary.</param>
        internal static void Write ( object toSerialize, SerializableType type, ref StringBuilder mainBuilder,
                                     string name = null, int indent = 0, bool calledByClassWriter = false, bool last = false, bool addAtSign = false )
        {
            if (!DoesntUseIndentation){
                for (int i = 0; i < indent; i++)
                {
                    mainBuilder.Append( '\t' );
                }
            }

            switch (type)
            {
                case SerializableType.Simple:
                    mainBuilder.Append( name );
                    if (calledByClassWriter){
                        mainBuilder.Append( ": " );
                    }
                    mainBuilder.Append( toSerialize );
                    break;
                case SerializableType.String:
                    if (calledByClassWriter){
                        mainBuilder.Append( name );
                        mainBuilder.Append( ": \"" );
                    }else{
                        mainBuilder.Append( '"' );
                    }
                    mainBuilder.Append(
                        Convert.ToString( toSerialize )
                        .Replace( "\t", "\\t" )
                        .Replace( "\n", "\\n" )
                        .Replace( "\"", "\\\"")
                    );
                    mainBuilder.Append( '"' );
                    break;
                case SerializableType.Char:
                    if (calledByClassWriter){
                        mainBuilder.Append( name );
                        mainBuilder.Append( ": " );
                    }
                    mainBuilder.Append( '\'' ).Append( toSerialize ).Append( '\'' );
                    break;
                case SerializableType.Class:
                    if (calledByClassWriter){
                        mainBuilder.Append( name );
                        mainBuilder.Append( ": " );
                    }
                    mainBuilder.Append( '{' );
                    if (!DoesntUseIndentation){
                        mainBuilder.Append( '\n' );
                    }
                    WriteClassStruct( toSerialize, indent + 1, ref mainBuilder );
                    if (!DoesntUseIndentation){
                        for (int i = 0; i < indent; i++)
                        {
                            mainBuilder.Append( '\t' );
                        }
                    }
                    mainBuilder.Append( '}' );
                    break;
                case SerializableType.Array:
                    if (calledByClassWriter){
                        mainBuilder.Append( name );
                        mainBuilder.Append( ": " );
                    }
                    mainBuilder.Append( '[' );
                    if (!DoesntUseIndentation){
                        mainBuilder.Append( '\n' );
                    }
                    if (toSerialize is IEnumerable tempList){
                        var enumerator = tempList.GetEnumerator();
                        var final = enumerator.MoveNext();
                        foreach (var item in tempList)
                        {
                            if (item is IList){
                                WriteArray( toSerialize, 1 + indent, ref mainBuilder );
                                break;
                            }
                            Write( item, FetchType( item ), ref mainBuilder, null, indent + 1, false, ( final == !enumerator.MoveNext() ), false );
                            if (!DoesntUseIndentation){
                                mainBuilder.Append( '\n' );
                            }
                        }
                    }
                    if (!DoesntUseIndentation){
                        for (int i = 0; i < indent; i++)
                        {
                            mainBuilder.Append( '\t' );
                        }
                    }
                    mainBuilder.Append( ']' );
                    break;
                case SerializableType.Dictionary:
                    if (calledByClassWriter){
                        mainBuilder.Append( name );
                        mainBuilder.Append( ": " );
                    }
                    if (addAtSign){
                        mainBuilder.Append('@');
                    }
                    mainBuilder.Append( '[' );
                    if (!DoesntUseIndentation){
                        mainBuilder.Append( '\n' );
                    }
                    if (toSerialize != null){
                        WriteDictionary( toSerialize, 1 + indent, ref mainBuilder );
                        if (!DoesntUseIndentation){
                            for (int i = 0; i < indent; i++)
                            {
                                mainBuilder.Append( '\t' );
                            }
                        }
                    }else{
                        for (int i = 0; i < indent; i++)
                        {
                            mainBuilder.Append( '\t' );
                        }
                    }
                    mainBuilder.Append( ']' );
                    break;
                case SerializableType.Tuple:
                    if (calledByClassWriter){
                        mainBuilder.Append( name );
                        mainBuilder.Append( ": " );
                    }
                    if (!DoesntUseIndentation){
                        mainBuilder.Append( '(' ).Append( '\n' );
                    }else{
                        mainBuilder.Append( '(' );
                    }
                    WriteTuple( toSerialize, 1 + indent, ref mainBuilder );
                    if (!DoesntUseIndentation){
                        for (int i = 0; i < indent; i++)
                        {
                            mainBuilder.Append( '\t' );
                        }
                    }
                    mainBuilder.Append( ')' );
                    break;
                case SerializableType.DateTime:
                    mainBuilder.Append( name );
                    if (calledByClassWriter){
                        mainBuilder.Append( ": \"" );
                    }else{
                        mainBuilder.Append( '"' );
                    }
                    mainBuilder.Append( toSerialize.ToString() );
                    mainBuilder.Append( '"' );
                    break;
                case SerializableType.DimensionalArray:
                    if (calledByClassWriter){
                        mainBuilder.Append( name );
                        mainBuilder.Append( ": " );
                    }
                    mainBuilder.Append( '[' );
                    if (!DoesntUseIndentation){
                        mainBuilder.Append( '\n' );
                    }
                    WriteDimensionalArray( toSerialize, 1 + indent, ref mainBuilder );
                    if (!DoesntUseIndentation){
                        mainBuilder.Append( '\n' );
                    }
                    mainBuilder.Append( ']' );
                    break;
            }

            if (!last){
                mainBuilder.Append( ',' );
            }
        }
    }
}