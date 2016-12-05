using System;
using System.Collections.Generic;
using System.Drawing;

namespace DynamicParser
{
    public sealed class Attach
    {
        public Point Point { get; set; }
        public List<Reg> Regs { get; set; }
    }

    public sealed class Attacher
    {
        readonly List<Attach> _attaches = new List<Attach>();

        public int Width { get; }

        public int Height { get; }

        public IEnumerable<Attach> Attaches => _attaches;

        public Attacher(int width, int height)
        {
            if (width <= 0)
                throw new ArgumentException();
            if (height <= 0)
                throw new ArgumentException();
            Width = width;
            Height = height;
        }

        public bool Contains(Point point)
        {
            return _attaches.Contains(new Attach { Point = point });
        }

        public void Add(Point point)
        {
            if (Contains(point))
                throw new ArgumentException();
            _attaches.Add(new Attach { Point = point });
        }
    }
}