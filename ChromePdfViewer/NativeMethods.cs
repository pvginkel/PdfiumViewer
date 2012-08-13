using System;
using System.Collections.Generic;
using System.Drawing;
using System.Runtime.ConstrainedExecution;
using System.Runtime.InteropServices;
using System.Security.Permissions;
using System.Text;
using Microsoft.Win32.SafeHandles;

namespace ChromePdfViewer
{
    internal static class NativeMethods
    {
        [DllImport("pdf.dll", SetLastError = true)]
        public static extern bool RenderPDFPageToDC(byte[] pdfBuffer, int bufferSize, int pageNumber, IntPtr dc, int dpiX, int dpiY, int boundsOriginX, int boundsOriginY, int boundsWidth, int boundsHeight, bool fitToBounds, bool stretchToBounds, bool keepAspectRation, bool centerInBounds, bool autoRotate);

        [DllImport("pdf.dll", SetLastError = true)]
        public static extern bool RenderPDFPageToDC(SafeHandle pdfBuffer, int bufferSize, int pageNumber, IntPtr dc, int dpiX, int dpiY, int boundsOriginX, int boundsOriginY, int boundsWidth, int boundsHeight, bool fitToBounds, bool stretchToBounds, bool keepAspectRation, bool centerInBounds, bool autoRotate);

        [DllImport("pdf.dll", SetLastError = true)]
        public static extern bool GetPDFDocInfo(byte[] pdfBuffer, int bufferSize, out int pageCount, out double maxPageWidth);

        [DllImport("pdf.dll", SetLastError = true)]
        public static extern bool GetPDFDocInfo(SafeHandle pdfBuffer, int bufferSize, out int pageCount, out double maxPageWidth);

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

        /// <summary>
        /// Scroll children within *lprcScroll.
        /// </summary>
        public const uint SW_SCROLLCHILDREN = 0x0001;
        /// <summary>
        /// Invalidate after scrolling.
        /// </summary>
        public const uint SW_INVALIDATE = 0x0002;
        /// <summary>
        /// If SW_INVALIDATE, don't send WM_ERASEBACKGROUND.
        /// </summary>
        public const uint SW_ERASE = 0x0004;
        /// <summary>
        /// Use smooth scrolling.
        /// </summary>
        public const uint SW_SMOOTHSCROLL = 0x0010;

        [StructLayout(LayoutKind.Sequential)]
        public struct RECT
        {
            private int _left;
            private int _top;
            private int _right;
            private int _bottom;

            public RECT(RECT rect)
                : this(rect.Left, rect.Top, rect.Right, rect.Bottom)
            {
            }

            public RECT(int left, int top, int right, int bottom)
            {
                _left = left;
                _top = top;
                _right = right;
                _bottom = bottom;
            }

            public int X
            {
                get { return _left; }
                set { _left = value; }
            }

            public int Y
            {
                get { return _top; }
                set { _top = value; }
            }

            public int Left
            {
                get { return _left; }
                set { _left = value; }
            }

            public int Top
            {
                get { return _top; }
                set { _top = value; }
            }

            public int Right
            {
                get { return _right; }
                set { _right = value; }
            }

            public int Bottom
            {
                get { return _bottom; }
                set { _bottom = value; }
            }

            public int Height
            {
                get { return _bottom - _top; }
                set { _bottom = value + _top; }
            }

            public int Width
            {
                get { return _right - _left; }
                set { _right = value + _left; }
            }

            public Point Location
            {
                get { return new Point(Left, Top); }
                set
                {
                    _left = value.X;
                    _top = value.Y;
                }
            }

            public Size Size
            {
                get { return new Size(Width, Height); }
                set
                {
                    _right = value.Width + _left;
                    _bottom = value.Height + _top;
                }
            }

            public static implicit operator Rectangle(RECT Rectangle)
            {
                return new Rectangle(Rectangle.Left, Rectangle.Top, Rectangle.Width, Rectangle.Height);
            }

            public static implicit operator RECT(Rectangle Rectangle)
            {
                return new RECT(Rectangle.Left, Rectangle.Top, Rectangle.Right, Rectangle.Bottom);
            }

            public static bool operator ==(RECT Rectangle1, RECT Rectangle2)
            {
                return Rectangle1.Equals(Rectangle2);
            }

            public static bool operator !=(RECT Rectangle1, RECT Rectangle2)
            {
                return !Rectangle1.Equals(Rectangle2);
            }

            public override int GetHashCode()
            {
                return ToString().GetHashCode();
            }

            public bool Equals(RECT other)
            {
                return other._left == _left && other._top == _top && other._right == _right && other._bottom == _bottom;
            }

            public override bool Equals(object obj)
            {
                if (obj is RECT)
                {
                    return Equals((RECT)obj);
                }
                else if (obj is Rectangle)
                {
                    return Equals(new RECT((Rectangle)obj));
                }

                return false;
            }
        }


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
    }
}
