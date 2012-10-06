using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;
using PDFLibNet;

namespace PdfViewer.XPdf
{
    internal class PdfDocument : global::PdfViewer.PdfDocument
    {
        private PDFWrapper _document = new PDFWrapper();
        private Control _dummyControl = new Control();
        private readonly string _path;
        private bool _disposed;

        static PdfDocument()
        {
            xPDFParams.Antialias = true;
            xPDFParams.VectorAntialias = true;
        }

        public PdfDocument(string path)
        {
            if (path == null)
                throw new ArgumentNullException("path");

            _path = path;

            _document.LoadPDF(path);
        }

        public PdfDocument(Stream stream)
        {
            if (stream == null)
                throw new ArgumentNullException("stream");

            _document.LoadPDF(stream);
        }

        public override int PageCount
        {
            get { return _document.PageCount; }
        }

        public override void Render(int page, Graphics graphics, float dpiX, float dpiY, Rectangle bounds)
        {
            _document.CurrentPage = page + 1;
            _document.RenderDPI = dpiX;
            _document.ClientBounds = bounds;
            _document.CurrentX = -bounds.Left;
            _document.CurrentY = -bounds.Top;

            var hdc = graphics.GetHdc();

            try
            {
                // xPDF uses the control to get sizing information. We use
                // a dummy control to satisfy this requirement.

                _dummyControl.Size = new Size(bounds.Width, 1);

                _document.FitToWidth(_dummyControl.Handle);
                _document.RenderPage(_dummyControl.Handle);
                _document.DrawPageHDC(hdc);
            }
            finally
            {
                graphics.ReleaseHdc(hdc);
            }
        }

        public override void Save(string path)
        {
            if (path == null)
                throw new ArgumentNullException("path");

            if (_path == null)
                throw new NotSupportedException();

            File.Copy(_path, path);
        }

        public override void Save(Stream stream)
        {
            if (stream == null)
                throw new ArgumentNullException("stream");

            if (_path == null)
                throw new NotSupportedException();

            using (var input = File.OpenRead(_path))
            {
                var buffer = new byte[4096];
                int read;

                while ((read = input.Read(buffer, 0, buffer.Length)) != 0)
                {
                    stream.Write(buffer, 0, read);
                }
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (!_disposed && disposing)
            {
                if (_document != null)
                {
                    _document.Dispose();
                    _document = null;
                }

                if (_dummyControl != null)
                {
                    _dummyControl.Dispose();
                    _dummyControl = null;
                }

                _disposed = true;
            }
        }
    }
}
