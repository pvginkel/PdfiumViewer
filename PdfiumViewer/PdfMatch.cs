using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Text;

namespace PdfiumViewer
{
    public class PdfMatch
    {
        public PointF Location { get; private set; }
        public string Text { get; private set; }
        public IList<RectangleF> TextBounds { get; private set; }
        public int Page { get; private set; }

        public PdfMatch(PointF location, string text, IList<RectangleF> textBounds, int page)
        {
            if (textBounds == null)
                throw new ArgumentNullException(nameof(textBounds));

            Location = location;
            Text = text;
            TextBounds = new ReadOnlyCollection<RectangleF>(textBounds);
            Page = page;
        }
    }
}
