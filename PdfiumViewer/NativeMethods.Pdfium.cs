using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;

namespace PdfiumViewer
{
    partial class NativeMethods
    {
        // Interned strings are cached over AppDomains. This means that when we
        // lock on this string, we actually lock over AppDomain's. The Pdfium
        // library is not thread safe, and this way of locking
        // guarantees that we don't access the Pdfium library from different
        // threads, even when there are multiple AppDomain's in play.
        private static readonly string LockString = String.Intern("e362349b-001d-4cb2-bf55-a71606a3e36f");

        public static void FPDF_AddRef()
        {
            lock (LockString)
            {
                Imports.FPDF_AddRef();
            }
        }

        public static void FPDF_Release()
        {
            lock (LockString)
            {
                Imports.FPDF_Release();
            }
        }

        public static IntPtr FPDF_LoadMemDocument(SafeHandle data_buf, int size, string password)
        {
            lock (LockString)
            {
                return Imports.FPDF_LoadMemDocument(data_buf, size, password);
            }
        }

        public static IntPtr FPDF_LoadMemDocument(byte[] data_buf, int size, string password)
        {
            lock (LockString)
            {
                return Imports.FPDF_LoadMemDocument(data_buf, size, password);
            }
        }

        public static void FPDF_CloseDocument(IntPtr document)
        {
            lock (LockString)
            {
                Imports.FPDF_CloseDocument(document);
            }
        }

        public static int FPDF_GetPageCount(IntPtr document)
        {
            lock (LockString)
            {
                return Imports.FPDF_GetPageCount(document);
            }
        }

        public static uint FPDF_GetDocPermissions(IntPtr document)
        {
            lock (LockString)
            {
                return Imports.FPDF_GetDocPermissions(document);
            }
        }

        public static IntPtr FPDFDOC_InitFormFillEnvironment(IntPtr document, ref FPDF_FORMFILLINFO formInfo)
        {
            lock (LockString)
            {
                return Imports.FPDFDOC_InitFormFillEnvironment(document, ref formInfo);
            }
        }

        public static void FPDF_SetFormFieldHighlightColor(IntPtr hHandle, int fieldType, uint color)
        {
            lock (LockString)
            {
                Imports.FPDF_SetFormFieldHighlightColor(hHandle, fieldType, color);
            }
        }

        public static void FPDF_SetFormFieldHighlightAlpha(IntPtr hHandle, byte alpha)
        {
            lock (LockString)
            {
                Imports.FPDF_SetFormFieldHighlightAlpha(hHandle, alpha);
            }
        }

        public static void FORM_DoDocumentJSAction(IntPtr hHandle)
        {
            lock (LockString)
            {
                Imports.FORM_DoDocumentJSAction(hHandle);
            }
        }

        public static void FORM_DoDocumentOpenAction(IntPtr hHandle)
        {
            lock (LockString)
            {
                Imports.FORM_DoDocumentOpenAction(hHandle);
            }
        }

        public static void FPDFDOC_ExitFormFillEnviroument(IntPtr hHandle)
        {
            lock (LockString)
            {
                Imports.FPDFDOC_ExitFormFillEnviroument(hHandle);
            }
        }

        public static void FORM_DoDocumentAAction(IntPtr hHandle, FPDFDOC_AACTION aaType)
        {
            lock (LockString)
            {
                Imports.FORM_DoDocumentAAction(hHandle, aaType);
            }
        }

        public static IntPtr FPDF_LoadPage(IntPtr document, int page_index)
        {
            lock (LockString)
            {
                return Imports.FPDF_LoadPage(document, page_index);
            }
        }

        public static IntPtr FPDFText_LoadPage(IntPtr page)
        {
            lock (LockString)
            {
                return Imports.FPDFText_LoadPage(page);
            }
        }

        public static void FORM_OnAfterLoadPage(IntPtr page, IntPtr _form)
        {
            lock (LockString)
            {
                Imports.FORM_OnAfterLoadPage(page, _form);
            }
        }

        public static void FORM_DoPageAAction(IntPtr page, IntPtr _form, FPDFPAGE_AACTION fPDFPAGE_AACTION)
        {
            lock (LockString)
            {
                Imports.FORM_DoPageAAction(page, _form, fPDFPAGE_AACTION);
            }
        }

        public static double FPDF_GetPageWidth(IntPtr page)
        {
            lock (LockString)
            {
                return Imports.FPDF_GetPageWidth(page);
            }
        }

        public static double FPDF_GetPageHeight(IntPtr page)
        {
            lock (LockString)
            {
                return Imports.FPDF_GetPageHeight(page);
            }
        }

        public static void FORM_OnBeforeClosePage(IntPtr page, IntPtr _form)
        {
            lock (LockString)
            {
                Imports.FORM_OnBeforeClosePage(page, _form);
            }
        }

        public static void FPDFText_ClosePage(IntPtr text_page)
        {
            lock (LockString)
            {
                Imports.FPDFText_ClosePage(text_page);
            }
        }

        public static void FPDF_ClosePage(IntPtr page)
        {
            lock (LockString)
            {
                Imports.FPDF_ClosePage(page);
            }
        }

