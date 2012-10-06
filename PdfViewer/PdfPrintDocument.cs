using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Printing;
using System.Text;

namespace PdfViewer
{
    internal class PdfPrintDocument : PrintDocument
    {
        private readonly PdfDocument _document;
        private int _currentPage;

        public PdfPrintDocument(PdfDocument document)
        {
            if (document == null)
                throw new ArgumentNullException("document");

            _document = document;
        }

        protected override void OnBeginPrint(PrintEventArgs e)
        {
            _currentPage = PrinterSettings.FromPage == 0 ? 0 : PrinterSettings.FromPage - 1;
        }

        protected override void OnPrintPage(PrintPageEventArgs e)
        {
            if (_currentPage < _document.PageCount)
            {
                _document.Render(
                    _currentPage++,
                    e.Graphics,
                    e.PageSettings.PrinterResolution.X,
                    e.PageSettings.PrinterResolution.Y,
                    new Rectangle(
                        0,
                        0,
                        (int)((e.PageBounds.Width / 100.0) * e.PageSettings.PrinterResolution.X),
                        (int)((e.PageBounds.Height / 100.0) * e.PageSettings.PrinterResolution.Y)
                    )
                );
            }

            int pageCount =
                PrinterSettings.ToPage == 0
                ? _document.PageCount
                : Math.Min(PrinterSettings.ToPage, _document.PageCount);

            e.HasMorePages = _currentPage < pageCount;
        }
    }
}
