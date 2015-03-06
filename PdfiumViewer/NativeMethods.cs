using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Runtime.ConstrainedExecution;
using System.Runtime.InteropServices;
using System.Security.Permissions;
using System.Text;
using Microsoft.Win32.SafeHandles;

namespace PdfiumViewer
{
    public static class NativeMethods
    {
        static NativeMethods()
        {
            // Load the platform dependent Pdfium.dll if it exists.

            string path = Path.GetDirectoryName(typeof(NativeMethods).Assembly.Location);

            if (IntPtr.Size == 4)
                path = Path.Combine(path, "x86");
            else
                path = Path.Combine(path, "x64");

            path = Path.Combine(path, "Pdfium.dll");

            if (File.Exists(path))
                LoadLibrary(path);
        }

        [DllImport("kernel32.dll", EntryPoint = "CopyMemory", SetLastError = false)]
        public static extern void CopyMemory(IntPtr dest, IntPtr src, uint count);

        [DllImport("kernel32", SetLastError = true, CharSet = CharSet.Ansi)]
        private static extern IntPtr LoadLibrary([MarshalAs(UnmanagedType.LPStr)]string lpFileName);

        // Pdfium stuff.

        [DllImport("pdfium.dll")]
        public static extern void FPDF_InitLibrary();


        [DllImport("pdfium.dll")]
        public static extern void FPDF_InitLibrary(IntPtr hInstance);

        [DllImport("pdfium.dll")]
        public static extern void FPDF_DestroyLibrary();

        [DllImport("pdfium.dll", CharSet = CharSet.Ansi)]
        public static extern IntPtr FPDF_LoadMemDocument(SafeHandle data_buf, int size, string password);

        [DllImport("pdfium.dll", CharSet = CharSet.Ansi)]
        public static extern IntPtr FPDF_LoadMemDocument(byte[] data_buf, int size, string password);

        [DllImport("pdfium.dll")]
        public static extern void FPDF_CloseDocument(IntPtr document);

        [DllImport("pdfium.dll")]
        public static extern int FPDF_GetPageCount(IntPtr document);

        [DllImport("pdfium.dll")]
        public static extern uint FPDF_GetDocPermissions(IntPtr document);

        [DllImport("pdfium.dll")]
        public static extern IntPtr FPDFDOC_InitFormFillEnvironment(IntPtr document, ref FPDF_FORMFILLINFO formInfo);

        [DllImport("pdfium.dll")]
        public static extern void FPDF_SetFormFieldHighlightColor(IntPtr hHandle, int fieldType, uint color);

        [DllImport("pdfium.dll")]
        public static extern void FPDF_SetFormFieldHighlightAlpha(IntPtr hHandle, byte alpha);

        [DllImport("pdfium.dll")]
        public static extern void FORM_DoDocumentJSAction(IntPtr hHandle);

        [DllImport("pdfium.dll")]
        public static extern void FORM_DoDocumentOpenAction(IntPtr hHandle);

        [DllImport("pdfium.dll")]
        public static extern void FPDFDOC_ExitFormFillEnviroument(IntPtr hHandle);

        [DllImport("pdfium.dll")]
        public static extern void FORM_DoDocumentAAction(IntPtr hHandle, FPDFDOC_AACTION aaType);

        [DllImport("pdfium.dll")]
        public static extern IntPtr FPDF_LoadPage(IntPtr document, int page_index);

        [DllImport("pdfium.dll")]
        public static extern IntPtr FPDFText_LoadPage(IntPtr page);

        [DllImport("pdfium.dll")]
        public static extern void FORM_OnAfterLoadPage(IntPtr page, IntPtr _form);

        [DllImport("pdfium.dll")]
        public static extern void FORM_DoPageAAction(IntPtr page, IntPtr _form, FPDFPAGE_AACTION fPDFPAGE_AACTION);

        [DllImport("pdfium.dll")]
        public static extern double FPDF_GetPageWidth(IntPtr page);

        [DllImport("pdfium.dll")]
        public static extern double FPDF_GetPageHeight(IntPtr page);

        [DllImport("pdfium.dll")]
        public static extern void FORM_OnBeforeClosePage(IntPtr page, IntPtr _form);

