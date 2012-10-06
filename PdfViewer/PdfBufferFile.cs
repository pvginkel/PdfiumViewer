using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace PdfViewer
{
    internal class PdfBufferFile : PdfFile
    {
        private readonly byte[] _buffer;
        private readonly int _length;

        public PdfBufferFile(byte[] buffer)
            : this(buffer, buffer.Length)
        {
        }

        public PdfBufferFile(byte[] buffer, int length)
        {
            if (buffer == null)
                throw new ArgumentNullException("buffer");

            _buffer = buffer;
            _length = length;
        }

        public override bool RenderPDFPageToDC(int pageNumber, IntPtr dc, int dpiX, int dpiY, int boundsOriginX, int boundsOriginY, int boundsWidth, int boundsHeight, bool fitToBounds, bool stretchToBounds, bool keepAspectRation, bool centerInBounds, bool autoRotate)
        {
            return NativeMethods.RenderPDFPageToDC(_buffer, _length, pageNumber, dc, dpiX, dpiY, boundsOriginX, boundsOriginY, boundsWidth, boundsHeight, fitToBounds, stretchToBounds, keepAspectRation, centerInBounds, autoRotate);
        }

        public override bool GetPDFDocInfo(out int pageCount, out double maxPageWidth)
        {
            return NativeMethods.GetPDFDocInfo(_buffer, _length, out pageCount, out maxPageWidth);
        }

        public override void Save(Stream stream)
        {
            if (stream == null)
                throw new ArgumentNullException("stream");

            stream.Write(_buffer, 0, _length);
        }
    }
}
