using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Windows.Forms;

namespace PdfiumViewer
{
    /// <summary>
    /// Control to host PDF documents with support for printing.
    /// </summary>
    public partial class PdfViewer : UserControl
    {
        private IPdfDocument _document;
        private bool _showBookmarks;

        /// <summary>
        /// Gets or sets the PDF document.
        /// </summary>
        [DefaultValue(null)]
        public IPdfDocument Document
        {
            get { return _document; }
            set
            {
                if (_document != value)
                {
                    _document = value;

                    if (_document != null)
                    {
                        _renderer.Load(_document);
                        UpdateBookmarks();
                    }

                    UpdateEnabled();
                }
            }
        }

        /// <summary>
        /// Get the <see cref="PdfRenderer"/> that renders the PDF document.
        /// </summary>
        public PdfRenderer Renderer
        {
            get { return _renderer; }
        }

        /// <summary>
        /// Gets or sets the default document name used when saving the document.
        /// </summary>
        [DefaultValue(null)]
        public string DefaultDocumentName { get; set; }

        /// <summary>
        /// Gets or sets the default print mode.
        /// </summary>
        [DefaultValue(PdfPrintMode.CutMargin)]
        public PdfPrintMode DefaultPrintMode { get; set; }

        /// <summary>
        /// Gets or sets the way the document should be zoomed initially.
        /// </summary>
        [DefaultValue(PdfViewerZoomMode.FitHeight)]
        public PdfViewerZoomMode ZoomMode
        {
            get { return _renderer.ZoomMode; }
            set { _renderer.ZoomMode = value; }
        }

        /// <summary>
        /// Gets or sets whether the toolbar should be shown.
        /// </summary>
        [DefaultValue(true)]
        public bool ShowToolbar
        {
            get { return _toolStrip.Visible; }
            set { _toolStrip.Visible = value; }
        }

        /// <summary>
        /// Gets or sets whether the bookmarks panel should be shown.
        /// </summary>
        [DefaultValue(true)]
        public bool ShowBookmarks
        {
            get { return _showBookmarks; }
            set
            {
                _showBookmarks = value;
                UpdateBookmarks();
            }
        }

        /// <summary>
        /// Gets or sets the pre-selected printer to be used when the print
        /// dialog shows up.
        /// </summary>
        [DefaultValue(null)]
        public string DefaultPrinter { get; set; }

        /// <summary>
        /// Occurs when a link in the pdf document is clicked.
        /// </summary>
        [Category("Action")]
        [Description("Occurs when a link in the pdf document is clicked.")]
        public event LinkClickEventHandler LinkClick;

        /// <summary>
        /// Called when a link is clicked.
        /// </summary>
        /// <param name="e"></param>
        protected virtual void OnLinkClick(LinkClickEventArgs e)
        {
            var handler = LinkClick;
            if (handler != null)
                handler(this, e);
        }

        private void UpdateBookmarks()
        {
            bool visible = _showBookmarks && _document != null && _document.Bookmarks.Count > 0;

            _container.Panel1Collapsed = !visible;

            if (visible)
            {
                _container.Panel1Collapsed = false;

                _bookmarks.Nodes.Clear();
                foreach (var bookmark in _document.Bookmarks)
                    _bookmarks.Nodes.Add(GetBookmarkNode(bookmark));
            }
        }

        /// <summary>
        /// Initializes a new instance of the PdfViewer class.
        /// </summary>
        public PdfViewer()
        {
            DefaultPrintMode = PdfPrintMode.CutMargin;

            InitializeComponent();

            ShowToolbar = true;
            ShowBookmarks = true;

            UpdateEnabled();
        }

        private void UpdateEnabled()
        {
            _toolStrip.Enabled = _document != null;
        }

        private void _zoomInButton_Click(object sender, EventArgs e)
        {
            _renderer.ZoomIn();
        }

        private void _zoomOutButton_Click(object sender, EventArgs e)
        {
            _renderer.ZoomOut();
        }

        private void _saveButton_Click(object sender, EventArgs e)
        {
            using (var form = new SaveFileDialog())
            {
                form.DefaultExt = ".pdf";
                form.Filter = Properties.Resources.SaveAsFilter;
                form.RestoreDirectory = true;
                form.Title = Properties.Resources.SaveAsTitle;
                form.FileName = DefaultDocumentName;

                if (form.ShowDialog(FindForm()) == DialogResult.OK)
                {
                    try
                    {
                        _document.Save(form.FileName);
                    }
                    catch
                    {
                        MessageBox.Show(
                            FindForm(),
                            Properties.Resources.SaveAsFailedText,
                            Properties.Resources.SaveAsFailedTitle,
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Error
                        );
                    }
                }
            }
        }

        private void _printButton_Click(object sender, EventArgs e)
        {
            using (var form = new PrintDialog())
            using (var document = _document.CreatePrintDocument(DefaultPrintMode))
            {
                form.AllowSomePages = true;
                form.Document = document;
                form.UseEXDialog = true;
                form.Document.PrinterSettings.FromPage = 1;
                form.Document.PrinterSettings.ToPage = _document.PageCount;
                if (DefaultPrinter != null)
                    form.Document.PrinterSettings.PrinterName = DefaultPrinter;

                if (form.ShowDialog(FindForm()) == DialogResult.OK)
                {
                    try
                    {
                        if (form.Document.PrinterSettings.FromPage <= _document.PageCount)
                            form.Document.Print();
                    }
                    catch
                    {
                        // Ignore exceptions; the printer dialog should take care of this.
                    }
                }
            }
        }

        private TreeNode GetBookmarkNode(PdfBookmark bookmark)
        {
            TreeNode node = new TreeNode(bookmark.Title);
            node.Tag = bookmark;
            if (bookmark.Children != null)
            {
                foreach (var child in bookmark.Children)
                    node.Nodes.Add(GetBookmarkNode(child));
            }
            return node;
        }

        private void _bookmarks_AfterSelect(object sender, TreeViewEventArgs e)
        {
            _renderer.Page = ((PdfBookmark)e.Node.Tag).PageIndex;
        }

        private void _renderer_LinkClick(object sender, LinkClickEventArgs e)
        {
            OnLinkClick(e);
        }
    }
}
