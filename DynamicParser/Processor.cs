using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading.Tasks;
using System.Collections.Concurrent;
using System.Linq;
using DynamicProcessor;

namespace DynamicParser
{
    public sealed class ProcClass
    {
        public ConcurrentDictionary<int, Processor> CurrentProcessors { get; } = new ConcurrentDictionary<int, Processor>();
        public SignValue? CurrentSignValue { get; }

        public ProcClass(SignValue? sv = null)
        {
            CurrentSignValue = sv;
        }

        public static int operator -(ProcClass pc1, ProcClass pc2)
        {
            if (pc1 == null)
                throw new ArgumentNullException();
            if (pc2 == null)
                throw new ArgumentNullException();
            if (pc1.CurrentSignValue == null || pc2.CurrentSignValue == null)
                throw new ArgumentException();
            return (pc1.CurrentSignValue.Value - pc2.CurrentSignValue.Value).Value;
        }
    }

    public sealed class Processor : ICloneable
    {
        readonly ProcClass[,] _bitmap;
        readonly List<Processor> _lstProcs = new List<Processor>();

        public int Width => _bitmap.GetLength(0);

        public int Height => _bitmap.GetLength(1);

        public int Length => Width * Height;

        public int MaxWidth => _lstProcs.Select(pr => pr.Width).Concat(new[] { 0 }).Max();

        public int MaxHeight => _lstProcs.Select(pr => pr.Height).Concat(new[] { 0 }).Max();

        public Processor GetMaxProcessor => new Processor(MaxWidth, MaxHeight);

        public event Action<string> LogEvent;

        public Processor(ProcClass[,] lst)
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
                    _bitmap[x, y] = new ProcClass(new SignValue(btm.GetPixel(x, y)));
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
                    _bitmap[x, y] = new ProcClass();
        }

        void WriteLog(string message)
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

        public object Clone()
        {
            return new Processor(_bitmap);
        }

        static bool GetMinIndex(IList<double[,]> db, int x, int y, int number)
        {
            if (x < 0 || y < 0 || number < 0)
                throw new ArgumentException();
            if (db == null)
                throw new ArgumentNullException();
            if (db.Count <= 0)
                throw new ArgumentException();
            double dbl = double.MaxValue;
            int n = 0;
            List<int> lst = new List<int>();
            for (int k = 0; k < db.Count; k++)
            {
                double[,] mas = db[k];
                if (x >= mas.GetLength(0) || y >= mas.GetLength(1))
                    continue;
                if (dbl < mas[x, y]) continue;
                if (Math.Abs(dbl - mas[x, y]) <= 0.0001)
                    lst.Add(k);
                dbl = mas[x, y];
                n = k;
            }
            if (lst.Contains(number))
                return true;
            return n == number;
        }

        static IEnumerable<int> GetMaxIndex(IList<double> lst)
        {
            if (lst == null)
                throw new ArgumentNullException();
            if (lst.Count <= 0)
                yield break;
            double db = lst.Max();
            for (int k = 0; k < lst.Count; k++)
                if (Math.Abs(lst[k] - db) < 0.00000001)
                    yield return k;
        }

        public Processor GetEqual()
        {
            Processor proc = new Processor(MaxWidth, MaxHeight);
            ParallelLoopResult pty = Parallel.For(0, Height, y1 =>
            {
                try
                {
                    ParallelLoopResult ptx = Parallel.For(0, Width, x1 =>
                    {
                        try
                        {
                            List<double[,]> procPercent = new List<double[,]>();
                            ParallelLoopResult pr1 = Parallel.For(0, _lstProcs.Count, j =>
                            {
                                try
                                {
                                    Processor ps = _lstProcs[j];
                                    double[,] pc = new double[ps.Width, ps.Height];
                                    if (ps.Width < Width - x1 || ps.Height < Height - y1)
                                        return;
                                    for (int y = 0; y < ps.Height;)
                                        for (int x = 0; x < ps.Width;)
                                        {
                                            ProcClass tpps = ps._bitmap[x++, y++], curp = _bitmap[x1++, y1++];
                                            if (tpps == null)
                                                throw new ArgumentException($"{nameof(GetEqual)}: Элемент проверяющей карты равен null", nameof(tpps));
                                            if (curp == null)
                                                throw new ArgumentException($"{nameof(GetEqual)}: Элемент текущей карты равен null", nameof(curp));
                                            pc[x, y] = tpps - curp;
                                        }
                                    procPercent[j] = pc;
                                }
                                catch (Exception ex)
                                {
                                    WriteLog(ex.Message);
                                }
                            });
                            if (!pr1.IsCompleted)
                                throw new Exception($"Ошибка при выполнении цикла обработки изображений ({nameof(pr1)})");
                            ParallelLoopResult pr2 = Parallel.For(0, proc.Height, y =>
                            {
                                try
                                {
                                    ParallelLoopResult pr3 = Parallel.For(0, proc.Width, x =>
                                    {
                                        try
                                        {
                                            if (_lstProcs.Count <= 0)
                                                throw new Exception();
                                            double[] lst = new double[_lstProcs.Count];
                                            for (int k = 0; k < _lstProcs.Count; k++)
                                            {
                                                if (proc.Width - x < _lstProcs[k].Width || proc.Height - y < _lstProcs[k].Height)
                                                    continue;
                                                for (int y2 = y, yp = y + proc.Height; y2 < yp; y2++)
                                                    for (int x2 = x, xp = x + proc.Width; x2 < xp; x2++)
                                                    {
                                                        if (!GetMinIndex(procPercent, x2, y2, k))
                                                            continue;
                                                        lst[k]++;
                                                    }
                                                lst[k] = lst[k] / Convert.ToDouble(_lstProcs[k].Length);
                                            }
                                            ProcClass pc = proc._bitmap[x, y];
                                            foreach (int i in GetMaxIndex(lst))
                                                pc.CurrentProcessors[i] = _lstProcs[i];
                                        }
                                        catch (Exception ex)
                                        {
                                            WriteLog(ex.Message);
                                        }
                                    });
                                    if (!pr3.IsCompleted)
                                        throw new Exception($"Ошибка при выполнении цикла обработки изображений ({nameof(pr3)})");
                                }
                                catch (Exception ex)
                                {
                                    WriteLog(ex.Message);
                                }
                            });
                            if (!pr2.IsCompleted)
                                throw new Exception($"Ошибка при выполнении цикла обработки изображений ({nameof(pr2)})");
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
            return proc;
        }
    }
}