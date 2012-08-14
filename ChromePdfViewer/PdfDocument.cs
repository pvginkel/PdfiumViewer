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
    /// <summary>
    /// Provides functionality to render a PDF document.
    /// </summary>
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

        /// <summary>
        /// Initializes a new instance of the PdfDocument class with the provided stream.
        /// </summary>
        /// <param name="stream"></param>
        public PdfDocument(Stream stream)
            : this(PdfFile.Create(stream))
        {
        }

        /// <summary>
        /// Initializes a new instance of the PdfDocument class with the provided path.
        /// </summary>
        /// <param name="path"></param>
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

        /// <summary>
        /// Renders a page of the PDF document to the provided graphics instance.
        /// </summary>
        /// <param name="page">Number of the page to render.</param>
        /// <param name="graphics">Graphics instance to render the page on.</param>
        /// <param name="dpiX">Horizontal DPI.</param>
        /// <param name="dpiY">Vertical DPI.</param>
        /// <param name="bounds">Bounds to render the page in.</param>
        /// <param name="fitToBounds">true to fit the rendered page into the specified bounds; otherwise false.</param>
        /// <param name="stretchToBounds">true to stretch the rendered page into the specified bounds; otherwise false.</param>
        /// <param name="keepAspectRatio">true to keep the original aspect ratio in tact; otherwise false.</param>
        /// <param name="centerInBounds">true to center the page within the specified bounds; otherwise false.</param>
        /// <param name="autoRotate">true to rotate the page depending on the specified bounds; otherwise false.</param>
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

        /// <summary>
        /// Save the PDF document to the specified location.
        /// </summary>
        /// <param name="path">Path to save the PDF document to.</param>
        public void Save(string path)
        {
            if (path == null)
                throw new ArgumentNullException("path");

            using (var stream = File.Create(path))
            {
                Save(stream);
            }
        }

        /// <summary>
        /// Save the PDF document to the specified location.
        /// </summary>
        /// <param name="stream">Stream to save the PDF document to.</param>
        public void Save(Stream stream)
        {
            if (stream == null)
                throw new ArgumentNullException("stream");

            _file.Save(stream);
        }

        /// <summary>
        /// Creates a <see cref="PrintDocument"/> for the PDF document.
        /// </summary>
        /// <returns></returns>
        public PrintDocument CreatePrintDocument()
        {
            return new PdfPrintDocument(this);
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        /// <filterpriority>2</filterpriority>
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
