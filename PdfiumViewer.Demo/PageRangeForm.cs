using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace PdfiumViewer.Demo
{
    public partial class PageRangeForm : Form
    {
        private readonly IPdfDocument _document;

        public IPdfDocument Document { get; private set; }

        public PageRangeForm(IPdfDocument document)
        {
            _document = document;

            InitializeComponent();

            _startPage.Text = "1";
            _endPage.Text = document.PageCount.ToString();
        }

        private void _acceptButton_Click(object sender, EventArgs e)
        {
            int startPage;
            int endPage;

            if (
                !int.TryParse(_startPage.Text, out startPage) ||
                !int.TryParse(_endPage.Text, out endPage) ||
                startPage < 1 ||
                endPage > _document.PageCount ||
                startPage > endPage
            )
            {
                MessageBox.Show(this, "Invalid start/end page");
            }
            else
            {
                Document = PdfRangeDocument.FromDocument(_document, startPage - 1, endPage - 1);

                DialogResult = DialogResult.OK;
            }
        }
    }
}
