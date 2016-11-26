using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading.Tasks;
using System.Linq;
using System.Collections.Concurrent;
using DynamicProcessor;

namespace DynamicParser
{
    public sealed class ProcClass
    {
        public ConcurrentBag<Processor> Processors { get; } = new ConcurrentBag<Processor>();
        public SignValue? CurrentSignValue { get; set; }

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

        public static double DiffEqual { get; } = 0.0001;

        public object Tag { get; }

        public int Width => _bitmap.GetLength(0);

        public int Height => _bitmap.GetLength(1);

        public int Length => Width * Height;

        public event Action<string> LogEvent;

        public int ProcessorCount => _lstProcs.Count;

        public Processor this[int index] => _lstProcs[index];

        public ProcClass this[int x, int y] => _bitmap[x, y];

        public Processor(ProcClass[,] lst, object tag)
        {
            if (lst == null)
                throw new ArgumentNullException();
            if (lst.Length <= 0)
                throw new ArgumentException();
            if (tag == null)
                throw new ArgumentNullException();
            ProcClass[,] list = new ProcClass[lst.GetLength(0), lst.GetLength(1)];
            for (int y = 0, ly = lst.GetLength(1); y < ly; y++)
                for (int x = 0, lx = lst.GetLength(0); x < lx; x++)
                    list[x, y] = lst[x, y];
            _bitmap = list;
            Tag = tag;
        }

        public Processor(Bitmap btm, object tag)
        {
            if (btm == null)
                throw new ArgumentNullException();
            if (btm.Width <= 0 || btm.Height <= 0)
                throw new ArgumentException();
            if (tag == null)
                throw new ArgumentNullException();
            _bitmap = new ProcClass[btm.Width, btm.Height];
            for (int y = 0; y < btm.Height; y++)
                for (int x = 0; x < btm.Width; x++)
                    _bitmap[x, y] = new ProcClass(new SignValue(btm.GetPixel(x, y)));
            Tag = tag;
        }

        public Processor(int mx, int my, object tag)
        {
            if (mx <= 0)
                throw new ArgumentException($"{nameof(Processor)}: mx <= 0: ({mx})", nameof(mx));
            if (my <= 0)
                throw new ArgumentException($"{nameof(Processor)}: my <= 0: ({my})", nameof(my));
            if (tag == null)
                throw new ArgumentNullException();
            _bitmap = new ProcClass[mx, my];
            for (int y = 0; y < my; y++)
                for (int x = 0; x < mx; x++)
                    _bitmap[x, y] = new ProcClass();
            Tag = tag;
        }

        public Processor(object tag, Processor first, params Processor[] processors)
        {
            if (processors == null)
                return;
            if (processors.Length <= 0)
                return;
            if (first == null)
                throw new ArgumentNullException();
            if (first.Length <= 0)
                throw new ArgumentException();
            if (tag == null)
                throw new ArgumentNullException();
            int mx = first.Width, my = first.Height;
            _bitmap = new ProcClass[mx, my];
            for (int y = 0; y < my; y++)
                for (int x = 0; x < mx; x++)
                    _bitmap[x, y] = new ProcClass();
            Tag = tag;
            foreach (Processor proc in processors)
            {
                if (proc == null)
                    continue;
                if (proc.Length <= 0)
                    continue;
                Add(proc);
            }
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
            if (proc.Width != Width)
                throw new ArgumentException();
            if (proc.Height != Height)
                throw new ArgumentException();
            _lstProcs.Add(proc);
        }

        public object Clone()
        {
            return new Processor(_bitmap, Tag);
        }

        static bool GetMinIndex(IDictionary<int, int[,]> db, int x, int y, int number)
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
            foreach (int key in db.Keys)
            {
                int[,] mas = db[key];
                if (x >= mas.GetLength(0) || y >= mas.GetLength(1))
                    continue;
                if (dbl < mas[x, y]) continue;
                if (Math.Abs(dbl - mas[x, y]) <= DiffEqual)
                    lst.Add(key);
                dbl = mas[x, y];
                n = key;
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
                if (Math.Abs(lst[k] - db) < DiffEqual)
                    yield return k;
        }

        public Processor GetEqual(Processor prc)
        {
            if (prc == null)
                throw new ArgumentNullException();
            if (prc.Width > Width || prc.Height > Height)
                throw new ArgumentException();
            if (prc.Length <= 0)
                throw new ArgumentException();
            if (prc.ProcessorCount <= 0)
                throw new ArgumentException();
            if (ProcessorCount <= 0)
                throw new Exception();
            WriteLog("Обработка начата");
            Processor proc = new Processor(Width, Height, Tag);
            ParallelLoopResult pty = Parallel.For(0, Height, y1 =>
            {
                try
                {
                    ParallelLoopResult ptx = Parallel.For(0, Width, x1 =>
                    {
                        try
                        {
                            ConcurrentDictionary<int, int[,]> procPercent = new ConcurrentDictionary<int, int[,]>();
                            ParallelLoopResult pr1 = Parallel.For(0, ProcessorCount, j =>
                            {
                                try
                                {
                                    Processor ps = prc[j];
                                    int[,] pc = new int[ps.Width, ps.Height];
                                    if (ps.Width < Width - x1 || ps.Height < Height - y1)
                                        return;
                                    for (int y = 0, yy = y1; y < prc.Height;)
                                        for (int x = 0, xx = x1; x < prc.Width;)
                                        {
                                            ProcClass tpps = ps[x++, y++], curp = this[xx++, yy++];
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
                            Processor pr = new Processor(Width, Height, Tag);
                            ParallelLoopResult pr2 = Parallel.For(0, Height - prc.Height, y =>
                            {
                                try
                                {
                                    ParallelLoopResult pr3 = Parallel.For(0, Width - prc.Width, x =>
                                    {
                                        try
                                        {
                                            double[] mas = new double[ProcessorCount];
                                            for (int k = 0; k < ProcessorCount; k++)
                                            {
                                                for (int y2 = y, yp = y + prc[k].Height; y2 < yp; y2++)
                                                    for (int x2 = x, xp = x + prc[k].Width; x2 < xp; x2++)
                                                        if (GetMinIndex(procPercent, x2, y2, k))
                                                            mas[k]++;
                                                mas[k] /= prc[k].Length;
                                            }
                                            ProcClass pc = pr[x, y];
                                            pc.CurrentSignValue = this[x1 + x, y1 + y].CurrentSignValue;
                                            foreach (int i in GetMaxIndex(mas))
                                                pc.Processors.Add(prc[i]);
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
                            proc[x1, y1].Processors.Add(pr);
                        }
                        catch (Exception ex)
                        {
                            WriteLog(ex.Message);
                        }
                    });
                    if (!ptx.IsCompleted)
                        throw new Exception($"Ошибка при выполнении цикла обработки изображения ({nameof(ptx)})");
                }
                catch (Exception ex)
                {
                    WriteLog(ex.Message);
                }
            });
            if (!pty.IsCompleted)
                throw new Exception($"Ошибка при выполнении цикла обработки изображения ({nameof(pty)})");
            WriteLog("Обработка успешно завершена");
            return proc;
        }
    }
}