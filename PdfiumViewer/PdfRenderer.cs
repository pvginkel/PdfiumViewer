using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;

namespace PdfiumViewer
{
    /// <summary>
    /// Control to render PDF documents.
    /// </summary>
    public class PdfRenderer : PanningZoomingScrollControl
    {
        private static readonly Padding PageMargin = new Padding(4);

        private int _height;
        private int _maxWidth;
        private int _maxHeight;
        private double _documentScaleFactor;
        private bool _disposed;
        private double _scaleFactor;
        private ShadeBorder _shadeBorder = new ShadeBorder();
        private int _suspendPaintCount;
        private PdfDocument _document;
        private ToolTip _toolTip;
        private PdfViewerZoomMode _zoomMode;
        private bool _pageCacheValid;
        private readonly List<PageCache> _pageCache = new List<PageCache>();
        private int _visiblePageStart;
        private int _visiblePageEnd;
        private PdfPageLink _cachedLink;
        private DragState _dragState;
        private PdfRotation _rotation;

        /// <summary>
        /// Gets or sets a value indicating whether the user can give the focus to this control using the TAB key.
        /// </summary>
        /// 
        /// <returns>
        /// true if the user can give the focus to the control using the TAB key; otherwise, false. The default is true.Note:This property will always return true for an instance of the <see cref="T:System.Windows.Forms.Form"/> class.
        /// </returns>
        /// <filterpriority>1</filterpriority><PermissionSet><IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/><IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/><IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence"/><IPermission class="System.Diagnostics.PerformanceCounterPermission, System, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/></PermissionSet>
        [DefaultValue(true)]
        public new bool TabStop
        {
            get { return base.TabStop; }
            set { base.TabStop = value; }
        }

        public int Page
        {
            get
            {
                if (_document == null || !_pageCacheValid)
                    return 0;

                int top = -DisplayRectangle.Top + (int)(ClientSize.Height * Zoom) / 2;

                for (int page = 0; page < _document.PageSizes.Count; page++)
                {
                    var pageCache = _pageCache[page].OuterBounds;
                    if (top >= pageCache.Top && top < pageCache.Bottom)
                        return page;
                }

                return _document.PageCount - 1;
            }
            set
            {
                if (_document == null)
                {
                    SetDisplayRectLocation(new Point(0, 0));
                }
                else
                {
                    int page = Math.Min(Math.Max(value, 0), _document.PageCount - 1);

                    SetDisplayRectLocation(new Point(0, -_pageCache[page].OuterBounds.Top));
                }
            }
        }

        /// <summary>
        /// Gets or sets the way the document should be zoomed initially.
        /// </summary>
        public PdfViewerZoomMode ZoomMode
        {
            get { return _zoomMode; }
            set
            {
                _zoomMode = value;
                PerformLayout();
            }
        }

        /// <summary>
        /// Gets or sets the current rotation of the PDF document.
        /// </summary>
        public PdfRotation Rotation
        {
            get { return _rotation; }
            set
            {
                if (_rotation != value)
                {
                    _rotation = value;
                    ResetFromRotation();
                }
            }
        }

        /// <summary>
        /// Initializes a new instance of the PdfRenderer class.
        /// </summary>
        public PdfRenderer()
        {
            SetStyle(ControlStyles.OptimizedDoubleBuffer | ControlStyles.ResizeRedraw | ControlStyles.AllPaintingInWmPaint, true);

            TabStop = true;

            _toolTip = new ToolTip();
        }

        /// <summary>
        /// Raises the <see cref="E:System.Windows.Forms.Control.Layout"/> event.
        /// </summary>
        /// <param name="levent">A <see cref="T:System.Windows.Forms.LayoutEventArgs"/> that contains the event data. </param>
        protected override void OnLayout(LayoutEventArgs levent)
        {
            base.OnLayout(levent);

            UpdateScrollbars();
        }

