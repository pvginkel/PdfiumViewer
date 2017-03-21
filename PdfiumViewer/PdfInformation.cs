using System;
using System.Collections.Generic;
using System.Text;

#pragma warning disable 1591

namespace PdfiumViewer
{
    /// <summary>
    /// Contains text from metadata of the document.
    /// </summary>
    public class PdfInformation
    {
        public string Author { get; set; }
        public string Creator { get; set; }
        public DateTime? CreationDate { get; set; }
        public string Keywords { get; set; }
        public DateTime? ModificationDate { get; set; }
        public string Producer { get; set; }
        public string Subject { get; set; }
        public string Title { get; set; }
    }
}
