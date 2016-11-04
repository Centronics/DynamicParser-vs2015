using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading;
using DynamicProcessor;

namespace DynamicParser
{
    [Serializable]
    public sealed class Processor
    {
        sealed class Tpoint
        {
            public Quant Value;
            public List<int> Number;
        }

        public struct Quant
        {
            public SignValue? Sv1 { get; set; }
            public SignValue? Sv2 { get; set; }
            public SignValue? Sv3 { get; set; }
            public SignValue? Sv4 { get; set; }

            public int? Index1 { get; set; }
            public int? Index2 { get; set; }
            public int? Index3 { get; set; }
            public int? Index4 { get; set; }

            public Quant Compare(Quant quant)
            {
                Quant qua = new Quant();
                if (Sv1 != null && quant.Sv1 != null)
                {
                    qua.Sv1 = Sv1.Value - quant.Sv1.Value;
                    Index1 = quant.Index1;
                }
                if (Sv2 != null && quant.Sv2 != null)
                {
                    qua.Sv2 = Sv2.Value - quant.Sv2.Value;
                    Index2 = quant.Index2;
                }
                if (Sv3 != null && quant.Sv3 != null)
                {
                    qua.Sv3 = Sv3.Value - quant.Sv3.Value;
                    Index3 = quant.Index3;
                }
                if (Sv4 == null || quant.Sv4 == null) return qua;
                qua.Sv4 = Sv4.Value - quant.Sv4.Value;
                Index4 = quant.Index4;
                return qua;
            }

            public IEnumerable<int> Indexes
            {
                get
                {
                    if (Index1 != null)
                        yield return Index1.Value;
                    if (Index2 != null)
                        yield return Index2.Value;
                    if (Index3 != null)
                        yield return Index3.Value;
                    if (Index4 != null)
                        yield return Index4.Value;
                }
            }

            static uint GetIndexCount(int index, IEnumerable<int> lst)
            {
                uint count = 0;
                foreach (int i in lst)
                    if (i == index)
                        count++;
                return count;
            }

            public IEnumerable<int> MaxIndex
            {
                get
                {
                    List<int> inds = new List<int>(Indexes);
                    uint i1 = 0, i2 = 0, i3 = 0, i4 = 0;
                    if (Index1 != null)
                        i1 = GetIndexCount(Index1.Value, inds);
                    if (Index2 != null)
                        i2 = GetIndexCount(Index2.Value, inds);
                    if (Index3 != null)
                        i3 = GetIndexCount(Index3.Value, inds);
                    if (Index4 != null)
                        i4 = GetIndexCount(Index4.Value, inds);
                    if (i1 > i2 && i1 > i3 && i1 > i4)
                        yield return 1;
                    if (i2 > i1 && i2 > i3 && i2 > i4)
                        yield return 2;
                    if (i3 > i1 && i3 > i2 && i3 > i4)
                        yield return 3;
                    yield return 4;
                }
            }

            public static Quant operator -(Quant q1, Quant q2)
            {
                Quant q = new Quant();
                if (q1.Sv1 != null && q2.Sv1 != null)
                    q.Sv1 = q1.Sv1 - q2.Sv1;
                if (q1.Sv2 != null && q2.Sv2 != null)
                    q.Sv2 = q1.Sv2 - q2.Sv2;
                if (q1.Sv3 != null && q2.Sv3 != null)
                    q.Sv3 = q1.Sv3 - q2.Sv3;
                if (q1.Sv4 != null && q2.Sv4 != null)
                    q.Sv4 = q1.Sv4 - q2.Sv4;
                q.Index1 = q2.Index1;
                q.Index2 = q2.Index2;
                q.Index3 = q2.Index3;
                q.Index4 = q2.Index4;
                return q;
            }

