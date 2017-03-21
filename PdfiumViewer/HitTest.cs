using System;
using System.Collections.Generic;
using System.Text;

#pragma warning disable 1591

namespace PdfiumViewer
{
    public enum HitTest
    {
        Border = NativeMethods.HitTestValues.HTBORDER,
        Bottom = NativeMethods.HitTestValues.HTBOTTOM,
        BottomLeft = NativeMethods.HitTestValues.HTBOTTOMLEFT,
        BottomRight = NativeMethods.HitTestValues.HTBOTTOMRIGHT,
        Caption = NativeMethods.HitTestValues.HTCAPTION,
        Client = NativeMethods.HitTestValues.HTCLIENT,
        CloseButton = NativeMethods.HitTestValues.HTCLOSE,
        Error = NativeMethods.HitTestValues.HTERROR,
        GrowBox = NativeMethods.HitTestValues.HTGROWBOX,
        HelpButton = NativeMethods.HitTestValues.HTHELP,
        HorizontalScroll = NativeMethods.HitTestValues.HTHSCROLL,
        Left = NativeMethods.HitTestValues.HTLEFT,
        MaximizeButton = NativeMethods.HitTestValues.HTMAXBUTTON,
        Menu = NativeMethods.HitTestValues.HTMENU,
        MinimizeButton = NativeMethods.HitTestValues.HTMINBUTTON,
        Nowhere = NativeMethods.HitTestValues.HTNOWHERE,
        Object = NativeMethods.HitTestValues.HTOBJECT,
        Right = NativeMethods.HitTestValues.HTRIGHT,
        SystemMenu = NativeMethods.HitTestValues.HTSYSMENU,
        Top = NativeMethods.HitTestValues.HTTOP,
        TopLeft = NativeMethods.HitTestValues.HTTOPLEFT,
        TopRight = NativeMethods.HitTestValues.HTTOPRIGHT,
        Transparent = NativeMethods.HitTestValues.HTTRANSPARENT,
        VerticalScroll = NativeMethods.HitTestValues.HTVSCROLL
    }
}
