using System;
using System.Text;

namespace Adeptar
{
    /// <summary>
    /// Internal struct holding the core shared state for a serialization operation.
    /// </summary>
    internal struct SerializationContext
    {
        /// <summary>The StringBuilder to write output to.</summary>
        public readonly StringBuilder Builder;

        /// <summary>The current serialization settings.</summary>
        public readonly AdeptarSettings Settings;

        /// <summary>The current indentation level (number of tabs).</summary>
        public int IndentLevel;

        /// <summary>
        /// Instructs <see cref="AdeptarSerializer.Write(object, SerializableType, string, ref SerializationContext, bool, bool)"/> method not to add a new line
        /// character, but only once.
        /// </summary>
        public bool SuppressNewLineOnce;

        /// <summary>
        /// Instructs the <see cref="AdeptarSerializer.Write(object, SerializableType, string, ref SerializationContext, bool, bool)"/> method to not
        /// apply full indentation, rather to only apply one singular space regardless of indentation level. Used for writing dictionary values after
        /// the key's ":".
        /// </summary>
        public bool SuppressFullIndentationOnce;

        /// <summary>Initializes a new serialization context.</summary>
        public SerializationContext( StringBuilder builder, AdeptarSettings settings )
        {
            Builder = builder ?? throw new ArgumentNullException( nameof( builder ) );
            Settings = settings ?? throw new ArgumentNullException( nameof( settings ) );
            IndentLevel = 0;
        }
    }
}