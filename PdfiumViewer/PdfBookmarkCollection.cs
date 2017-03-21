using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

#pragma warning disable 1591

namespace PdfiumViewer
{
    public class PdfBookmark
    {
        public string Title { get; set; }
        public int PageIndex { get; set; }

        public PdfBookmarkCollection Children { get; }

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
