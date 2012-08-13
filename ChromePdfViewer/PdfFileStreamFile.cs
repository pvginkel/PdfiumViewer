using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;

namespace ChromePdfViewer
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

        public override void Save(Stream stream)
        {
            if (stream == null)
                throw new ArgumentNullException("stream");

            long offset = _stream.Position;

            try
            {
                StreamExtensions.CopyStream(_stream, stream);
            }
            finally
            {
                _stream.Position = offset;
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
