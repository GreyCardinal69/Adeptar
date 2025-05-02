using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
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
        /// This is the main public entry point for internal serialization logic.
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

            WriteInternal( target, type, null, ref context, true, false );

            return builder.ToString();
        }


































































        /// <summary>
        /// Serializes the object to a .Adeptar string representation and writes it to a file. Used in the ID feature.
        /// </summary>
        /// <param name="path">The path of the file.</param>
        /// <param name="target">The object to serialize.</param>
        /// <param name="type">The <see cref="SerializableType"/> of the object.</param>
        /// <param name="mode">The serialization mode.</param>
        /// <param name="id">The optional id provided used in the id feature.</param>
        internal static void SerializeWrite( string path, object target, SerializableType type, SerializationMode mode, string id = null )
        {
            if ( mode == SerializationMode.Append || mode == SerializationMode.ChangeAppended )
            {
                List<string> ids = new();
                foreach ( string line in File.ReadLines( path ) )
                {
                    if ( line[0] == '~' && line[line.Length - 1] == '~' && line.Length > 1 )
                    {
                        ids.Add( line );
                    }
                }
                if ( mode == SerializationMode.Append )
                {
                    if ( ids.Contains( $"~{id}~" ) )
                    {
                        throw new AdeptarException( "Can not append the object, an object with the same id already exists." );
                    }
                }
                else
                {
                    if ( !ids.Contains( $"~{id}~" ) )
                    {
                        throw new AdeptarException( $"Can not change appended object with id: {id}, an object with such an id does not exist." );
                    }
                }
            }

            if ( CurrentSettings.UseIndentation )
            {
                Write( target, type, _result, 0, true, false );
            }
            else
            {
                WriteNoIndentation( target, type, _result, true, false );
            }

            switch ( mode )
            {
                case SerializationMode.Append:
                    bool noText = File.ReadAllLines( path ).Length < 2;
                    File.AppendAllText( path, noText ? $"~{id}~\n{_result.ToString()}" : $"\n~{id}~\n{_result.ToString()}" );
                    break;
                case SerializationMode.ChangeAppended:
                    StringBuilder final = new();
                    StringBuilder name = new();
                    bool inString = false;
                    bool falseEnd = false;
                    bool inId = false;
                    bool exit = false;
                    int i = 0, j = 0, w = -1;
                    int index = -1;

                    ReadOnlySpan<char> text = File.ReadAllText( path );

                    foreach ( char item in text )
                    {
                        index++;
                        if ( exit )
                        {
                            break;
                        }
                        switch ( item )
                        {
                            case '"':
                                if ( falseEnd && inString )
                                    falseEnd = false;
                                else if ( !falseEnd )
                                    inString = !inString;
                                break;
                            case '\\':
                                if ( inString )
                                    falseEnd = true;
                                break;
                            case '~':
                                if ( !inString )
                                {
                                    inId = !inId;
                                    if ( inId )
                                    {
                                        w = index;
                                    }
                                    if ( !inId && name.ToString() == id )
                                    {
                                        i = w;
                                    }
                                    if ( inId && i != 0 )
                                    {
                                        j = index;
                                        exit = true;
                                        break;
                                    }
                                    if ( inId && i == 0 )
                                    {
                                        name.Clear();
                                    }
                                }
                                break;
                            default:
                                if ( inId )
                                {
                                    name.Append( item );
                                }
                                break;
                        }
                    }

                    for ( int e = 0; e < i; e++ )
                    {
                        final.Append( text[e] );
                    }

                    final.Append( '~' )
                         .Append( id )
                         .Append( '~' )
                         .Append( '\n' )
                         .Append( _result )
                         .Append( '\n' );

                    if ( j != 0 )
                    {
                        for ( int e = j; e < text.Length; e++ )
                        {
                            final.Append( text[e] );
                        }
                    }

                    File.WriteAllText( path, final.ToString() );
                    break;
                case SerializationMode.SetShared:
                    int start = -1;
                    int end = 0;
                    bool inShared = false;
                    bool leave = false;
                    string fileText = File.ReadAllText( path );

                    foreach ( char ch in fileText )
                    {
                        start++;
                        switch ( ch )
                        {
                            case '&':
                                inShared = !inShared;
                                if ( !inShared )
                                {
                                    leave = true;
                                }
                                else { end = start; }
                                break;
                        }
                        if ( leave )
                        {
                            break;
                        }
                    }

                    if ( end == 0 && start == fileText.Length - 1 )
                    {
                        _result.Insert( 0, '&' ).Append( '&' ).Append( fileText );
                    }
                    else
                    {
                        _result.Insert( 0, '&' ).Append( '&' ).Append( fileText.Remove( end, ( start - end + 1 ) ) );
                    }
                    File.WriteAllText( path, _result.ToString() );
                    break;
                case SerializationMode.Default:
                    File.WriteAllText( path, _result.ToString() );
                    break;
            }
            _result.Clear();
            CurrentSettings = DefaultSettings;
        }

        /// <summary>
        /// The core function that handles serialization of object via recursion.
        /// </summary>
        /// <param name="toSerialize">The object to serialize.</param>
        /// <param name="type">The <see cref="SerializableType"/> of the object.</param>
        /// <param name="mainBuilder">The <see cref="StringBuilder"/> instance text is appended to.</param>
        /// <param name="indent">The amount of indentation.</param>
        /// <param name="last">If true tells the writer to not append a comma.</param>
        /// <param name="addAtSign">True if the object is a complex key of a dictionary.</param>
        internal static void Write( object toSerialize, SerializableType type, StringBuilder mainBuilder,
                                     int indent = 0, bool last = false, bool addAtSign = false )
        {
            for ( int i = 0; i < indent; i++ )
            {
                mainBuilder.Append( '\t' );
            }

            bool calledByClassWriter = false;
            string name = "";

            if ( FieldPropertyName is not null )
            {
                calledByClassWriter = true;
                name = FieldPropertyName;
                FieldPropertyName = null;
            }

            switch ( type )
            {
                case SerializableType.Simple:
                    if ( calledByClassWriter )
                    {
                        mainBuilder.Append( name ).Append( ':' ).Append( ' ' );
                    }
                    mainBuilder.Append( Convert.ToString( toSerialize, CultureInfo.InvariantCulture ) );
                    break;
                case SerializableType.String:
                    if ( calledByClassWriter )
                    {
                        mainBuilder.Append( name ).Append( ':' ).Append( ' ' );
                    }
                    mainBuilder.Append( '"' );
                    string str = Convert.ToString( toSerialize, CultureInfo.InvariantCulture );
                    for ( int i = 0; i < str.Length; i++ )
                    {
                        switch ( str[i] )
                        {
                            case '\n':
                                mainBuilder.Append( "\\n" );
                                break;
                            case '\t':
                                mainBuilder.Append( "\\t" );
                                break;
                            case '\\':
                                mainBuilder.Append( '\\' );
                                break;
                            default:
                                mainBuilder.Append( str[i] );
                                break;
                        }
                    }
                    mainBuilder.Append( '"' );
                    break;
                case SerializableType.Char:
                    if ( calledByClassWriter )
                    {
                        mainBuilder.Append( name ).Append( ':' ).Append( ' ' );
                    }
                    mainBuilder.Append( '\'' ).Append( toSerialize ).Append( '\'' );
                    break;
                case SerializableType.Class:
                    if ( calledByClassWriter )
                    {
                        mainBuilder.Append( name ).Append( ':' ).Append( ' ' );
                    }
                    mainBuilder.Append( '{' );
                    if ( toSerialize is not null )
                    {
                        mainBuilder.Append( '\n' );
                        WriteClassStruct( toSerialize, indent + 1, mainBuilder );
                        for ( int i = 0; i < indent; i++ )
                        {
                            mainBuilder.Append( '\t' );
                        }
                    }
                    mainBuilder.Append( '}' );
                    break;
                case SerializableType.Array:
                    if ( calledByClassWriter )
                    {
                        mainBuilder.Append( name ).Append( ':' ).Append( ' ' );
                    }
                    mainBuilder.Append( '[' );
                    if ( toSerialize is not null )
                    {
                        mainBuilder.Append( '\n' );
                        IList tempList = toSerialize as IList;
                        int count = tempList.Count;
                        for ( int i = 0; i < count; i++ )
                        {
                            Write( tempList[i], FetchSerializableTypeOf( tempList[i] ), mainBuilder, indent + 1, count - 1 == i, false );
                            mainBuilder.Append( '\n' );
                        }
                        for ( int i = 0; i < indent; i++ )
                        {
                            mainBuilder.Append( '\t' );
                        }
                    }
                    mainBuilder.Append( ']' );
                    break;
                case SerializableType.Dictionary:
                    if ( calledByClassWriter )
                    {
                        mainBuilder.Append( name ).Append( ':' ).Append( ' ' );
                    }
                    if ( addAtSign )
                    {
                        mainBuilder.Append( '@' );
                    }
                    mainBuilder.Append( '[' );
                    if ( toSerialize is not null )
                    {
                        mainBuilder.Append( '\n' );
                        WriteDictionary( toSerialize, 1 + indent, mainBuilder );
                        for ( int i = 0; i < indent; i++ )
                        {
                            mainBuilder.Append( '\t' );
                        }
                    }
                    mainBuilder.Append( ']' );
                    break;
                case SerializableType.Tuple:
                    if ( calledByClassWriter )
                    {
                        mainBuilder.Append( name ).Append( ':' ).Append( ' ' );
                    }
                    mainBuilder.Append( '(' );
                    if ( toSerialize is not null )
                    {
                        mainBuilder.Append( '\n' );
                        WriteTuple( toSerialize, 1 + indent, mainBuilder );
                        for ( int i = 0; i < indent; i++ )
                        {
                            mainBuilder.Append( '\t' );
                        }
                    }
                    mainBuilder.Append( ')' );
                    break;
                case SerializableType.DateTime:
                    if ( calledByClassWriter )
                    {
                        mainBuilder.Append( name ).Append( ':' ).Append( ' ' );
                    }
                    mainBuilder.Append( '"' );
                    mainBuilder.Append( toSerialize );
                    mainBuilder.Append( '"' );
                    break;
                case SerializableType.DimensionalArray:
                    if ( calledByClassWriter )
                    {
                        mainBuilder.Append( name ).Append( ':' ).Append( ' ' );
                    }
                    mainBuilder.Append( '[' );
                    if ( toSerialize is not null )
                    {
                        mainBuilder.Append( '\n' );
                        WriteDimensionalArray( toSerialize, 1 + indent, mainBuilder );
                        mainBuilder.Append( '\n' );
                    }
                    mainBuilder.Append( ']' );
                    break;
            }

            if ( !last )
            {
                mainBuilder.Append( ',' );
            }
        }

        /// <summary>
        /// Alternative of <see cref="Write(object, SerializableType, StringBuilder, int, bool, bool)"/>
        /// that doesnt write indentation.
        /// </summary>
        /// <param name="toSerialize">The object to serialize.</param>
        /// <param name="type">The <see cref="SerializableType"/> of the object.</param>
        /// <param name="mainBuilder">The <see cref="StringBuilder"/> instance text is appended to.</param>
        /// <param name="last">If true tells the writer to not append a comma.</param>
        /// <param name="addAtSign">True if the object is a complex key of a dictionary.</param>
        internal static void WriteNoIndentation( object toSerialize, SerializableType type, StringBuilder mainBuilder,
                                                  bool last = false, bool addAtSign = false )
        {
            bool calledByClassWriter = false;
            string name = "";

            if ( FieldPropertyName is not null )
            {
                calledByClassWriter = true;
                name = FieldPropertyName;
                FieldPropertyName = null;
            }

            switch ( type )
            {
                case SerializableType.Simple:
                    if ( calledByClassWriter )
                    {
                        mainBuilder.Append( name ).Append( ':' ).Append( ' ' );
                    }
                    mainBuilder.Append( toSerialize );
                    break;
                case SerializableType.String:
                    if ( calledByClassWriter )
                    {
                        mainBuilder.Append( name ).Append( ':' ).Append( ' ' );
                    }
                    mainBuilder.Append( '"' );
                    string str = Convert.ToString( toSerialize, CultureInfo.InvariantCulture );
                    for ( int i = 0; i < str.Length; i++ )
                    {
                        switch ( str[i] )
                        {
                            case '\n':
                                mainBuilder.Append( "\\n" );
                                break;
                            case '\t':
                                mainBuilder.Append( "\\t" );
                                break;
                            case '\\':
                                mainBuilder.Append( '\\' );
                                break;
                            default:
                                mainBuilder.Append( str[i] );
                                break;
                        }
                    }
                    mainBuilder.Append( '"' );
                    break;
                case SerializableType.Char:
                    if ( calledByClassWriter )
                    {
                        mainBuilder.Append( name ).Append( ':' ).Append( ' ' );
                    }
                    mainBuilder.Append( '\'' ).Append( toSerialize ).Append( '\'' );
                    break;
                case SerializableType.Class:
                    if ( calledByClassWriter )
                    {
                        mainBuilder.Append( name ).Append( ':' ).Append( ' ' );
                    }
                    mainBuilder.Append( '{' );
                    if ( toSerialize is not null )
                    {
                        WriteClassStruct( toSerialize, 0, mainBuilder );
                    }
                    mainBuilder.Append( '}' );
                    break;
                case SerializableType.Array:
                    if ( calledByClassWriter )
                    {
                        mainBuilder.Append( name ).Append( ':' ).Append( ' ' );
                    }
                    mainBuilder.Append( '[' );
                    if ( toSerialize is not null )
                    {
                        IList tempList = toSerialize as IList;
                        int count = tempList.Count;
                        for ( int i = 0; i < count; i++ )
                        {
                            WriteNoIndentation( tempList[i], FetchSerializableTypeOf( tempList[i] ), mainBuilder, count - 1 == i, false );
                        }
                    }
                    mainBuilder.Append( ']' );
                    break;
                case SerializableType.Dictionary:
                    if ( calledByClassWriter )
                    {
                        mainBuilder.Append( name ).Append( ':' ).Append( ' ' );
                    }
                    if ( addAtSign )
                    {
                        mainBuilder.Append( '@' );
                    }
                    mainBuilder.Append( '[' );
                    if ( toSerialize is not null )
                    {
                        WriteDictionary( toSerialize, 0, mainBuilder );
                    }
                    mainBuilder.Append( ']' );
                    break;
                case SerializableType.Tuple:
                    if ( calledByClassWriter )
                    {
                        mainBuilder.Append( name ).Append( ':' ).Append( ' ' );
                    }
                    mainBuilder.Append( '(' );
                    if ( toSerialize is not null )
                    {
                        WriteTuple( toSerialize, 0, mainBuilder );
                    }
                    mainBuilder.Append( ')' );
                    break;
                case SerializableType.DateTime:
                    if ( calledByClassWriter )
                    {
                        mainBuilder.Append( name ).Append( ':' ).Append( ' ' );
                    }
                    mainBuilder.Append( '"' );
                    mainBuilder.Append( toSerialize );
                    mainBuilder.Append( '"' );
                    break;
                case SerializableType.DimensionalArray:
                    if ( calledByClassWriter )
                    {
                        mainBuilder.Append( name ).Append( ':' ).Append( ' ' );
                    }
                    mainBuilder.Append( '[' );
                    if ( toSerialize is not null )
                    {
                        WriteDimensionalArray( toSerialize, 0, mainBuilder );
                    }
                    mainBuilder.Append( ']' );
                    break;
            }

            if ( !last )
            {
                mainBuilder.Append( ',' );
            }
        }

        /// <summary>
        /// Called by <see cref="ClassWriter"/> to write null objects.
        /// </summary>
        /// <param name="toSerialize">The object to serialize.</param>
        /// <param name="type">The <see cref="SerializableType"/> of the object.</param>
        /// <param name="mainBuilder">The <see cref="StringBuilder"/> instance text is appended to.</param>
        /// <param name="indent">The amount of indentation.</param>
        /// <param name="last">If true tells the writer to not append a comma.</param>
        internal static void WriteRaw( object toSerialize, SerializableType type, StringBuilder mainBuilder, int indent, bool last )
        {
            if ( AdeptarWriter.CurrentSettings.UseIndentation )
            {
                for ( int i = 0; i < indent; i++ )
                {
                    mainBuilder.Append( '\t' );
                }
            }

            mainBuilder.Append( FieldPropertyName );
            mainBuilder.Append( ':' );
            mainBuilder.Append( ' ' );

            FieldPropertyName = null;

            switch ( type )
            {
                case SerializableType.Simple:
                    mainBuilder.Append( toSerialize );
                    break;
                case SerializableType.String:
                    mainBuilder.Append( '"' ).Append( '"' );
                    break;
                case SerializableType.Char:
                    mainBuilder.Append( '\'' ).Append( '\'' );
                    break;
                case SerializableType.Class:
                    mainBuilder.Append( '{' ).Append( '}' );
                    break;
                case SerializableType.Tuple:
                    mainBuilder.Append( '(' ).Append( ')' );
                    break;
                case SerializableType.DateTime:
                    mainBuilder.Append( '"' ).Append( '"' );
                    break;
                default:
                    mainBuilder.Append( '[' ).Append( ']' );
                    break;
            }

            if ( !last )
            {
                mainBuilder.Append( ',' );
            }

            if ( AdeptarWriter.CurrentSettings.UseIndentation )
            {
                mainBuilder.Append( '\n' );
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