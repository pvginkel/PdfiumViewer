using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace PdfiumViewer.Demo
{
    public partial class ExportBitmapsForm : Form
    {
        private int _dpiX;
        private int _dpiY;

        public int DpiX
        {
            get { return _dpiX; }
        }

        public int DpiY
        {
            get { return _dpiY; }
        }

        public ExportBitmapsForm()
        {
            InitializeComponent();
            UpdateEnabled();
        }

        private void _acceptButton_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
        }

        private void _dpiX_TextChanged(object sender, EventArgs e)
        {
            UpdateEnabled();
        }

        private void _dpiY_TextChanged(object sender, EventArgs e)
        {
            UpdateEnabled();
        }

        private void UpdateEnabled()
        {
            _acceptButton.Enabled =
                int.TryParse(_dpiXTextBox.Text, out _dpiX) &&
                int.TryParse(_dpiYTextBox.Text, out _dpiY);
        }
    }
}
