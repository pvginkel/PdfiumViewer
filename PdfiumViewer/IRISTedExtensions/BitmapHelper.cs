using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Text;

namespace PdfiumViewer.IRISTedExtensions
{
    public static class BitmapHelper
    {
        public static Bitmap Convert_BGRA_TO_ARGB(byte[] DATA, int width, int height)
        {
            Bitmap Bm = new Bitmap(width, height, PixelFormat.Format32bppArgb);

            int index;
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    // BGRA TO ARGB
                    index = 4 * (x + (y * width));
                    Color c = Color.FromArgb(
                         DATA[index + 3],
                        DATA[index + 2],
                        DATA[index + 1],
                        DATA[index + 0]);
                    Bm.SetPixel(x, y, c);
                }
            }
            return Bm;
        }
    }
}
