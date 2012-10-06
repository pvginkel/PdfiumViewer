using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Text;

namespace PdfViewer.Chrome
{
    internal abstract class PdfFileHandleFile : PdfFile
    {
        private SafeHandle _mappedHandle;
        private SafeHandle _buffer;
        private readonly int _length;
        private bool _disposed;

        protected PdfFileHandleFile(SafeHandle handle, int length)
        {
            if (handle == null)
                throw new ArgumentNullException("handle");

            _length = length;

            _mappedHandle = NativeMethods.CreateFileMapping(handle, IntPtr.Zero, NativeMethods.FileMapProtection.PageReadonly, 0, (uint)_length, null);

            if (_mappedHandle.IsInvalid)
                throw new Win32Exception();

            _buffer = NativeMethods.MapViewOfFile(_mappedHandle, NativeMethods.FileMapAccess.FileMapRead, 0, 0, (uint)_length);

            if (_buffer.IsInvalid)
                throw new Win32Exception();
        }

        public override bool RenderPDFPageToDC(int pageNumber, IntPtr dc, int dpiX, int dpiY, int boundsOriginX, int boundsOriginY, int boundsWidth, int boundsHeight, bool fitToBounds, bool stretchToBounds, bool keepAspectRation, bool centerInBounds, bool autoRotate)
        {
            if (_disposed)
                throw new ObjectDisposedException(GetType().Name);

            return NativeMethods.RenderPDFPageToDC(_buffer, _length, pageNumber, dc, dpiX, dpiY, boundsOriginX, boundsOriginY, boundsWidth, boundsHeight, fitToBounds, stretchToBounds, keepAspectRation, centerInBounds, autoRotate);
        }

        public override bool GetPDFDocInfo(out int pageCount, out double maxPageWidth)
        {
            if (_disposed)
                throw new ObjectDisposedException(GetType().Name);

            return NativeMethods.GetPDFDocInfo(_buffer, _length, out pageCount, out maxPageWidth);
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
