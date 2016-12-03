using System;
using System.Collections.Generic;
using System.Drawing;

namespace DynamicParser
{
    public struct ProcPerc
    {
        public double Percent;
        public Processor[] Procs;
    }

    public struct Reg
    {
        public Point Position;
        public Processor[] Procs;
    }

    public sealed class SearchResults
    {
        const double DiffEqual = 0.01;

        readonly ProcPerc[,] _coords;

        public int Width => _coords.GetLength(0);

        public int Height => _coords.GetLength(1);

        public SearchResults(int mx, int my)
        {
            if (mx <= 0)
                throw new ArgumentException();
            if (my <= 0)
                throw new ArgumentException();
            _coords = new ProcPerc[mx, my];
        }

        public ProcPerc this[int x, int y]
        {
            get { return _coords[x, y]; }
            set { _coords[x, y] = value; }
        }

        List<Reg> Find(Rectangle rect)
        {
            if (rect.Width > Width)
                throw new ArgumentException();
            if (rect.Height > Height)
                throw new ArgumentException();
            double max = -1.0;
            for (int y = rect.Y; y < rect.Bottom; y++)
                for (int x = rect.X; x < rect.Right; x++)
                {
                    if (max < _coords[x, y].Percent)
                        max = _coords[x, y].Percent;
                    if (max >= 1.0)
                        goto exit;
                }
            exit: List<Reg> procs = new List<Reg>();
            for (int y = rect.Y; y < rect.Bottom; y++)
                for (int x = rect.X; x < rect.Right; x++)
                {
                    ProcPerc pp = _coords[x, y];
                    if (Math.Abs(pp.Percent - max) <= DiffEqual)
                        procs.Add(new Reg { Position = new Point(x, y), Procs = pp.Procs });
                }
            return procs;
        }

        public void FindRegion(Region region)
        {
            if (region == null)
                throw new ArgumentNullException();
            for (int y = 0; y < region.Height; y++)
                for (int x = 0; x < region.Width; x++)
                {
                    Registered reg = region[x, y];
                    if (reg == null)
                        continue;
                    reg.Register = Find(reg.Region);
                }
        }
    }
}