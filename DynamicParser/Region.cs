using System;
using System.Collections.Generic;
using System.Drawing;

namespace DynamicParser
{
    public sealed class Registered
    {
        public Rectangle Region;
        public List<Reg> Register;
    }

    public sealed class Region
    {
        readonly Registered[,] _rects;

        public Registered this[int x, int y] => _rects[x, y];

        public int Width => _rects.GetLength(0);

        public int Height => _rects.GetLength(1);

        public Region(int mx, int my)
        {
            if (mx <= 0)
                throw new ArgumentException();
            if (my <= 0)
                throw new ArgumentException();
            _rects = new Registered[mx, my];
        }

        public void Add(Rectangle rect)
        {
            _rects[rect.X, rect.Y] = new Registered { Region = rect };
        }
    }
}