        public static void FPDF_RenderPage(IntPtr dc, IntPtr page, int start_x, int start_y, int size_x, int size_y, int rotate, FPDF flags)
        {
            lock (LockString)
            {
                Imports.FPDF_RenderPage(dc, page, start_x, start_y, size_x, size_y, rotate, flags);
            }
        }

        public static void FPDF_RenderPageBitmap(IntPtr bitmapHandle, IntPtr page, int start_x, int start_y, int size_x, int size_y, int rotate, FPDF flags)
        {
            lock (LockString)
            {
                Imports.FPDF_RenderPageBitmap(bitmapHandle, page, start_x, start_y, size_x, size_y, rotate, flags);
            }
        }

        public static int FPDF_GetPageSizeByIndex(IntPtr document, int page_index, out double width, out double height)
        {
            lock (LockString)
            {
                return Imports.FPDF_GetPageSizeByIndex(document, page_index, out width, out height);
            }
        }

        public static IntPtr FPDFBitmap_CreateEx(int width, int height, int format, IntPtr first_scan, int stride)
        {
            lock (LockString)
            {
                return Imports.FPDFBitmap_CreateEx(width, height, format, first_scan, stride);
            }
        }

        public static void FPDFBitmap_FillRect(IntPtr bitmapHandle, int left, int top, int width, int height, uint color)
        {
            lock (LockString)
            {
                Imports.FPDFBitmap_FillRect(bitmapHandle, left, top, width, height, color);
            }
        }

        public static IntPtr FPDFBitmap_Destroy(IntPtr bitmapHandle)
        {
            lock (LockString)
            {
                return Imports.FPDFBitmap_Destroy(bitmapHandle);
            }
        }

        public static IntPtr FPDFText_FindStart(IntPtr page, byte[] findWhat, FPDF_SEARCH_FLAGS flags, int start_index)
        {
            lock (LockString)
            {
                return Imports.FPDFText_FindStart(page, findWhat, flags, start_index);
            }
        }

        public static int FPDFText_GetSchResultIndex(IntPtr handle)
        {
            lock (LockString)
            {
                return Imports.FPDFText_GetSchResultIndex(handle);
            }
        }

        public static int FPDFText_GetSchCount(IntPtr handle)
        {
            lock (LockString)
            {
                return Imports.FPDFText_GetSchCount(handle);
            }
        }

        public static int FPDFText_GetText(IntPtr page, int start_index, int count, byte[] result)
        {
            lock (LockString)
            {
                return Imports.FPDFText_GetText(page, start_index, count, result);
            }
        }

        public static void FPDFText_GetCharBox(IntPtr page, int index, out double left, out double right, out double bottom, out double top)
        {
            lock (LockString)
            {
                Imports.FPDFText_GetCharBox(page, index, out left, out right, out bottom, out top);
            }
        }

        public static bool FPDFText_FindNext(IntPtr handle)
        {
            lock (LockString)
            {
                return Imports.FPDFText_FindNext(handle);
            }
        }

        public static void FPDFText_FindClose(IntPtr handle)
        {
            lock (LockString)
            {
                Imports.FPDFText_FindClose(handle);
            }
        }

        private static class Imports
        {
            [DllImport("pdfium.dll")]
            public static extern void FPDF_AddRef();

            [DllImport("pdfium.dll")]
            public static extern void FPDF_Release();

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
            public static extern int FPDF_GetPageSizeByIndex(IntPtr document, int page_index, out double width, out double height);

            [DllImport("pdfium.dll")]
            public static extern IntPtr FPDFBitmap_CreateEx(int width, int height, int format, IntPtr first_scan, int stride);

            [DllImport("pdfium.dll")]
            public static extern void FPDFBitmap_FillRect(IntPtr bitmapHandle, int left, int top, int width, int height, uint color);

            [DllImport("pdfium.dll")]
            public static extern IntPtr FPDFBitmap_Destroy(IntPtr bitmapHandle);

            [DllImport("pdfium.dll")]
            public static extern IntPtr FPDFText_FindStart(IntPtr page, byte[] findWhat, FPDF_SEARCH_FLAGS flags, int start_index);

            [DllImport("pdfium.dll")]
            public static extern int FPDFText_GetSchResultIndex(IntPtr handle);

            [DllImport("pdfium.dll")]
            public static extern int FPDFText_GetSchCount(IntPtr handle);

            [DllImport("pdfium.dll")]
            public static extern int FPDFText_GetText(IntPtr page, int start_index, int count, byte[] result);

            [DllImport("pdfium.dll")]
            public static extern void FPDFText_GetCharBox(IntPtr page, int index, out double left, out double right, out double bottom, out double top);

            [DllImport("pdfium.dll")]
            public static extern bool FPDFText_FindNext(IntPtr handle);

            [DllImport("pdfium.dll")]
            public static extern void FPDFText_FindClose(IntPtr handle);
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

        [Flags]
        public enum FPDF_SEARCH_FLAGS
        {
            FPDF_MATCHCASE = 1,
            FPDF_MATCHWHOLEWORD = 2
        }
    }
}
