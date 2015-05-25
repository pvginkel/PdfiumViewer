using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Printing;
using System.Text;

namespace PdfiumViewer
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

        protected override void OnQueryPageSettings(QueryPageSettingsEventArgs e)
        {
            if (_currentPage < _document.PageCount)
                e.PageSettings.Landscape = GetOrientation(_document.PageSizes[_currentPage]) == Orientation.Landscape;
        }

        protected override void OnPrintPage(PrintPageEventArgs e)
        {
            if (_currentPage < _document.PageCount)
            {
                var pageOrientation = GetOrientation(_document.PageSizes[_currentPage]);
                var printOrientation = GetOrientation(e.PageBounds.Size);

                e.PageSettings.Landscape = pageOrientation == Orientation.Landscape;

                int width = e.PageBounds.Width;
                int height = e.PageBounds.Height;

                if (pageOrientation != printOrientation)
                {
                    int tmp = width;
                    width = height;
                    height = tmp;
                }

                _document.Render(
                    _currentPage++,
                    e.Graphics,
                    e.Graphics.DpiX,
                    e.Graphics.DpiY,
                    new Rectangle(
                        0,
                        0,
                        (int)((width / 100.0) * e.Graphics.DpiX),
                        (int)((height / 100.0) * e.Graphics.DpiY)
                    ),
                    true
                );
            }

            int pageCount =
                PrinterSettings.ToPage == 0
                ? _document.PageCount
                : Math.Min(PrinterSettings.ToPage, _document.PageCount);

            e.HasMorePages = _currentPage < pageCount;
        }

        private Orientation GetOrientation(SizeF pageSize)
        {
            if (pageSize.Height > pageSize.Width)
                return Orientation.Portrait;
            return Orientation.Landscape;
        }

        private enum Orientation
        {
            Portrait,
            Landscape
        }
    }
}
