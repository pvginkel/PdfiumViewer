using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace PdfiumViewer.Demo
{
    public partial class SearchForm : Form
    {
        private readonly PdfSearchManager _searchManager;
        private bool _findDirty;

        public SearchForm(PdfRenderer renderer)
        {
            if (renderer == null)
                throw new ArgumentNullException(nameof(renderer));

            InitializeComponent();

            _searchManager = new PdfSearchManager(renderer);

            _matchCase.Checked = _searchManager.MatchCase;
            _matchWholeWord.Checked = _searchManager.MatchWholeWord;
            _highlightAll.Checked = _searchManager.HighlightAllMatches;
        }

        private void _matchCase_CheckedChanged(object sender, EventArgs e)
        {
            _findDirty = true;
            _searchManager.MatchCase = _matchCase.Checked;
        }

        private void _matchWholeWord_CheckedChanged(object sender, EventArgs e)
        {
            _findDirty = true;
            _searchManager.MatchWholeWord = _matchWholeWord.Checked;
        }

        private void _highlightAll_CheckedChanged(object sender, EventArgs e)
        {
            _searchManager.HighlightAllMatches = _highlightAll.Checked;
        }

        private void _find_TextChanged(object sender, EventArgs e)
        {
            _findDirty = true;
        }

        private void _findPrevious_Click(object sender, EventArgs e)
        {
            Find(false);
        }

        private void _findNext_Click(object sender, EventArgs e)
        {
            Find(true);
        }

        private void Find(bool forward)
        {
            if (_findDirty)
            {
                _findDirty = false;

                if (!_searchManager.Search(_find.Text))
                {
                    MessageBox.Show(this, "No matches found.");
                    return;
                }
            }

            if (!_searchManager.FindNext(forward))
                MessageBox.Show(this, "Find reached the starting point of the search.");
        }
    }
}
