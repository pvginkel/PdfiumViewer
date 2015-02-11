using PdfiumViewer.IRISTedExtensions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Drawing;
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

        /// <summary>
        /// Renders a page of the PDF document to a Bitmap using FDIB (Foxit Device Independent Bitmap)
        /// </summary>
        /// <param name="page">Number of the page to render.</param>        
        /// <param name="dpiX">Horizontal DPI.</param>
        /// <param name="dpiY">Vertical DPI.</param>
        /// <param name="bounds">Bounds to render the page in.</param>
        /// <param name="forPrinting">Render the page for printing.</param>
        /// <returns>A Bitmap drawn with the given page of the PDF</returns>
        public Bitmap RenderToBitmapUsingRenderPageBitmap(int page, float dpiX, float dpiY, Rectangle bounds, bool forPrinting)
        {
            IntPtr bitmapHandle = IntPtr.Zero;
            Bitmap bitmap = null;

            if (_disposed)
                throw new ObjectDisposedException(GetType().Name);

            try
            {
                int width = bounds.Width;
                int height = bounds.Height;

                bitmapHandle = NativeMethods.FPDFBitmap_Create(bounds.Width, bounds.Height, 1);
                //NativeMethods.FPDFBitmap_FillRect(bitmapHandle, 0, 0, bounds.Width, bounds.Height, 255, 255, 255, 255);

                IntPtr brush = Win32API.CreateSolidBrush((int)ColorTranslator.ToWin32(Color.Orange));
                Win32API.FillRgn(bitmapHandle, Win32API.CreateRectRgn(0, 0, width, height), brush);
                Win32API.DeleteObject(brush);

                bool success = _file.RenderPDFPageToBitmap(
                    page,
                    bitmapHandle,
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
                else
                {
                    byte[] bytes = new byte[bounds.Width * bounds.Height * 4];
                    Marshal.Copy(bitmapHandle, bytes, 0, bytes.Length);
                    bitmap = BitmapHelper.Convert_BGRA_TO_ARGB(bytes, bounds.Width, bounds.Height);
                }
            }
            catch (Exception e)
            {
            }
            finally
            {
                NativeMethods.FPDFBitmap_Destroy(bitmapHandle);
            }

            return bitmap;
        }

        /// <summary>
        /// Renders a page of the PDF document to a Bitmap using Memory Device Context
        /// </summary>
        /// <param name="page">Number of the page to render.</param>        
        /// <param name="dpiX">Horizontal DPI.</param>
        /// <param name="dpiY">Vertical DPI.</param>
        /// <param name="bounds">Bounds to render the page in.</param>
        /// <param name="forPrinting">Render the page for printing.</param>
        /// <returns>A Bitmap drawn with the given page of the PDF</returns>
        public Bitmap RenderToBitmapUsingRenderPage(int page, float dpiX, float dpiY, Rectangle bounds, bool forPrinting)
        {
            IntPtr hDC = IntPtr.Zero;
            IntPtr hMemDC = IntPtr.Zero;
            IntPtr hBitmap = IntPtr.Zero;

            Bitmap bitmap = null;

            if (_disposed)
                throw new ObjectDisposedException(GetType().Name);

            try
            {
                int width = bounds.Width;
                int height = bounds.Height;

                hMemDC = Win32API.CreateCompatibleDC(IntPtr.Zero);
                hDC = Win32API.GetDC(IntPtr.Zero);

                // Create a bitmap for output 
                hBitmap = Win32API.CreateCompatibleBitmap(hDC, width, height);

                // Select the bitmap into the memory DC 
                hBitmap = Win32API.SelectObject(hMemDC, hBitmap);

                IntPtr brush = Win32API.CreateSolidBrush((int)ColorTranslator.ToWin32(Color.White));
                Win32API.FillRgn(hMemDC, Win32API.CreateRectRgn(0, 0, width, height), brush);
                Win32API.DeleteObject(brush);

                //Now render the page onto the memory DC, which in turn changes the bitmap
                bool success = _file.RenderPDFPageToDC(
                     page,
                     hMemDC,
                     (int)dpiX, (int)dpiY,
                     bounds.X, bounds.Y, bounds.Width, bounds.Height,
                     true /* fitToBounds */,
                     true /* stretchToBounds */,
                     true /* keepAspectRatio */,
                     true /* centerInBounds */,
                     true /* autoRotate */,
                     forPrinting
                );


                hBitmap = Win32API.SelectObject(hMemDC, hBitmap);

                bitmap = Bitmap.FromHbitmap(hBitmap);

                if (!success)
                    throw new Win32Exception();
            }
            catch (Exception e)
            {
            }
            finally
            {
                Win32API.DeleteDC(hMemDC);

                Win32API.ReleaseDC(IntPtr.Zero, hDC);

                Win32API.DeleteObject(hBitmap);

                GC.Collect();
            }

            return bitmap;
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
