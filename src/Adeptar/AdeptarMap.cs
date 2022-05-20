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
    ///
    /// </summary>
    public struct AdeptarMap
    {
        /// <summary>
        ///
        /// </summary>
        /// <param name="dynamics"></param>
        /// <param name="types"></param>
        /// <param name="keys"></param>
        public AdeptarMap ( DynamicType[] dynamics, Type[] types, string[] keys)
        {
            _maps = new();

            for (int i = 0; i < dynamics.Length; i++)
            {
                _maps.Add( dynamics[i], new Dictionary<string, Type>() { { keys[i], types[i] } } );
            }
        }


        /// <summary>
        ///
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public AdeptarMap FromType( Type type )
        {
            throw new NotImplementedException();
        }


        private Dictionary<DynamicType, Dictionary<string, Type>> _maps;
    }
}