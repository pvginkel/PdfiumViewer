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
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this._fitWidthButton = new System.Windows.Forms.ToolStripButton();
            this._fitHeightButton = new System.Windows.Forms.ToolStripButton();
            this._fitBestButton = new System.Windows.Forms.ToolStripButton();
            this._renderer = new PdfiumViewer.PdfRenderer();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this._rotateLeftButton = new System.Windows.Forms.ToolStripButton();
            this._rotateRightButton = new System.Windows.Forms.ToolStripButton();
            this._toolStrip.SuspendLayout();
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
            this._zoomOutButton,
            this.toolStripSeparator2,
            this._fitWidthButton,
            this._fitHeightButton,
            this._fitBestButton,
            this.toolStripSeparator3,
            this._rotateLeftButton,
            this._rotateRightButton});
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
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            resources.ApplyResources(this.toolStripSeparator2, "toolStripSeparator2");
            // 
            // _fitWidthButton
            // 
            this._fitWidthButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this._fitWidthButton.Image = global::PdfiumViewer.Properties.Resources.fit_width;
            resources.ApplyResources(this._fitWidthButton, "_fitWidthButton");
            this._fitWidthButton.Name = "_fitWidthButton";
            this._fitWidthButton.Click += new System.EventHandler(this._fitWidthButton_Click);
            // 
            // _fitHeightButton
            // 
            this._fitHeightButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this._fitHeightButton.Image = global::PdfiumViewer.Properties.Resources.fit_height;
            resources.ApplyResources(this._fitHeightButton, "_fitHeightButton");
            this._fitHeightButton.Name = "_fitHeightButton";
            this._fitHeightButton.Click += new System.EventHandler(this._fitHeightButton_Click);
            // 
            // _fitBestButton
            // 
            this._fitBestButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this._fitBestButton.Image = global::PdfiumViewer.Properties.Resources.fit_best;
            resources.ApplyResources(this._fitBestButton, "_fitBestButton");
            this._fitBestButton.Name = "_fitBestButton";
            this._fitBestButton.Click += new System.EventHandler(this._fitBestButton_Click);
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
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            resources.ApplyResources(this.toolStripSeparator3, "toolStripSeparator3");
            // 
            // _rotateLeftButton
            // 
            this._rotateLeftButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this._rotateLeftButton.Image = global::PdfiumViewer.Properties.Resources.rotate_left;
            resources.ApplyResources(this._rotateLeftButton, "_rotateLeftButton");
            this._rotateLeftButton.Name = "_rotateLeftButton";
            this._rotateLeftButton.Click += new System.EventHandler(this._rotateLeftButton_Click);
            // 
            // _rotateRightButton
            // 
            this._rotateRightButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this._rotateRightButton.Image = global::PdfiumViewer.Properties.Resources.rotate_right;
            resources.ApplyResources(this._rotateRightButton, "_rotateRightButton");
            this._rotateRightButton.Name = "_rotateRightButton";
            this._rotateRightButton.Click += new System.EventHandler(this._rotateRightButton_Click);
            // 
            // PdfViewer
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this._renderer);
            this.Controls.Add(this._toolStrip);
            this.Name = "PdfViewer";
            this._toolStrip.ResumeLayout(false);
            this._toolStrip.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ToolStrip _toolStrip;
        private global::PdfiumViewer.PdfRenderer _renderer;
        private System.Windows.Forms.ToolStripButton _saveButton;
        private System.Windows.Forms.ToolStripButton _printButton;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripButton _zoomInButton;
        private System.Windows.Forms.ToolStripButton _zoomOutButton;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripButton _fitWidthButton;
        private System.Windows.Forms.ToolStripButton _fitBestButton;
        private System.Windows.Forms.ToolStripButton _fitHeightButton;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
        private System.Windows.Forms.ToolStripButton _rotateLeftButton;
        private System.Windows.Forms.ToolStripButton _rotateRightButton;
    }
}
