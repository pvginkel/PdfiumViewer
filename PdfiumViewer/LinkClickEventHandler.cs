using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace PdfiumViewer
{
    public class LinkClickEventArgs : HandledEventArgs
    {
        /// <summary>
        /// Gets the link that was clicked.
        /// </summary>
        public PdfPageLink Link { get; private set; }
        
        public LinkClickEventArgs(PdfPageLink link)
        {
            Link = link;
        }
    }

    public delegate void LinkClickEventHandler(object sender, LinkClickEventArgs e);
}
