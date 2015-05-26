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
        private PdfDocument _document;

        /// <summary>
        /// Gets or sets the PDF document.
        /// </summary>
        public PdfDocument Document
        {
            get { return _document; }
            set
            {
                if (_document != value)
                {
                    _document = value;

                    if (_document != null)
                        _renderer.Load(_document);

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
        public string DefaultDocumentName { get; set; }

        /// <summary>
        /// Gets or sets the default print mode.
        /// </summary>
        [DefaultValue(PdfPrintMode.CutMargin)]
        public PdfPrintMode DefaultPrintMode { get; set; }

        /// <summary>
        /// Gets or sets the way the document should be zoomed initially.
        /// </summary>
        public PdfViewerZoomMode ZoomMode
        {
            get { return _renderer.ZoomMode; }
            set { _renderer.ZoomMode = value; }
        }

        /// <summary>
        /// Initializes a new instance of the PdfViewer class.
        /// </summary>
        public PdfViewer()
        {
            DefaultPrintMode = PdfPrintMode.CutMargin;

            InitializeComponent();

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
    }
}
