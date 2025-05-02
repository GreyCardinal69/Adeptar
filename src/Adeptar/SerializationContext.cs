using System.Text;
using System;

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

        /// <summary>Initializes a new serialization context.</summary>
        public SerializationContext( StringBuilder builder, AdeptarSettings settings )
        {
            Builder = builder ?? throw new ArgumentNullException( nameof( builder ) );
            Settings = settings ?? throw new ArgumentNullException( nameof( settings ) );
            IndentLevel = 0;
        }
    }
}