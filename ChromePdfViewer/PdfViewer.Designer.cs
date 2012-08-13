namespace ChromePdfViewer
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
            this._toolStrip = new System.Windows.Forms.ToolStrip();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this._saveButton = new System.Windows.Forms.ToolStripButton();
            this._printButton = new System.Windows.Forms.ToolStripButton();
            this._zoomInButton = new System.Windows.Forms.ToolStripButton();
            this._zoomOutButton = new System.Windows.Forms.ToolStripButton();
            this._renderer = new ChromePdfViewer.PdfRenderer();
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
            this._zoomOutButton});
            this._toolStrip.Location = new System.Drawing.Point(0, 0);
            this._toolStrip.Name = "_toolStrip";
            this._toolStrip.Size = new System.Drawing.Size(588, 25);
            this._toolStrip.TabIndex = 0;
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(6, 25);
            // 
            // _saveButton
            // 
            this._saveButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this._saveButton.Image = global::ChromePdfViewer.Properties.Resources.disk_blue;
            this._saveButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this._saveButton.Name = "_saveButton";
            this._saveButton.Size = new System.Drawing.Size(23, 22);
            this._saveButton.Text = "Save";
            this._saveButton.Click += new System.EventHandler(this._saveButton_Click);
            // 
            // _printButton
            // 
            this._printButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this._printButton.Image = global::ChromePdfViewer.Properties.Resources.printer;
            this._printButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this._printButton.Name = "_printButton";
            this._printButton.Size = new System.Drawing.Size(23, 22);
            this._printButton.Text = "Print";
            this._printButton.Click += new System.EventHandler(this._printButton_Click);
            // 
            // _zoomInButton
            // 
            this._zoomInButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this._zoomInButton.Image = global::ChromePdfViewer.Properties.Resources.zoom_in;
            this._zoomInButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this._zoomInButton.Name = "_zoomInButton";
            this._zoomInButton.Size = new System.Drawing.Size(23, 22);
            this._zoomInButton.Text = "Zoom In";
            this._zoomInButton.Click += new System.EventHandler(this._zoomInButton_Click);
            // 
            // _zoomOutButton
            // 
            this._zoomOutButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this._zoomOutButton.Image = global::ChromePdfViewer.Properties.Resources.zoom_out;
            this._zoomOutButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this._zoomOutButton.Name = "_zoomOutButton";
            this._zoomOutButton.Size = new System.Drawing.Size(23, 22);
            this._zoomOutButton.Text = "Zoom Out";
            this._zoomOutButton.Click += new System.EventHandler(this._zoomOutButton_Click);
            // 
            // _renderer
            // 
            this._renderer.Dock = System.Windows.Forms.DockStyle.Fill;
            this._renderer.Location = new System.Drawing.Point(0, 25);
            this._renderer.Name = "_renderer";
            this._renderer.Size = new System.Drawing.Size(588, 413);
            this._renderer.TabIndex = 1;
            this._renderer.Text = "pdfRenderer1";
            this._renderer.Zoom = 1D;
            // 
            // PdfViewer
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this._renderer);
            this.Controls.Add(this._toolStrip);
            this.Name = "PdfViewer";
            this.Size = new System.Drawing.Size(588, 438);
            this._toolStrip.ResumeLayout(false);
            this._toolStrip.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ToolStrip _toolStrip;
        private PdfRenderer _renderer;
        private System.Windows.Forms.ToolStripButton _saveButton;
        private System.Windows.Forms.ToolStripButton _printButton;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripButton _zoomInButton;
        private System.Windows.Forms.ToolStripButton _zoomOutButton;
    }
}
