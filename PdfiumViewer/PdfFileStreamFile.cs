using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace PdfiumViewer
{
    internal class PdfFileStreamFile : PdfFileHandleFile
    {
        private FileStream _stream;
        private bool _disposed;

        public PdfFileStreamFile(FileStream stream)
            : base(stream.SafeFileHandle, (int)stream.Length)
        {
            if (stream == null)
                throw new ArgumentNullException("stream");

            _stream = stream;
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
