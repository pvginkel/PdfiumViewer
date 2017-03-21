using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace PdfiumViewer
{
    public interface IPdfMarker
    {
        int Page { get; }
        void Draw(PdfRenderer renderer, Graphics graphics, int page);
    }
}
