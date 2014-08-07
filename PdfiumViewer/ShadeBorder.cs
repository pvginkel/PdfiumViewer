using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Text;
using System.Windows.Forms;

namespace PdfiumViewer
{
    internal class ShadeBorder : IDisposable
    {
        public static readonly Padding Size = new Padding(4);

        private bool _disposed;
        private TextureBrush _n = new TextureBrush(Properties.Resources.ShadeBorder_N, WrapMode.Tile);
        private Image _ne = Properties.Resources.ShadeBorder_NE;
        private TextureBrush _e = new TextureBrush(Properties.Resources.ShadeBorder_E, WrapMode.Tile);
        private Image _se = Properties.Resources.ShadeBorder_SE;
        private TextureBrush _s = new TextureBrush(Properties.Resources.ShadeBorder_S, WrapMode.Tile);
        private Image _sw = Properties.Resources.ShadeBorder_SW;
        private TextureBrush _w = new TextureBrush(Properties.Resources.ShadeBorder_W, WrapMode.Tile);
        private Image _nw = Properties.Resources.ShadeBorder_NW;

        public void Draw(Graphics graphics, Rectangle bounds)
        {
            if (_disposed)
                throw new ObjectDisposedException(GetType().Name);
            if (graphics == null)
                throw new ArgumentNullException("graphics");

            _n.ResetTransform();
            _n.TranslateTransform(0, bounds.Top - Size.Top);

            graphics.FillRectangle(
                _n,
                bounds.Left + (_nw.Width - Size.Left),
                bounds.Top - Size.Top,
                bounds.Width - (_nw.Width - Size.Left) - (_nw.Width - Size.Right),
                Size.Top
            );

            _e.ResetTransform();
            _e.TranslateTransform(bounds.Right, 0);

            graphics.FillRectangle(
                _e,
                bounds.Right,
                bounds.Top + (_ne.Height - Size.Top),
                Size.Right,
                bounds.Height - (_ne.Height - Size.Top) - (_se.Height - Size.Bottom)
            );

            _s.ResetTransform();
            _s.TranslateTransform(0, bounds.Bottom);

            graphics.FillRectangle(
                _s,
                bounds.Left + (_sw.Width - Size.Left),
                bounds.Bottom,
                bounds.Width - (_sw.Width - Size.Left) - (_sw.Width - Size.Right),
                Size.Bottom
            );

            _w.ResetTransform();
            _w.TranslateTransform(bounds.Left - Size.Left, 0);

            graphics.FillRectangle(
                _w,
                bounds.Left - Size.Left,
                bounds.Top + (_nw.Height - Size.Top),
                Size.Left,
                bounds.Height - (_nw.Height - Size.Top) - (_sw.Height - Size.Bottom)
            );

            graphics.DrawImageUnscaled(
                _ne,
                (bounds.Right + Size.Right) - _ne.Width,
                bounds.Top - Size.Top
            );

            graphics.DrawImageUnscaled(
                _se,
                (bounds.Right + Size.Right) - _se.Width,
                (bounds.Bottom + Size.Bottom) - _se.Height
            );

            graphics.DrawImageUnscaled(
                _sw,
                bounds.Left - Size.Left,
                (bounds.Bottom + Size.Bottom) - _sw.Height
            );

            graphics.DrawImageUnscaled(
                _nw,
                bounds.Left - Size.Left,
                bounds.Top - Size.Top
            );
        }

        public void Dispose()
        {
            if (!_disposed)
            {
                if (_n != null)
                {
                    _n.Dispose();
                    _n = null;
                }

                if (_ne != null)
                {
                    _ne.Dispose();
                    _ne = null;
                }

                if (_e != null)
                {
                    _e.Dispose();
                    _e = null;
                }

                if (_se != null)
                {
                    _se.Dispose();
                    _se = null;
                }

                if (_s != null)
                {
                    _s.Dispose();
                    _s = null;
                }

                if (_sw != null)
                {
                    _sw.Dispose();
                    _sw = null;
                }

                if (_w != null)
                {
                    _w.Dispose();
                    _w = null;
                }

                if (_nw != null)
                {
                    _nw.Dispose();
                    _nw = null;
                }

                _disposed = true;
            }
        }
    }
}
