using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Printing;
using System.IO;
using System.Text;

namespace PdfiumViewer
{
    /// <summary>
    /// Represents a PDF document.
    /// </summary>
    public interface IPdfDocument : IDisposable
    {
        /// <summary>
        /// Number of pages in the PDF document.
        /// </summary>
        int PageCount { get; }

        /// <summary>
        /// Bookmarks stored in this PdfFile
        /// </summary>
        PdfBookmarkCollection Bookmarks { get; }

        /// <summary>
        /// Size of each page in the PDF document.
        /// </summary>
        IList<SizeF> PageSizes { get; }

        /// <summary>
        /// Renders a page of the PDF document to the provided graphics instance.
        /// </summary>
        /// <param name="page">Number of the page to render.</param>
        /// <param name="graphics">Graphics instance to render the page on.</param>
        /// <param name="dpiX">Horizontal DPI.</param>
        /// <param name="dpiY">Vertical DPI.</param>
        /// <param name="bounds">Bounds to render the page in.</param>
        /// <param name="forPrinting">Render the page for printing.</param>
        void Render(int page, Graphics graphics, float dpiX, float dpiY, Rectangle bounds, bool forPrinting);

        /// <summary>
        /// Renders a page of the PDF document to the provided graphics instance.
        /// </summary>
        /// <param name="page">Number of the page to render.</param>
        /// <param name="graphics">Graphics instance to render the page on.</param>
        /// <param name="dpiX">Horizontal DPI.</param>
        /// <param name="dpiY">Vertical DPI.</param>
        /// <param name="bounds">Bounds to render the page in.</param>
        /// <param name="flags">Flags used to influence the rendering.</param>
        void Render(int page, Graphics graphics, float dpiX, float dpiY, Rectangle bounds, PdfRenderFlags flags);

        /// <summary>
        /// Renders a page of the PDF document to an image.
        /// </summary>
        /// <param name="page">Number of the page to render.</param>
        /// <param name="dpiX">Horizontal DPI.</param>
        /// <param name="dpiY">Vertical DPI.</param>
        /// <param name="forPrinting">Render the page for printing.</param>
        /// <returns>The rendered image.</returns>
        Image Render(int page, float dpiX, float dpiY, bool forPrinting);

        /// <summary>
        /// Renders a page of the PDF document to an image.
        /// </summary>
        /// <param name="page">Number of the page to render.</param>
        /// <param name="dpiX">Horizontal DPI.</param>
        /// <param name="dpiY">Vertical DPI.</param>
        /// <param name="flags">Flags used to influence the rendering.</param>
        /// <returns>The rendered image.</returns>
        Image Render(int page, float dpiX, float dpiY, PdfRenderFlags flags);

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
        Image Render(int page, int width, int height, float dpiX, float dpiY, bool forPrinting);

        /// <summary>
        /// Renders a page of the PDF document to an image.
        /// </summary>
        /// <param name="page">Number of the page to render.</param>
        /// <param name="width">Width of the rendered image.</param>
        /// <param name="height">Height of the rendered image.</param>
        /// <param name="dpiX">Horizontal DPI.</param>
        /// <param name="dpiY">Vertical DPI.</param>
        /// <param name="flags">Flags used to influence the rendering.</param>
        /// <returns>The rendered image.</returns>
        Image Render(int page, int width, int height, float dpiX, float dpiY, PdfRenderFlags flags);

        /// <summary>
        /// Renders a page of the PDF document to an image.
        /// </summary>
        /// <param name="page">Number of the page to render.</param>
        /// <param name="width">Width of the rendered image.</param>
        /// <param name="height">Height of the rendered image.</param>
        /// <param name="dpiX">Horizontal DPI.</param>
        /// <param name="dpiY">Vertical DPI.</param>
        /// <param name="rotate">Rotation.</param>
        /// <param name="flags">Flags used to influence the rendering.</param>
        /// <returns>The rendered image.</returns>
        Image Render(int page, int width, int height, float dpiX, float dpiY, PdfRotation rotate, PdfRenderFlags flags);

        /// <summary>
        /// Save the PDF document to the specified location.
        /// </summary>
        /// <param name="path">Path to save the PDF document to.</param>
        void Save(string path);

        /// <summary>
        /// Save the PDF document to the specified location.
        /// </summary>
        /// <param name="stream">Stream to save the PDF document to.</param>
        void Save(Stream stream);

        /// <summary>
        /// Finds all occurences of text.
        /// </summary>
        /// <param name="text">The text to search for.</param>
        /// <param name="matchCase">Whether to match case.</param>
        /// <param name="wholeWord">Whether to match whole words only.</param>
        /// <returns>All matches.</returns>
        PdfMatches Search(string text, bool matchCase, bool wholeWord);

        /// <summary>
        /// Finds all occurences of text.
        /// </summary>
        /// <param name="text">The text to search for.</param>
        /// <param name="matchCase">Whether to match case.</param>
        /// <param name="wholeWord">Whether to match whole words only.</param>
        /// <param name="page">The page to search on.</param>
        /// <returns>All matches.</returns>
        PdfMatches Search(string text, bool matchCase, bool wholeWord, int page);

        /// <summary>
        /// Finds all occurences of text.
        /// </summary>
        /// <param name="text">The text to search for.</param>
        /// <param name="matchCase">Whether to match case.</param>
        /// <param name="wholeWord">Whether to match whole words only.</param>
        /// <param name="startPage">The page to start searching.</param>
        /// <param name="endPage">The page to end searching.</param>
        /// <returns>All matches.</returns>
        PdfMatches Search(string text, bool matchCase, bool wholeWord, int startPage, int endPage);

