using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace PdfViewer.Demo
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
        }

        private void MainForm_Shown(object sender, EventArgs e)
        {
            var args = Environment.GetCommandLineArgs();

            if (args.Length > 1)
            {
                pdfViewer1.Document = PdfDocument.Load(args[1]);
            }
            else
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

                    pdfViewer1.Document = PdfDocument.Load(form.FileName);
                }
            }
        }
    }
}