        [DllImport("pdfium.dll")]
        public static extern void FPDFText_ClosePage(IntPtr text_page);

        [DllImport("pdfium.dll")]
        public static extern void FPDF_ClosePage(IntPtr page);

        [DllImport("pdfium.dll")]
        public static extern void FPDF_RenderPage(IntPtr dc, IntPtr page, int start_x, int start_y, int size_x, int size_y, int rotate, FPDF flags);

        [DllImport("pdfium.dll")]
        public static extern void FPDF_RenderPageBitmap(IntPtr bitmapHandle, IntPtr page, int start_x, int start_y, int size_x, int size_y, int rotate, FPDF flags);

        [DllImport("pdfium.dll")]
        public static extern IntPtr FPDFBitmap_Destroy(IntPtr bitmapHandle);

        [DllImport("gdi32.dll")]
        public static extern bool FillRgn(IntPtr hdc, IntPtr hrgn, IntPtr hbr);
        [DllImport("gdi32.dll")]
        public static extern IntPtr CreateRectRgn(int nLeftRect, int nTopRect, int nRightRect, int nBottomRect);

        [DllImport("pdfium.dll")]
        public static extern IntPtr FPDFBitmap_Create(int width, int height, int alpha);

        [DllImport("pdfium.dll")]
        public static extern IntPtr FPDFBitmap_CreateEx(int width, int height, int format, byte[] first_scan, int stride);

        [DllImport("pdfium.dll")]
        public static extern IntPtr FPDFBitmap_CreateEx(int width, int height, int format, IntPtr first_scan, int stride);

        [DllImport("pdfium.dll")]
        public static extern void FPDFBitmap_FillRect(IntPtr bitmapHandle, int left, int top, int width, int height,
            FPDFColor color);

        [DllImport("pdfium.dll")]
        internal static extern IntPtr FPDFBitmap_GetBuffer(IntPtr bitmapHandle);

        [DllImport("pdfium.dll")]
        public static extern int FPDF_GetPageSizeByIndex(IntPtr document, int page_index, out double width, out double height);

        #region FPDFText Methods
        /// <summary>
        /// Get number of characters in a page
        /// </summary>
        /// <param name="page">Handle to a text page information structure. Returned by FPDFText_LoadPage function.</param>
        /// <returns>Number of characters in the page. Return -1 for error. Generated characters, like additional space characters, new line characters, are also counted.</returns>
        [DllImport("pdfium.dll")]
        public static extern int FPDFText_CountChars(IntPtr page);

        /// <summary>
        /// Get Unicode of a character in a page
        /// </summary>
        /// <param name="page">Handle to a text page information structure. Returned by FPDFText_LoadPage function.</param>
        /// <param name="index">Zero-based index of the character</param>
        /// <returns>The Unicode of the particular character. If a character is not encoded in Unicode and Foxit engine can't convert to Unicode, the return value will be zero.</returns>
        [DllImport("pdfium.dll")]
        public static extern uint FPDFText_GetUnicode(IntPtr page, int index);

        /// <summary>
        /// Indicates whether a character is a generated character
        /// "Generated character" is character not actually encoded in the PDF page, but generated by FPDFTEXT engine to keep formatting information. 
        /// This happens in two cases: 
        /// 1) an extra space character will be generated if two characters in the same line appears to be apart quite some space
        /// 2) a new line character will be generated if two consecutive characters appears to be on different line.This characters are useful when doing the search.
        /// </summary>
        /// <param name="page">Handle to a text page information structure.</param>
        /// <param name="index">Zero-based index of the character</param>
        /// <returns>TRUE indicates a generated character and FALSE indicates an actual character in the PDF page.</returns>
        [DllImport("pdfium.dll")]
        public static extern bool FPDFText_IsGenerated(IntPtr page, int index);

        /// <summary>
        /// Get the font size of a particular character
        /// </summary>
        /// <param name="page">Handle to a text page information structure.</param>
        /// <param name="index">Zero-based index of the character.</param>
        /// <returns>The font size of the particular character, measured in points (about 1/72 inch). This is the typographic size of the font (so called "em size").</returns>
        [DllImport("pdfium.dll")]
        public static extern double FPDFText_GetFontSize(IntPtr page, int index);

