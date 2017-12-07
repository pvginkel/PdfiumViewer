using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace PdfiumViewer
{
    public struct PdfCharacterInformation
    {
        public int Page { get; }
        public int Offset { get; }
        public double FontSize { get; }
        public char Character { get; }
        public RectangleF Bounds { get; }

        public PdfCharacterInformation(int page, int offset, char character, double fontSize, RectangleF bounds)
        {
            Page = page;
            Offset = offset;
            FontSize = fontSize;
            Bounds = bounds;
            Character = character;
        }

    }
}
