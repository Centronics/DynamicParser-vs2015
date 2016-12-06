using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace DynamicParser
{
    public sealed class Registered
    {
        public Rectangle Region { get; set; }
        public List<Reg> Register { get; set; }

        public bool IsEmpty => (Register?.Count ?? 0) <= 0;

        public int X => Region.X;

        public int Y => Region.Y;

        public int Right => Region.Right;

        public int Bottom => Region.Bottom;

        public bool IsConflict(Rectangle rect)
        {
            if (rect.X >= X && rect.Right <= Right)
                return true;
            if (rect.Y >= Y && rect.Bottom <= Bottom)
                return true;
            if (rect.Right >= X && rect.Right <= Right)
                return true;
            if (rect.Bottom >= Y && rect.Bottom <= Bottom)
                return true;
            if (rect.X <= X && rect.Right >= Right)
                return true;
            return rect.Y <= Y && rect.Bottom >= Bottom;
        }
    }

    public sealed class Region
    {
        readonly SortedDictionary<ulong, Registered> _rects = new SortedDictionary<ulong, Registered>();

        public int Width { get; }

        public int Height { get; }

        public IEnumerable<Registered> Elements => _rects.Values;

        public IEnumerable<Rectangle> Rectangles => _rects.Values.Select(reg => reg.Region);

        public Registered this[Point pt] => this[pt.X, pt.Y];

        public Region(int width, int height)
        {
            if (width <= 0)
                throw new ArgumentException();
            if (height <= 0)
                throw new ArgumentException();
            Width = width;
            Height = height;
        }

        ulong GetIndex(int x, int y)
        {
            if (x < 0)
                throw new ArgumentException();
            if (y < 0)
                throw new ArgumentException();
            if (x >= Width)
                throw new ArgumentException();
            if (y >= Height)
                throw new ArgumentException();
            return Convert.ToUInt64(Width * y + x);
        }

        public Registered this[int x, int y]
        {
            get
            {
                if (x < 0)
                    throw new ArgumentException();
                if (y < 0)
                    throw new ArgumentException();
                return Elements.FirstOrDefault(reg => x >= reg.X && x <= reg.Right && y >= reg.Y && y <= reg.Bottom);
            }
        }

        public void SetMask(Attacher attacher)
        {
            if (attacher == null)
                throw new ArgumentNullException();
            if (attacher.Width <= 0)
                throw new ArgumentException();
            if (attacher.Height <= 0)
                throw new ArgumentException();
            foreach (Attach attach in attacher.Attaches)
                attach.Regs = this[attach.Point].Register;
        }

        public bool Contains(int x, int y)
        {
            return _rects.ContainsKey(GetIndex(x, y));
        }

        public bool IsConflict(Rectangle rect)
        {
            return _rects.Values.Any(reg => reg.IsConflict(rect));
        }

        public void Add(Rectangle rect)
        {
            if (IsConflict(rect))
                throw new ArgumentException($"{nameof(Add)}: Попытка вставить элемент, конфликтующий с существующими");
            _rects[GetIndex(rect.X, rect.Y)] = new Registered { Region = rect };
        }
    }
}