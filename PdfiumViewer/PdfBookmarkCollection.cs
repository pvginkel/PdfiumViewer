using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace PdfiumViewer
{
    public class PdfBookmark
    {
        public string Title { get; internal set; }
        public int PageIndex { get; internal set; }
        //public IntPtr Action { get; private set; }
        //public ulong ActionType { get; private set; }

        public PdfBookmarkCollection Children { get; private set; }

        public PdfBookmark()
        {
            Children = new PdfBookmarkCollection();
        }

        public override string ToString()
        {
            return Title;
        }
    }

    public class PdfBookmarkCollection : Collection<PdfBookmark>
    {
    }
}
