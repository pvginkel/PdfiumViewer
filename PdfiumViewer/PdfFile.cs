using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;

namespace PdfiumViewer
{
    internal abstract class PdfFile : IDisposable
    {
        private IntPtr _document;
        private IntPtr _form;
        private bool _disposed;
        private NativeMethods.FPDF_FORMFILLINFO _formCallbacks;
        private GCHandle _formCallbacksHandle;

        public static PdfFile Create(Stream stream)
        {
            if (stream == null)
                throw new ArgumentNullException("stream");

            if (stream is MemoryStream)
                return new PdfMemoryStreamFile((MemoryStream)stream);
            if (stream is FileStream)
                return new PdfFileStreamFile((FileStream)stream);
            return new PdfBufferFile(StreamExtensions.ToByteArray(stream));
        }

        protected PdfFile()
        {
            PdfLibrary.EnsureLoaded();
        }

        public bool RenderPDFPageToDC(int pageNumber, IntPtr dc, int dpiX, int dpiY, int boundsOriginX, int boundsOriginY, int boundsWidth, int boundsHeight, bool fitToBounds, bool stretchToBounds, bool keepAspectRation, bool centerInBounds, bool autoRotate, bool forPrinting)
        {
            if (_disposed)
                throw new ObjectDisposedException(GetType().Name);

            using (var pageData = new PageData(_document, _form, pageNumber))
            {
                NativeMethods.FPDF_RenderPage(dc, pageData.Page, boundsOriginX, boundsOriginY, boundsWidth, boundsHeight, 0, forPrinting ? NativeMethods.FPDF.PRINTING : 0);
            }

            return true;
        }

        public bool RenderPDFPageToBitmap(int pageNumber, IntPtr bitmapHandle, int dpiX, int dpiY, int boundsOriginX, int boundsOriginY, int boundsWidth, int boundsHeight, bool fitToBounds, bool stretchToBounds, bool keepAspectRation, bool centerInBounds, bool autoRotate, bool forPrinting)
        {
            if (_disposed)
                throw new ObjectDisposedException(GetType().Name);

            using (var pageData = new PageData(_document, _form, pageNumber))
            {
                NativeMethods.FPDF_RenderPageBitmap(bitmapHandle, pageData.Page, boundsOriginX, boundsOriginY, boundsWidth, boundsHeight, 0, forPrinting ? NativeMethods.FPDF.PRINTING : 0);
            }

            return true;
        }


        public SearchResult Search(string text, int pageNumber, bool matchCase, bool wholeWord, bool fromStart)
        {
            if (!fromStart && LastSearchPageData != null)
            {
                var res = LastSearchPageData.Search(text, matchCase, wholeWord, fromStart);

                if (!res.IsFound)
                {
                    LastSearchPageData.Dispose();
                    LastSearchPageData = null;
                }

                return res;
            }
            else
            {
                if (LastSearchPageData != null)
                    LastSearchPageData.Dispose();

                using (var pageData = new PageData(_document, _form, pageNumber))
                {
                    var res = pageData.Search(text, matchCase, wholeWord, fromStart);

                    if (res.IsFound)
                        LastSearchPageData = pageData;

                    return res;
                }
            }
        }

        private PageData LastSearchPageData { get; set; }

        public List<SizeF> GetPDFDocInfo()
        {
            if (_disposed)
                throw new ObjectDisposedException(GetType().Name);

            int pageCount = NativeMethods.FPDF_GetPageCount(_document);
            var result = new List<SizeF>(pageCount);

            for (int i = 0; i < pageCount; i++)
            {
                double height;
                double width;
                NativeMethods.FPDF_GetPageSizeByIndex(_document, i, out width, out height);

                result.Add(new SizeF((float)width, (float)height));
            }

            return result;
        }

        public abstract void Save(Stream stream);

        protected void LoadDocument(IntPtr document)
        {
            _document = document;

            NativeMethods.FPDF_GetDocPermissions(_document);

            _formCallbacks = new NativeMethods.FPDF_FORMFILLINFO();
            _formCallbacksHandle = GCHandle.Alloc(_formCallbacks);
            _formCallbacks.version = 1;

            _form = NativeMethods.FPDFDOC_InitFormFillEnvironment(_document, ref _formCallbacks);
            NativeMethods.FPDF_SetFormFieldHighlightColor(_form, 0, 0xFFE4DD);
            NativeMethods.FPDF_SetFormFieldHighlightAlpha(_form, 100);

            NativeMethods.FORM_DoDocumentJSAction(_form);
            NativeMethods.FORM_DoDocumentOpenAction(_form);
        }

