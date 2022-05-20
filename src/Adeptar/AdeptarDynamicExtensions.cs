using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Adeptar
{
    /// <summary>
    /// Provides extension methods for <see cref="AdeptarDynamic"/> and <see cref="AdeptarMap"/> objects.
    /// </summary>
    public static class AdeptarDynamicExtensions
    {
        /// <summary>
        /// Creates a <see cref="AdeptarDynamic"/> object from a file that contains
        /// a .Adeptar object.
        /// </summary>
        /// <param name="dynm"></param>
        /// <param name="path"></param>
        public static AdeptarDynamic FromFile ( this AdeptarDynamic dynm, string path )
            => AdeptarDynamic.FromFile( path );

        /// <summary>
        /// Creates a <see cref="AdeptarDynamic"/> object from the .Adeptar string.
        /// </summary>
        /// <param name="dynm"></param>
        /// <param name="str"></param>
        public static AdeptarDynamic FromString ( this AdeptarDynamic dynm, string str )
            => AdeptarDynamic.FromString( str );
    }
}