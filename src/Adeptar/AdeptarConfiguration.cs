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
using System.Collections.Generic;

namespace Adeptar
{
    /// <summary>
    /// A class that provides functionality for field/property serialization manipulations.
    /// Such objects are to be put as a field/property only once and are not serialized by the <see cref="AdeptarWriter"/>.
    /// </summary>
    public sealed class AdeptarConfiguration
    {
        /// <summary>
        /// Creates a new empty instance of an <see cref="AdeptarConfiguration"/> class.
        /// </summary>
        public AdeptarConfiguration () { }

        /// <summary>
        /// Creates a new instance of an <see cref="AdeptarConfiguration"/> class with an array of fields and or properties to ignore.
        /// </summary>
        /// <param name="toIgnore"></param>
        public AdeptarConfiguration ( string[] toIgnore )
        {
            ToIgnore = toIgnore;
        }

        /// <summary>
        /// Creates a new instance of an <see cref="AdeptarConfiguration"/> class with an array of fields and or properties to ignore.
        /// As well as a <see cref="Dictionary{TKey, TValue}"/> of mappings for fields and properties.
        /// </summary>
        /// <param name="toIgnore"></param>
        /// <param name="reMaps"></param>
        public AdeptarConfiguration ( string[] toIgnore, Dictionary<string, string> reMaps  )
        {
            ToIgnore = toIgnore;
            Remaps = reMaps;
        }

        /// <summary>
        /// An array of field and or property names to ignore.
        /// </summary>
        public string[] ToIgnore { get; init; }

        /// <summary>
        /// A <see cref="Dictionary{TKey, TValue}"/> of field and or property names to remap.
        /// </summary>
        public Dictionary<string, string> Remaps { get; init; }

        /// <summary>
        /// The name of the <see cref="AdeptarConfiguration"/> field or property inside the class or struct it is in.
        /// </summary>
        private string _name;

        /// <summary>
        /// The name of the <see cref="AdeptarConfiguration"/> field or property inside the class or struct it is in.
        /// </summary>
        internal string Name => _name;

        /// <summary>
        /// Sets the private name of the <see cref="AdeptarConfiguration"/> instance.
        /// </summary>
        /// <param name="name"></param>
        internal void SetName(string name )
        {
            _name = name;
        }
    }
}