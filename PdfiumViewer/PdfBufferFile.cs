using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;

namespace PdfiumViewer
{
    internal class PdfBufferFile : PdfFile
    {
        private readonly byte[] _buffer;
        private readonly int _length;
        private IntPtr _copy;

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

            _copy = Marshal.AllocHGlobal(buffer.Length);
            Marshal.Copy(_buffer, 0, _copy, _buffer.Length);
            LoadDocument(NativeMethods.FPDF_LoadMemDocument(_copy, length, null));
        }

        public override void Save(Stream stream)
        {
            if (stream == null)
                throw new ArgumentNullException("stream");

            stream.Write(_buffer, 0, _length);
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            if (_copy != IntPtr.Zero) {
                Marshal.FreeHGlobal(_copy);
                _copy = IntPtr.Zero;
            }
        }

        ~PdfBufferFile()
        {
            Dispose(true);
        }
    }
}