        /// <summary>
        /// Creates a <see cref="PrintDocument"/> for the PDF document.
        /// </summary>
        /// <returns></returns>
        PrintDocument CreatePrintDocument();

        /// <summary>
        /// Creates a <see cref="PrintDocument"/> for the PDF document.
        /// </summary>
        /// <param name="printMode">Specifies the mode for printing. The default
        /// value for this parameter is CutMargin.</param>
        /// <returns></returns>
        PrintDocument CreatePrintDocument(PdfPrintMode printMode);

        /// <summary>
        /// Creates a <see cref="PrintDocument"/> for the PDF document.
        /// </summary>
        /// <param name="settings">The settings used to configure the print document.</param>
        /// <returns></returns>
        PrintDocument CreatePrintDocument(PdfPrintSettings settings);

        /// <summary>
        /// Returns all links on the PDF page.
        /// </summary>
        /// <param name="page">The page to get the links for.</param>
        /// <param name="size">The size of the page.</param>
        /// <returns>A collection with the links on the page.</returns>
        PdfPageLinks GetPageLinks(int page, Size size);

        /// <summary>
        /// Delete the page from the PDF document.
        /// </summary>
        /// <param name="page">The page to delete.</param>
        void DeletePage(int page);

        /// <summary>
        /// Rotate the page.
        /// </summary>
        /// <param name="page">The page to rotate.</param>
        /// <param name="rotation">How to rotate the page.</param>
        void RotatePage(int page, PdfRotation rotation);

        /// <summary>
        /// Get metadata information from the PDF document.
        /// </summary>
        /// <returns>The PDF metadata.</returns>
        PdfInformation GetInformation();

        /// <summary>
        /// Get all text on the page.
        /// </summary>
        /// <param name="page">The page to get the text for.</param>
        /// <returns>The text on the page.</returns>
        string GetPdfText(int page);

        /// <summary>
        /// Get all text matching the text span.
        /// </summary>
        /// <param name="textSpan">The span to get the text for.</param>
        /// <returns>The text matching the span.</returns>
        string GetPdfText(PdfTextSpan textSpan);

        /// <summary>
        /// Get all bounding rectangles for the text span.
        /// </summary>
        /// <description>
        /// The algorithm used to get the bounding rectangles tries to join
        /// adjacent character bounds into larger rectangles.
        /// </description>
        /// <param name="textSpan">The span to get the bounding rectangles for.</param>
        /// <returns>The bounding rectangles.</returns>
        IList<PdfRectangle> GetTextBounds(PdfTextSpan textSpan);

        /// <summary>
        /// Convert a point from device coordinates to page coordinates.
        /// </summary>
        /// <param name="page">The page number where the point is from.</param>
        /// <param name="point">The point to convert.</param>
        /// <returns>The converted point.</returns>
        PointF PointToPdf(int page, Point point);

        /// <summary>
        /// Convert a point from page coordinates to device coordinates.
        /// </summary>
        /// <param name="page">The page number where the point is from.</param>
        /// <param name="point">The point to convert.</param>
        /// <returns>The converted point.</returns>
        Point PointFromPdf(int page, PointF point);

        /// <summary>
        /// Convert a rectangle from device coordinates to page coordinates.
        /// </summary>
        /// <param name="page">The page where the rectangle is from.</param>
        /// <param name="rect">The rectangle to convert.</param>
        /// <returns>The converted rectangle.</returns>
        RectangleF RectangleToPdf(int page, Rectangle rect);

        /// <summary>
        /// Convert a rectangle from page coordinates to device coordinates.
        /// </summary>
        /// <param name="page">The page where the rectangle is from.</param>
        /// <param name="rect">The rectangle to convert.</param>
        /// <returns>The converted rectangle.</returns>
        Rectangle RectangleFromPdf(int page, RectangleF rect);

        /// <summary>
        /// Get the character index at or nearby a specific position. 
        /// </summary>
        /// <param name="location">The location to inspect</param>
        /// <param name="xTolerance">An x-axis tolerance value for character hit detection, in point unit.</param>
        /// <param name="yTolerance">A y-axis tolerance value for character hit detection, in point unit.</param>
        /// <returns>The zero-based index of the character at, or nearby the point specified by parameter x and y. If there is no character at or nearby the point, it will return -1.</returns>
        int GetCharacterIndexAtPosition(PdfPoint location, double xTolerance, double yTolerance);

        /// <summary>
        /// Get the full word at or nearby a specific position
        /// </summary>
        /// <param name="location">The location to inspect</param>
        /// <param name="xTolerance">An x-axis tolerance value for character hit detection, in point unit.</param>
        /// <param name="yTolerance">A y-axis tolerance value for character hit detection, in point unit.</param>
        /// <param name="span">The location of the found word, if any</param>
        /// <returns>A value indicating whether a word was found at the specified location</returns>
        bool GetWordAtPosition(PdfPoint location, double xTolerance, double yTolerance, out PdfTextSpan span);

        /// <summary>
        /// Get number of characters in a page.
        /// </summary>
        /// <param name="page">The page to get the character count from</param>
        /// <returns>Number of characters in the page. Generated characters, like additional space characters, new line characters, are also counted.</returns>
        int CountCharacters(int page);

        /// <summary>
        /// Gets the rectangular areas occupied by a segment of text
        /// </summary>
        /// <param name="page">The page to get the rectangles from</param>
        /// <returns>The rectangular areas occupied by a segment of text</returns>
        List<PdfRectangle> GetTextRectangles(int page, int startIndex, int count);
    }
}
