using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

#pragma warning disable 1591

namespace PdfiumViewer
{
    public class SetCursorEventArgs : EventArgs
    {
        public Point Location { get; private set; }

        public HitTest HitTest { get; private set; }

        public Cursor Cursor { get; set; }

        public SetCursorEventArgs(Point location, HitTest hitTest)
        {
            Location = location;
            HitTest = hitTest;
        }
    }

    public delegate void SetCursorEventHandler(object sender, SetCursorEventArgs e);
}
