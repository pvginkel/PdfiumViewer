using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace ChromePdfViewer.Demo
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
        }

        private void MainForm_Shown(object sender, EventArgs e)
        {
            using (var form = new OpenFileDialog())
            {
                form.Filter = "PDF Files (*.pdf)|*.pdf|All Files (*.*)|*.*";
                form.RestoreDirectory = true;
                form.Title = "Open PDF File";

                if (form.ShowDialog(this) != DialogResult.OK)
                {
                    Dispose();
                    return;
                }

                pdfViewer1.Document = new PdfDocument(form.FileName);
            }
        }
    }
}
