using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace PdfiumViewer.Demo
{
    public partial class PrintMultiplePagesForm : Form
    {
        private readonly PdfViewer _viewer;

        public PrintMultiplePagesForm(PdfViewer viewer)
        {
            if (viewer == null)
                throw new ArgumentNullException(nameof(viewer));

            _viewer = viewer;

            InitializeComponent();
        }

        private void _acceptButton_Click(object sender, EventArgs e)
        {
            int horizontal;
            int vertical;
            float margin;

            if (!int.TryParse(_horizontal.Text, out horizontal))
            {
                MessageBox.Show(this, "Invalid horizontal");
            }
            else if (!int.TryParse(_vertical.Text, out vertical))
            {
                MessageBox.Show(this, "Invalid vertical");
            }
            else if (!float.TryParse(_margin.Text, out margin))
            {
                MessageBox.Show(this, "Invalid margin");
            }
            else
            {
                var settings = new PdfPrintSettings(
                    _viewer.DefaultPrintMode,
                    new PdfPrintMultiplePages(
                        horizontal,
                        vertical,
                        _horizontalOrientation.Checked ? Orientation.Horizontal : Orientation.Vertical,
                        margin
                    )
                );

                using (var form = new PrintPreviewDialog())
                {
                    form.Document = _viewer.Document.CreatePrintDocument(settings);
                    form.ShowDialog(this);
                }

                DialogResult = DialogResult.OK;
            }
        }
    }
}
