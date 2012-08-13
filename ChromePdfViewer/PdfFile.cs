using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace ChromePdfViewer
{
    internal abstract class PdfFile : IDisposable
    {
        public static PdfFile Create(Stream stream)
        {
            if (stream == null)
                throw new ArgumentNullException("stream");

            if (stream is MemoryStream)
                return new PdfMemoryStreamFile((MemoryStream)stream);
            else if (stream is FileStream)
                return new PdfFileStreamFile((FileStream)stream);
            else
                return new PdfBufferFile(StreamExtensions.ToByteArray(stream));
        }

        public abstract bool RenderPDFPageToDC(int pageNumber, IntPtr dc, int dpiX, int dpiY, int boundsOriginX, int boundsOriginY, int boundsWidth, int boundsHeight, bool fitToBounds, bool stretchToBounds, bool keepAspectRation, bool centerInBounds, bool autoRotate);

        public abstract bool GetPDFDocInfo(out int pageCount, out double maxPageWidth);

        public abstract void Save(Stream stream);

        public void Dispose()
        {
            Dispose(true);

            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
        }
    }
}