        protected override void OnZoomChanged(EventArgs e)
        {
            base.OnZoomChanged(e);

            UpdateScrollbars();
        }

        /// <summary>
        /// Load a <see cref="PdfDocument"/> into the control.
        /// </summary>
        /// <param name="document">Document to load.</param>
        public void Load(PdfDocument document)
        {
            if (document == null)
                throw new ArgumentNullException("document");
            if (document.PageCount == 0)
                throw new ArgumentException("Document does not contain any pages", "document");

            _document = document;

            SetDisplayRectLocation(new Point(0, 0));

            ReloadDocument();
        }

        private void ReloadDocument()
        {
            _height = 0;
            _maxWidth = 0;
            _maxHeight = 0;

            foreach (var size in _document.PageSizes)
            {
                var translated = TranslateSize(size);
                _height += (int)translated.Height;
                _maxWidth = Math.Max((int)translated.Width, _maxWidth);
                _maxHeight = Math.Max((int)translated.Height, _maxHeight);
            }

            _documentScaleFactor = _maxHeight != 0 ? (double)_maxWidth / _maxHeight : 0D;

            UpdateScrollbars();

            Invalidate();
        }

        private void UpdateScrollbars()
        {
            if (_document == null)
                return;

            UpdateScaleFactor(ScrollBars.Both);

            var bounds = GetScrollClientArea(ScrollBars.Both);

            var documentSize = GetDocumentBounds().Size;

            bool horizontalVisible = documentSize.Width > bounds.Width;

            if (!horizontalVisible)
            {
                UpdateScaleFactor(ScrollBars.Vertical);

                documentSize = GetDocumentBounds().Size;
            }

            _suspendPaintCount++;

            try
            {
                SetDisplaySize(documentSize);
            }
            finally
            {
                _suspendPaintCount--;
            }

            RebuildPageCache();
        }

        private void RebuildPageCache()
        {
            if (_document == null || _suspendPaintCount > 0)
                return;

            _pageCacheValid = true;

            int maxWidth = (int)(_maxWidth * _scaleFactor) + ShadeBorder.Size.Horizontal + PageMargin.Horizontal;
            int leftOffset = -maxWidth / 2;

            int offset = 0;

            for (int page = 0; page < _document.PageSizes.Count; page++)
            {
                var size = TranslateSize(_document.PageSizes[page]);
                int height = (int)(size.Height * _scaleFactor);
                int fullHeight = height + ShadeBorder.Size.Vertical + PageMargin.Vertical;
                int width = (int)(size.Width * _scaleFactor);
                int maxFullWidth = (int)(_maxWidth * _scaleFactor) + ShadeBorder.Size.Horizontal + PageMargin.Horizontal;
                int fullWidth = width + ShadeBorder.Size.Horizontal + PageMargin.Horizontal;
                int thisLeftOffset = leftOffset + (maxFullWidth - fullWidth) / 2;

                while (_pageCache.Count <= page)
                {
                    _pageCache.Add(new PageCache());
                }

                var pageCache = _pageCache[page];

                pageCache.Links = null;
                pageCache.Bounds = new Rectangle(
                    thisLeftOffset + ShadeBorder.Size.Left + PageMargin.Left,
                    offset + ShadeBorder.Size.Top + PageMargin.Top,
                    width,
                    height
                );
                pageCache.OuterBounds = new Rectangle(
                    thisLeftOffset,
                    offset,
                    width + ShadeBorder.Size.Horizontal + PageMargin.Horizontal,
                    height + ShadeBorder.Size.Vertical + PageMargin.Vertical
                );

                offset += fullHeight;
            }
        }

        private PdfPageLinks GetPageLinks(int page)
        {
            var pageCache = _pageCache[page];
            if (pageCache.Links == null)
                pageCache.Links = _document.GetPageLinks(page, pageCache.Bounds.Size);

            return pageCache.Links;
        }

