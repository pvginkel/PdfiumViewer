using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Security.Permissions;
using System.Text;
using System.Windows.Forms;

#pragma warning disable 1591

namespace PdfiumViewer
{
    public class CustomScrollControl : Control
    {
        private Size _displaySize;
        private Rectangle _displayRect;
        private readonly ScrollProperties _verticalScroll;
        private readonly ScrollProperties _horizontalScroll;

        public event ScrollEventHandler Scroll;

        protected virtual void OnScroll(ScrollEventArgs se)
        {
            var ev = Scroll;

            if (ev != null)
                ev(this, se);
        }

        public event EventHandler DisplayRectangleChanged;

        protected virtual void OnDisplayRectangleChanged(EventArgs e)
        {
            var ev = DisplayRectangleChanged;
            if (ev != null)
                ev(this, e);
        }

        public event SetCursorEventHandler SetCursor;

        protected virtual void OnSetCursor(SetCursorEventArgs e)
        {
            var handler = SetCursor;
            if (handler != null)
                handler(this, e);
        }

        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams cp = base.CreateParams;

                if (HScroll || _horizontalScroll.Visible)
                    cp.Style |= NativeMethods.WS_HSCROLL;
                else
                    cp.Style &= (~NativeMethods.WS_HSCROLL);
                if (VScroll || _verticalScroll.Visible)
                    cp.Style |= NativeMethods.WS_VSCROLL;
                else
                    cp.Style &= (~NativeMethods.WS_VSCROLL);

