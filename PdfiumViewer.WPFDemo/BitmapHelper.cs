using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using System.Drawing;
using System.Drawing.Imaging;
using PdfiumViewer.IRISTedExtensions;

namespace PdfiumViewer.WPFDemo
{
    internal class BitmapHelper
    {

        /// <summary>
        /// Convert an IImage to a WPF BitmapSource. The result can be used in the Set Property of Image.Source
        /// </summary>
        /// <param name="image">The Emgu CV Image</param>
        /// <returns>The equivalent BitmapSource</returns>
        public static BitmapSource ToBitmapSource(System.Drawing.Bitmap image)
        {
            if (image == null) return null;

            using (System.Drawing.Bitmap source = (System.Drawing.Bitmap)image.Clone())
            {
                IntPtr ptr = source.GetHbitmap(); //obtain the Hbitmap

                BitmapSource bs = System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(
                    ptr,
                    IntPtr.Zero,
                    System.Windows.Int32Rect.Empty,
                    System.Windows.Media.Imaging.BitmapSizeOptions.FromEmptyOptions());

                Win32API.DeleteObject(ptr); //release the HBitmap
                bs.Freeze();
                return bs;
            }
        }
    }
}