        private Rectangle GetScrollClientArea()
        {
            ScrollBars scrollBarsVisible;

            if (HScroll && VScroll)
                scrollBarsVisible = ScrollBars.Both;
            else if (HScroll)
                scrollBarsVisible = ScrollBars.Horizontal;
            else if (VScroll)
                scrollBarsVisible = ScrollBars.Vertical;
            else
                scrollBarsVisible = ScrollBars.None;

            return GetScrollClientArea(scrollBarsVisible);
        }

        private Rectangle GetScrollClientArea(ScrollBars scrollbars)
        {
            return new Rectangle(
                0,
                0,
                scrollbars == ScrollBars.Vertical || scrollbars == ScrollBars.Both ? Width - SystemInformation.VerticalScrollBarWidth : Width,
                scrollbars == ScrollBars.Horizontal || scrollbars == ScrollBars.Both ? Height - SystemInformation.HorizontalScrollBarHeight : Height
            );
        }

        private void UpdateScaleFactor(ScrollBars scrollBars)
        {
            var bounds = GetScrollClientArea(scrollBars);

            // Scale factor determines what we need to multiply the dimensions
            // of the metafile with to get the size in the control.

            var zoomMode = CalculateZoomModeForFitBest(bounds);

            if (zoomMode == PdfViewerZoomMode.FitHeight)
            {
                int height = bounds.Height - ShadeBorder.Size.Vertical - PageMargin.Vertical;

                _scaleFactor = ((double)height / _maxHeight) * Zoom;
            }
            else
            {
                int width = bounds.Width - ShadeBorder.Size.Horizontal - PageMargin.Horizontal;

                _scaleFactor = ((double)width / _maxWidth) * Zoom;
            }
        }

        private PdfViewerZoomMode CalculateZoomModeForFitBest(Rectangle bounds)
        {
            if (ZoomMode != PdfViewerZoomMode.FitBest)
            {
                return ZoomMode;
            }

            var controlScaleFactor = (double)bounds.Width / bounds.Height;

            return controlScaleFactor >= _documentScaleFactor ? PdfViewerZoomMode.FitHeight : PdfViewerZoomMode.FitWidth;
        }

        /// <summary>
        /// Raises the <see cref="E:System.Windows.Forms.Control.Paint"/> event.
        /// </summary>
        /// <param name="e">A <see cref="T:System.Windows.Forms.PaintEventArgs"/> that contains the event data. </param>
        protected override void OnPaint(PaintEventArgs e)
        {
            if (_document == null || _suspendPaintCount > 0)
                return;

            var bounds = GetScrollClientArea();
            int maxWidth = (int)(_maxWidth * _scaleFactor) + ShadeBorder.Size.Horizontal + PageMargin.Horizontal;
            int leftOffset = (HScroll ? DisplayRectangle.X : (bounds.Width - maxWidth) / 2) + maxWidth / 2;
            int topOffset = VScroll ? DisplayRectangle.Y : 0;

            using (var brush = new SolidBrush(BackColor))
            {
                e.Graphics.FillRectangle(brush, e.ClipRectangle);
            }

            _visiblePageStart = -1;
            _visiblePageEnd = -1;

            for (int page = 0; page < _document.PageSizes.Count; page++)
            {
                var pageCache = _pageCache[page];
                var rectangle = pageCache.OuterBounds;
                rectangle.Offset(leftOffset, topOffset);

                if (_visiblePageStart == -1 && rectangle.Bottom >= 0)
                    _visiblePageStart = page;
                if (_visiblePageEnd == -1 && rectangle.Top > bounds.Height)
                    _visiblePageEnd = page - 1;

                if (e.ClipRectangle.IntersectsWith(rectangle))
                {
                    var pageBounds = pageCache.Bounds;
                    pageBounds.Offset(leftOffset, topOffset);

                    e.Graphics.FillRectangle(Brushes.White, pageBounds);

                    DrawPageImage(e.Graphics, page, pageBounds);

                    _shadeBorder.Draw(e.Graphics, pageBounds);
                }
            }

            if (_visiblePageStart == -1)
                _visiblePageStart = 0;
            if (_visiblePageEnd == -1)
                _visiblePageEnd = _document.PageCount - 1;
        }

