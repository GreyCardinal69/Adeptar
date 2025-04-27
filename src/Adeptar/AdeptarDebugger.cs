using System;

namespace Adeptar
{
    /// <summary>
    /// Provides internal utility methods helpful for debugging Adeptar serialization and deserialization,
    /// such as generating previews of data for exception messages.
    /// </summary>
    internal static class AdeptarDebugger
    {
        /// <summary>
        /// Generates a preview string from a ReadOnlySpan, truncated if it exceeds a maximum length.
        /// </summary>
        /// <param name="span">The span to preview.</param>
        /// <param name="maxLength">The maximum number of characters to include in the preview before truncation.</param>
        /// <returns>A string representation of the span, potentially truncated with "..." appended.</returns>
        internal static string PreviewSpan( ReadOnlySpan<char> span, int maxLength = 100 )
        {
            if ( span.Length <= maxLength )
            {
                return span.ToString();
            }
            return span.Slice( 0, maxLength ).ToString() + "...";
        }
    }
}