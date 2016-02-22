using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace PdfiumViewer
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

            LoadDocument(NativeMethods.FPDF_LoadMemDocument(_buffer, length, null));
        }

    }
}
