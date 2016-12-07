using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace DynamicParser
{
    public sealed class Attach
    {
        public struct Proc
        {
            public Point Place;
            public IEnumerable<Processor> Procs;
        }

        public Point Point { get; set; }
        public List<Reg> Regs { get; set; }

        public IEnumerable<Processor> Processors => (from rg in Regs from proc in rg.Procs select proc).ToList();

        public Proc Unique => new Proc { Place = Point, Procs = UniqueMas };

        IEnumerable<Processor> UniqueMas
        {
            get
            {
                if (Regs == null)
                    yield break;
                IEnumerable<Processor> lst = Processors;
                List<Processor> uni = new List<Processor>();
                foreach (Processor pr in lst)
                    if (!Inclusive(uni, pr.Tag))
                    {
                        uni.Add(pr);
                        yield return pr;
                    }
            }
        }

        static bool Inclusive(IEnumerable<Processor> lst, string str)
        {
            return lst != null && lst.Any(s => string.Compare(s.Tag?.Trim() ?? string.Empty, str.Trim(), StringComparison.OrdinalIgnoreCase) == 0);
        }
    }

    public sealed class Attacher
    {
        readonly List<Attach> _attaches = new List<Attach>();

        public int Width { get; }

        public int Height { get; }

        public IEnumerable<Attach> Attaches => _attaches;

        public IEnumerable<Point> Points => _attaches.Select(att => att.Point);

        public Attacher(int width, int height)
        {
            if (width <= 0)
                throw new ArgumentException();
            if (height <= 0)
                throw new ArgumentException();
            Width = width;
            Height = height;
        }

        public static bool IsConflict(Point pt, Rectangle rect)
        {
            return pt.X >= rect.X && pt.X <= rect.Right && pt.Y >= rect.Y && pt.Y <= rect.Bottom;
        }

        public bool IsConflict(Rectangle rect)
        {
            bool one = false;
            foreach (Point pt in Points)
                if (IsConflict(pt, rect))
                    if (one)
                        return true;
                    else
                        one = true;
            return false;
        }

        public bool IsConflict(Region region)
        {
            if (region == null)
                throw new ArgumentNullException();
            return region.Rectangles.Any(IsConflict);
        }

        public void SetMask(Region region)
        {
            if (IsConflict(region))
                throw new ArgumentException();
            foreach (Attach att in Attaches)
                att.Regs = region[att.Point]?.Register;
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

        public void Add(int x, int y)
        {
            Add(new Point(x, y));
        }
    }
}