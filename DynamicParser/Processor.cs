using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading.Tasks;
using DynamicProcessor;

namespace DynamicParser
{
    public sealed class Processor : ICloneable
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

            public decimal Percentage => Proc == null ? 0 : Count / Convert.ToDecimal(Proc.Length);
        }

        public class ProcStruct
        {
            public int X { get; }
            public int Y { get; }
            public List<ProcCount> Procs { get; } = new List<ProcCount>();
            public ProcCount MaxElement => Procs[MaxIndex];
            public uint MaxItemCount => Procs[MaxIndex].Count;
            public decimal MaxItemPercentage => Procs[MaxIndex].Percentage;
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
                        if (Procs[k].Percentage > Procs[t].Percentage)
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

        Processor(SignValue[,] lst)
        {
            if (lst == null)
                throw new ArgumentNullException();
            if (lst.Length <= 0)
                throw new ArgumentException();
            SignValue[,] list = new SignValue[lst.GetLength(0), lst.GetLength(1)];
            for (int y = 0, ly = lst.GetLength(1); y < ly; y++)
                for (int x = 0, lx = lst.GetLength(0); x < lx; x++)
                    list[x, y] = lst[x, y];
            _bitmap = list;
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

        public void WriteLog(string str)
        {
            try
            {

            }
            catch
            {
                //
            }
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

        public Processor GetProcessor(Processor proc)
        {

        }

        public IEnumerable<ProcStruct> GetEqual()
        {
            Tpoint[,] signValues = new Tpoint[Width, Height];
            object thisLock = new object();
            ParallelLoopResult pr = Parallel.For(0, _lstProcs.Count, j =>
            {
                try
                {
                    Processor ps = _lstProcs[j];
                    ParallelLoopResult pry = Parallel.For(0, ps.Height, y =>
                    {
                        try
                        {
                            ParallelLoopResult prx = Parallel.For(0, ps.Width, x =>
                            {
                                try
                                {
                                    if (ps.Width < Width - x || ps.Height < Height - y)
                                        return;
                                    lock (thisLock)
                                    {
                                        Tpoint tp = signValues[x, y];
                                        SignValue val = _bitmap[x, y] - ps.GetSignValue(x, y);
                                        if (tp != null)
                                        {
                                            if (val > tp.Value) return;
                                            tp.Value = val;
                                            tp.Number.Add(j);
                                            return;
                                        }
                                        signValues[x, y] = new Tpoint
                                        {
                                            Value = val,
                                            Number = new List<int> { j }
                                        };
                                    }
                                }
                                catch (Exception ex)
                                {
                                    WriteLog(ex.Message);
                                }
                            });
                            if (!prx.IsCompleted)
                                throw new Exception("Ошибка при выполнении цикла обработки изображения (X)");
                        }
                        catch (Exception ex)
                        {
                            WriteLog(ex.Message);
                        }
                    });
                    if (!pry.IsCompleted)
                        throw new Exception("Ошибка при выполнении цикла обработки изображения (Y)");
                }
                catch (Exception ex)
                {
                    WriteLog(ex.Message);
                }
            });
            if (!pr.IsCompleted)
                throw new Exception("Ошибка при выполнении цикла обработки изображения (общий)");
            ProcStruct[] lst = new ProcStruct[_lstProcs.Count];
            ParallelLoopResult prx1 = Parallel.For(0, Width, x =>
            {
                try
                {
                    ParallelLoopResult pry1 = Parallel.For(0, Height, y =>
                    {
                        try
                        {
                            ParallelLoopResult prj1 = Parallel.For(0, _lstProcs.Count, j =>
                            {
                                try
                                {
                                    if (Width - x < _lstProcs[j].Width || Height - y < _lstProcs[j].Height)
                                        return;
                                    uint count = 0;
                                    for (int x1 = x; x1 < Width; x1++)
                                        for (int y1 = y; y1 < Height; y1++)
                                            if (signValues[x, y].Number.Contains(j))
                                                count++;
                                    lock (thisLock)
                                    {
                                        if (lst[j] == null)
                                            lst[j] = new ProcStruct(x, y);
                                        lst[j].Procs.Add(new ProcCount { Count = count, Proc = _lstProcs[j] });
                                    }
                                }
                                catch (Exception ex)
                                {
                                    WriteLog(ex.Message);
                                }
                            });
                            if (!prj1.IsCompleted)
                                throw new Exception("Ошибка при выполнении цикла обработки изображения (J1)");
                        }
                        catch (Exception ex)
                        {
                            WriteLog(ex.Message);
                        }
                    });
                    if (!pry1.IsCompleted)
                        throw new Exception("Ошибка при выполнении цикла обработки изображения (Y1)");
                }
                catch (Exception ex)
                {
                    WriteLog(ex.Message);
                }
            });
            if (!prx1.IsCompleted)
                throw new Exception("Ошибка при выполнении цикла обработки изображения (X1)");
            Sort(lst);
            foreach (ProcStruct t in lst)
                yield return t;
        }

        public object Clone()
        {
            return new Processor(_bitmap);
        }
    }
}