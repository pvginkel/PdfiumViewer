using System;
using System.Collections.Generic;
using System.Drawing.Printing;
using System.Text;
using System.Windows.Forms;

namespace ChromePdfViewer
{
    internal class DefaultSettings
    {
        public int DpiX { get; private set; }
        public int DpiY { get; private set; }
        public int Width { get; private set; }
        public int Height { get; private set; }

        public DefaultSettings()
        {
            using (var dialog = new PrintDialog())
            {
                bool found = false;

                try
                {
                    foreach (PrinterResolution resolution in dialog.PrinterSettings.PrinterResolutions)
                    {
                        if (resolution.Kind == PrinterResolutionKind.Custom)
                        {
                            DpiX = resolution.X;
                            DpiY = resolution.Y;
                            Width = (int)((dialog.PrinterSettings.DefaultPageSettings.PaperSize.Width / 100.0) * resolution.X);
                            Height = (int)((dialog.PrinterSettings.DefaultPageSettings.PaperSize.Height / 100.0) * resolution.Y);

                            found = true;
                            break;
                        }
                    }
                }
                catch
                {
                    // Ignore any exceptions; just use defaults.
                }

                if (!found)
                {
                    // Default to A4 size.

                    DpiX = 600;
                    DpiY = 500;
                    Width = (int)(8.27 * DpiX);
                    Height = (int)(11.69 * DpiY);
                }
            }
        }
    }
}
