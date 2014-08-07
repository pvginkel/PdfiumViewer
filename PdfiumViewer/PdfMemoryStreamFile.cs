using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace PdfiumViewer
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
