using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

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
        public double Percent;
    }

    public enum RegionStatus
    {
        Ok,
        Null,
        WidthNull,
        HeightNull,
        WidthBig,
        HeightBig,
        Conflict
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
            if (rect.Right >= Width || rect.Height >= Height)
                return null;
            double max = -1.0;
            for (int y = rect.Y; y < rect.Bottom; y++)
                for (int x = rect.X; x < rect.Right; x++)
                {
                    if (max < _coords[x, y].Percent)
                        max = _coords[x, y].Percent;
                    if (max >= 1.0)
                        goto exit;
                }
            if (max <= 0)
                return null;
            exit: List<Reg> procs = new List<Reg>();
            for (int y = rect.Y; y < rect.Bottom; y++)
                for (int x = rect.X; x < rect.Right; x++)
                {
                    ProcPerc pp = _coords[x, y];
                    if (Math.Abs(pp.Percent - max) <= DiffEqual)
                        procs.Add(new Reg { Position = new Point(x, y), Procs = pp.Procs, Percent = pp.Percent });
                }
            return procs;
        }

        bool InRange(Region region)
        {
            return region != null && region.Elements.Where(reg => reg != null).All(reg => reg.Right < Width && reg.Bottom < Height);
        }

        public RegionStatus RegionCorrect(Region region)
        {
            if (region == null)
                return RegionStatus.Null;
            if (region.Width <= 0)
                return RegionStatus.WidthNull;
            if (region.Height <= 0)
                return RegionStatus.HeightNull;
            if (region.Width > Width)
                return RegionStatus.WidthBig;
            if (region.Height > Height)
                return RegionStatus.HeightBig;
            return InRange(region) ? RegionStatus.Ok : RegionStatus.Conflict;
        }

        public void FindRegion(Region region)
        {
            if (RegionCorrect(region) != RegionStatus.Ok)
                throw new ArgumentException();
            foreach (Registered reg in region.Elements)
                reg.Register = Find(reg.Region);
        }
    }
}