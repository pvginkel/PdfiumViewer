using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace PdfiumViewer
{
    /// <summary>
    /// Represents a marker on a PDF page.
    /// </summary>
    public interface IPdfMarker
    {
        /// <summary>
        /// The page where the marker is drawn on.
        /// </summary>
        int Page { get; }

        /// <summary>
        /// Draw the marker.
        /// </summary>
        /// <param name="renderer">The PdfRenderer to draw the marker with.</param>
        /// <param name="graphics">The Graphics to draw the marker with.</param>
        void Draw(PdfRenderer renderer, Graphics graphics);
    }
}
