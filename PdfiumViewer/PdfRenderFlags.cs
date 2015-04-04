using System;
using System.Collections.Generic;
using System.Text;

namespace PdfiumViewer
{
    /// <summary>
    /// Flags that influence the page rendering process.
    /// </summary>
    [Flags]
    public enum PdfRenderFlags
    {
        /// <summary>
        /// No flags.
        /// </summary>
        None = 0,
        /// <summary>
        /// Render for printing.
        /// </summary>
        ForPrinting = NativeMethods.FPDF.PRINTING,
        /// <summary>
        /// Set if annotations are to be rendered.
        /// </summary>
        Annotations = NativeMethods.FPDF.ANNOT,
        /// <summary>
        /// Set if using text rendering optimized for LCD display.
        /// </summary>
        LcdText = NativeMethods.FPDF.LCD_TEXT,
        /// <summary>
        /// Don't use the native text output available on some platforms.
        /// </summary>
        NoNativeText = NativeMethods.FPDF.NO_NATIVETEXT,
        /// <summary>
        /// Grayscale output.
        /// </summary>
        Grayscale = NativeMethods.FPDF.GRAYSCALE,
        /// <summary>
        /// Limit image cache size.
        /// </summary>
        LimitImageCacheSize = NativeMethods.FPDF.RENDER_LIMITEDIMAGECACHE,
        /// <summary>
        /// Always use halftone for image stretching.
        /// </summary>
        ForceHalftone = NativeMethods.FPDF.RENDER_FORCEHALFTONE,
        /// <summary>
        /// Render with a transparent background.
        /// </summary>
        Transparent = 0x1000
    }
}
