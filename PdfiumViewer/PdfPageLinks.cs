using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace PdfiumViewer
{
    /// <summary>
    /// Describes all links on a page.
    /// </summary>
    public class PdfPageLinks
    {
        /// <summary>
        /// All links of the page.
        /// </summary>
        public IList<PdfPageLink> Links { get; private set; }

        internal PdfPageLinks(IList<PdfPageLink> links)
        {
            if (links == null)
                throw new ArgumentNullException("links");

            Links = new ReadOnlyCollection<PdfPageLink>(links);
        }
    }
}