        private void DrawPageImage(Graphics graphics, int page, Rectangle pageBounds)
        {
            if (Rotation == PdfRotation.Rotate0)
            {
                _document.Render(page, graphics, graphics.DpiX, graphics.DpiY, pageBounds, PdfRenderFlags.Annotations);
            }
            else
            {
                // This is not ideal. Rather than this, we should set a world
                // transform in PdfDocument.Render to natively rotate the output.
                // However, this does not work, and the control isn't updated
                // anymore. So we fall back to rendering to a Bitmap and rotating
                // that.

                RotateFlipType type;
                Size size;

                switch (Rotation)
                {
                    case PdfRotation.Rotate90:
                        type = RotateFlipType.Rotate90FlipNone;
                        size = new Size(pageBounds.Height, pageBounds.Width);
                        break;
                    case PdfRotation.Rotate180:
                        type = RotateFlipType.Rotate180FlipNone;
                        size = pageBounds.Size;
                        break;
                    case PdfRotation.Rotate270:
                        type = RotateFlipType.Rotate270FlipNone;
                        size = new Size(pageBounds.Height, pageBounds.Width);
                        break;
                    default:
                        throw new InvalidOperationException();
                }

                using (var rendered = _document.Render(page, size.Width, size.Height, graphics.DpiX, graphics.DpiY, PdfRenderFlags.Annotations))
                {
                    rendered.RotateFlip(type);

                    graphics.DrawImageUnscaled(
                        rendered,
                        pageBounds.Location
                    );
                }
            }
        }

        protected override Rectangle GetDocumentBounds()
        {
            int height = (int)(_height * _scaleFactor + (ShadeBorder.Size.Vertical + PageMargin.Vertical) * _document.PageCount);
            int width = (int)(_maxWidth * _scaleFactor + ShadeBorder.Size.Horizontal + PageMargin.Horizontal);
            
            var center = new Point(
                DisplayRectangle.Width / 2,
                DisplayRectangle.Height / 2
            );

            if (
                DisplayRectangle.Width > ClientSize.Width ||
                DisplayRectangle.Height > ClientSize.Height
            ) {
                center.X += DisplayRectangle.Left;
                center.Y += DisplayRectangle.Top;
            }

            return new Rectangle(
                center.X - width / 2,
                center.Y - height / 2,
                width,
                height
            );
        }

        protected override void OnSetCursor(SetCursorEventArgs e)
        {
            _cachedLink = null;

            if (_pageCacheValid)
            {
                var bounds = GetScrollClientArea();
                int maxWidth = (int)(_maxWidth * _scaleFactor) + ShadeBorder.Size.Horizontal + PageMargin.Horizontal;
                int leftOffset = (HScroll ? DisplayRectangle.X : (bounds.Width - maxWidth) / 2) + maxWidth / 2;

                var displayLocation = DisplayRectangle.Location;

                var location = new Point(
                    e.Location.X,
                    e.Location.Y - displayLocation.Y
                );

                for (int page = _visiblePageStart; page <= _visiblePageEnd; page++)
                {
                    var links = GetPageLinks(page);

                    var pageLocation = location;
                    var pageBounds = _pageCache[page].Bounds;
                    pageLocation.X -= leftOffset + pageBounds.Left;
                    pageLocation.Y -= pageBounds.Top;

                    foreach (var link in links.Links)
                    {
                        if (link.Bounds.Contains(pageLocation))
                        {
                            _cachedLink = link;
                            e.Cursor = Cursors.Hand;
                            return;
                        }
                    }
                }
            }

            base.OnSetCursor(e);
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);

            _dragState = null;

