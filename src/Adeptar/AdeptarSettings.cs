namespace Adeptar
{
    /// <summary>
    /// A class that controls object serialization rules.
    /// </summary>
    public class AdeptarSettings
    {
        /// <summary>
        /// Tells the <see cref="AdeptarWriter"/> whether to check for .Adeptar attributes on fields or properties.
        /// </summary>
        public bool CheckClassAttributes { get; set; }

        /// <summary>
        /// Tells the <see cref="AdeptarWriter"/> whether to apply indentation when serializing objects.
        /// </summary>
        public bool UseIndentation { get; set; }

        /// <summary>
        /// Tells the <see cref="AdeptarWriter"/> to not serialize fields or properties whos value is the default of their type.
        /// </summary>
        public bool IgnoreDefaultValues { get; set; }

        /// <summary>
        /// Tells the <see cref="AdeptarWriter"/> to not serialize fields or properties that are null.
        /// </summary>
        public bool IgnoreNullValues { get; set; }
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