        public void Dispose()
        {
            Dispose(true);

            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (_form != IntPtr.Zero)
                {
                    NativeMethods.FORM_DoDocumentAAction(_form, NativeMethods.FPDFDOC_AACTION.WC);
                    NativeMethods.FPDFDOC_ExitFormFillEnviroument(_form);
                    _form = IntPtr.Zero;
                }

                if (_document != IntPtr.Zero)
                {
                    NativeMethods.FPDF_CloseDocument(_document);
                    _document = IntPtr.Zero;
                }

                if (_formCallbacksHandle.IsAllocated)
                    _formCallbacksHandle.Free();

                _disposed = true;
            }
        }

        private class PageData : IDisposable
        {
            private int previousSearchIndex = 0;
            private readonly IntPtr _form;
            private bool _disposed;

            public IntPtr Page { get; private set; }

            public IntPtr TextPage { get; private set; }

            public double Width { get; private set; }

            public double Height { get; private set; }

            public PageData(IntPtr document, IntPtr form, int pageNumber)
            {
                _form = form;

                Page = NativeMethods.FPDF_LoadPage(document, pageNumber);
                TextPage = NativeMethods.FPDFText_LoadPage(Page);
                NativeMethods.FORM_OnAfterLoadPage(Page, form);
                NativeMethods.FORM_DoPageAAction(Page, form, NativeMethods.FPDFPAGE_AACTION.OPEN);

                Width = NativeMethods.FPDF_GetPageWidth(Page);
                Height = NativeMethods.FPDF_GetPageHeight(Page);
            }

            public SearchResult Search(string text, bool matchCase, bool wholeWord, bool fromStart)
            {
                IntPtr pSCHHandle = IntPtr.Zero;

                // Set the find flag 
                int nFlag = 0;

                double left = 0, right = 0, bottom = 0, top = 0;
                double x = 0, y = 0;
                bool result = false;
                string foundString = null;

                if (matchCase)
                {
                    nFlag |= (int)(NativeMethods.FPDF_SEARCH_FLAGS.FPDF_MATCHCASE);
                }

                if (wholeWord)
                {
                    nFlag |= (int)(NativeMethods.FPDF_SEARCH_FLAGS.FPDF_MATCHWHOLEWORD);
                }

                byte[] bytes = Encoding.Unicode.GetBytes(text);

                if (fromStart)
                {
                    previousSearchIndex = 0;
                }

                // Get the search context handle                
                pSCHHandle = NativeMethods.FPDFText_FindStart(TextPage, bytes, nFlag, 0);

                // Progress in direction of search up to previousSearchIndex to find next
                for (int i = 0; i < previousSearchIndex; i++)
                    result = NativeMethods.FPDFText_FindNext(pSCHHandle);

                // Search the text string. 
                result = NativeMethods.FPDFText_FindNext(pSCHHandle);

                int index = 0, matchedCount = 0;

                if (result)
                {
                    index = NativeMethods.FPDFText_GetSchResultIndex(pSCHHandle);

                    matchedCount = NativeMethods.FPDFText_GetSchCount(pSCHHandle);

                    ushort[] buffer = new ushort[matchedCount];
                    int extractedCount = NativeMethods.FPDFText_GetText(TextPage, index, matchedCount, buffer);

                    char[] charArray = Array.ConvertAll<ushort, char>(buffer, new Converter<ushort, char>((input) => { return (char)input; }));
                    foundString = new string(charArray);

                    NativeMethods.FPDFText_GetCharBox(TextPage, index, ref left, ref right, ref bottom, ref top);

                    previousSearchIndex++;
                }

                NativeMethods.FPDFText_FindClose(pSCHHandle);

                return new SearchResult()
                {
                    IsFound = result,
                    StartIndex = index,
                    Count = matchedCount,
                    X = left,
                    Y = top,
                    Text = foundString
                };
            }

            public void Dispose()
            {
                if (!_disposed)
                {
                    NativeMethods.FORM_DoPageAAction(Page, _form, NativeMethods.FPDFPAGE_AACTION.CLOSE);
                    NativeMethods.FORM_OnBeforeClosePage(Page, _form);
                    NativeMethods.FPDFText_ClosePage(TextPage);
                    NativeMethods.FPDF_ClosePage(Page);

                    _disposed = true;
                }
            }
        }
    }

    public struct SearchResult
    {
        public bool IsFound;
        public int StartIndex;
        public int Count;
        public double X;
        public double Y;
        public string Text;
    }
}
