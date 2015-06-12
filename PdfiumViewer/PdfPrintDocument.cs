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
        private readonly PdfPrintMode _printMode;
        private int _currentPage;

        public PdfPrintDocument(PdfDocument document, PdfPrintMode printMode)
        {
            if (document == null)
                throw new ArgumentNullException("document");

            _document = document;
            _printMode = printMode;
        }

        protected override void OnBeginPrint(PrintEventArgs e)
        {
            _currentPage = PrinterSettings.FromPage == 0 ? 0 : PrinterSettings.FromPage - 1;
        }

        protected override void OnQueryPageSettings(QueryPageSettingsEventArgs e)
        {
            if (_currentPage < _document.PageCount)
            {
                // Some printers misreport landscape. The below check verifies
                // whether the page rotation matches the landscape setting.
                bool inverseLandscape = e.PageSettings.Bounds.Width > e.PageSettings.Bounds.Height != e.PageSettings.Landscape;

                bool landscape = GetOrientation(_document.PageSizes[_currentPage]) == Orientation.Landscape;

                if (inverseLandscape)
                    landscape = !landscape;

                e.PageSettings.Landscape = landscape;
            }
        }

        protected override void OnPrintPage(PrintPageEventArgs e)
        {
            if (_currentPage < _document.PageCount)
            {
                var pageOrientation = GetOrientation(_document.PageSizes[_currentPage]);
                var printOrientation = GetOrientation(e.PageBounds.Size);

                e.PageSettings.Landscape = pageOrientation == Orientation.Landscape;

                double left;
                double top;
                double width;
                double height;

                if (_printMode == PdfPrintMode.ShrinkToMargin)
                {
                    left = 0;
                    top = 0;
                    width = e.PageBounds.Width - e.PageSettings.HardMarginX * 2;
                    height = e.PageBounds.Height - e.PageSettings.HardMarginY * 2;
                }
                else
                {
                    left = -e.PageSettings.HardMarginX;
                    top = -e.PageSettings.HardMarginY;
                    width = e.PageBounds.Width;
                    height = e.PageBounds.Height;
                }

                if (pageOrientation != printOrientation)
                {
                    double tmp = width;
                    width = height;
                    height = tmp;

                    tmp = left;
                    left = top;
                    top = tmp;
                }

                _document.Render(
                    _currentPage++,
                    e.Graphics,
                    e.Graphics.DpiX,
                    e.Graphics.DpiY,
                    new Rectangle(
                        AdjustDpi(e.Graphics.DpiX, left),
                        AdjustDpi(e.Graphics.DpiY, top),
                        AdjustDpi(e.Graphics.DpiX, width),
                        AdjustDpi(e.Graphics.DpiY, height)
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

        private static int AdjustDpi(double value, double dpi)
        {
            return (int)((value / 100.0) * dpi);
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
