using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Printing;
using System.IO;
using System.Text;

namespace PdfViewer
{
    /// <summary>
    /// Provides functionality to render a PDF document.
    /// </summary>
    public abstract class PdfDocument : IDisposable
    {
        private static readonly PdfEngine _defaultEngine;

        static PdfDocument()
        {
            if (HaveAssembly("PDFLibNet.dll"))
                _defaultEngine = PdfEngine.XPdf;
            else if (HaveAssembly("pdf.dll"))
                _defaultEngine = PdfEngine.Chrome;
            else
                throw new Exception("Cannot resolve default PDF engine");
        }

        private static bool HaveAssembly(string fileName)
        {
            if (File.Exists(fileName))
                return true;

            string assemblyPath = Path.GetDirectoryName(typeof(PdfDocument).Assembly.Location);

            return File.Exists(Path.Combine(assemblyPath, fileName));
        }

        /// <summary>
        /// Initializes a new instance of the PdfDocument class with the provided path.
        /// </summary>
        /// <param name="path"></param>
        public static PdfDocument Load(string path)
        {
            return Load(path, PdfEngine.AutoDetect);
        }

        /// <summary>
        /// Initializes a new instance of the PdfDocument class with the provided path.
        /// </summary>
        /// <param name="path"></param>
        public static PdfDocument Load(string path, PdfEngine engine)
        {
            if (engine == PdfEngine.AutoDetect)
                engine = _defaultEngine;

            switch (engine)
            {
                case PdfEngine.Chrome:
                    return new Chrome.PdfDocument(path);

                case PdfEngine.XPdf:
                    return new XPdf.PdfDocument(path);

                default:
                    throw new ArgumentOutOfRangeException("engine");
            }
        }

        /// <summary>
        /// Initializes a new instance of the PdfDocument class with the provided stream.
        /// </summary>
        /// <param name="stream"></param>
        public static PdfDocument Load(Stream stream)
        {
            return Load(stream, PdfEngine.AutoDetect);
        }

        /// <summary>
        /// Initializes a new instance of the PdfDocument class with the provided stream.
        /// </summary>
        /// <param name="stream"></param>
        public static PdfDocument Load(Stream stream, PdfEngine engine)
        {
            if (engine == PdfEngine.AutoDetect)
                engine = _defaultEngine;

            switch (engine)
            {
                case PdfEngine.Chrome:
                    return new Chrome.PdfDocument(stream);

                case PdfEngine.XPdf:
                    return new XPdf.PdfDocument(stream);

                default:
                    throw new ArgumentOutOfRangeException("engine");
            }
        }

        /// <summary>
        /// Number of pages in the PDF document.
        /// </summary>
        public abstract int PageCount { get; }

        /// <summary>
        /// Renders a page of the PDF document to the provided graphics instance.
        /// </summary>
        /// <param name="page">Number of the page to render.</param>
        /// <param name="graphics">Graphics instance to render the page on.</param>
        /// <param name="dpiX">Horizontal DPI.</param>
        /// <param name="dpiY">Vertical DPI.</param>
        /// <param name="bounds">Bounds to render the page in.</param>
        public abstract void Render(int page, Graphics graphics, float dpiX, float dpiY, Rectangle bounds);

        /// <summary>
        /// Save the PDF document to the specified location.
        /// </summary>
        /// <param name="path">Path to save the PDF document to.</param>
        public abstract void Save(string path);

        /// <summary>
        /// Save the PDF document to the specified location.
        /// </summary>
        /// <param name="stream">Stream to save the PDF document to.</param>
        public abstract void Save(Stream stream);

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

        protected virtual void Dispose(bool disposing)
        {
        }
    }
}