        /// <summary>
        /// Get origin position of a particular character
        /// </summary>
        /// <param name="page">Handle to a text page information structure.</param>
        /// <param name="index">Zero-based index of the character.</param>
        /// <param name="x">Pointer to a double number receiving X position of the character origin.</param>
        /// <param name="y">Pointer to a double number receiving Y position of the character origin.</param>
        [DllImport("pdfium.dll")]
        public static extern void FPDFText_GetOrigin(IntPtr page, int index, ref double x, ref double y);

        /// <summary>
        /// Get bounding box of a particular character
        /// 
        /// Comment: All positions are measured in PDF "user space".
        /// </summary>
        /// <param name="page">Handle to a text page information structure.</param>
        /// <param name="index">Zero-based index of the character.</param>
        /// <param name="left">Pointer to a double number receiving left position of the character box.</param>
        /// <param name="right">Pointer to a double number receiving right position of the character box.</param>
        /// <param name="bottom">Pointer to a double number receiving bottom position of the character box.</param>
        /// <param name="top">Pointer to a double number receiving top position of the character box.</param>
        [DllImport("pdfium.dll")]
        public static extern void FPDFText_GetCharBox(IntPtr page, int index, ref double left, ref double right, ref double bottom, ref double top);

        /// <summary>
        /// Get the matrix of a particular character.
        /// 
        /// A matrix defines transformation of coordinate from one space to another. 
        /// In PDF, a matrix is defined by the following equations: 
        /// x' = a * x + c * y + e; 
        /// y' = b * x + d * y + f; 
        /// FPDFText_GetMatrix function is used to get a,b,c,d coefficients of the transformation from "text space" to "user space". 
        /// The e, f coefficients are actually the origin position, which can be fetched by FPDFText_GetOrigin function.
        /// </summary>
        /// <param name="page">Handle to a text page information structure.</param>
        /// <param name="index">Zero-based index of the character</param>
        /// <param name="a">Pointer to a double value receiving coefficient "a" of the matrix</param>
        /// <param name="b">Pointer to a double value receiving coefficient "b" of the matrix</param>
        /// <param name="c">Pointer to a double value receiving coefficient "c" of the matrix</param>
        /// <param name="d">Pointer to a double value receiving coefficient "d" of the matrix</param>
        [DllImport("pdfium.dll")]
        public static extern void FPDFText_GetMatrix(IntPtr page, int index, ref double a, ref double b, ref double c, ref double d);

        /// <summary>
        /// Get font of a particular character
        /// </summary>
        /// <param name="page">Handle to a text page information structure.</param>
        /// <param name="index">Zero-based index of the character.</param>
        /// <returns>A handle to the font used by the particular character. 
        /// This handle can be used in FPDFFont_xxx functions for more information about the font.</returns>
        [DllImport("pdfium.dll")]
        public static extern FPDF_FONT FPDFText_GetFont(IntPtr page, int index);

        /// <summary>
        /// Get font ascent (in 1/1000 em)
        /// </summary>
        /// <param name="font">Handle to a font. Returned by FPDFText_GetFont function.</param>
        /// <returns>The ascent (typically the above-baseline height of letter "h"), measured in 1/1000 of em size. So if a character uses a font size (em size) of 10 points, and it has an ascent value of 500 (meaning half of the em), then the ascent height will be 5 points (5/72 inch).</returns>
        [DllImport("pdfium.dll")]
        public static extern int FPDFFont_GetAscent(FPDF_FONT font);

        /// <summary>
        /// Get font descent (in 1/1000 em)
        /// </summary>
        /// <param name="font">Handle to a font. Returned by FPDFText_GetFont function.</param>
        /// <returns>The descent (typically the under-baseline height of letter "g"), measured in 1/1000 of em size. Most fonts have a negative descent value.</returns>
        [DllImport("pdfium.dll")]
        public static extern int FPDFFont_GetDescent(FPDF_FONT font);

        /// <summary>
        /// Get the Name of a font.
        /// </summary>
        /// <param name="font">Handle to a font. Returned by FPDFText_GetFont function.</param>
        /// <returns>A pointer to a null-terminated string that specifies the name of the font. Application can't modify the returned string.</returns>
        [DllImport("pdfium.dll")]
        public static extern string FPDFFont_GetName(FPDF_FONT font);

