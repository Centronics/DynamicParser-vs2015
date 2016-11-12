using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading.Tasks;
using DynamicProcessor;

namespace DynamicParser
{
    public sealed class Processor : ICloneable
    {
        sealed class ProcClass : ICloneable
        {
            public struct ProcCount
            {
                public int Index;
                public Processor Proc;
                public SignValue Sign;

                public decimal Percentage => Proc == null ? 0 : Index / Convert.ToDecimal(Proc.Length);
            }

            public int X { get; }
            public int Y { get; }
            public SignValue Value { get; set; }
            public List<ProcCount> Procs { get; } = new List<ProcCount>();
            public ProcCount MaxElement => Procs[MaxIndex];
            public int MaxItemCount => Procs[MaxIndex].Index;
            public decimal MaxItemPercentage => Procs[MaxIndex].Percentage;
            public Processor MaxItemProcessor => Procs[MaxIndex].Proc;

            public ProcClass(int x, int y, SignValue? sv = null)
            {
                if (x < 0)
                    throw new ArgumentException();
                if (y < 0)
                    throw new ArgumentException();
                X = x;
                Y = y;
                Value = sv ?? SignValue.MaxValue;
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

            public object Clone()
            {
                ProcClass pc = new ProcClass(X, Y, Value);
                for (int k = 0; k < Procs.Count; k++)
                    pc.Procs.Add(Procs[k]);
                return pc;
            }
        }

        readonly ProcClass[,] _bitmap;
        readonly List<Processor> _lstProcs = new List<Processor>();

        public int Width => _bitmap.GetLength(0);

        public int Height => _bitmap.GetLength(1);

        public int Length => Width * Height;

        public static event Action<string> LogEvent;

        Processor(ProcClass[,] lst)
        {
            if (lst == null)
                throw new ArgumentNullException();
            if (lst.Length <= 0)
                throw new ArgumentException();
            ProcClass[,] list = new ProcClass[lst.GetLength(0), lst.GetLength(1)];
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
            _bitmap = new ProcClass[btm.Width, btm.Height];
            for (int y = 0; y < btm.Height; y++)
                for (int x = 0; x < btm.Width; x++)
                    _bitmap[x, y] = new ProcClass(x, y, new SignValue(btm.GetPixel(x, y)));
        }

        public Processor(int mx, int my)
        {
            if (mx <= 0)
                throw new ArgumentException($"{nameof(Processor)}: mx < 0: ({mx})", nameof(mx));
            if (my <= 0)
                throw new ArgumentException($"{nameof(Processor)}: my < 0: ({my})", nameof(my));
            _bitmap = new ProcClass[mx, my];
            for (int y = 0; y < my; y++)
                for (int x = 0; x < mx; x++)
                    _bitmap[x, y] = new ProcClass(x, y);
        }

        static void WriteLog(string message)
        {
            try
            {
                LogEvent?.Invoke($@"{DateTime.Now}: {message}");
            }
            catch
            {
                // ignored
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

        public Processor GetEqual()
        {
            Processor processor = new Processor(Width, Height);
            ParallelLoopResult pty = Parallel.For(0, Height, y1 =>
            {
                try
                {
                    ParallelLoopResult ptx = Parallel.For(0, Width, x1 =>
                    {
                        try
                        {
                            ParallelLoopResult pr1 = Parallel.For(0, _lstProcs.Count, j =>
                            {
                                try
                                {
                                    Processor ps = _lstProcs[j];
                                    if (ps.Width < Width - x1 || ps.Height < Height - y1)
                                        return;
                                    for (int y = 0; y < ps.Height; y++)
                                        for (int x = 0; x < ps.Width; x++)
                                        {
                                            ProcClass tp = processor._bitmap[x1, y1], tpps = ps._bitmap[x, y];
                                            if (tp == null)
                                                throw new ArgumentException("Элемент проверяемой карты равен null");
                                            if (tpps == null)
                                                throw new ArgumentException("Элемент проверяющей карты равен null");
                                            SignValue val = tp.Value - tpps.Value;
                                            if (val > tp.Value.Value) continue;
                                            tp.Value = val;
                                        }
                                }
                                catch (Exception ex)
                                {
                                    WriteLog(ex.Message);
                                }
                            });
                            if (!pr1.IsCompleted)
                                throw new Exception("Ошибка при выполнении цикла обработки изображений (J)");
                        }
                        catch (Exception ex)
                        {
                            WriteLog(ex.Message);
                        }
                    });
                    if (!ptx.IsCompleted)
                        throw new Exception("Ошибка при выполнении цикла обработки изображения (общий)");
                }
                catch (Exception ex)
                {
                    WriteLog(ex.Message);
                }
            });
            if (!pty.IsCompleted)
                throw new Exception("Ошибка при выполнении цикла обработки изображения (общий)");
            return processor;
        }

        public object Clone()
        {
            return new Processor(_bitmap);
        }
    }
}