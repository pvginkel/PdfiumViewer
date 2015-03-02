using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Printing;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;

namespace PdfiumViewer
{
    /// <summary>
    /// Provides functionality to render a PDF document.
    /// </summary>
    public class PdfDocument : IDisposable
    {
        private bool _disposed;
        private PdfFile _file;

        /// <summary>
        /// Initializes a new instance of the PdfDocument class with the provided path.
        /// </summary>
        /// <param name="path"></param>
        public static PdfDocument Load(string path)
        {
            return new PdfDocument(path);
        }

        /// <summary>
        /// Initializes a new instance of the PdfDocument class with the provided stream.
        /// </summary>
        /// <param name="stream"></param>
        public static PdfDocument Load(Stream stream)
        {
            return new PdfDocument(stream);
        }

        /// <summary>
        /// Number of pages in the PDF document.
        /// </summary>
        public int PageCount
        {
            get { return PageSizes.Count; }
        }

        /// <summary>
        /// Size of each page in the PDF document.
        /// </summary>
        public IList<SizeF> PageSizes { get; private set; }

        private PdfDocument(Stream stream)
            : this(PdfFile.Create(stream))
        {
        }

        private PdfDocument(string path)
            : this(File.OpenRead(path))
        {
        }

        private PdfDocument(PdfFile file)
        {
            if (file == null)
                throw new ArgumentNullException("file");

            _file = file;

            var pageSizes = file.GetPDFDocInfo();
            if (pageSizes == null)
                throw new Win32Exception();

            PageSizes = new ReadOnlyCollection<SizeF>(pageSizes);
        }

        /// <summary>
        /// Renders a page of the PDF document to the provided graphics instance.
        /// </summary>
        /// <param name="page">Number of the page to render.</param>
        /// <param name="graphics">Graphics instance to render the page on.</param>
        /// <param name="dpiX">Horizontal DPI.</param>
        /// <param name="dpiY">Vertical DPI.</param>
        /// <param name="bounds">Bounds to render the page in.</param>
        /// <param name="forPrinting">Render the page for printing.</param>
        public void Render(int page, Graphics graphics, float dpiX, float dpiY, Rectangle bounds, bool forPrinting)
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
                    true /* autoRotate */,
                    forPrinting
                );