            public static Quant operator >(Quant q1, Quant q2)
            {
                Quant q = new Quant();
                if (q1.Sv1 != null && q2.Sv1 != null)
                    if (q1.Sv1.Value > q2.Sv1.Value)
                    {
                        q.Sv1 = q1.Sv1.Value - q2.Sv1.Value;
                        q.Index1 = q1.Index1;
                    }
                if (q1.Sv2 != null && q2.Sv2 != null)
                    if (q1.Sv2.Value > q2.Sv2.Value)
                    {
                        q.Sv1 = q1.Sv2.Value - q2.Sv2.Value;
                        q.Index2 = q1.Index2;
                    }
                if (q1.Sv3 != null && q2.Sv3 != null)
                    if (q1.Sv3.Value > q2.Sv3.Value)
                    {
                        q.Sv1 = q1.Sv3.Value - q2.Sv3.Value;
                        q.Index3 = q1.Index3;
                    }
                if (q1.Sv4 != null && q2.Sv4 != null)
                    if (q1.Sv4.Value > q2.Sv4.Value)
                    {
                        q.Sv1 = q1.Sv4.Value - q2.Sv4.Value;
                        q.Index4 = q1.Index4;
                    }
                return q;
            }

            public static Quant operator <(Quant q1, Quant q2)
            {

            }
        }

        public struct ProcStruct
        {
            public int X, Y;
            public Processor Proc;
        }

        Quant[,] _bitmap;
        readonly List<Processor> _lstProcs = new List<Processor>();
        Thread _loadThread;
        string _loadErrorString;

        public int Width => _bitmap.GetLength(0);

        public int Height => _bitmap.GetLength(1);

        public int Length => Width * Height;

        public Quant GetSignValue(int x, int y)
        {
            return _bitmap[x, y];
        }

        public void LoadWait(int time = -1)
        {
            try
            {
                if (time <= 0)
                {
                    _loadThread?.Join();
                    return;
                }
                _loadThread?.Join(time);

            }
            finally
            {
                if (!string.IsNullOrEmpty(_loadErrorString))
                {
                    string str = _loadErrorString;
                    _loadErrorString = string.Empty;
                    throw new Exception(str);
                }
            }
        }

        public void LoadImage(Bitmap btm)
        {
            _loadErrorString = string.Empty;
            if (btm == null)
                throw new ArgumentNullException();
            if (btm.Width <= 0 || btm.Height <= 0)
                throw new ArgumentException();
            _bitmap = new Quant[btm.Width, btm.Height];
            (_loadThread = new Thread(() =>
            {
                try
                {
                    for (int y = 0; y < btm.Height; y++)
                        for (int x = 0; x < btm.Width; x++)
                        {
                            Quant quant = new Quant
                            {
                                Sv1 = new SignValue(btm.GetPixel(x, y)),
                                Index1 = 0
                            };
                            if (btm.Width - x >= 1)
                            {
                                quant.Sv2 = new SignValue(btm.GetPixel(x + 1, y));
                                quant.Index2 = 0;
                            }
                            if (btm.Height - y >= 1)
                            {
                                quant.Sv3 = new SignValue(btm.GetPixel(x, y + 1));
                                quant.Index3 = 0;
                            }
                            if (btm.Width - x >= 1 && btm.Height - y >= 1)
                            {
                                quant.Sv4 = new SignValue(btm.GetPixel(x + 1, y + 1));
                                quant.Index4 = 0;
                            }
                            _bitmap[x, y] = quant;
                        }
                }
                catch (Exception ex)
                {
                    _loadErrorString = ex.Message;
                }
            })
            {
                IsBackground = true,
                Name = nameof(LoadImage),
                Priority = ThreadPriority.AboveNormal
            }).Start();
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

        public ProcStruct? GetEqual()
        {
            if (_bitmap == null)
                throw new ArgumentNullException();
            Tpoint[,] signValues = new Tpoint[Width, Height];
            ProcStruct ret = new ProcStruct();
            for (int j = 0; j < _lstProcs.Count; j++)
            {
                Processor ps = _lstProcs[j];
                for (int y = 0; y < ps.Height; y++)
                    for (int x = 0; x < ps.Width; x++)
                    {
                        Tpoint tp = signValues[x, y];
                        Quant q = _bitmap[x, y], q1 = ps.GetSignValue(x, y);
                        if (tp != null)
                        {
                            Quant val = q - q1;
                            if (val > tp.Value) continue;
                            if (tp.Value == val)
                            {
                                tp.Number.Add(j);
                                continue;
                            }
                            tp.Value = val;
                            tp.Number.Add(j);
                            continue;
                        }
                        signValues[x, y] = new Tpoint
                        {
                            Value = q - q1,
                            Number = new List<int> { j }
                        };
                    }
            }
            int[] lst = new int[Length];
            for (int k = 0; k < Length; k++)
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
            ret.Proc = _lstProcs[index];
            return ret;
        }
    }
}