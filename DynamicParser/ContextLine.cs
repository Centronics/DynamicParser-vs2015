using System;
using System.Collections.Generic;
using System.Drawing;
using DynamicProcessor;

namespace DynamicParser
{
    public struct Points
    {
        public Point Pt;
        public SignValue Sign;
    }

    public struct Line
    {
        public Points PtStart, PtEnd;

        public Line(Points ptx, Points pty)
        {
            if (ptx.Pt.Y != pty.Pt.Y)
                if (ptx.Pt.X != pty.Pt.X)
                    throw new Exception();
            if (ptx.Pt.X != pty.Pt.X)
                if (ptx.Pt.Y != pty.Pt.Y)
                    throw new Exception();
            PtStart = ptx;
            PtEnd = pty;
        }

        public int Length => PtStart.Pt.Y == PtEnd.Pt.Y
            ? (PtStart.Pt.X > PtEnd.Pt.X ? PtStart.Pt.X - PtEnd.Pt.X : PtEnd.Pt.X - PtStart.Pt.X)
            : (PtStart.Pt.Y > PtEnd.Pt.Y ? PtStart.Pt.Y - PtEnd.Pt.Y : PtEnd.Pt.Y - PtStart.Pt.Y);
    }

    public class ContextLine
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

        public IEnumerable<Line> GetHorizontalLine(SignValue diff)
        {
            if (_lastY >= Height)
            {
                _lastX = _lastY = 0;
                yield break;
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
            yield return new Line(pts, pt.Value);
        }

        public IEnumerable<Line> GetVerticalLine(SignValue diff)
        {
            if (_lastX >= Width)
            {
                _lastX = _lastY = 0;
                yield break;
            }
            Points pts = GetPixel(_lastX, _lastY);
            Points? pt = null;
            for (_lastY++; _lastY < Height; _lastY++)
            {
                pt = GetPixel(_lastX, _lastY);
                if (pts.Sign - pt.Value.Sign > diff)
                    pt = new Points { Sign = pts.Sign - pt.Value.Sign, Pt = pt.Value.Pt };
            }
            if (_lastY >= Height)
            {
                _lastY = 0;
                _lastX++;
            }
            if (pt == null)
                throw new Exception();
            yield return new Line(pts, pt.Value);
        }
    }
}