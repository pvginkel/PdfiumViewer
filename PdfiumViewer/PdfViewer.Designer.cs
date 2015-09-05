using System;
using System.Collections.Generic;
using System.Text;

namespace PdfiumViewer
{
    partial class PdfViewer
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PdfViewer));
            this._toolStrip = new System.Windows.Forms.ToolStrip();
            this._saveButton = new System.Windows.Forms.ToolStripButton();
            this._printButton = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this._zoomInButton = new System.Windows.Forms.ToolStripButton();
            this._zoomOutButton = new System.Windows.Forms.ToolStripButton();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this._bookmarks = new PdfiumViewer.NativeTreeView();
            this._renderer = new PdfiumViewer.PdfRenderer();
            this._toolStrip.SuspendLayout();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.SuspendLayout();
            // 
            // _toolStrip
            // 
            this._toolStrip.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this._toolStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this._saveButton,
            this._printButton,
            this.toolStripSeparator1,
            this._zoomInButton,
            this._zoomOutButton});
            resources.ApplyResources(this._toolStrip, "_toolStrip");
            this._toolStrip.Name = "_toolStrip";
            // 
            // _saveButton
            // 
            this._saveButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this._saveButton.Image = global::PdfiumViewer.Properties.Resources.disk_blue;
            resources.ApplyResources(this._saveButton, "_saveButton");
            this._saveButton.Name = "_saveButton";
            this._saveButton.Click += new System.EventHandler(this._saveButton_Click);
            // 
            // _printButton
            // 
            this._printButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this._printButton.Image = global::PdfiumViewer.Properties.Resources.printer;
            resources.ApplyResources(this._printButton, "_printButton");
            this._printButton.Name = "_printButton";
            this._printButton.Click += new System.EventHandler(this._printButton_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            resources.ApplyResources(this.toolStripSeparator1, "toolStripSeparator1");
            // 
            // _zoomInButton
            // 
            this._zoomInButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this._zoomInButton.Image = global::PdfiumViewer.Properties.Resources.zoom_in;
            resources.ApplyResources(this._zoomInButton, "_zoomInButton");
            this._zoomInButton.Name = "_zoomInButton";
            this._zoomInButton.Click += new System.EventHandler(this._zoomInButton_Click);
            // 
            // _zoomOutButton
            // 
            this._zoomOutButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this._zoomOutButton.Image = global::PdfiumViewer.Properties.Resources.zoom_out;
            resources.ApplyResources(this._zoomOutButton, "_zoomOutButton");
            this._zoomOutButton.Name = "_zoomOutButton";
            this._zoomOutButton.Click += new System.EventHandler(this._zoomOutButton_Click);
            // 
            // splitContainer1
            // 
            resources.ApplyResources(this.splitContainer1, "splitContainer1");
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this._bookmarks);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this._renderer);
            this.splitContainer1.TabStop = false;
            this.splitContainer1.SplitterMoved += new System.Windows.Forms.SplitterEventHandler(this.splitContainer1_SplitterMoved);
            // 
            // _bookmarks
            // 
            resources.ApplyResources(this._bookmarks, "_bookmarks");
            this._bookmarks.FullRowSelect = true;
            this._bookmarks.Name = "_bookmarks";
            this._bookmarks.ShowLines = false;
            this._bookmarks.NodeMouseDoubleClick += new System.Windows.Forms.TreeNodeMouseClickEventHandler(this._bookmarks_NodeMouseDoubleClick);
            // 
            // _renderer
            // 
            this._renderer.Cursor = System.Windows.Forms.Cursors.Default;
            resources.ApplyResources(this._renderer, "_renderer");
            this._renderer.Name = "_renderer";
            this._renderer.Page = 0;
            this._renderer.Rotation = PdfiumViewer.PdfRotation.Rotate0;
            this._renderer.ZoomMode = PdfiumViewer.PdfViewerZoomMode.FitHeight;
            // 
            // PdfViewer
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.splitContainer1);
            this.Controls.Add(this._toolStrip);
            this.Name = "PdfViewer";
            this._toolStrip.ResumeLayout(false);
            this._toolStrip.PerformLayout();
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            this.splitContainer1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ToolStrip _toolStrip;
        private System.Windows.Forms.ToolStripButton _saveButton;
        private System.Windows.Forms.ToolStripButton _printButton;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripButton _zoomInButton;
        private System.Windows.Forms.ToolStripButton _zoomOutButton;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private NativeTreeView _bookmarks;
        private PdfRenderer _renderer;
    }
}
