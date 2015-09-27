using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace PdfiumViewer
{
    public class PdfBookmark
    {
        private IntPtr _document;
        private IntPtr _bookmark;

        public string Title { get; private set; }
        public uint PageIndex { get; private set; }
        //public IntPtr Action { get; private set; }
        //public ulong ActionType { get; private set; }

        public PdfBookmarks Children { get; private set; }

        public PdfBookmark(IntPtr document, IntPtr bookmark)
        {
            _document = document;
            _bookmark = bookmark;
            Title = GetBookmarkTitle(_bookmark);
            PageIndex = GetPageIndex(_bookmark);

            //Action = NativeMethods.FPDF_BookmarkGetAction(_bookmark);
            //if (Action != IntPtr.Zero)
            //    ActionType = NativeMethods.FPDF_ActionGetType(Action);

            IntPtr child = NativeMethods.FPDF_BookmarkGetFirstChild(document, bookmark);
            if (child != IntPtr.Zero)
                Children = new PdfBookmarks(document, child);
        }

        private string GetBookmarkTitle(IntPtr bookmark)
        {
            uint length = NativeMethods.FPDF_BookmarkGetTitle(bookmark, null, 0);
            byte[] buffer = new byte[length];
            NativeMethods.FPDF_BookmarkGetTitle(bookmark, buffer, length);

            string result = Encoding.Unicode.GetString(buffer);
            return result.Substring(0, result.Length - 1);
        }

        private uint GetPageIndex(IntPtr bookmark)
        {
            IntPtr dest = NativeMethods.FPDF_BookmarkGetDest(_document, bookmark);
            if (dest != IntPtr.Zero)
                return NativeMethods.FPDFDest_GetPageIndex(_document, dest);

            return 0;
        }

        public override string ToString()
        {
            return Title;
        }
    }

    public class PdfBookmarks : IEnumerable<PdfBookmark>
    {
        private IntPtr _document;
        private List<PdfBookmark> _list;

        public PdfBookmarks(IntPtr document)
        {
            _document = document;
            _list = new List<PdfBookmark>();
            IntPtr bookmark = NativeMethods.FPDF_BookmarkGetFirstChild(document, IntPtr.Zero);
            if (bookmark != IntPtr.Zero)
            {
                _list.Add(new PdfBookmark(document, bookmark));
                while ((bookmark = NativeMethods.FPDF_BookmarkGetNextSibling(document, bookmark)) != IntPtr.Zero)
                    _list.Add(new PdfBookmark(document, bookmark));
            }
        }

        public PdfBookmarks(IntPtr document, IntPtr bookmark)
        {
            _document = document;
            _list = new List<PdfBookmark>();
            _list.Add(new PdfBookmark(document, bookmark));
            while ((bookmark = NativeMethods.FPDF_BookmarkGetNextSibling(document, bookmark)) != IntPtr.Zero)
                _list.Add(new PdfBookmark(document, bookmark));
        }

        public IEnumerator<PdfBookmark> GetEnumerator()
        {
            return _list.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _list.GetEnumerator();
        }

        public int Count
        {
            get
            {
                return _list.Count;
            }
        }

        public int TotalCount
        {
            get
            {
                int result = _list.Count;
                foreach (var item in _list)
                {
                    if (item.Children != null)
                        result += item.Children.TotalCount;
                }
                return result;
            }
        }
    }
}
