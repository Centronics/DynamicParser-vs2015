using System;
using System.Collections.Generic;
using System.Drawing;
using DynamicProcessor;

namespace DynamicParser
{
    struct Points
    {
        public Point Pt;
        public SignValue Sign;
    }

    struct Line
    {
        public Points PtX, PtY;

        public Line(Points ptx, Points pty)
        {
            if (ptx.Pt.Y != pty.Pt.Y)
                if (ptx.Pt.X != pty.Pt.X)
                    throw new Exception();
            if (ptx.Pt.X != pty.Pt.X)
                if (ptx.Pt.Y != pty.Pt.Y)
                    throw new Exception();
            PtX = ptx;
            PtY = pty;
        }

        public int Length => PtX.Pt.Y == PtY.Pt.Y
            ? (PtX.Pt.X > PtY.Pt.X ? PtX.Pt.X - PtY.Pt.X : PtY.Pt.X - PtX.Pt.X)
            : (PtX.Pt.Y > PtY.Pt.Y ? PtX.Pt.Y - PtY.Pt.Y : PtY.Pt.Y - PtX.Pt.Y);
    }

    class ContextLine
    {
        readonly List<Points> _points = new List<Points>();
        public int Width { get; }
        public int Height { get; }
        int _lastY = -1, _lastX = -1;

        public ContextLine(Bitmap btm)
        {
            if (btm == null)
                throw new ArgumentNullException();
            if (btm.Height <= 0 || btm.Width <= 0)
                throw new ArgumentException();
            Width = btm.Width;
            Height = btm.Height;
            for (int y = 0; y < btm.Height; y++)
                for (int x = 0; x < btm.Width; x++)
                    _points.Add(new Points
                    {
                        Sign = new SignValue(btm.GetPixel(x, y)),
                        Pt = new Point
                        {
                            X = x,
                            Y = y
                        }
                    });
        }

        public Points GetPixel(int x, int y)
        {
            if (x < 0 || y < 0)
                throw new ArgumentException();
            if (x >= Width || y >= Height)
                throw new ArgumentException();
            return _points[Width * y + x];
        }

        public Line? GetLine(SignValue diff)
        {
            if (_lastY >= Height)
            {
                _lastX = _lastY = 0;
                return null;
            }
            Points pts = GetPixel(_lastX, _lastY);
            Points? pt = null;
            for (_lastX++; _lastX < Width; _lastX++)
            {
                pt = GetPixel(_lastX, _lastY);
                if (pts.Sign - pt.Value.Sign > diff)
                    pt = new Points { Sign = pts.Sign - pt.Value.Sign, Pt = pt.Value.Pt };
            }
            if (_lastX >= Width)
            {
                _lastX = 0;
                _lastY++;
            }
            if (pt == null)
                throw new Exception();
            return new Line(pts, pt.Value);
        }
    }
}