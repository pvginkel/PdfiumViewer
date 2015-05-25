using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace PdfiumViewer
{
    public class SetCursorEventArgs : EventArgs
    {
        public Point Location { get; private set; }

        public Cursor Cursor { get; set; }

        public SetCursorEventArgs(Point location)
        {
            Location = location;
        }
    }

    public delegate void SetCursorEventHandler(object sender, SetCursorEventArgs e);
}
