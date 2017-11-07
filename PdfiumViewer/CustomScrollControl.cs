using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
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
        private IScrollManager _scrollManager;
        private ScrollStyle _scrollStyle;
        private bool _disposed;

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

        [DefaultValue(ScrollStyle.Normal)]
        public ScrollStyle ScrollStyle
        {
            get { return _scrollStyle; }
            set
            {
                _scrollStyle = value;

                _scrollManager?.Dispose();

                switch (value)
                {
                    case ScrollStyle.Auto:
                        if (SystemInformation.TerminalServerSession)
                            _scrollManager = new DirectScrollManager(this);
                        else
                            _scrollManager = new SmoothScrollManager(this);
                        break;
                    case ScrollStyle.Normal:
                        _scrollManager = new DirectScrollManager(this);
                        break;
                    case ScrollStyle.Smooth:
                        _scrollManager = new SmoothScrollManager(this);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(value), value, null);
                }
            }
        }

        public CustomScrollControl()
        {
            SetStyle(ControlStyles.ContainerControl, true);
            SetStyle(ControlStyles.AllPaintingInWmPaint, false);

            ScrollStyle = ScrollStyle.Normal;

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
                _scrollManager.ScrollLocation(0, -e.Delta);

                if (e is HandledMouseEventArgs)
                    ((HandledMouseEventArgs)e).Handled = true;
            }
            else if (HScroll)
            {
                _scrollManager.ScrollLocation(-e.Delta, 0);

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
            var style = SetDisplayLocationStyle.None;
            if (!preserveContents)
                style |= SetDisplayLocationStyle.ClearContent;

            _scrollManager.SetLocation(offset.X, offset.Y, style);
        }

        private void SetDisplayRectLocation(int x, int y, SetDisplayLocationStyle style)
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

            if ((xDelta != 0 || yDelta != 0) && IsHandleCreated && (style & SetDisplayLocationStyle.ClearContent) == 0)
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

            if ((style & SetDisplayLocationStyle.LeaveScrollBars) == 0)
                SyncScrollbars();
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

                _scrollManager.SetLocation(x, y, SetDisplayLocationStyle.LeaveScrollBars);
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

            _scrollManager.SetLocation(x, y, SetDisplayLocationStyle.ClearContent | SetDisplayLocationStyle.LeaveScrollBars);

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
                _horizontalScroll.SmallChange = ClientRectangle.Width / 10;

                if (-displayRect.X >= 0 && -displayRect.X < _horizontalScroll.Maximum)
                    _horizontalScroll.Value = -displayRect.X;

                _horizontalScroll.UpdateScrollInfo();
            }

            if (VScroll)
            {
                _verticalScroll.Maximum = displayRect.Height - 1;
                _verticalScroll.LargeChange = ClientRectangle.Height;
                _verticalScroll.SmallChange = ClientRectangle.Height / 10;

                if (-displayRect.Y >= 0 && -displayRect.Y < _verticalScroll.Maximum)
                    _verticalScroll.Value = -displayRect.Y;

                _verticalScroll.UpdateScrollInfo();
            }
        }

        private void WmHScroll(ref Message m)
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
                    _scrollManager.SetLocation(
                        -ScrollThumbPosition(NativeMethods.SB_HORZ),
                        _displayRect.Y,
                        SetDisplayLocationStyle.None
                    );
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

        private void WmVScroll(ref Message m)
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
                    _scrollManager.SetLocation(
                        _displayRect.X,
                        -ScrollThumbPosition(NativeMethods.SB_VERT),
                        SetDisplayLocationStyle.None
                    );
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
                int delta;

                switch (action)
                {
                    case ScrollAction.LineUp:
                        delta = -_horizontalScroll.SmallChange;
                        break;

                    case ScrollAction.LineDown:
                        delta = _horizontalScroll.SmallChange;
                        break;

                    case ScrollAction.PageUp:
                        delta = -_horizontalScroll.LargeChange;
                        break;

                    case ScrollAction.PageDown:
                        delta = _horizontalScroll.LargeChange;
                        break;

                    case ScrollAction.Home:
                        delta = int.MinValue;
                        break;

                    case ScrollAction.End:
                        delta = int.MaxValue;
                        break;

                    default:
                        return;
                }

                _scrollManager.ScrollLocation(delta, 0);
            }
            else
            {
                int delta;

                switch (action)
                {
                    case ScrollAction.LineUp:
                        delta = -_verticalScroll.SmallChange;
                        break;

                    case ScrollAction.LineDown:
                        delta = _verticalScroll.SmallChange;
                        break;

                    case ScrollAction.PageUp:
                        delta = -_verticalScroll.LargeChange;
                        break;

                    case ScrollAction.PageDown:
                        delta = _verticalScroll.LargeChange;
                        break;

                    case ScrollAction.Home:
                        delta = int.MinValue;
                        break;

                    case ScrollAction.End:
                        delta = int.MaxValue;
                        break;

                    default:
                        return;
                }

                _scrollManager.ScrollLocation(0, delta);
            }
        }

        private void WmOnScroll(ref Message m, int oldValue, int value, ScrollOrientation scrollOrientation)
        {
            var type = (ScrollEventType)NativeMethods.Util.LOWORD(m.WParam);

            if (type != ScrollEventType.EndScroll)
                OnScroll(new ScrollEventArgs(type, oldValue, value, scrollOrientation));
        }

        [EditorBrowsable(EditorBrowsableState.Advanced)]
        [SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.UnmanagedCode)]
        protected override void WndProc(ref Message m)
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

        protected override void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (_scrollManager != null)
                {
                    _scrollManager.Dispose();
                    _scrollManager = null;
                }

                _disposed = true;
            }

            base.Dispose(disposing);
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

        private interface IScrollManager : IDisposable
        {
            void SetLocation(int x, int y, SetDisplayLocationStyle style);
            void ScrollLocation(int deltaX, int deltaY);
        }

        private class DirectScrollManager : IScrollManager
        {
            private readonly CustomScrollControl _owner;

            public DirectScrollManager(CustomScrollControl owner)
            {
                _owner = owner;
            }

            public void SetLocation(int x, int y, SetDisplayLocationStyle style)
            {
                _owner.SetDisplayRectLocation(x, y, style);
            }

            public void ScrollLocation(int deltaX, int deltaY)
            {
                SetLocation(_owner._displayRect.X + deltaX, _owner._displayRect.Y + deltaY, SetDisplayLocationStyle.None);
            }

            public void Dispose()
            {
            }
        }

        private class SmoothScrollManager : IScrollManager
        {
            private const int AnimationDuration = 180;
            private const int StepDuration = AnimationDuration / 2;

            private readonly CustomScrollControl _owner;
            private Timer _timer = new Timer();
            private readonly Stopwatch _stopwatch = new Stopwatch();
            private Point _from;
            private ScrollOrientation _direction;
            private int _steps;
            private int _lead;
            private int _travel;
            private int _tail;
#if DEBUG
            private int _animations;
#endif
            private bool _disposed;

            public SmoothScrollManager(CustomScrollControl owner)
            {
                _owner = owner;
                _timer.Interval = 10;
                _timer.Tick += _timer_Tick;
            }

            private void _timer_Tick(object sender, EventArgs e)
            {
                long elapsed = _stopwatch.ElapsedMilliseconds;

                if (elapsed >= _steps * StepDuration)
                {
#if DEBUG
                    double fps = _animations / (elapsed / 1000.0);
                    Console.WriteLine("FPS: " + (int)fps);
#endif

                    Stop(true);
                    return;
                }

                int step = (int)(elapsed / StepDuration);
                int delta;

                if (step == 0)
                    delta = GetDelta(_lead, elapsed, StepDuration, DeltaStage.Lead);
                else if (step < _steps - 1)
                    delta = _lead + GetDelta(_travel, elapsed - StepDuration, (_steps - 2) * StepDuration, DeltaStage.Travel);
                else
                    delta = _lead + _travel + GetDelta(_tail, elapsed - (_steps - 1) * StepDuration, StepDuration, DeltaStage.Tail);

                int x = _from.X;
                int y = _from.Y;

                if (_direction == ScrollOrientation.HorizontalScroll)
                    x -= delta;
                else
                    y -= delta;

                _owner.SetDisplayRectLocation(x, y, SetDisplayLocationStyle.None);
                _owner.Update();

#if DEBUG
                _animations++;
#endif
            }

            private int GetDelta(int delta, long elapsed, long duration, DeltaStage stage)
            {
                // Calculate the delta. If we're in a lead or tail, we use an interpolation function
                // from https://en.wikipedia.org/wiki/Smoothstep. For the travel, we just interpolate.

                switch (stage)
                {
                    case DeltaStage.Lead:
                        return (int)(MathEx.SmoothStep(0, duration * 2, elapsed) * 2 * delta);
                    case DeltaStage.Travel:
                        return (int)(delta * elapsed / duration);
                    case DeltaStage.Tail:
                        return (int)((MathEx.SmoothStep(0, duration * 2, elapsed + duration) - 0.5) * 2 * delta);
                    default:
                        throw new ArgumentOutOfRangeException(nameof(stage), stage, null);
                }
            }

            private void Stop(bool applyDelta)
            {
                if (_stopwatch.IsRunning)
                {
                    _stopwatch.Reset();
                    _timer.Stop();

                    if (applyDelta)
                    {
                        int x = _from.X;
                        int y = _from.Y;

                        var delta = _lead + _travel + _tail;

                        if (_direction == ScrollOrientation.HorizontalScroll)
                            x -= delta;
                        else
                            y -= delta;

                        _owner.SetDisplayRectLocation(x, y, SetDisplayLocationStyle.None);
                    }
                }
            }

            public void SetLocation(int x, int y, SetDisplayLocationStyle style)
            {
                Stop(false);
                _owner.SetDisplayRectLocation(x, y, style);
            }

            public void ScrollLocation(int deltaX, int deltaY)
            {
                // We can't animate both directions.

                if (deltaX != 0 && deltaY != 0)
                {
                    SetLocation(_owner._displayRect.X + deltaX, _owner._displayRect.Y + deltaY, SetDisplayLocationStyle.None);
                    return;
                }

                var direction = deltaX != 0 ? ScrollOrientation.HorizontalScroll : ScrollOrientation.VerticalScroll;
                int delta = deltaX != 0 ? deltaX : deltaY;

                var client = _owner.ClientRectangle;
                int maxX = Math.Max(_owner._displayRect.Width - client.Width, 0);
                int maxY = Math.Max(_owner._displayRect.Height - client.Height, 0);

                // Do we need to add to the currently running animation?

                int currentDelta = _lead + _travel + _tail;

                if (_stopwatch.IsRunning && direction == _direction && Math.Sign(delta) == Math.Sign(currentDelta))
                {
                    if (direction == ScrollOrientation.HorizontalScroll)
                    {
                        if (delta < 0)
                            delta = MathEx.Clamp(delta, _from.X + currentDelta, 0);
                        else
                            delta = MathEx.Clamp(delta, 0, maxX - (-_from.X + currentDelta));
                    }
                    else
                    {
                        if (delta < 0)
                            delta = MathEx.Clamp(delta, _from.Y + currentDelta, 0);
                        else
                            delta = MathEx.Clamp(delta, 0, maxY - (-_from.Y + currentDelta));
                    }

                    // Nothing to scroll?

                    if (delta == 0)
                        return;

                    // Add to the current animation.

                    int half = delta / 2;
                    _travel += _tail + half;
                    _tail = delta - half;
                    _steps += 2;
                }
                else
                {
                    // If we can't update the current animation, or we're starting a new one,
                    // we always start from the current position.

                    _from = _owner._displayRect.Location;

                    if (direction == ScrollOrientation.HorizontalScroll)
                    {
                        if (delta < 0)
                            delta = MathEx.Clamp(delta, _from.X, 0);
                        else
                            delta = MathEx.Clamp(delta, 0, maxX + _from.X);
                    }
                    else
                    {
                        if (delta < 0)
                            delta = MathEx.Clamp(delta, _from.Y, 0);
                        else
                            delta = MathEx.Clamp(delta, 0, maxY + _from.Y);
                    }

                    if (delta == 0)
                        return;

                    _direction = direction;
                    int half = delta / 2;
                    _lead = half;
                    _tail = delta - half;
                    _travel = 0;
                    _steps = 2;
#if DEBUG
                    _animations = 0;
#endif

                    _stopwatch.Reset();
                    _stopwatch.Start();
                    _timer.Start();
                }
            }

            public void Dispose()
            {
                if (!_disposed)
                {
                    Stop(false);

                    if (_timer != null)
                    {
                        _timer.Dispose();
                        _timer = null;
                    }

                    _disposed = true;
                }
            }

            private enum DeltaStage
            {
                Lead,
                Travel,
                Tail
            }
        }

        [Flags]
        private enum SetDisplayLocationStyle
        {
            None = 0,
            ClearContent = 1,
            LeaveScrollBars = 2
        }
    }
}
