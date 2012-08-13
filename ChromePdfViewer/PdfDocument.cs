using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Printing;
using System.IO;
using System.Text;

namespace ChromePdfViewer
{
    public class PdfDocument : IDisposable
    {
        private bool _disposed;
        private PdfFile _file;

        /// <summary>
        /// Number of pages in the PDF document.
        /// </summary>
        public int PageCount { get; private set; }

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

            PageCount = pageCount;
            MaximumPageWidth = maxPageWidth;
        }

        public void Render(int page, Graphics graphics, float dpiX, float dpiY, Rectangle bounds, bool fitToBounds, bool stretchToBounds, bool keepAspectRatio, bool centerInBounds, bool autoRotate)
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
                    fitToBounds,
                    stretchToBounds,
                    keepAspectRatio,
                    centerInBounds,
                    autoRotate
                );

                if (!success)
                    throw new Win32Exception();
            }
            finally
            {
                graphics.ReleaseHdc(dc);
            }
        }

        public Metafile[] RenderToMetafiles(float dpiX, float dpiY, int width, int height)
        {
            var result = new Metafile[PageCount];

            for (int i = 0; i < PageCount; i++)
            {
                result[i] = RenderToMetafile(i, dpiX, dpiY, width, height);
            }

            return result;
        }

        public Metafile RenderToMetafile(int page, float dpiX, float dpiY, int width, int height)
        {
            using (var graphics = Graphics.FromHwnd(IntPtr.Zero))
            {
                int targetWidth = (int)((double)width / dpiX * graphics.DpiX);
                int targetHeight = (int)((double)height / dpiY * graphics.DpiY);

                var dc = graphics.GetHdc();

                try
                {
                    var metafile = new Metafile(dc, new Rectangle(0, 0, targetWidth, targetHeight), MetafileFrameUnit.Pixel);

                    using (var drawingGraphics = Graphics.FromImage(metafile))
                    {
                        Render(
                            page,
                            drawingGraphics,
                            dpiX, dpiY,
                            new Rectangle(
                                0,
                                0,
                                width,
                                height
                            ),
                            true /* fitToBounds */,
                            true /* stretchToBounds */,
                            true /* keepAspectRatio */,
                            false /* centerInBounds */,
                            false /* autoRotate */
                        );
                    }

                    return metafile;
                }
                finally
                {
                    graphics.ReleaseHdc(dc);
                }
            }
        }

        public void Save(string path)
        {
            if (path == null)
                throw new ArgumentNullException("path");

            using (var stream = File.Create(path))
            {
                Save(stream);
            }
        }

        public void Save(Stream stream)
        {
            if (stream == null)
                throw new ArgumentNullException("stream");

            _file.Save(stream);
        }

        public PrintDocument CreatePrintDocument()
        {
            return new PdfPrintDocument(this);
        }

        public void Dispose()
        {
            if (!_disposed)
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
