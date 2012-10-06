using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace ChromePdfViewer
{
    /// <summary>
    /// Control to render PDF documents.
    /// </summary>
    public class PdfRenderer : PanningZoomingScrollControl
    {
        private static readonly Padding PageMargin = new Padding(4);
        private static readonly DefaultSettings DefaultSettings = new DefaultSettings();

        private int _height;
        private bool _disposed;
        private double _scaleFactor;
        private readonly LinkedList<CachedPage> _pageCache = new LinkedList<CachedPage>();
        private int _maximumPageCache;
        private ShadeBorder _shadeBorder = new ShadeBorder();
        private int _suspendPaintCount;
        private PdfDocument _document;
        private ToolTip _toolTip;

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

            _height = DefaultSettings.Height * _document.PageCount;

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

                bounds = GetScrollClientArea(ScrollBars.Vertical);

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

            int averagePageHeight = documentSize.Height / _document.PageCount;

            // We cache at most three pages at zoom level 1.

            _maximumPageCache = Math.Max((bounds.Height * 3) / averagePageHeight, 2);

            DisposePageCache();
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

            int height = bounds.Height - ShadeBorder.Size.Vertical - PageMargin.Vertical;

            // Scale factor determines what we need to multiply the dimensions
            // of the metafile with to get the size in the control.

            _scaleFactor = ((double)height / DefaultSettings.Height) * Zoom;
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

            int maxWidth = (int)(DefaultSettings.Width * _scaleFactor) + ShadeBorder.Size.Horizontal + PageMargin.Horizontal;
            int leftOffset = HScroll ? DisplayRectangle.X : (bounds.Width - maxWidth) / 2;
            int topOffset = VScroll ? DisplayRectangle.Y : 0;

            using (var brush = new SolidBrush(BackColor))
            {
                e.Graphics.FillRectangle(brush, e.ClipRectangle);
            }

            int offset = 0;

            for (int page = 0; page < _document.PageCount; page++)
            {
                int height = (int)(DefaultSettings.Height * _scaleFactor);
                int fullHeight = height + ShadeBorder.Size.Vertical + PageMargin.Vertical;
                int width = (int)(DefaultSettings.Width * _scaleFactor);
                int fullWidth = width + ShadeBorder.Size.Horizontal + PageMargin.Horizontal;

                if (e.ClipRectangle.IntersectsWith(new Rectangle(
                    leftOffset,
                    offset + topOffset,
                    fullWidth,
                    fullHeight
                )))
                {
                    var pageBounds = new Rectangle(
                        leftOffset + ShadeBorder.Size.Left + PageMargin.Left,
                        offset + topOffset + ShadeBorder.Size.Top + PageMargin.Top,
                        width,
                        height
                    );

                    e.Graphics.FillRectangle(Brushes.White, pageBounds);

                    DrawPageImage(e.Graphics, page, pageBounds, e.ClipRectangle);

                    _shadeBorder.Draw(e.Graphics, pageBounds);
                }

                offset += fullHeight;
            }
        }

        private void DrawPageImage(Graphics graphics, int page, Rectangle pageBounds, Rectangle clip)
        {
            var pageImage = GetPageImage(page, pageBounds.Size);

            if (pageImage == null)
                return;

            var srcBounds = new Rectangle(
                Math.Max(clip.Left - pageBounds.Left, 0),
                Math.Max(clip.Top - pageBounds.Top, 0),
                Math.Min(clip.Right - pageBounds.Left, pageBounds.Width),
                Math.Min(clip.Bottom - pageBounds.Top, pageBounds.Height)
            );

            var destBounds = new Rectangle(
                pageBounds.Left + srcBounds.Left,
                pageBounds.Top + srcBounds.Top,
                srcBounds.Width,
                srcBounds.Height
            );

            graphics.DrawImage(pageImage, destBounds, srcBounds, GraphicsUnit.Pixel);
        }

        private Image GetPageImage(int page, Size size)
        {
            // Find an existing page.

            var node = _pageCache.First;

            while (node != null)
            {
                if (node.Value.Page == page)
                    break;

                node = node.Next;
            }

            // Put it at the front of the list to keep our MRU stack.

            if (node != null)
            {
                _pageCache.Remove(node);
                _pageCache.AddFirst(node);

                return node.Value.Image;
            }

            // Trim the cache if we have too many items.

            while (_pageCache.Count > _maximumPageCache - 1)
            {
                node = _pageCache.Last;

                _pageCache.RemoveLast();

                node.Value.Dispose();
            }

            // We didn't have the page cached. Create a new cached page.

            Bitmap image;

            try
            {
                // This throws when there isn't enough memory available.

                image = new Bitmap(size.Width, size.Height);
            }
            catch
            {
                return null;
            }

            // We render at a minimum of 150 DPI. Everything below this turns
            // into crap.

            int imageDpi = (int)(((double)size.Width / DefaultSettings.Width) * DefaultSettings.DpiX);
            int renderDpi = Math.Max(150, imageDpi);

            int targetWidth = (int)(((double)DefaultSettings.Width / DefaultSettings.DpiX) * renderDpi);
            int targetHeight = (int)(((double)DefaultSettings.Height / DefaultSettings.DpiY) * renderDpi);

            if (imageDpi == renderDpi)
            {
                RenderPage(page, image);
            }
            else
            {
                using (var fullImage = new Bitmap(targetWidth, targetHeight))
                {
                    RenderPage(page, fullImage);

                    using (var graphics = Graphics.FromImage(image))
                    {
                        graphics.DrawImage(
                            fullImage,
                            new Rectangle(
                                0,
                                0,
                                image.Width,
                                image.Height
                            )
                        );
                    }
                }
            }

            node = new LinkedListNode<CachedPage>(new CachedPage(page, image));

            _pageCache.AddFirst(node);

            return image;
        }

        private void RenderPage(int page, Bitmap image)
        {
            using (var graphics = Graphics.FromImage(image))
            {
                _document.Render(
                    page,
                    graphics,
                    graphics.DpiX,
                    graphics.DpiY,
                    new Rectangle(
                        0,
                        0,
                        image.Width,
                        image.Height
                    ),
                    true /* fitToBounds */,
                    true /* stretchToBounds */,
                    true /* keepAspectRatio */,
                    true /* centerInBounds */,
                    true /* autoRotate */
                );
            }
        }

        private void DisposePageCache()
        {
            foreach (var cachedPage in _pageCache)
            {
                cachedPage.Dispose();
            }

            _pageCache.Clear();
        }

        protected override Rectangle GetDocumentBounds()
        {
            int height = (int)(_height * _scaleFactor + (ShadeBorder.Size.Vertical + PageMargin.Vertical) * _document.PageCount);
            int width = (int)(DefaultSettings.Width * _scaleFactor + ShadeBorder.Size.Horizontal + PageMargin.Horizontal);
            
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

        /// <summary>
        /// Releases the unmanaged resources used by the <see cref="T:System.Windows.Forms.Control"/> and its child controls and optionally releases the managed resources.
        /// </summary>
        /// <param name="disposing">true to release both managed and unmanaged resources; false to release only unmanaged resources. </param>
        protected override void Dispose(bool disposing)
        {
            if (!_disposed && disposing)
            {
                DisposePageCache();

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

        private class CachedPage : IDisposable
        {
            private bool _disposed;

            public int Page { get; private set; }

            public Image Image { get; private set; }

            public CachedPage(int page, Image image)
            {
                Page = page;
                Image = image;
            }

            public void Dispose()
            {
                if (!_disposed)
                {
                    if (Image != null)
                    {
                        Image.Dispose();
                        Image = null;
                    }

                    _disposed = true;
                }
            }
        }
    }
}
