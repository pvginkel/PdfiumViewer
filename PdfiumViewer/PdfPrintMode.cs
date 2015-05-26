using System;
using System.Collections.Generic;
using System.Text;

namespace PdfiumViewer
{
    /// <summary>
    /// Specifies the mode in which the document should be printed.
    /// </summary>
    /// <remarks>
    /// Printers have a hard margin. This is a (small) margin on which it is not
    /// possible to print. PdfPrintMode specifies whether the page should be
    /// scaled to fit into this margin, or that the margin should be cut off of
    /// the page.
    /// </remarks>
    public enum PdfPrintMode
    {
        /// <summary>
        /// Shrink the print area to fall within the hard printer margin.
        /// </summary>
        ShrinkToMargin,
        /// <summary>
        /// Cut the hard printer margin from the output.
        /// </summary>
        CutMargin
    }
}