                if (!success)
                    throw new Win32Exception();
            }
            finally
            {
                graphics.ReleaseHdc(dc);
            }
        }

        public Image Render(int page, int width, int height, float dpiX, float dpiY, bool forPrinting,
            bool useFDIB = false)
        {
            if (useFDIB)
                return RenderUsingFDIB(page, width, height, dpiX, dpiY, forPrinting);
            else
                return Render(page, width, height, dpiX, dpiY, forPrinting);
        }

        /// <summary>
        /// Renders a page of the PDF document to an image.
        /// </summary>
        /// <param name="page">Number of the page to render.</param>
        /// <param name="width">Width of the rendered image.</param>
        /// <param name="height">Height of the rendered image.</param>
        /// <param name="dpiX">Horizontal DPI.</param>
        /// <param name="dpiY">Vertical DPI.</param>
        /// <param name="forPrinting">Render the page for printing.</param>
        /// <returns>The rendered image.</returns>
        public Image Render(int page, int width, int height, float dpiX, float dpiY, bool forPrinting)
        {
            if (_disposed)
                throw new ObjectDisposedException(GetType().Name);

            var dc = NativeMethods.CreateCompatibleDC(IntPtr.Zero);
            try
            {
                int dcDpiX = NativeMethods.GetDeviceCaps(dc, NativeMethods.LOGPIXELSX);
                int dcDpiY = NativeMethods.GetDeviceCaps(dc, NativeMethods.LOGPIXELSY);

                if (dcDpiX != (int)dpiX || dcDpiY != (int)dpiY)
                {
                    var transform = new NativeMethods.XFORM
                    {
                        eM11 = dcDpiX / dpiX,
                        eM22 = dcDpiY / dpiY
                    };

                    NativeMethods.SetGraphicsMode(dc, NativeMethods.GM_ADVANCED);
                    NativeMethods.ModifyWorldTransform(dc, ref transform, NativeMethods.MWT_LEFTMULTIPLY);
                }

                var bitmap = NativeMethods.CreateCompatibleBitmap(NativeMethods.GetDC(IntPtr.Zero), width, height);
                try
                {
                    var oldBitmap = NativeMethods.SelectObject(dc, bitmap);
                    try
                    {
                        var brush = NativeMethods.CreateSolidBrush(ColorTranslator.ToWin32(Color.White));
                        try
                        {
                            var oldBrush = NativeMethods.SelectObject(dc, brush);
                            try
                            {
                                NativeMethods.Rectangle(dc, 0, 0, width, height);
                            }
                            finally
                            {
                                NativeMethods.SelectObject(dc, oldBrush);
                            }
                        }
                        finally
                        {
                            NativeMethods.DeleteObject(brush);
                        }

                        // _file is an instance of PdfFile from PdfViewer port
                        // Now render the page onto the memory DC, which in turn changes the bitmap                 
                        bool success = _file.RenderPDFPageToDC(
                            page,
                            dc,
                            (int)dpiX, (int)dpiY,
                            0, 0, width, height,
                            true /* fitToBounds */,
                            true /* stretchToBounds */,
                            true /* keepAspectRatio */,
                            true /* centerInBounds */,
                            true /* autoRotate */,
                            forPrinting
                        );

                        if (!success)
                            throw new Win32Exception();
                    }
                    finally
                    {
                        NativeMethods.SelectObject(dc, oldBitmap);
                    }

                    return Bitmap.FromHbitmap(bitmap);
                }
                finally
                {
                    NativeMethods.DeleteObject(bitmap);
                }
            }
            finally
            {
                NativeMethods.DeleteObject(dc);
            }
        }

        /// <summary>
        /// Renders a page of the PDF document to a Bitmap using FDIB (Foxit Device Independent Bitmap)
        /// </summary>
        /// <param name="page">Number of the page to render.</param>        
        /// <param name="dpiX">Horizontal DPI.</param>
        /// <param name="dpiY">Vertical DPI.</param>
        /// <param name="bounds">Bounds to render the page in.</param>
        /// <param name="forPrinting">Render the page for printing.</param>
        /// <returns>The result Bitmap</returns>
        public Image RenderUsingFDIB(int page, int width, int height, float dpiX, float dpiY, bool forPrinting)
        {
            if (_disposed)
                throw new ObjectDisposedException(GetType().Name);

            IntPtr bitmapHandle = IntPtr.Zero;
            Bitmap bitmap = new Bitmap(width, height, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            BitmapData bData = bitmap.LockBits(
                    new Rectangle(0, 0, width, height), ImageLockMode.ReadWrite, bitmap.PixelFormat);

            try
            {
                // During external function call bitmap data will be pinned automatically, so no reason to worry GC will reallocate it
                bitmapHandle = NativeMethods.FPDFBitmap_CreateEx(width, height, 4, bData.Scan0, width * 4);
                NativeMethods.FPDFBitmap_FillRect(bitmapHandle, 0, 0, width, height, new FPDFColor(0xFFFFFFFF));

                bool success = _file.RenderPDFPageToBitmap(
                    page,
                    bitmapHandle,
                    (int)dpiX, (int)dpiY,
                    0, 0, width, height,
                    true /* fitToBounds */,
                    true /* stretchToBounds */,
                    true /* keepAspectRatio */,
                    true /* centerInBounds */,
                    true /* autoRotate */,
                    forPrinting
                );

                if (!success)
                    throw new Win32Exception();
                else
                {
                    bitmap.UnlockBits(bData);
                }
            }
            catch (Exception e)
            {
            }
            finally
            {
                // This call will not destroy external buffer
                NativeMethods.FPDFBitmap_Destroy(bitmapHandle);
            }

            return bitmap;
        }

        /// <summary>
        /// Renders a page of the PDF document to a byte[] using FDIB (Foxit Device Independent Bitmap)
        /// </summary>
        /// <param name="page">Number of the page to render.</param>        
        /// <param name="dpiX">Horizontal DPI.</param>
        /// <param name="dpiY">Vertical DPI.</param>
        /// <param name="bounds">Bounds to render the page in.</param>
        /// <param name="forPrinting">Render the page for printing.</param>
        /// <returns>The raw byte array of pixel data</returns>
        public byte[] RenderToByteArray(int page, int width, int height, float dpiX, float dpiY, bool forPrinting)
        {
            if (_disposed)
                throw new ObjectDisposedException(GetType().Name);

            IntPtr bitmapHandle = IntPtr.Zero;
            var bytes = new byte[width * height * 4];

            try
            {
                // During external function call bitmap data will be pinned automatically, so no reason to worry GC will reallocate it
                bitmapHandle = NativeMethods.FPDFBitmap_CreateEx(width, height, 4, bytes, width * 4);
                NativeMethods.FPDFBitmap_FillRect(bitmapHandle, 0, 0, width, height, new FPDFColor(0xFFFFFFFF));

                bool success = _file.RenderPDFPageToBitmap(
                    page,
                    bitmapHandle,
                    (int)dpiX, (int)dpiY,
                    0, 0, width, height,
                    true /* fitToBounds */,
                    true /* stretchToBounds */,
                    true /* keepAspectRatio */,
                    true /* centerInBounds */,
                    true /* autoRotate */,
                    forPrinting
                );

                if (!success)
                    throw new Win32Exception();
                else
                    return bytes;
            }
            catch (Exception e)
            {
            }
            finally
            {
                // This call will not destroy external buffer
                NativeMethods.FPDFBitmap_Destroy(bitmapHandle);
            }

            return null;
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

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected void Dispose(bool disposing)
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
