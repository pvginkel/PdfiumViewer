using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Printing;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace PdfViewer.Chrome
{
    internal class PdfDocument : global::PdfViewer.PdfDocument
    {
        private bool _disposed;
        private PdfFile _file;
        private readonly int _pageCount;

        public override int PageCount
        {
            get { return _pageCount; }
        }

        /// <summary>
        /// Maximum page width in pixels based on 72 DPI.
        /// </summary>
        public double MaximumPageWidth { get; private set; }

        public PdfDocument(Stream stream)
            : this(PdfFile.Create(stream))
        {
        }

        public PdfDocument(string path)
            : this(File.OpenRead(path))
        {
        }

        private PdfDocument(PdfFile file)
        {
            if (file == null)
                throw new ArgumentNullException("file");

            _file = file;

            int pageCount;
            double maxPageWidth;

            bool success = file.GetPDFDocInfo(out pageCount, out maxPageWidth);

            if (!success)
                throw new Win32Exception();

            _pageCount = pageCount;
            MaximumPageWidth = maxPageWidth;
        }

        public override void Render(int page, Graphics graphics, float dpiX, float dpiY, Rectangle bounds)
        {
            if (graphics == null)
                throw new ArgumentNullException("graphics");
            if (_disposed)
                throw new ObjectDisposedException(GetType().Name);

            float graphicsDpiX = graphics.DpiX;
            float graphicsDpiY = graphics.DpiY;

            var dc = graphics.GetHdc();

            try
            {
                if ((int)graphicsDpiX != (int)dpiX || (int)graphicsDpiY != (int)dpiY)
                {
                    var transform = new NativeMethods.XFORM
                    {
                        eM11 = graphicsDpiX / dpiX,
                        eM22 = graphicsDpiY / dpiY
                    };

                    NativeMethods.SetGraphicsMode(dc, NativeMethods.GM_ADVANCED);
                    NativeMethods.ModifyWorldTransform(dc, ref transform, NativeMethods.MWT_LEFTMULTIPLY);
                }

                bool success = _file.RenderPDFPageToDC(
                    page,
                    dc,
                    (int)dpiX, (int)dpiY,
                    bounds.X, bounds.Y, bounds.Width, bounds.Height,
                    true /* fitToBounds */,
                    true /* stretchToBounds */,
                    true /* keepAspectRatio */,
                    true /* centerInBounds */,
                    true /* autoRotate */
                );

                if (!success)
                    throw new Win32Exception();
            }
            finally
            {
                graphics.ReleaseHdc(dc);
            }
        }

        public override void Save(string path)
        {
            if (path == null)
                throw new ArgumentNullException("path");

            using (var stream = File.Create(path))
            {
                Save(stream);
            }
        }

        public override void Save(Stream stream)
        {
            if (stream == null)
                throw new ArgumentNullException("stream");

            _file.Save(stream);
        }

        protected override void Dispose(bool disposing)
        {
            if (!_disposed && disposing)
            {
                if (_file != null)
                {
                    _file.Dispose();
                    _file = null;
                }

                _disposed = true;
            }
        }
    }
}
