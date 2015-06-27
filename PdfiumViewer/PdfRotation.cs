using System;
using System.Collections.Generic;
using System.Text;

namespace PdfiumViewer
{
    /// <summary>
    /// Specifies the rotation of pages shown in the PDF renderer.
    /// </summary>
    public enum PdfRotation
    {
        /// <summary>
        /// Rotates the output 0 degrees.
        /// </summary>
        Rotate0,
        /// <summary>
        /// Rotates the output 90 degrees.
        /// </summary>
        Rotate90,
        /// <summary>
        /// Rotates the output 180 degrees.
        /// </summary>
        Rotate180,
        /// <summary>
        /// Rotates the output 270 degrees.
        /// </summary>
        Rotate270
    }
}
