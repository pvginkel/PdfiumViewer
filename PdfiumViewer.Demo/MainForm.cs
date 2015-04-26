using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Drawing.Text;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;

namespace PdfiumViewer.Demo
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();

            renderToBitmapsToolStripMenuItem.Enabled = false;

            pdfViewer1.Renderer.DisplayRectangleChanged += Renderer_DisplayRectangleChanged;
        }

        void Renderer_DisplayRectangleChanged(object sender, EventArgs e)
        {
            _page.Text = (pdfViewer1.Renderer.Page + 1).ToString();
        }

        private void MainForm_Shown(object sender, EventArgs e)
        {
            var args = Environment.GetCommandLineArgs();

            if (args.Length > 1)
            {
                pdfViewer1.Document = PdfDocument.Load(args[1]);
                renderToBitmapsToolStripMenuItem.Enabled = true;
            }
            else
            {
                OpenFile();
            }
        }

        private void OpenFile()
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
                renderToBitmapsToolStripMenuItem.Enabled = true;
            }
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFile();
        }

        private void renderToBitmapsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string path;

            using (var form = new FolderBrowserDialog())
            {
                if (form.ShowDialog(this) != DialogResult.OK)
                    return;

                path = form.SelectedPath;
            }

            var document = pdfViewer1.Document;

            for (int i = 0; i < document.PageCount; i++ )
            {
                using (var image = document.Render(
                        i, (int)document.PageSizes[i].Width, (int)document.PageSizes[i].Height, 96, 96, false))
                {
                    image.Save(Path.Combine(path, "Page " + i + ".png"));
                }
            }
        }

        private void _page_TextChanged(object sender, EventArgs e)
        {
            int page;
            if (int.TryParse(_page.Text, out page))
                pdfViewer1.Renderer.Page = page - 1;
        }

        private void toolStripButton1_Click_1(object sender, EventArgs e)
        {
            pdfViewer1.Renderer.Page--;
        }

        private void toolStripButton2_Click(object sender, EventArgs e)
        {
            pdfViewer1.Renderer.Page++;
        }
    }
}