        /// <summary>
        /// Get the index of a character at or nearby a certain position on the page
        /// </summary>
        /// <param name="page">Handle to a text page information structure.</param>
        /// <param name="x">X position in PDF "user space".</param>
        /// <param name="y">Y position in PDF "user space".</param>
        /// <param name="xTolerance">An x-axis tolerance value for character hit detection, in point unit.</param>
        /// <param name="yTolerance">A y-axis tolerance value for character hit detection, in point unit.</param>
        /// <returns>The zero-based index of the character at, or nearby the point (x,y). If there is no character at or nearby the point, return value will be -1. If an error occurs, -3 will be returned.</returns>
        [DllImport("pdfium.dll")]
        public static extern int FPDFText_GetCharIndexAtPos(IntPtr page, double x, double y, double xTolerance, double yTolerance);

        /// <summary>
        /// Move the character index in different directions and get new character index, from a specific character.
        /// 
        /// FPDFTEXT moves the character pointer according to "stream order". For example, left will move to the previous character, right will move to next character. Because in PDF, "stream order" can be different from "appearance order" (the order that appears to human eyes), so it's possible the moving direction doesn't match the actually position movement. For example, using FPDFTEXT_LEFT may actually result in a character that's all the way down in the page.
        /// </summary>
        /// <param name="page">Handle to a text page information structure.</param>
        /// <param name="index">Zero-based index for the current character</param>
        /// <param name="direction">A number indicating the moving direction. 
        /// Can be one of the followings: 
        /// FPDFTEXT_LEFT, 
        /// FPDFTEXT_UP, 
        /// FPDFTEXT_RIGHT, 
        /// FPDFTEXT_DOWN</param>
        /// <returns>Zero-base character index for the new position. -1 if beginning of the page reached; -2 if end of the page reached. -3 for failures.</returns>
        [DllImport("pdfium.dll")]
        public static extern int FPDFText_GetCharIndexByDirection(IntPtr page, int index, int direction);

        /// <summary>
        /// Extract unicode text string from the page
        /// 
        /// Comment: This function ignores characters without unicode information.
        /// </summary>
        /// <param name="page">Handle to a text page information structure.</param>
        /// <param name="start_index">Index for the start characters.</param>
        /// <param name="count">Number of characters to be extracted.</param>
        /// <param name="result">A buffer (allocated by application) receiving the extracted unicodes. The size of the buffer must be able to hold the number of characters plus a terminator.</param>
        /// <returns>Number of characters written into the result buffer, excluding the trailing terminator.</returns>
        [DllImport("pdfium.dll")]
        public static extern int FPDFText_GetText(IntPtr page, int start_index, int count, ushort[] result);

        /// <summary>
        /// Count number of rectangular areas occupied by a segment of texts.
        /// 
        /// Comment: This function, along with FPDFText_GetRect can be used by applications to detect the position on the page for a text segment, so proper areas can be highlighted or something. FPDFTEXT will automatically merge small character boxes into bigger one if those characters are on the same line and use same font settings.
        /// </summary>
        /// <param name="page">Handle to a text page information structure.</param>
        /// <param name="start_index">Index for the start characters</param>
        /// <param name="count">Number of characters</param>
        /// <returns>Number of rectangles. Zero for error.</returns>
        [DllImport("pdfium.dll")]
        public static extern int FPDFText_CountRects(IntPtr page, int start_index, int count);

        /// <summary>
        /// Get a rectangular area from the result generated by FPDFText_CountRects.
        /// </summary>
        /// <param name="page">Handle to a text page information structure.</param>
        /// <param name="rect_index">Zero-based index for the rectangle.</param>
        /// <param name="left">Pointer to a double value receiving the rectangle left boundary.</param>
        /// <param name="top">Pointer to a double value receiving the rectangle top boundary.</param>
        /// <param name="right">Pointer to a double value receiving the rectangle right boundary.</param>
        /// <param name="bottom">Pointer to a double value receiving the rectangle bottom boundary.</param>
        [DllImport("pdfium.dll")]
        public static extern void FPDFText_GetRect(IntPtr page, int rect_index, ref double left, ref double top, ref double right, ref double bottom);

