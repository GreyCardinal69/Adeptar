using System.IO;

namespace Adeptar.Unity
{
    /// <summary>
    /// A class containing helper methods for different uses.
    /// </summary>
    public class AdeptarHelpers
    {
        /// <summary>
        /// Checks if the provided file already has an object appended using the Id feature with the specified id.
        /// The id is limited to numbers and letters.
        /// </summary>
        /// <param name="path">The path of the file.</param>
        /// <param name="id">The id of the object to check for.</param>
        /// <returns>True if the already exists in the file.</returns>
        public static bool ContainsId ( string path, string id ) => ContainsIdInternal( path, id );

        /// <summary>
        /// Checks if the provided file already has an object appended using the Id feature with the specified id.
        /// The id is limited to numbers and letters.
        /// </summary>
        /// <param name="path">The path of the file.</param>
        /// <param name="id">The id of the object to check for.</param>
        /// <returns>True if the already exists in the file.</returns>
        private static bool ContainsIdInternal ( string path, string id )
        {
            bool inString = false;
            bool startWrite = false;
            string temp = "";

            foreach (char item in File.ReadAllText( path ))
            {
                switch (item)
                {
                    case '~':
                        if (!inString){
                            startWrite = true;
                        }
                        break;
                    case '"':
                        inString = !inString;
                        break;
                    default:
                        if (startWrite && !inString){
                            if (!char.IsLetterOrDigit(item)){
                                startWrite = false;
                                if (temp == id){
                                    return true;
                                }else{
                                    temp = "";
                                }
                            }else{
                                temp += item;
                            }
                        }
                        break;
                }
            }
            return false;
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