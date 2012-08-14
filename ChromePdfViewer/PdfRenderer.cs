using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Printing;
using System.Text;
using System.Windows.Forms;

namespace ChromePdfViewer
{
    /// <summary>
    /// Control to render PDF documents.
    /// </summary>
    public class PdfRenderer : Control
    {
        private static readonly Padding PageMargin = new Padding(4);
        private const double ZoomMin = 0.1;
        private const double ZoomMax = 10;

        private ScrollBars _scrollbarsVisible;
        private int _height;
        private bool _disposed;
        private readonly HScrollBar _hScrollBar;
        private readonly VScrollBar _vScrollBar;
        private readonly Control _filler;
        private double _zoom = 1;
        private double _scaleFactor;
        private int _previousHValue = -1;
        private int _previousVValue = -1;
        private readonly LinkedList<CachedPage> _pageCache = new LinkedList<CachedPage>();
        private int _maximumPageCache;
        private ShadeBorder _shadeBorder = new ShadeBorder();
        private int _suspendPaintCount;
        private static readonly int _defaultDpiX;
        private static readonly int _defaultDpiY;
        private static readonly int _defaultWidth;
        private static readonly int _defaultHeight;
        private PdfDocument _document;

        static PdfRenderer()
        {
            using (var dialog = new PrintDialog())
            {
                bool found = false;

                try
                {
                    foreach (PrinterResolution resolution in dialog.PrinterSettings.PrinterResolutions)
                    {
                        if (resolution.Kind == PrinterResolutionKind.Custom)
                        {
                            _defaultDpiX = resolution.X;
                            _defaultDpiY = resolution.Y;
                            _defaultWidth = (int)((dialog.PrinterSettings.DefaultPageSettings.PaperSize.Width / 100.0) * resolution.X);
                            _defaultHeight = (int)((dialog.PrinterSettings.DefaultPageSettings.PaperSize.Height / 100.0) * resolution.Y);

                            found = true;
                            break;
                        }
                    }
                }
                catch
                {
                    // Ignore any exceptions; just use defaults.
                }

                if (!found)
                {
                    // Default to A4 size.

                    _defaultDpiX = 600;
                    _defaultDpiY = 500;
                    _defaultWidth = (int)(8.27 * _defaultDpiX);
                    _defaultHeight = (int)(11.69 * _defaultDpiY);
                }
            }
        }

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

        internal ScrollBars ScrollbarsVisible
        {
            get { return _scrollbarsVisible; }
            set
            {
                if (_scrollbarsVisible != value)
                {
                    _scrollbarsVisible = value;

                    _hScrollBar.Visible = _scrollbarsVisible == ScrollBars.Horizontal || _scrollbarsVisible == ScrollBars.Both;
                    _vScrollBar.Visible = _scrollbarsVisible == ScrollBars.Vertical || _scrollbarsVisible == ScrollBars.Both;
                    _filler.Visible = _scrollbarsVisible == ScrollBars.Both;

                    PerformLayout();
                }
            }
        }

