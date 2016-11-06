using System;
using System.Collections.Generic;
using System.Drawing;
using DynamicProcessor;

namespace DynamicParser
{
    public sealed class Processor
    {
        sealed class Tpoint
        {
            public SignValue Value;
            public List<int> Number;
        }

        public struct ProcCount
        {
            public uint Count;
            public Processor Proc;
        }

        public class ProcStruct
        {
            public int X { get; }
            public int Y { get; }
            public List<ProcCount> Procs { get; } = new List<ProcCount>();
            public ProcCount MaxElement => Procs[MaxIndex];
            public uint MaxItemCount => Procs[MaxIndex].Count;
            public Processor MaxItemProcessor => Procs[MaxIndex].Proc;

            public ProcStruct(int x, int y)
            {
                if (x < 0)
                    throw new ArgumentException();
                if (y < 0)
                    throw new ArgumentException();
                X = x;
                Y = y;
            }

            public int MaxIndex
            {
                get
                {
                    if (Procs == null)
                        return -1;
                    if (Procs.Count <= 0)
                        return -1;
                    int t = 0;
                    for (int k = 0; k < Procs.Count; k++)
                        if (Procs[k].Count > Procs[t].Count)
                            t = k;
                    return t;
                }
            }
        }

        readonly SignValue[,] _bitmap;
        readonly List<Processor> _lstProcs = new List<Processor>();

        public int Width => _bitmap.GetLength(0);

        public int Height => _bitmap.GetLength(1);

        public int Length => Width * Height;

        public SignValue GetSignValue(int x, int y)
        {
            return _bitmap[x, y];
        }

        public Processor(Bitmap btm)
        {
            if (btm == null)
                throw new ArgumentNullException();
            if (btm.Width <= 0 || btm.Height <= 0)
                throw new ArgumentException();
            _bitmap = new SignValue[btm.Width, btm.Height];
            for (int y = 0; y < btm.Height; y++)
                for (int x = 0; x < btm.Width; x++)
                    _bitmap[x, y] = new SignValue(btm.GetPixel(x, y));
        }

        public void Add(Processor proc)
        {
            if (proc == null)
                throw new ArgumentNullException();
            if (proc.Length <= 0)
                throw new ArgumentException();
            if (proc.Width > Width)
                throw new ArgumentException();
            if (proc.Height > Height)
                throw new ArgumentException();
            _lstProcs.Add(proc);
        }

        static void Sort(IList<ProcStruct> lst)
        {
            if (lst == null)
                throw new ArgumentNullException();
            if (lst.Count < 1)
                return;
            for (int j = 0; j < lst.Count; j++)
                for (int k = 0; k < lst.Count; k++)
                    if (lst[k].MaxItemCount > lst[j].MaxItemCount)
                        lst[j] = lst[k];
        }

        public IEnumerable<ProcStruct> GetEqual()
        {
            Tpoint[,] signValues = new Tpoint[Width, Height];
            for (int j = 0; j < _lstProcs.Count; j++)
            {
                Processor ps = _lstProcs[j];
                for (int y = 0; y < ps.Height; y++)
                    for (int x = 0; x < ps.Width; x++)
                    {
                        if (ps.Width < Width - x || ps.Height < Height - y)
                        {
                            y = int.MaxValue;
                            break;
                        }
                        Tpoint tp = signValues[x, y];
                        SignValue val = _bitmap[x, y] - ps.GetSignValue(x, y);
                        if (tp != null)
                        {
                            if (val > tp.Value) continue;
                            tp.Value = val;
                            tp.Number.Add(j);
                            continue;
                        }
                        signValues[x, y] = new Tpoint
                        {
                            Value = val,
                            Number = new List<int> { j }
                        };
                    }
            }
            ProcStruct[] lst = new ProcStruct[_lstProcs.Count];
            for (int x = 0; x < Width; x++)
                for (int y = 0; y < Height; y++)
                    for (int j = 0; j < _lstProcs.Count; j++)
                    {
                        if (Width - x < _lstProcs[j].Width || Height - y < _lstProcs[j].Height)
                            continue;
                        uint count = 0;
                        for (int x1 = x; x1 < Width; x1++)
                            for (int y1 = y; y1 < Height; y1++)
                                if (signValues[x, y].Number.Contains(j))
                                    count++;
                        if (lst[j] == null)
                            lst[j] = new ProcStruct(x, y);
                        lst[j].Procs.Add(new ProcCount { Count = count, Proc = _lstProcs[j] });
                    }
            Sort(lst);
            foreach (ProcStruct t in lst)
                yield return t;
        }
    }
}