        /// <summary>
        /// Extract unicode text within a rectangular boundary on the page
        /// </summary>
        /// <param name="page">Handle to a text page information structure.</param>
        /// <param name="left">Left boundary.</param>
        /// <param name="top">Top boundary.</param>
        /// <param name="right">Right boundary.</param>
        /// <param name="bottom">Bottom boundary.</param>
        /// <param name="buffer">A unicode buffer.</param>
        /// <param name="bufferLength">Number of characters (not bytes) for the buffer, excluding an additional terminator</param>
        /// <returns>If buffer is NULL or buflen is zero, number of characters (not bytes) needed, otherwise, number of characters copied into the buffer.</returns>
        [DllImport("pdfium.dll")]
        public static extern int FPDFText_GetBoundedText(IntPtr page, double left, double top, double right, double bottom, ref ushort buffer, int bufferLength);

        /// <summary>
        /// Get number of text segments within a rectangular boundary on the page
        /// </summary>
        /// <param name="page">Handle to a text page information structure.</param>
        /// <param name="left">Left boundary.</param>
        /// <param name="top">Top boundary.</param>
        /// <param name="right">Right boundary.</param>
        /// <param name="bottom">Bottom boundary.</param>
        /// <returns>Number of segments.</returns>
        [DllImport("pdfium.dll")]
        public static extern int FPDFText_CountBoundedSegments(IntPtr page, double left, double top, double right, double bottom);

        /// <summary>
        /// Get a particular segment in the result generated by FPDFText_CountBoundedSegments function.
        /// </summary>
        /// <param name="page">Handle to a text page information structure.</param>
        /// <param name="seg_index">Zero-based index for the segment</param>
        /// <param name="start_index">Pointer to an integer receiving the start character index for the segment.</param>
        /// <param name="count">Pointer to an integer receiving number of characters in the segment.</param>
        [DllImport("pdfium.dll")]
        public static extern void FPDFText_GetBoundedSegment(IntPtr page, int seg_index, ref int start_index, ref int count);

        /// <summary>
        /// Start a search.
        /// </summary>
        /// <param name="page">Handle to a text page information structure.</param>
        /// <param name="findWhat">A unicode match pattern.</param>
        /// <param name="flags">Option flags.</param>
        /// <param name="start_index">Start from this character. -1 for end of the page.</param>
        /// <returns>A handle for the search context. FPDFText_FindClose must be called to release this handle.</returns>
        [DllImport("pdfium.dll")]
        public static extern IntPtr FPDFText_FindStart(IntPtr page, byte[] findWhat, int flags, int start_index);

        /// <summary>
        /// Search in the direction from page start to end.
        /// </summary>
        /// <param name="handle">A search context handle returned by FPDFText_FindStart.</param>
        /// <returns>Whether a match is found.</returns>
        [DllImport("pdfium.dll")]
        public static extern bool FPDFText_FindNext(IntPtr handle);

        /// <summary>
        /// Search in the direction from page end to start.
        /// </summary>
        /// <param name="handle">A search context handle returned by FPDFText_FindStart.</param>
        /// <returns>Whether a match is found.</returns>
        [DllImport("pdfium.dll")]
        public static extern bool FPDFText_FindPrev(IntPtr handle);

        /// <summary>
        /// Get the starting character index of the search result.
        /// </summary>
        /// <param name="handle">A search context handle returned by FPDFText_FindStart.</param>
        /// <returns>Index for the starting character.</returns>
        [DllImport("pdfium.dll")]
        public static extern int FPDFText_GetSchResultIndex(IntPtr handle);

        /// <summary>
        /// Get the number of matched characters in the search result.
        /// </summary>
        /// <param name="handle">A search context handle returned by FPDFText_FindStart.</param>
        /// <returns>Number of matched characters.</returns>
        [DllImport("pdfium.dll")]
        public static extern int FPDFText_GetSchCount(IntPtr handle);

        /// <summary>
        /// Release a search context.
        /// </summary>
        /// <param name="handle">A search context handle returned by FPDFText_FindStart.</param>
        [DllImport("pdfium.dll")]
        public static extern void FPDFText_FindClose(IntPtr handle);