        /// <summary>
        /// Gets or sets the current zoom level.
        /// </summary>
        public double Zoom
        {
            get { return _zoom; }
            set
            {
                if (value >= ZoomMin && value <= ZoomMax)
                {
                    _zoom = value;

                    UpdateScrollbars();

                    Invalidate();
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

            SuspendLayout();

            _hScrollBar = new HScrollBar
            {
                TabStop = false,
                TabIndex = 1
            };

            _hScrollBar.ValueChanged += _hScrollBar_ValueChanged;

            Controls.Add(_hScrollBar);

            _vScrollBar = new VScrollBar
            {
                TabStop = false,
                TabIndex = 2
            };

            _vScrollBar.ValueChanged += _vScrollBar_ValueChanged;

            Controls.Add(_vScrollBar);

            _filler = new Control
            {
                TabStop = false,
                TabIndex = 3
            };

            Controls.Add(_filler);

            _scrollbarsVisible = ScrollBars.Both;
            ScrollbarsVisible = ScrollBars.None;

            ResumeLayout();
        }

        void _hScrollBar_ValueChanged(object sender, EventArgs e)
        {
            if (_previousHValue != -1)
            {
                if (_suspendPaintCount == 0)
                {
                    NativeMethods.ScrollWindowEx(
                        Handle,
                        -(_hScrollBar.Value - _previousHValue),
                        0,
                        IntPtr.Zero,
                        IntPtr.Zero,
                        IntPtr.Zero,
                        IntPtr.Zero,
                        NativeMethods.SW_INVALIDATE
                    );
                }

                _previousHValue = _hScrollBar.Value;
            }
            else
            {
                _previousHValue = _hScrollBar.Value;

                Invalidate();
            }
        }

        void _vScrollBar_ValueChanged(object sender, EventArgs e)
        {
            if (_previousVValue != -1)
            {
                if (_suspendPaintCount == 0)
                {
                    NativeMethods.ScrollWindowEx(
                        Handle,
                        0,
                        -(_vScrollBar.Value - _previousVValue),
                        IntPtr.Zero,
                        IntPtr.Zero,
                        IntPtr.Zero,
                        IntPtr.Zero,
                        NativeMethods.SW_INVALIDATE
                    );
                }

                _previousVValue = _vScrollBar.Value;
            }
            else
            {
                _previousVValue = _vScrollBar.Value;

                Invalidate();
            }
        }

        /// <summary>
        /// Raises the <see cref="E:System.Windows.Forms.Control.Layout"/> event.
        /// </summary>
        /// <param name="levent">A <see cref="T:System.Windows.Forms.LayoutEventArgs"/> that contains the event data. </param>
        protected override void OnLayout(LayoutEventArgs levent)
        {
            base.OnLayout(levent);

            if (_scrollbarsVisible == ScrollBars.Vertical || _scrollbarsVisible == ScrollBars.Both)
            {
                _vScrollBar.SetBounds(
                    Width - _vScrollBar.Width,
                    0,
                    _vScrollBar.Width,
                    _scrollbarsVisible == ScrollBars.Both ? Height - _hScrollBar.Height : Height
                );
            }

            if (_scrollbarsVisible == ScrollBars.Horizontal || _scrollbarsVisible == ScrollBars.Both)
            {
                _hScrollBar.SetBounds(
                    0,
                    Height - _hScrollBar.Height,
                    _scrollbarsVisible == ScrollBars.Both ? Width - _vScrollBar.Width : Width,
                    _hScrollBar.Height
                );
            }

            if (_scrollbarsVisible == ScrollBars.Both)
            {
                _filler.SetBounds(
                    Width - _vScrollBar.Width,
                    Height - _hScrollBar.Height,
                    _vScrollBar.Width,
                    _hScrollBar.Height
                );
            }

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

            _height = _defaultHeight * _document.PageCount;

            UpdateScrollbars();

            Invalidate();
        }

        private void UpdateScrollbars()
        {
            if (_document == null)
                return;

            UpdateScaleFactor(ScrollBars.Both);

            var bounds = GetScrollClientArea(ScrollBars.Both);

            int fullHeight = (int)(_height * _scaleFactor + ShadeBorder.Size.Vertical * _document.PageCount + PageMargin.Vertical * _document.PageCount);
            int pageWidth = (int)(_defaultWidth * _scaleFactor + ShadeBorder.Size.Horizontal + PageMargin.Horizontal);

            bool horizontalVisible = pageWidth > bounds.Width;

            if (!horizontalVisible)
            {
                UpdateScaleFactor(ScrollBars.Vertical);

                bounds = GetScrollClientArea(ScrollBars.Vertical);

                fullHeight = (int)(_height * _scaleFactor + ShadeBorder.Size.Vertical * _document.PageCount + PageMargin.Vertical * _document.PageCount);
                pageWidth = (int)(_defaultWidth * _scaleFactor + ShadeBorder.Size.Horizontal + PageMargin.Horizontal);

            }

            bool verticalVisible = fullHeight > bounds.Height;

            double verticalValue = (double)_vScrollBar.Value / _vScrollBar.Maximum;
            double horizontalValue = (double)_hScrollBar.Value / _hScrollBar.Maximum;

            _suspendPaintCount++;

            try
            {
                _vScrollBar.Maximum = fullHeight;
                _vScrollBar.LargeChange = bounds.Height;
                _vScrollBar.SmallChange = _vScrollBar.LargeChange / 10;
                _vScrollBar.Value = (int)(verticalValue * _vScrollBar.Maximum);

                _hScrollBar.Maximum = pageWidth;
                _hScrollBar.LargeChange = bounds.Width;
                _hScrollBar.SmallChange = _hScrollBar.LargeChange / 10;
                _hScrollBar.Value = (int)(horizontalValue * _hScrollBar.Maximum);
            }
            finally
            {
                _suspendPaintCount--;
            }

            ScrollbarsVisible =
                horizontalVisible && verticalVisible
                ? ScrollBars.Both
                :
                    horizontalVisible
                    ? ScrollBars.Horizontal
                    :
                        verticalVisible
                        ? ScrollBars.Vertical
                        : ScrollBars.None;

            _previousHValue = -1;
            _previousVValue = -1;

            int averagePageHeight = fullHeight / _document.PageCount;

            // We cache at most three pages at zoom level 1.

            _maximumPageCache = Math.Max((bounds.Height * 3) / averagePageHeight, 2);

            DisposePageCache();
        }

        private Rectangle GetScrollClientArea()
        {
            return GetScrollClientArea(_scrollbarsVisible);
        }

        private Rectangle GetScrollClientArea(ScrollBars scrollbars)
        {
            return new Rectangle(
                0,
                0,
                scrollbars == ScrollBars.Vertical || scrollbars == ScrollBars.Both ? Width - _vScrollBar.Width : Width,
                scrollbars == ScrollBars.Horizontal || scrollbars == ScrollBars.Both ? Height - _hScrollBar.Height : Height
            );
        }

        private void UpdateScaleFactor(ScrollBars scrollBars)
        {
            var bounds = GetScrollClientArea(scrollBars);

            int height = bounds.Height - ShadeBorder.Size.Vertical - PageMargin.Vertical;

            // Scale factor determines what we need to multiply the dimensions
            // of the metafile with to get the size in the control.

            _scaleFactor = ((double)height / _defaultHeight) * _zoom;
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

            int maxWidth = (int)(_defaultWidth * _scaleFactor) + ShadeBorder.Size.Horizontal + PageMargin.Horizontal;
            int leftOffset = _scrollbarsVisible == ScrollBars.Horizontal || _scrollbarsVisible == ScrollBars.Both ? -_hScrollBar.Value : (bounds.Width - maxWidth) / 2;
            int topOffset = _scrollbarsVisible == ScrollBars.Vertical || _scrollbarsVisible == ScrollBars.Both ? -_vScrollBar.Value : 0;

            using (var brush = new SolidBrush(BackColor))
            {
                e.Graphics.FillRectangle(brush, e.ClipRectangle);
            }

            int offset = 0;

            for (int page = 0; page < _document.PageCount; page++)
            {
                int height = (int)(_defaultHeight * _scaleFactor);
                int fullHeight = height + ShadeBorder.Size.Vertical + PageMargin.Vertical;
                int width = (int)(_defaultWidth * _scaleFactor);
                int fullWidth = width + ShadeBorder.Size.Horizontal + PageMargin.Horizontal;

                if (e.ClipRectangle.IntersectsWith(new Rectangle(
                    leftOffset,
                    offset + topOffset,
                    fullWidth,
                    fullHeight
                ))) {
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

            _previousHValue = _hScrollBar.Value;
            _previousVValue = _vScrollBar.Value;
        }

        private void DrawPageImage(Graphics graphics, int page, Rectangle pageBounds, Rectangle clip)
        {
            var pageImage = GetPageImage(page, pageBounds.Size);

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

            // We didn't have the page cached. Create a new cached page.

            var image = new Bitmap(size.Width, size.Height);

            // We render at a minimum of 150 DPI. Everything below this turns
            // into crap.

            int imageDpi = (int)(((double)size.Width / _defaultWidth) * _defaultDpiX);
            int renderDpi = Math.Max(150, imageDpi);

            int targetWidth = (int)(((double)_defaultWidth / _defaultDpiX) * renderDpi);
            int targetHeight = (int)(((double)_defaultHeight / _defaultDpiY) * renderDpi);

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

            // Trim the cache if we have too many items.

            while (_pageCache.Count > _maximumPageCache)
            {
                node = _pageCache.Last;

                _pageCache.RemoveLast();

                node.Value.Dispose();
            }

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

        /// <summary>
        /// Raises the <see cref="E:System.Windows.Forms.Control.MouseWheel"/> event.
        /// </summary>
        /// <param name="e">A <see cref="T:System.Windows.Forms.MouseEventArgs"/> that contains the event data. </param>
        protected override void OnMouseWheel(MouseEventArgs e)
        {
            base.OnMouseWheel(e);

            int offset = e.Delta < 0 ? 1 : -1;

            PerformScroll(
                offset,
                (ModifierKeys & Keys.Control) != 0 ? ScrollMode.Zoom : ScrollMode.Small
            );
        }

        private void PerformScroll(int offset, ScrollMode mode)
        {
            if (mode == ScrollMode.Zoom)
            {
                if (offset < 0)
                    ZoomIn();
                else
                    ZoomOut();
            }
            else if (_vScrollBar.Visible)
            {
                _vScrollBar.Value =
                    Math.Max(
                        0,
                        Math.Min(
                            _vScrollBar.Value + (mode == ScrollMode.Large ? _vScrollBar.LargeChange : _vScrollBar.SmallChange) * offset,
                            _vScrollBar.Maximum - _vScrollBar.LargeChange
                        )
                    );
            }
        }

        /// <summary>
        /// Determines whether the specified key is a regular input key or a special key that requires preprocessing.
        /// </summary>
        /// <returns>
        /// true if the specified key is a regular input key; otherwise, false.
        /// </returns>
        /// <param name="keyData">One of the <see cref="T:System.Windows.Forms.Keys"/> values. </param>
        protected override bool IsInputKey(Keys keyData)
        {
            switch ((keyData) & Keys.KeyCode)
            {
                case Keys.Up:
                    PerformScroll(-1, ScrollMode.Small);
                    return true;

                case Keys.Down:
                    PerformScroll(1, ScrollMode.Small);
                    return true;

                case Keys.PageUp:
                    PerformScroll(-1, ScrollMode.Large);
                    return true;

                case Keys.PageDown:
                    PerformScroll(1, ScrollMode.Large);
                    return true;

                case Keys.Add:
                case Keys.Oemplus:
                    if ((keyData & Keys.Modifiers) == Keys.Control)
                        PerformScroll(-1, ScrollMode.Zoom);
                    return true;

                case Keys.Subtract:
                case Keys.OemMinus:
                    if ((keyData & Keys.Modifiers) == Keys.Control)
                        PerformScroll(1, ScrollMode.Zoom);
                    return true;

                case Keys.Home:
                    _vScrollBar.Value = 0;
                    return true;

                case Keys.End:
                    _vScrollBar.Value = _vScrollBar.Maximum - _vScrollBar.LargeChange;
                    return true;

                default:
                    return base.IsInputKey(keyData);
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

                _disposed = true;
            }

            base.Dispose(disposing);
        }

        /// <summary>
        /// Zooms the PDF document in one step.
        /// </summary>
        public void ZoomIn()
        {
            Zoom *= 1.1;
        }

        /// <summary>
        /// Zooms the PDF document out one step.
        /// </summary>
        public void ZoomOut()
        {
            Zoom /= 1.1;
        }

        private enum ScrollMode
        {
            Small,
            Large,
            Zoom
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
