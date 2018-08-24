using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

#pragma warning disable 1591

namespace PdfiumViewer
{
    public abstract class PanningZoomingScrollControl : CustomScrollControl
    {
        public const double DefaultZoomMin = 0.1;
        public const double DefaultZoomMax = 5;
        public const double DefaultZoomFactor = 1.2;

        private static readonly Cursor PanCursor;

        static PanningZoomingScrollControl()
        {
            Application.AddMessageFilter(new WheelFilter());

            using (var stream = typeof(PanningZoomingScrollControl).Assembly.GetManifestResourceStream(typeof(PanningZoomingScrollControl).Namespace + ".pan.cur"))
            {
                PanCursor = new Cursor(stream);
            }
        }

        private double _zoom = 1;
        private bool _canPan;
        private Point _dragStart;
        private Point _startOffset;
        private double _zoomMax;
        private double _zoomMin;

        public event EventHandler ZoomChanged;

        protected virtual void OnZoomChanged(EventArgs e)
        {
            var ev = ZoomChanged;

            if (ev != null)
                ev(this, e);
        }

        /// <summary>
        /// Gets or sets the current zoom level.
        /// </summary>
        [Browsable(false)]
        [DefaultValue(1.0)]
        public double Zoom
        {
            get { return _zoom; }
            set
            {
                value = Math.Min(Math.Max(value, ZoomMin), ZoomMax);

                SetZoom(value, null);
            }
        }

        protected virtual void SetZoom(double value, Point? focus)
        {
            _zoom = value;

            OnZoomChanged(EventArgs.Empty);

            Invalidate();
        }

        [DefaultValue(DefaultZoomFactor)]
        public double ZoomFactor { get; set; }

        protected PanningZoomingScrollControl()
        {
            ZoomFactor = DefaultZoomFactor;
            _zoomMin = DefaultZoomMin;
            _zoomMax = DefaultZoomMax;
        }

        [DefaultValue(DefaultZoomMin)]
        public double ZoomMin
        {
            get { return _zoomMin; }
            set
            {
                _zoomMin = value;
                Zoom = Zoom;
            }
        }

        [DefaultValue(DefaultZoomMax)]
        public double ZoomMax
        {
            get { return _zoomMax; }
            set
            {
                _zoomMax = value;
                Zoom = Zoom;
            }
        }

        /// <summary>
        /// Zooms the PDF document in one step.
        /// </summary>
        public void ZoomIn()
        {
            Zoom *= ZoomFactor;
        }

        /// <summary>
        /// Zooms the PDF document out one step.
        /// </summary>
        public void ZoomOut()
        {
            Zoom /= ZoomFactor;
        }

        [DefaultValue(MouseWheelMode.PanAndZoom)]
        public MouseWheelMode MouseWheelMode { get; set; }

        protected bool MousePanningEnabled { get; set; } = true;

        /// <summary>
        /// Raises the <see cref="E:System.Windows.Forms.Control.MouseWheel"/> event.
        /// </summary>
        /// <param name="e">A <see cref="T:System.Windows.Forms.MouseEventArgs"/> that contains the event data. </param>
        protected override void OnMouseWheel(MouseEventArgs e)
        {
            base.OnMouseWheel(e);

            bool doZoom;

            switch (MouseWheelMode)
            {
                case MouseWheelMode.PanAndZoom:
                    doZoom = (ModifierKeys & Keys.Control) != 0;
                    break;
                case MouseWheelMode.Zoom:
                    doZoom = true;
                    break;
                default:
                    doZoom = false;
                    break;
            }

            if (doZoom)
            {
                double zoom = _zoom;

                if (e.Delta > 0)
                    zoom *= ZoomFactor;
                else
                    zoom /= ZoomFactor;

                zoom = Math.Min(Math.Max(zoom, ZoomMin), ZoomMax);

                SetZoom(zoom, e.Location);
            }
            else
            {
                base.OnMouseWheel(e);
            }
        }

        protected abstract Rectangle GetDocumentBounds();

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

                case Keys.Left:
                    PerformScroll(ScrollAction.LineUp, Orientation.Horizontal);
                    return true;

                case Keys.Right:
                    PerformScroll(ScrollAction.LineDown, Orientation.Horizontal);
                    return true;

                case Keys.PageUp:
                    PerformScroll(ScrollAction.PageUp, Orientation.Vertical);
                    return true;

                case Keys.PageDown:
                    PerformScroll(ScrollAction.PageDown, Orientation.Vertical);
                    return true;

                case Keys.Add:
                case Keys.Oemplus:
                    if ((keyData & Keys.Modifiers) == Keys.Control)
                        ZoomIn();
                    return true;

                case Keys.Subtract:
                case Keys.OemMinus:
                    if ((keyData & Keys.Modifiers) == Keys.Control)
                        ZoomOut();
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

        protected override void OnSetCursor(SetCursorEventArgs e)
        {
            if (MousePanningEnabled && _canPan && e.HitTest == HitTest.Client)
                e.Cursor = PanCursor;

            base.OnSetCursor(e);
        }

        protected override void OnLayout(LayoutEventArgs levent)
        {
            _canPan = DisplayRectangle.Width > ClientSize.Width || DisplayRectangle.Height > ClientSize.Height;

            base.OnLayout(levent);
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);

            if (!MousePanningEnabled || e.Button != MouseButtons.Left || !_canPan)
                return;

            Capture = true;
            _dragStart = e.Location;
            _startOffset = DisplayRectangle.Location;
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);

            if (!MousePanningEnabled || !Capture)
                return;

            var offset = new Point(e.Location.X - _dragStart.X, e.Location.Y - _dragStart.Y);

            SetDisplayRectLocation(new Point(_startOffset.X + offset.X, _startOffset.Y + offset.Y));
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);

            if (MousePanningEnabled)
                Capture = false;
        }

        private class WheelFilter : IMessageFilter
        {
            public bool PreFilterMessage(ref Message m)
            {
                if (m.Msg != NativeMethods.WM_MOUSEWHEEL)
                    return false;

                var control = Control.FromHandle(NativeMethods.WindowFromPoint(Cursor.Position));

                while (control != null && !(control is PanningZoomingScrollControl))
                {
                    control = control.Parent;
                }

                if (control == null)
                    return false;

                NativeMethods.SendMessage(control.Handle, m.Msg, m.WParam, m.LParam);
                return true;
            }
        }
    }
}