                return cp;
            }
        }

        public override Rectangle DisplayRectangle
        {
            get
            {
                Rectangle rect = ClientRectangle;

                if (!_displayRect.IsEmpty)
                {
                    rect.X = _displayRect.X;
                    rect.Y = _displayRect.Y;

                    if (HScroll)
                        rect.Width = _displayRect.Width;
                    if (VScroll)
                        rect.Height = _displayRect.Height;
                }

                return rect;
            }
        }

        [Browsable(false)]
        public bool HScroll { get; private set; }

        [Browsable(false)]
        public bool VScroll { get; private set; }

        public CustomScrollControl()
        {
            SetStyle(ControlStyles.Selectable, true);
            SetStyle(ControlStyles.UserMouse, true);
            SetStyle(ControlStyles.AllPaintingInWmPaint, false);

            _horizontalScroll = new ScrollProperties(this, NativeMethods.SB_HORZ);
            _verticalScroll = new ScrollProperties(this, NativeMethods.SB_VERT);
        }

        protected void SetDisplaySize(Size size)
        {
            if (_displaySize != size)
            {
                _displaySize = size;

                AdjustFormScrollbars();
            }
        }

        private void AdjustFormScrollbars()
        {
            if (ApplyScrollbarChanges())
                PerformLayout();
        }

        private bool ApplyScrollbarChanges()
        {
            var fullClient = ClientRectangle;
            var minClient = fullClient;

            if (HScroll)
                fullClient.Height += SystemInformation.HorizontalScrollBarHeight;
            else
                minClient.Height -= SystemInformation.HorizontalScrollBarHeight;

            if (VScroll)
                fullClient.Width += SystemInformation.VerticalScrollBarWidth;
            else
                minClient.Width -= SystemInformation.VerticalScrollBarWidth;

            int maxX = _displaySize.Width;
            int maxY = _displaySize.Height;

            // Check maxX/maxY against the clientRect, we must compare it to the
            // clientRect without any scrollbars, and then we can check it against
            // the clientRect with the "new" scrollbars. This will make the 
            // scrollbars show and hide themselves correctly at the boundaries.
            // 
            bool needHscroll = maxX > fullClient.Width;
            bool needVscroll = maxY > fullClient.Height;

            var clientToBe = fullClient;

            if (needHscroll)
                clientToBe.Height -= SystemInformation.HorizontalScrollBarHeight;
            if (needVscroll)
                clientToBe.Width -= SystemInformation.VerticalScrollBarWidth;

            if (needHscroll && maxY > clientToBe.Height)
                needVscroll = true;
            if (needVscroll && maxX > clientToBe.Width)
                needHscroll = true;

            if (!needHscroll)
                maxX = clientToBe.Width;
            if (!needVscroll)
                maxY = clientToBe.Height;

            bool needLayout = SetVisibleScrollbars(needHscroll, needVscroll);

            if (HScroll || VScroll)
                needLayout = (SetDisplayRectangleSize(maxX, maxY) || needLayout);
            else
                SetDisplayRectangleSize(maxX, maxY);

            SyncScrollbars();

            return needLayout;
        }

        protected override void OnLayout(LayoutEventArgs levent)
        {
            AdjustFormScrollbars();

            base.OnLayout(levent);
        }

        protected override void OnMouseWheel(MouseEventArgs e)
        {
            // Favor the vertical scroll bar, since it's the most
            // common use.  However, if there isn't a vertical
            // scroll and the horizontal is on, then wheel it around.

            if (VScroll)
            {
                var client = ClientRectangle;
                int pos = -_displayRect.Y;
                int maxPos = -(client.Height - _displayRect.Height);

                pos = Math.Max(pos - e.Delta, 0);
                pos = Math.Min(pos, maxPos);

                SetDisplayRectLocation(_displayRect.X, -pos);
                SyncScrollbars();

                if (e is HandledMouseEventArgs)
                    ((HandledMouseEventArgs)e).Handled = true;
            }
            else if (HScroll)
            {
                var client = ClientRectangle;
                int pos = -_displayRect.X;
                int maxPos = -(client.Width - _displayRect.Width);

                pos = Math.Max(pos - e.Delta, 0);
                pos = Math.Min(pos, maxPos);

                SetDisplayRectLocation(-pos, _displayRect.Y);
                SyncScrollbars();

                if (e is HandledMouseEventArgs)
                    ((HandledMouseEventArgs)e).Handled = true;
            }

            base.OnMouseWheel(e);
        }

        protected override void OnVisibleChanged(EventArgs e)
        {
            if (Visible)
                PerformLayout();

            base.OnVisibleChanged(e);
        }

        public void SetDisplayRectLocation(Point offset)
        {
            SetDisplayRectLocation(offset, true);
        }

        public void SetDisplayRectLocation(Point offset, bool preserveContents)
        {
            SetDisplayRectLocation(offset.X, offset.Y, preserveContents);
            SyncScrollbars();
        }

        private void SetDisplayRectLocation(int x, int y)
        {
            SetDisplayRectLocation(x, y, true);
        }

        private void SetDisplayRectLocation(int x, int y, bool preserveContents)
        {
            int xDelta = 0;
            int yDelta = 0;

            var client = ClientRectangle;
            var displayRectangle = _displayRect;
            int minX = Math.Min(client.Width - displayRectangle.Width, 0);
            int minY = Math.Min(client.Height - displayRectangle.Height, 0);

            if (x > 0)
                x = 0;
            if (x < minX)
                x = minX;
            if (y > 0)
                y = 0;
            if (y < minY)
                y = minY;

            if (displayRectangle.X != x)
                xDelta = x - displayRectangle.X;
            if (displayRectangle.Y != y)
                yDelta = y - displayRectangle.Y;

            _displayRect.X = x;
            _displayRect.Y = y;

            if ((xDelta != 0 || yDelta != 0) && IsHandleCreated && preserveContents)
            {
                var cr = ClientRectangle;
                var rcClip = new NativeMethods.RECT(cr);
                var rcUpdate = new NativeMethods.RECT(cr);

                NativeMethods.ScrollWindowEx(
                    new HandleRef(this, Handle), xDelta, yDelta,
                    IntPtr.Zero,
                    ref rcClip,
                    IntPtr.Zero,
                    ref rcUpdate,
                    NativeMethods.SW_INVALIDATE | NativeMethods.SW_SCROLLCHILDREN
                );
            }

            OnDisplayRectangleChanged(EventArgs.Empty);
        }

        private int ScrollThumbPosition(int fnBar)
        {
            var si = new NativeMethods.SCROLLINFO
            {
                fMask = NativeMethods.SIF_TRACKPOS
            };

            NativeMethods.GetScrollInfo(new HandleRef(this, Handle), fnBar, si);

            return si.nTrackPos;
        }

        private void ResetScrollProperties(ScrollProperties scrollProperties)
        {
            // Set only these two values as when the ScrollBars are not visible ...
            // there is no meaning of the "value" property.

            scrollProperties.Visible = false;
            scrollProperties.Value = 0;
        }

        private bool SetVisibleScrollbars(bool horiz, bool vert)
        {
            bool needLayout = !horiz && HScroll || horiz && !HScroll || !vert && VScroll || vert && !VScroll;

            if (needLayout)
            {
                int x = _displayRect.X;
                int y = _displayRect.Y;

                if (!horiz)
                    x = 0;
                if (!vert)
                    y = 0;

                SetDisplayRectLocation(x, y);
                HScroll = horiz;
                VScroll = vert;

                //Update the visible member of ScrollBars....
                if (horiz)
                    _horizontalScroll.Visible = true;
                else
                    ResetScrollProperties(_horizontalScroll);
                if (vert)
                    _verticalScroll.Visible = true;
                else
                    ResetScrollProperties(_verticalScroll);

                UpdateStyles();
            }

            return needLayout;
        }

        private bool SetDisplayRectangleSize(int width, int height)
        {
            bool needLayout = false;

            double hScale = (double)width / _displayRect.Height;
            double vScale = (double)height / _displayRect.Height;

            if (_displayRect.Width != width || _displayRect.Height != height)
            {
                _displayRect.Width = width;
                _displayRect.Height = height;

                needLayout = true;
            }

            int minX = ClientRectangle.Width - width;
            int minY = ClientRectangle.Height - height;

            if (minX > 0)
                minX = 0;
            if (minY > 0)
                minY = 0;

            int x = (int)(_displayRect.X * hScale);
            int y = (int)(_displayRect.Y * vScale);

            if (!HScroll || x > 0)
                x = 0;
            if (!VScroll || y > 0)
                y = 0;

            if (x < minX)
                x = minX;
            if (y < minY)
                y = minY;

            SetDisplayRectLocation(x, y);

            return needLayout;
        }

        private void SyncScrollbars()
        {
            Rectangle displayRect = _displayRect;

            if (!IsHandleCreated)
                return;

            if (HScroll)
            {
                _horizontalScroll.Maximum = displayRect.Width - 1;
                _horizontalScroll.LargeChange = ClientRectangle.Width;
                _horizontalScroll.SmallChange = 5;

                if (-displayRect.X >= 0 && -displayRect.X < _horizontalScroll.Maximum)
                    _horizontalScroll.Value = -displayRect.X;

                _horizontalScroll.UpdateScrollInfo();
            }

            if (VScroll)
            {
                _verticalScroll.Maximum = displayRect.Height - 1;
                _verticalScroll.LargeChange = ClientRectangle.Height;
                _verticalScroll.SmallChange = 5;

                if (-displayRect.Y >= 0 && -displayRect.Y < _verticalScroll.Maximum)
                    _verticalScroll.Value = -displayRect.Y;

                _verticalScroll.UpdateScrollInfo();
            }
        }

        private void WmHScroll(ref System.Windows.Forms.Message m)
        {
            if (m.LParam != IntPtr.Zero)
            {
                base.WndProc(ref m);
                return;
            }

            int pos = -_displayRect.X;
            int oldValue = pos;

            switch (NativeMethods.Util.LOWORD(m.WParam))
            {
                case NativeMethods.SB_THUMBPOSITION:
                case NativeMethods.SB_THUMBTRACK:
                    SetDisplayRectLocation(
                        -ScrollThumbPosition(NativeMethods.SB_HORZ),
                        _displayRect.Y
                    );
                    SyncScrollbars();
                    break;

                case NativeMethods.SB_LINEUP:
                    PerformScroll(ScrollAction.LineUp, Orientation.Horizontal);
                    break;

                case NativeMethods.SB_LINEDOWN:
                    PerformScroll(ScrollAction.LineDown, Orientation.Horizontal);
                    break;

                case NativeMethods.SB_PAGEUP:
                    PerformScroll(ScrollAction.PageUp, Orientation.Horizontal);
                    break;

                case NativeMethods.SB_PAGEDOWN:
                    PerformScroll(ScrollAction.PageDown, Orientation.Horizontal);
                    break;

                case NativeMethods.SB_LEFT:
                    PerformScroll(ScrollAction.Home, Orientation.Horizontal);
                    break;

                case NativeMethods.SB_RIGHT:
                    PerformScroll(ScrollAction.End, Orientation.Horizontal);
                    break;
            }

            WmOnScroll(ref m, oldValue, pos, ScrollOrientation.HorizontalScroll);
        }

        private void WmVScroll(ref System.Windows.Forms.Message m)
        {
            if (m.LParam != IntPtr.Zero)
            {
                base.WndProc(ref m);
                return;
            }

            int pos = -_displayRect.Y;
            int oldValue = pos;

            switch (NativeMethods.Util.LOWORD(m.WParam))
            {
                case NativeMethods.SB_THUMBPOSITION:
                case NativeMethods.SB_THUMBTRACK:
                    SetDisplayRectLocation(
                        _displayRect.X,
                        -ScrollThumbPosition(NativeMethods.SB_VERT)
                    );
                    SyncScrollbars();
                    break;

                case NativeMethods.SB_LINEUP:
                    PerformScroll(ScrollAction.LineUp, Orientation.Vertical);
                    break;

                case NativeMethods.SB_LINEDOWN:
                    PerformScroll(ScrollAction.LineDown, Orientation.Vertical);
                    break;

                case NativeMethods.SB_PAGEUP:
                    PerformScroll(ScrollAction.PageUp, Orientation.Vertical);
                    break;

                case NativeMethods.SB_PAGEDOWN:
                    PerformScroll(ScrollAction.PageDown, Orientation.Vertical);
                    break;

                case NativeMethods.SB_TOP:
                    PerformScroll(ScrollAction.Home, Orientation.Vertical);
                    break;

                case NativeMethods.SB_BOTTOM:
                    PerformScroll(ScrollAction.End, Orientation.Vertical);
                    break;
            }

            WmOnScroll(ref m, oldValue, pos, ScrollOrientation.VerticalScroll);
        }

        public void PerformScroll(ScrollAction action, Orientation scrollBar)
        {
            if (scrollBar == Orientation.Horizontal)
            {
                int pos = -_displayRect.X;
                int maxPos = _horizontalScroll.Maximum;

                switch (action)
                {
                    case ScrollAction.LineUp:
                        if (pos > _horizontalScroll.SmallChange)
                            pos -= _horizontalScroll.SmallChange;
                        else
                            pos = 0;
                        break;

                    case ScrollAction.LineDown:
                        if (pos < maxPos - _horizontalScroll.SmallChange)
                            pos += _horizontalScroll.SmallChange;
                        else
                            pos = maxPos;
                        break;

                    case ScrollAction.PageUp:
                        if (pos > _horizontalScroll.LargeChange)
                            pos -= _horizontalScroll.LargeChange;
                        else
                            pos = 0;
                        break;

                    case ScrollAction.PageDown:
                        if (pos < maxPos - _horizontalScroll.LargeChange)
                            pos += _horizontalScroll.LargeChange;
                        else
                            pos = maxPos;
                        break;

                    case ScrollAction.Home:
                        pos = 0;
                        break;

                    case ScrollAction.End:
                        pos = maxPos;
                        break;
                }

                SetDisplayRectLocation(-pos, _displayRect.Y);
            }
            else
            {
                int pos = -_displayRect.Y;
                int maxPos = _verticalScroll.Maximum;

                switch (action)
                {
                    case ScrollAction.LineUp:
                        if (pos > 0)
                            pos -= _verticalScroll.SmallChange;
                        else
                            pos = 0;
                        break;

                    case ScrollAction.LineDown:
                        if (pos < maxPos - _verticalScroll.SmallChange)
                            pos += _verticalScroll.SmallChange;
                        else
                            pos = maxPos;
                        break;

                    case ScrollAction.PageUp:
                        if (pos > _verticalScroll.LargeChange)
                            pos -= _verticalScroll.LargeChange;
                        else
                            pos = 0;
                        break;

                    case ScrollAction.PageDown:
                        if (pos < maxPos - _verticalScroll.LargeChange)
                            pos += _verticalScroll.LargeChange;
                        else
                            pos = maxPos;
                        break;

                    case ScrollAction.Home:
                        pos = 0;
                        break;

                    case ScrollAction.End:
                        pos = maxPos;
                        break;
                }

                SetDisplayRectLocation(_displayRect.X, -pos);
            }

            SyncScrollbars();
        }

        private void WmOnScroll(ref System.Windows.Forms.Message m, int oldValue, int value, ScrollOrientation scrollOrientation)
        {
            var type = (ScrollEventType)NativeMethods.Util.LOWORD(m.WParam);

            if (type != ScrollEventType.EndScroll)
                OnScroll(new ScrollEventArgs(type, oldValue, value, scrollOrientation));
        }

        [EditorBrowsable(EditorBrowsableState.Advanced)]
        [SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.UnmanagedCode)]
        protected override void WndProc(ref System.Windows.Forms.Message m)
        {
            switch (m.Msg)
            {
                case NativeMethods.WM_VSCROLL:
                    WmVScroll(ref m);
                    break;

                case NativeMethods.WM_HSCROLL:
                    WmHScroll(ref m);
                    break;

                case NativeMethods.WM_SETCURSOR:
                    WmSetCursor(ref m);
                    break;


                default:
                    base.WndProc(ref m);
                    break;
            }
        }

        private void WmSetCursor(ref Message m)
        {
            var e = new SetCursorEventArgs(
                PointToClient(Cursor.Position),
                (HitTest)(m.LParam.ToInt32() & 0xffff)
            );
            OnSetCursor(e);
            Cursor.Current = e.Cursor ?? Cursor;
        }

        /// <summary>
        /// Determines whether the specified key is a regular input key or a special key that requires preprocessing.
        /// </summary>
        /// <returns>
        /// true if the specified key is a regular input key; otherwise, false.
        /// </returns>
        /// <param name="keyData">One of the <see cref="T:System.Windows.Forms.Keys"/> values. </param>
        protected override bool IsInputKey(Keys keyData)
        {
            switch ((keyData) & Keys.KeyCode)
            {
                case Keys.Up:
                    PerformScroll(ScrollAction.LineUp, Orientation.Vertical);
                    return true;

                case Keys.Down:
                    PerformScroll(ScrollAction.LineDown, Orientation.Vertical);
                    return true;

                case Keys.PageUp:
                    PerformScroll(ScrollAction.PageUp, Orientation.Vertical);
                    return true;

                case Keys.PageDown:
                    PerformScroll(ScrollAction.PageDown, Orientation.Vertical);
                    return true;

                case Keys.Home:
                    PerformScroll(ScrollAction.Home, Orientation.Vertical);
                    return true;

                case Keys.End:
                    PerformScroll(ScrollAction.End, Orientation.Vertical);
                    return true;

                default:
                    return base.IsInputKey(keyData);
            }
        }

        private class ScrollProperties
        {
            private int _smallChange = 1;
            private int _largeChange = 10;
            private readonly int _orientation;
            private readonly CustomScrollControl _parentControl;

            public ScrollProperties(CustomScrollControl container, int orientation)
            {
                _parentControl = container;
                _orientation = orientation;

                Maximum = 100;
            }

            public int Maximum { get; set; }

            public int Value { get; set; }

            public bool Visible { get; set; }

            public int LargeChange
            {
                get { return Math.Min(_largeChange, Maximum + 1); }
                set { _largeChange = value; }
            }

            public int SmallChange
            {
                get { return Math.Min(_smallChange, LargeChange); }
                set { _smallChange = value; }
            }

            public void UpdateScrollInfo()
            {
                if (!_parentControl.IsHandleCreated || !Visible)
                    return;

                var si = new NativeMethods.SCROLLINFO
                {
                    cbSize = Marshal.SizeOf(typeof(NativeMethods.SCROLLINFO)),
                    fMask = NativeMethods.SIF_ALL,
                    nMin = 0,
                    nMax = Maximum,
                    nPage = LargeChange,
                    nPos = Value,
                    nTrackPos = 0
                };

                NativeMethods.SetScrollInfo(new HandleRef(_parentControl, _parentControl.Handle), _orientation, si, true);
            }
        }
    }
}