        /// <summary>
        /// Convert a PDF file to a TXT File.
        /// </summary>
        /// <param name="sourceFile">Path to the PDF file you want to convert.</param>
        /// <param name="destFile">The path of the file you want to save.</param>
        /// <param name="flag">0 for stream order ,1 for appearance order.</param>
        /// <param name="password">A string used as the password for PDF file. If no password needed, empty or NULL can be used.</param>
        /// <returns>TURE for succeed, False for failed.</returns>
        [DllImport("pdfium.dll")]
        public static extern bool FPDFText_PDFToText(string sourceFile, string destFile, int flag, string password);

        //Need Review
        /// <summary>
        /// Convert a PDF page data to a text buffer.
        /// </summary>
        /// <param name="doc">Handle to document. Returned by FPDF_LoadDocument function.</param>
        /// <param name="page_index">Index number of the page. 0 for the first page.</param>
        /// <param name="buf">An output buffer used to hold the text of the page.</param>
        /// <param name="size">Size of the buffer</param>
        /// <param name="flag">0 for stream order ,1 for appearance order.</param>
        /// <returns>If buf is NULL or size is zero, number of characters (not bytes) needed, otherwise, number of characters copied into the buf.</returns>
        [DllImport("pdfium.dll")]
        public static extern int FPDFText_PageToText(IntPtr doc, int page_index, ref char[] buf, int size, int flag);

        #endregion

        [StructLayout(LayoutKind.Sequential)]
        public class FPDF_FONT
        {

        }

        [Flags]
        public enum FPDF_SEARCH_FLAGS
        {
            FPDF_MATCHCASE = 1,
            FPDF_MATCHWHOLEWORD = 2
        }

        [StructLayout(LayoutKind.Sequential)]
        public class FPDF_FORMFILLINFO
        {
            public int version;

            private IntPtr Release;

            private IntPtr FFI_Invalidate;

            private IntPtr FFI_OutputSelectedRect;

            private IntPtr FFI_SetCursor;

            private IntPtr FFI_SetTimer;

            private IntPtr FFI_KillTimer;

            private IntPtr FFI_GetLocalTime;

            private IntPtr FFI_OnChange;

            private IntPtr FFI_GetPage;

            private IntPtr FFI_GetCurrentPage;

            private IntPtr FFI_GetRotation;

            private IntPtr FFI_ExecuteNamedAction;

            private IntPtr FFI_SetTextFieldFocus;

            private IntPtr FFI_DoURIAction;

            private IntPtr FFI_DoGoToAction;

            private IntPtr m_pJsPlatform;
        }

        public enum FPDF_UNSP
        {
            DOC_XFAFORM = 1,
            DOC_PORTABLECOLLECTION = 2,
            DOC_ATTACHMENT = 3,
            DOC_SECURITY = 4,
            DOC_SHAREDREVIEW = 5,
            DOC_SHAREDFORM_ACROBAT = 6,
            DOC_SHAREDFORM_FILESYSTEM = 7,
            DOC_SHAREDFORM_EMAIL = 8,
            ANNOT_3DANNOT = 11,
            ANNOT_MOVIE = 12,
            ANNOT_SOUND = 13,
            ANNOT_SCREEN_MEDIA = 14,
            ANNOT_SCREEN_RICHMEDIA = 15,
            ANNOT_ATTACHMENT = 16,
            ANNOT_SIG = 17
        }

        public enum FPDFDOC_AACTION
        {
            WC = 0x10,
            WS = 0x11,
            DS = 0x12,
            WP = 0x13,
            DP = 0x14
        }

        public enum FPDFPAGE_AACTION
        {
            OPEN = 0,
            CLOSE = 1
        }

        [Flags]
        public enum FPDF
        {
            ANNOT = 0x01,
            LCD_TEXT = 0x02,
            NO_NATIVETEXT = 0x04,
            GRAYSCALE = 0x08,
            DEBUG_INFO = 0x80,
            NO_CATCH = 0x100,
            RENDER_LIMITEDIMAGECACHE = 0x200,
            RENDER_FORCEHALFTONE = 0x400,
            PRINTING = 0x800,
            REVERSE_BYTE_ORDER = 0x10
        }

