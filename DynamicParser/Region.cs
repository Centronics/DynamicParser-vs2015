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

        public int MaxRight { get; private set; }

        public int MaxBottom { get; private set; }

        public IEnumerable<Registered> Elements => _rects.Values;

        public Region(int mx, int my)
        {
            if (mx <= 0)
                throw new ArgumentException();
            if (my <= 0)
                throw new ArgumentException();
            Width = mx;
            Height = my;
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
                Registered reg;
                return _rects.TryGetValue(GetIndex(x, y), out reg) ? reg : null;
            }
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
            if (MaxRight < rect.Right)
                MaxRight = rect.Right;
            if (MaxBottom < rect.Bottom)
                MaxBottom = rect.Bottom;
            _rects[GetIndex(rect.X, rect.Y)] = new Registered { Region = rect };
        }
    }
}