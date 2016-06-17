using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
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
            pdfViewer1.Renderer.ZoomChanged += Renderer_ZoomChanged;

            cutMarginsWhenPrintingToolStripMenuItem.PerformClick();

            _zoom.Text = pdfViewer1.Renderer.Zoom.ToString();

            Disposed += (s, e) => pdfViewer1.Document?.Dispose();
        }

        void Renderer_ZoomChanged(object sender, EventArgs e)
        {
            _zoom.Text = pdfViewer1.Renderer.Zoom.ToString();
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
                pdfViewer1.Document?.Dispose();
                pdfViewer1.Document = OpenDocument(args[1]);
                renderToBitmapsToolStripMenuItem.Enabled = true;
            }
            else
            {
                OpenFile();
            }

            _showBookmarks.Checked = pdfViewer1.ShowBookmarks;
            _showToolbar.Checked = pdfViewer1.ShowToolbar;
        }

        private PdfDocument OpenDocument(string fileName)
        {
            try
            {
                return PdfDocument.Load(this, fileName);
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, ex.Message, Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return null;
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

                pdfViewer1.Document?.Dispose();
                pdfViewer1.Document = OpenDocument(form.FileName);
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
            int dpiX;
            int dpiY;

            using (var form = new ExportBitmapsForm())
            {
                if (form.ShowDialog() != DialogResult.OK)
                    return;

                dpiX = form.DpiX;
                dpiY = form.DpiY;
            }

            string path;

            using (var form = new FolderBrowserDialog())
            {
                if (form.ShowDialog(this) != DialogResult.OK)
                    return;

                path = form.SelectedPath;
            }

            var document = pdfViewer1.Document;

            for (int i = 0; i < document.PageCount; i++)
            {
                using (var image = document.Render(i, (int)document.PageSizes[i].Width, (int)document.PageSizes[i].Height, dpiX, dpiY, false))
                {
                    image.Save(Path.Combine(path, "Page " + i + ".png"));
                }
            }
        }

        private void toolStripButton1_Click_1(object sender, EventArgs e)
        {
            pdfViewer1.Renderer.Page--;
        }

        private void toolStripButton2_Click(object sender, EventArgs e)
        {
            pdfViewer1.Renderer.Page++;
        }

        private void cutMarginsWhenPrintingToolStripMenuItem_Click(object sender, EventArgs e)
        {
            cutMarginsWhenPrintingToolStripMenuItem.Checked = true;
            shrinkToMarginsWhenPrintingToolStripMenuItem.Checked = false;

            pdfViewer1.DefaultPrintMode = PdfPrintMode.CutMargin;
        }

        private void shrinkToMarginsWhenPrintingToolStripMenuItem_Click(object sender, EventArgs e)
        {
            shrinkToMarginsWhenPrintingToolStripMenuItem.Checked = true;
            cutMarginsWhenPrintingToolStripMenuItem.Checked = false;

            pdfViewer1.DefaultPrintMode = PdfPrintMode.ShrinkToMargin;
        }

        private void printPreviewToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (var form = new PrintPreviewDialog())
            {
                form.Document = pdfViewer1.Document.CreatePrintDocument(pdfViewer1.DefaultPrintMode);
                form.ShowDialog(this);
            }
        }

        private void _fitWidth_Click(object sender, EventArgs e)
        {
            FitPage(PdfViewerZoomMode.FitWidth);
        }

        private void FitPage(PdfViewerZoomMode zoomMode)
        {
            int page = pdfViewer1.Renderer.Page;
            pdfViewer1.ZoomMode = zoomMode;
            pdfViewer1.Renderer.Zoom = 1;
            pdfViewer1.Renderer.Page = page;
        }

        private void _fitHeight_Click(object sender, EventArgs e)
        {
            FitPage(PdfViewerZoomMode.FitHeight);
        }

        private void _fitBest_Click(object sender, EventArgs e)
        {
            FitPage(PdfViewerZoomMode.FitBest);
        }

        private void _page_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.Handled = true;

                int page;
                if (int.TryParse(_page.Text, out page))
                    pdfViewer1.Renderer.Page = page - 1;
            }
        }

        private void _zoom_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.Handled = true;

                float zoom;
                if (float.TryParse(_zoom.Text, out zoom))
                    pdfViewer1.Renderer.Zoom = zoom;
            }
        }

        private void toolStripButton4_Click(object sender, EventArgs e)
        {
            pdfViewer1.Renderer.ZoomIn();
        }

        private void toolStripButton3_Click(object sender, EventArgs e)
        {
            pdfViewer1.Renderer.ZoomOut();
        }

        private void _rotateLeft_Click(object sender, EventArgs e)
        {
            pdfViewer1.Renderer.RotateLeft();
        }

        private void _rotateRight_Click(object sender, EventArgs e)
        {
            pdfViewer1.Renderer.RotateRight();
        }

        private void _hideToolbar_Click(object sender, EventArgs e)
        {
            pdfViewer1.ShowToolbar = _showToolbar.Checked;
        }

        private void _hideBookmarks_Click(object sender, EventArgs e)
        {
            pdfViewer1.ShowBookmarks = _showBookmarks.Checked;
        }

        private void deleteCurrentPageToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // PdfRenderer does not support changes to the loaded document,
            // so we fake it by reloading the document into the renderer.

            int page = pdfViewer1.Renderer.Page;
            var document = pdfViewer1.Document;
            pdfViewer1.Document = null;
            document.DeletePage(page);
            pdfViewer1.Document = document;
            pdfViewer1.Renderer.Page = page;
        }

        private void rotate0ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Rotate(PdfRotation.Rotate0);
        }

        private void rotate90ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Rotate(PdfRotation.Rotate90);
        }

        private void rotate180ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Rotate(PdfRotation.Rotate180);
        }

        private void rotate270ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Rotate(PdfRotation.Rotate270);
        }

        private void Rotate(PdfRotation rotate)
        {
            // PdfRenderer does not support changes to the loaded document,
            // so we fake it by reloading the document into the renderer.

            int page = pdfViewer1.Renderer.Page;
            var document = pdfViewer1.Document;
            pdfViewer1.Document = null;
            document.RotatePage(page, rotate);
            pdfViewer1.Document = document;
            pdfViewer1.Renderer.Page = page;
        }
    }
}
