using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace PdfiumViewer
{
    public class PdfMarker
    {
        public int Page { get; }
        public RectangleF Bounds { get; }
        public Color Color { get; }
        public Color BorderColor { get; }
        public float BorderWidth { get; }

        public PdfMarker(int page, RectangleF bounds, Color color)
            : this(page, bounds, color, Color.Transparent, 0)
        {
        }

        public PdfMarker(int page, RectangleF bounds, Color color, Color borderColor, float borderWidth)
        {
            Page = page;
            Bounds = bounds;
            Color = color;
            BorderColor = borderColor;
            BorderWidth = borderWidth;
        }
    }
}
