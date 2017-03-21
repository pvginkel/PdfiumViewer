using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace PdfiumViewer
{
    /// <summary>
    /// Configuration for printing multiple PDF pages on a single page.
    /// </summary>
    public class PdfPrintMultiplePages
    {
        /// <summary>
        /// Gets the number of pages to print horizontally.
        /// </summary>
        public int Horizontal { get; }

        /// <summary>
        /// Gets the number of pages to print vertically.
        /// </summary>
        public int Vertical { get; }

        /// <summary>
        /// Gets the orientation in which PDF pages are layed out on the
        /// physical page.
        /// </summary>
        public Orientation Orientation { get; }

        /// <summary>
        /// Gets the margin between PDF pages in device units.
        /// </summary>
        public float Margin { get; }

        /// <summary>
        /// Creates a new instance of the PdfPrintMultiplePages class.
        /// </summary>
        /// <param name="horizontal">The number of pages to print horizontally.</param>
        /// <param name="vertical">The number of pages to print vertically.</param>
        /// <param name="orientation">The orientation in which PDF pages are layed out on
        /// the physical page.</param>
        /// <param name="margin">The margin between PDF pages in device units.</param>
        public PdfPrintMultiplePages(int horizontal, int vertical, Orientation orientation, float margin)
        {
            if (horizontal < 1)
                throw new ArgumentOutOfRangeException("horizontal cannot be less than one");
            if (vertical < 1)
                throw new ArgumentOutOfRangeException("vertical cannot be less than one");
            if (margin < 0)
                throw new ArgumentOutOfRangeException("margin cannot be less than zero");

            Horizontal = horizontal;
            Vertical = vertical;
            Orientation = orientation;
            Margin = margin;
        }
    }
}
