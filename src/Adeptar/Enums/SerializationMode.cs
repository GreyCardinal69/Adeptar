namespace Adeptar
{
    /// <summary>
    /// Defines how the object should be serialized.
    /// </summary>
    internal enum SerializationMode
    {
        /// <summary>
        /// Default serialization mode serializes the provided object to a .Adeptar string, overwrites the file at the given path.
        /// </summary>
        Default,
        /// <summary>
        /// Append mode serializes the provided object to a .Adeptar string and appends it to the file in the given path. Also known as the ID feature.
        /// </summary>
        Append,
        /// <summary>
        /// ChangeAppended serializes the provided object to a .Adeptar string,
        /// reads the file at the given path, overwrites the old version of the object with the new one and rewrites the file,
        /// leaving other objects untouched.
        /// </summary>
        ChangeAppended,
        /// <summary>
        /// Sets or changes the shared data section of an Index feature object collection.
        /// </summary>
        SetShared,
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