using System;
using System.Collections.Generic;
using System.Drawing;
using DynamicProcessor;

namespace DynamicParser
{
    [Serializable]
    public sealed class Processor
    {
        sealed class Tpoint
        {
            public SignValue Value;
            public List<int> Number;
        }

        public struct ProcStruct
        {
            public Processor Proc;
            public int X, Y;
        }

        struct Equal
        {
            public ProcStruct Proc;
            public int Count;
        }

        public int Width => _bitmap.GetLength(0);

        public int Height => _bitmap.GetLength(1);

        public int Length => Width * Height;

        readonly SignValue?[,] _bitmap;
        readonly List<ProcStruct> _lstProcs = new List<ProcStruct>();

        public SignValue? GetSignValue(int x, int y)
        {
            return _bitmap[x, y];
        }

        public Processor(Bitmap btm, Color? color = null)
        {
            if (btm == null)
                throw new ArgumentNullException();
            if (btm.Width <= 0 || btm.Height <= 0)
                throw new ArgumentException();
            _bitmap = new SignValue?[btm.Width, btm.Height];
            for (int y = 0; y < btm.Height; y++)
                for (int x = 0; x < btm.Width; x++)
                {
                    SignValue sv = new SignValue(btm.GetPixel(x, y));
                    if (color == null)
                    {
                        _bitmap[x, y] = sv;
                        continue;
                    }
                    if (sv.ValueColor == color.Value)
                        _bitmap[x, y] = sv;
                }
        }

        public void Add(Processor proc, int x, int y)
        {
            if (proc == null)
                throw new ArgumentNullException();
            if (proc.Length <= 0)
                throw new ArgumentException();
            if (x < 0 || y < 0)
                throw new ArgumentException();
            if (proc.Width + x >= Width)
                throw new ArgumentException();
            if (proc.Height + y >= Height)
                throw new ArgumentException();
            _lstProcs.Add(new ProcStruct
            {
                X = x,
                Y = y,
                Proc = proc
            });
        }

        public ProcStruct? GetEqual()
        {
            if (_bitmap == null)
                throw new ArgumentNullException();
            List<Equal> lstEq = new List<Equal>();
            foreach (ProcStruct ps in _lstProcs)
            {
                if (ps.X < 0)
                    throw new ArgumentException();
                if (ps.Y < 0)
                    throw new ArgumentException();
                Tpoint[,] signValues = new Tpoint[Width, Height];
                for (int k = 0; k < ps.Proc._lstProcs.Count; k++)
                    for (int y = ps.Y; y < Height; y++)
                        for (int x = ps.X; x < Width; x++)
                        {
                            Tpoint tp = signValues[x, y];
                            SignValue? sv = _bitmap[x, y], sv1 = ps.Proc.GetSignValue(x, y);
                            if (sv == null || sv1 == null)
                                continue;
                            if (tp != null)
                            {
                                SignValue? val = sv.Value - sv1.Value;
                                if (val.Value > tp.Value) continue;
                                if (tp.Value == val.Value)
                                {
                                    tp.Number.Add(k);
                                    continue;
                                }
                                tp.Value = val.Value;
                                tp.Number.Add(k);
                                continue;
                            }
                            signValues[x, y] = new Tpoint
                            {
                                Value = sv.Value - sv1.Value,
                                Number = new List<int> { k }
                            };
                        }
                int[] lst = new int[ps.Proc._bitmap.Length];
                for (int k = 0; k < ps.Proc._bitmap.Length; k++)
                    foreach (Tpoint tp in signValues)
                        if (tp.Number.Contains(k))
                            lst[k]++;
                int index = -1, ind = 0;
                for (int k = 0; k < lst.Length; k++)
                    if (lst[k] > ind)
                    {
                        index = k;
                        ind = lst[k];
                    }
                lstEq.Add(new Equal
                {
                    Count = ind,
                    Proc = _lstProcs[index]
                });
            }
            ProcStruct? cur = null;
            for (int k = 0, cou = -1; k < lstEq.Count; k++)
                if (lstEq[k].Count > cou)
                {
                    cou = lstEq[k].Count;
                    cur = lstEq[k].Proc;
                }
            return cur;
        }
    }
}