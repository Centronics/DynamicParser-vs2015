using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using DynamicProcessor;

namespace DynamicParser
{
    struct Points
    {
        public Point Pt;
        public SignValue Sign;
    }

    class ContextLine
    {
        readonly List<Points> _points = new List<Points>();
        public int Width { get; }
        public int Height { get; }

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
                {
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
        }

        public Points GetPixel(int x, int y)
        {
            if (x < 0 || y < 0)
                throw new ArgumentException();
            if (x >= Width || y >= Height)
                throw new ArgumentException();
            return _points[Width * y + x];
        }
    }
}