using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace PdfiumViewer
{
    public class PdfPoint
    {
        public int Page { get; }
        public Point Location { get; }

        public PdfPoint(int page, Point location)
        {
            Page = page;
            Location = location;
        }
    }
}