        // Windows stuff.

        [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern MemoryMappedHandle CreateFileMapping(SafeHandle hFile, IntPtr lpFileMappingAttributes, FileMapProtection flProtect, uint dwMaximumSizeHigh, uint dwMaximumSizeLow, [MarshalAs(UnmanagedType.LPTStr)] string lpName);

        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool CloseHandle(IntPtr hObject);

        [Flags]
        public enum FileMapProtection : uint
        {
            PageReadonly = 0x02,
            PageReadWrite = 0x04,
            PageWriteCopy = 0x08,
            PageExecuteRead = 0x20,
            PageExecuteReadWrite = 0x40,
            SectionCommit = 0x8000000,
            SectionImage = 0x1000000,
            SectionNoCache = 0x10000000,
            SectionReserve = 0x4000000,
        }

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern MappedViewHandle MapViewOfFile(SafeHandle hFileMappingObject, FileMapAccess dwDesiredAccess, uint dwFileOffsetHigh, uint dwFileOffsetLow, uint dwNumberOfBytesToMap);

        [Flags]
        public enum FileMapAccess : uint
        {
            FileMapCopy = 0x0001,
            FileMapWrite = 0x0002,
            FileMapRead = 0x0004,
            FileMapAllAccess = 0x001f,
            FileMapExecute = 0x0020,
        }

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool UnmapViewOfFile(IntPtr lpBaseAddress);

        [DllImport("user32.dll")]
        public static extern int ScrollWindowEx(IntPtr hWnd, int dx, int dy, RECT prcScroll, RECT prcClip, IntPtr hrgnUpdate, IntPtr prcUpdate, uint flags);

        [DllImport("user32.dll")]
        public static extern int ScrollWindowEx(IntPtr hWnd, int dx, int dy, IntPtr prcScroll, IntPtr prcClip, IntPtr hrgnUpdate, IntPtr prcUpdate, uint flags);

        [SecurityPermission(SecurityAction.InheritanceDemand, UnmanagedCode = true)]
        [SecurityPermission(SecurityAction.Demand, UnmanagedCode = true)]
        public class MemoryMappedHandle : SafeHandleZeroOrMinusOneIsInvalid
        {
            public MemoryMappedHandle()
                : base(true)
            {
            }

            [ReliabilityContract(Consistency.WillNotCorruptState, Cer.MayFail)]
            protected override bool ReleaseHandle()
            {
                return CloseHandle(handle);
            }
        }

        public class MappedViewHandle : SafeHandleZeroOrMinusOneIsInvalid
        {
            public MappedViewHandle()
                : base(true)
            {
            }

            protected override bool ReleaseHandle()
            {
                return UnmapViewOfFile(handle);
            }
        }

        public const int GM_ADVANCED = 2;

        [DllImport("gdi32.dll")]
        public static extern int SetGraphicsMode(IntPtr hdc, int iMode);

        [StructLayout(LayoutKind.Sequential)]
        public struct XFORM
        {
            public float eM11;
            public float eM12;
            public float eM21;
            public float eM22;
            public float eDx;
            public float eDy;
        }

        public const uint MWT_LEFTMULTIPLY = 2;

        [DllImport("gdi32.dll")]
        public static extern bool ModifyWorldTransform(IntPtr hdc, [In] ref XFORM lpXform, uint iMode);

        [StructLayout(LayoutKind.Sequential)]
        public struct POINT
        {
            public int X;
            public int Y;
        }

        [DllImport("gdi32.dll")]
        public static extern bool SetViewportOrgEx(IntPtr hdc, int X, int Y, out POINT lpPoint);

        public const uint SW_ERASE = 0x0004;
        public const uint SW_SMOOTHSCROLL = 0x0010;
        public const int WS_VSCROLL = 0x00200000;
        public const int WS_HSCROLL = 0x00100000;
        public const int WM_MOUSEWHEEL = 0x20a;
        public const int SB_HORZ = 0x0;
        public const int SB_VERT = 0x1;
        public const uint SW_INVALIDATE = 0x0002;
        public const uint SW_SCROLLCHILDREN = 0x0001;
        public const int SB_LINEUP = 0;
        public const int SB_LINELEFT = 0;
        public const int SB_LINEDOWN = 1;
        public const int SB_LINERIGHT = 1;
        public const int SB_PAGEUP = 2;
        public const int SB_PAGELEFT = 2;
        public const int SB_PAGEDOWN = 3;
        public const int SB_PAGERIGHT = 3;
        public const int SB_THUMBPOSITION = 4;
        public const int SB_THUMBTRACK = 5;
        public const int SB_TOP = 6;
        public const int SB_LEFT = 6;
        public const int SB_BOTTOM = 7;
        public const int SB_RIGHT = 7;
        public const int SB_ENDSCROLL = 8;
        public const int WM_HSCROLL = 0x114;
        public const int WM_VSCROLL = 0x115;
        public const int SIF_TRACKPOS = 0x10;
        public const int SIF_RANGE = 0x1;
        public const int SIF_POS = 0x4;
        public const int SIF_PAGE = 0x2;
        public const int SIF_ALL = SIF_RANGE | SIF_PAGE | SIF_POS | SIF_TRACKPOS;

        [StructLayout(LayoutKind.Sequential)]
        public struct RECT
        {
            public int left;
            public int top;
            public int right;
            public int bottom;

            public RECT(Rectangle r)
            {
                left = r.Left;
                top = r.Top;
                right = r.Right;
                bottom = r.Bottom;
            }

            public Rectangle ToRectangle()
            {
                return new Rectangle(left, top, right - left, bottom - top);
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        public class SCROLLINFO
        {
            public int cbSize = Marshal.SizeOf(typeof(SCROLLINFO));
            public int fMask;
            public int nMin;
            public int nMax;
            public int nPage;
            public int nPos;
            public int nTrackPos;

            public SCROLLINFO()
            {
            }

            public SCROLLINFO(int mask, int min, int max, int page, int pos)
            {
                fMask = mask;
                nMin = min;
                nMax = max;
                nPage = page;
                nPos = pos;
            }
        }

        [DllImport("user32.dll")]
        public static extern int ScrollWindowEx(HandleRef hWnd, int dx, int dy, IntPtr prcScroll, ref RECT prcClip, IntPtr hrgnUpdate, ref RECT prcUpdate, uint flags);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool GetScrollInfo(HandleRef hwnd, int fnBar, SCROLLINFO lpsi);

        [DllImport("user32.dll")]
        public static extern int SetScrollInfo(HandleRef hwnd, int fnBar, [In] SCROLLINFO lpsi, bool fRedraw);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern UIntPtr SendMessage(IntPtr handle, int message, IntPtr wParam, IntPtr lParam);

        [DllImport("user32.dll")]
        public static extern IntPtr WindowFromPoint(Point pt);

        public static class Util
        {
            private static int LOWORD(int n)
            {
                return n & 0xffff;
            }

            public static int LOWORD(IntPtr n)
            {
                return LOWORD(unchecked((int)(long)n));
            }
        }

        [DllImport("gdi32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool DeleteObject([In] IntPtr hObject);

        public const int LOGPIXELSX = 88;
        public const int LOGPIXELSY = 89;

        [DllImport("gdi32.dll")]
        public static extern int GetDeviceCaps(IntPtr hdc, int nIndex);

        [DllImport("gdi32.dll")]
        public static extern bool Rectangle(IntPtr hdc, int nLeftRect, int nTopRect, int nRightRect, int nBottomRect);

        [DllImport("gdi32.dll", SetLastError = true)]
        public static extern IntPtr CreateCompatibleDC([In] IntPtr hdc);

        [DllImport("user32.dll")]
        public static extern IntPtr GetDC(IntPtr hWnd);

        [DllImport("gdi32.dll")]
        public static extern IntPtr CreateCompatibleBitmap([In] IntPtr hdc, int nWidth, int nHeight);

        [DllImport("gdi32.dll")]
        public static extern IntPtr SelectObject([In] IntPtr hdc, [In] IntPtr hgdiobj);

        [DllImport("gdi32.dll")]
        public static extern IntPtr CreateSolidBrush(int crColor);
    }
}
