using System;

namespace Adeptar
{
    /// <summary>
    /// Instructs the Adeptar serializer to skip the attributed property or field during serialization.
    /// </summary>
    /// <remarks>
    /// Apply this attribute to public fields or properties of a class or struct that should be
    /// excluded when serializing an object using Adeptar.
    /// Note: Serialization behavior might depend on <see cref="AdeptarSettings"/> configured
    /// in the serializer (e.g., <see cref="AdeptarSettings.CheckClassAttributes"/> might need to be enabled).
    /// </remarks>
    /// <seealso cref="AdeptarWriter"/>
    /// <seealso cref="AdeptarSettings"/>
    [AttributeUsage( AttributeTargets.Field | AttributeTargets.Property, Inherited = true, AllowMultiple = false )]
    public sealed class AdeptarIgnoreAttribute : Attribute // Added 'sealed' and 'Attribute' suffix
    {
        // This is a marker attribute, so no constructor or properties are needed.
        // The presence of the attribute itself provides the necessary information.
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