            if (_cachedLink != null)
            {
                _dragState = new DragState
                {
                    Link = _cachedLink,
                    Location = e.Location
                };
            }
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);

            if (_dragState == null)
                return;

            int dx = Math.Abs(e.Location.X - _dragState.Location.X);
            int dy = Math.Abs(e.Location.Y - _dragState.Location.Y);

            var link = _dragState.Link;
            _dragState = null;

            if (link == null)
                return;

            if (dx <= SystemInformation.DragSize.Width && dy <= SystemInformation.DragSize.Height)
            {
                if (link.TargetPage.HasValue)
                    Page = link.TargetPage.Value;

                if (link.Uri != null)
                {
                    try
                    {
                        Process.Start(link.Uri);
                    }
                    catch
                    {
                        // Some browsers (Firefox) will cause an exception to
                        // be thrown (when it auto-updates).
                    }
                }
            }
        }

        /// <summary>
        /// Rotate the PDF document left.
        /// </summary>
        public void RotateLeft()
        {
            Rotation = (PdfRotation)(((int)Rotation + 3) % 4);
        }

        /// <summary>
        /// Rotate the PDF document right.
        /// </summary>
        public void RotateRight()
        {
            Rotation = (PdfRotation)(((int)Rotation + 1) % 4);
        }

        private void ResetFromRotation()
        {
            var offsetX = (double)DisplayRectangle.Left / DisplayRectangle.Width;
            var offsetY = (double)DisplayRectangle.Top / DisplayRectangle.Height;

            ReloadDocument();

            SetDisplayRectLocation(new Point(
                (int)(DisplayRectangle.Width * offsetX),
                (int)(DisplayRectangle.Height * offsetY)
            ));
        }

        private SizeF TranslateSize(SizeF size)
        {
            switch (Rotation)
            {
                case PdfRotation.Rotate90:
                case PdfRotation.Rotate270:
                    return new SizeF(size.Height, size.Width);

                default:
                    return size;
            }
        }

        protected override void SetZoom(double zoom, Point? focus)
        {
            Point location;

            if (focus.HasValue)
            {
                var bounds = GetDocumentBounds();

                location = new Point(
                    focus.Value.X - bounds.X,
                    focus.Value.Y - bounds.Y
                );
            }
            else
            {
                var bounds = _pageCacheValid
                    ? _pageCache[Page].Bounds
                    : GetDocumentBounds();

                location = new Point(
                    bounds.X,
                    bounds.Y
                );
            }

            double oldScale = Zoom;

            base.SetZoom(zoom, null);

            var newLocation = new Point(
                (int)(location.X * (zoom / oldScale)),
                (int)(location.Y * (zoom / oldScale))
            );

            SetDisplayRectLocation(
                new Point(
                    DisplayRectangle.Left - (newLocation.X - location.X),
                    DisplayRectangle.Top - (newLocation.Y - location.Y)
                ),
                false
            );
        }

        /// <summary>
        /// Releases the unmanaged resources used by the <see cref="T:System.Windows.Forms.Control"/> and its child controls and optionally releases the managed resources.
        /// </summary>
        /// <param name="disposing">true to release both managed and unmanaged resources; false to release only unmanaged resources. </param>
        protected override void Dispose(bool disposing)
        {
            if (!_disposed && disposing)
            {
                if (_shadeBorder != null)
                {
                    _shadeBorder.Dispose();
                    _shadeBorder = null;
                }

                if (_toolTip != null)
                {
                    _toolTip.Dispose();
                    _toolTip = null;
                }

                _disposed = true;
            }

            base.Dispose(disposing);
        }

        private class PageCache
        {
            public PdfPageLinks Links { get; set; }
            public Rectangle Bounds { get; set; }
            public Rectangle OuterBounds { get; set; }
        }

        private class DragState
        {
            public PdfPageLink Link { get; set; }
            public Point Location { get; set; }
        }
    }
}
