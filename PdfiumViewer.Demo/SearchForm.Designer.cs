namespace PdfiumViewer.Demo
{
    partial class SearchForm
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

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.label1 = new System.Windows.Forms.Label();
            this._find = new System.Windows.Forms.TextBox();
            this._matchCase = new System.Windows.Forms.CheckBox();
            this._matchWholeWord = new System.Windows.Forms.CheckBox();
            this._highlightAll = new System.Windows.Forms.CheckBox();
            this._findPrevious = new System.Windows.Forms.Button();
            this._findNext = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 15);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(30, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Find:";
            // 
            // _find
            // 
            this._find.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this._find.Location = new System.Drawing.Point(65, 12);
            this._find.Name = "_find";
            this._find.Size = new System.Drawing.Size(321, 20);
            this._find.TabIndex = 1;
            this._find.TextChanged += new System.EventHandler(this._find_TextChanged);
            // 
            // _matchCase
            // 
            this._matchCase.AutoSize = true;
            this._matchCase.Location = new System.Drawing.Point(65, 38);
            this._matchCase.Name = "_matchCase";
            this._matchCase.Size = new System.Drawing.Size(82, 17);
            this._matchCase.TabIndex = 2;
            this._matchCase.Text = "Match case";
            this._matchCase.UseVisualStyleBackColor = true;
            this._matchCase.CheckedChanged += new System.EventHandler(this._matchCase_CheckedChanged);
            // 
            // _matchWholeWord
            // 
            this._matchWholeWord.AutoSize = true;
            this._matchWholeWord.Location = new System.Drawing.Point(65, 61);
            this._matchWholeWord.Name = "_matchWholeWord";
            this._matchWholeWord.Size = new System.Drawing.Size(113, 17);
            this._matchWholeWord.TabIndex = 3;
            this._matchWholeWord.Text = "Match whole word";
            this._matchWholeWord.UseVisualStyleBackColor = true;
            this._matchWholeWord.CheckedChanged += new System.EventHandler(this._matchWholeWord_CheckedChanged);
            // 
            // _highlightAll
            // 
            this._highlightAll.AutoSize = true;
            this._highlightAll.Location = new System.Drawing.Point(65, 84);
            this._highlightAll.Name = "_highlightAll";
            this._highlightAll.Size = new System.Drawing.Size(123, 17);
            this._highlightAll.TabIndex = 4;
            this._highlightAll.Text = "Highlight all matches";
            this._highlightAll.UseVisualStyleBackColor = true;
            this._highlightAll.CheckedChanged += new System.EventHandler(this._highlightAll_CheckedChanged);
            // 
            // _findPrevious
            // 
            this._findPrevious.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this._findPrevious.Location = new System.Drawing.Point(186, 115);
            this._findPrevious.Name = "_findPrevious";
            this._findPrevious.Size = new System.Drawing.Size(97, 23);
            this._findPrevious.TabIndex = 5;
            this._findPrevious.Text = "Find &Previous";
            this._findPrevious.UseVisualStyleBackColor = true;
            this._findPrevious.Click += new System.EventHandler(this._findPrevious_Click);
            // 
            // _findNext
            // 
            this._findNext.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this._findNext.Location = new System.Drawing.Point(289, 115);
            this._findNext.Name = "_findNext";
            this._findNext.Size = new System.Drawing.Size(97, 23);
            this._findNext.TabIndex = 6;
            this._findNext.Text = "Find &Next";
            this._findNext.UseVisualStyleBackColor = true;
            this._findNext.Click += new System.EventHandler(this._findNext_Click);
            // 
            // SearchForm
            // 
            this.AcceptButton = this._findNext;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(398, 150);
            this.Controls.Add(this._findNext);
            this.Controls.Add(this._findPrevious);
            this.Controls.Add(this._highlightAll);
            this.Controls.Add(this._matchWholeWord);
            this.Controls.Add(this._matchCase);
            this.Controls.Add(this._find);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "SearchForm";
            this.Text = "Search";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox _find;
        private System.Windows.Forms.CheckBox _matchCase;
        private System.Windows.Forms.CheckBox _matchWholeWord;
        private System.Windows.Forms.CheckBox _highlightAll;
        private System.Windows.Forms.Button _findPrevious;
        private System.Windows.Forms.Button _findNext;
    }
}