using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Text;

namespace PdfiumViewer
{
    internal abstract class PdfFileHandleFile : PdfFile
    {
        private SafeHandle _mappedHandle;
        private SafeHandle _buffer;
        private bool _disposed;

        protected PdfFileHandleFile(SafeHandle handle, int length)
        {
            if (handle == null)
                throw new ArgumentNullException("handle");

            _mappedHandle = NativeMethods.CreateFileMapping(handle, IntPtr.Zero, NativeMethods.FileMapProtection.PageReadonly, 0, (uint)length, null);

            if (_mappedHandle.IsInvalid)
                throw new Win32Exception();

            _buffer = NativeMethods.MapViewOfFile(_mappedHandle, NativeMethods.FileMapAccess.FileMapRead, 0, 0, (uint)length);

            if (_buffer.IsInvalid)
                throw new Win32Exception();

            LoadDocument(NativeMethods.FPDF_LoadMemDocument(_buffer, length, null));
        }

        protected override void Dispose(bool disposing)
        {
            if (!_disposed && disposing)
            {
                if (_buffer != null)
                {
                    _buffer.Dispose();
                    _buffer = null;
                }

                if (_mappedHandle != null)
                {
                    _mappedHandle.Dispose();
                    _mappedHandle = null;
                }

                _disposed = true;
            }

            base.Dispose(disposing);
        }
    }
}
