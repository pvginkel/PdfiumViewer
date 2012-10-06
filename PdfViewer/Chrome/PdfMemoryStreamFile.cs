using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace PdfViewer.Chrome
{
    internal class PdfMemoryStreamFile : PdfBufferFile
    {
        private MemoryStream _stream;
        private bool _disposed;

        public PdfMemoryStreamFile(MemoryStream stream)
            : base(GetBuffer(stream), (int)stream.Length)
        {
            if (stream == null)
                throw new ArgumentNullException("stream");

            _stream = stream;
        }

        private static byte[] GetBuffer(MemoryStream stream)
        {
            if (stream == null)
                throw new ArgumentNullException("stream");

            // GetBuffer will throw a UnauthorizedAccessException when the
            // internal array is not exposable. However, no interface is
            // exposed to check whether GetBuffer is going to throw this.

            try
            {
                return stream.GetBuffer();
            }
            catch (UnauthorizedAccessException)
            {
                return stream.ToArray();
            }
        }

        public override bool GetPDFDocInfo(out int pageCount, out double maxPageWidth)
        {
            if (_disposed)
                throw new ObjectDisposedException(GetType().Name);

            return base.GetPDFDocInfo(out pageCount, out maxPageWidth);
        }

        public override bool RenderPDFPageToDC(int pageNumber, IntPtr dc, int dpiX, int dpiY, int boundsOriginX, int boundsOriginY, int boundsWidth, int boundsHeight, bool fitToBounds, bool stretchToBounds, bool keepAspectRation, bool centerInBounds, bool autoRotate)
        {
            if (_disposed)
                throw new ObjectDisposedException(GetType().Name);

            return base.RenderPDFPageToDC(pageNumber, dc, dpiX, dpiY, boundsOriginX, boundsOriginY, boundsWidth, boundsHeight, fitToBounds, stretchToBounds, keepAspectRation, centerInBounds, autoRotate);
        }

        protected override void Dispose(bool disposing)
        {
            if (!_disposed && disposing)
            {
                if (_stream != null)
                {
                    _stream.Dispose();
                    _stream = null;
                }

                _disposed = true;
            }

            base.Dispose(disposing);
        }
    }
}
