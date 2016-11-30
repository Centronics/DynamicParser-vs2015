using System;
using System.Collections.Generic;
using System.Drawing;
//using System.Threading.Tasks;
using System.Linq;
using System.Collections.Concurrent;
using System.Threading;
using DynamicProcessor;

namespace DynamicParser
{
    public sealed class ProcessorContainer
    {
        readonly List<Processor> _lstProcs = new List<Processor>();

        public Processor this[int index] => _lstProcs[index];

        public int Count => _lstProcs.Count;

        public int Width { get; }

        public int Height { get; }

        public ProcessorContainer(Processor first, params Processor[] processors)
        {
            if (first == null)
                throw new ArgumentNullException();
            if (first.Length <= 0)
                throw new ArgumentException();
            if (!InOneSize(first, processors))
                throw new ArgumentException();
            _lstProcs.Add(first);
            Width = first.Width;
            Height = first.Height;
            if (processors == null)
                return;
            if (processors.Length <= 0)
                return;
            foreach (Processor proc in processors)
            {
                if (proc == null)
                    continue;
                if (proc.Length <= 0)
                    continue;
                _lstProcs.Add(proc);
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

        public static bool InOneSize(Processor proc, Processor[] processors)
        {
            if (proc == null)
                return false;
            return processors != null && processors.All(pr => pr?.Width == proc.Width && pr.Height == proc.Height);
        }
    }

    public class RectSign
    {
        public Rectangle Rect { get; set; }
        public List<ProcClass> LstProc { get; } = new List<ProcClass>();
    }

    public sealed class ProcClass : ICloneable
    {
        public ConcurrentBag<Processor> Processors { get; } = new ConcurrentBag<Processor>();
        public double Percent { get; set; }
        public SignValue? CurrentSignValue { get; set; }
        public ConcurrentBag<RectSign> Map { get; } = new ConcurrentBag<RectSign>();
        public object Tag { get; set; }

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

        public object Clone()
        {
            ProcClass pc = new ProcClass(CurrentSignValue);
            foreach (Processor pr in Processors)
                pc.Processors.Add((Processor)pr.Clone());
            return pc;
        }

        public bool ContainsTag(object tag)
        {
            if (tag == null)
                throw new ArgumentNullException();
            return Processors.Where(pr => pr != null).Any(pr => pr.Tag == tag);
        }
    }

    public sealed class Processor : ICloneable
    {
        const double DiffEqual = 0.01;

        readonly ProcClass[,] _bitmap;

        public object Tag { get; }

        public int Width => _bitmap.GetLength(0);

        public int Height => _bitmap.GetLength(1);

        public int Length => Width * Height;

        public event Action<string> LogEvent;

        public ProcClass this[int x, int y] => _bitmap[x, y];

        public IEnumerable<RectSign> Mapping => from ProcClass pc in _bitmap where pc?.Map.Count > 0 from rc in pc.Map select rc;

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
                    list[x, y] = (ProcClass)lst[x, y].Clone();
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

        public Processor GetEqual()
        {
            Processor proc = new Processor(Width, Height, Tag);
            List<Thread> thrs = new List<Thread>();
            foreach (RectSign map in Mapping)
            {
                Thread thr = new Thread(o =>
                {
                    try
                    {
                        if (o == null)
                            throw new Exception("o == null");
                        RectSign rs = (RectSign)o;
                        double perc = -1;
                        List<ProcClass> prc = new List<ProcClass>();
                        for (int y = rs.Rect.Y; y < rs.Rect.Bottom; y++)
                            for (int x = rs.Rect.X; x < rs.Rect.Right; x++)
                            {
                                ProcClass pc = _bitmap[x, y];
                                double db = pc.Percent;
                                if (perc < 0)
                                {
                                    perc = db;
                                    prc.Add(pc);
                                    continue;
                                }
                                if (Math.Abs(perc - db) > DiffEqual)
                                    continue;
                                perc = db;
                                prc.Add(pc);
                                proc._bitmap[x, y].Map.Add(rs);
                            }
                        foreach (ProcClass pr in prc)
                            rs.LstProc.Add(pr);
                    }
                    catch (Exception ex)
                    {
                        WriteLog(ex.Message);
                    }
                })
                {
                    IsBackground = true,
                    Name = nameof(GetEqual),
                    Priority = ThreadPriority.AboveNormal
                };
                thr.Start(map);
                thrs.Add(thr);
            }
            foreach (Thread t in thrs)
                t.Join();
            return proc;
        }

        void WriteLog(string message)
        {
            try
            {
                if (LogEvent == null)
                    return;
                foreach (Delegate del in LogEvent.GetInvocationList())
                {
                    try
                    {
                        ((Action<string>)del).Invoke($@"{DateTime.Now}: ID:({Tag}): {message}");
                    }
                    catch
                    {
                        //ignored
                    }
                }
            }
            catch
            {
                // ignored
            }
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
            int ind = int.MaxValue, n = -1;
            foreach (int key in db.Keys)
            {
                int[,] mas = db[key];
                if (ind < mas[x, y]) continue;
                ind = mas[x, y];
                n = key;
            }
            return n >= 0 && db.Keys.Where(key => ind == db[key][x, y]).ToList().Contains(number);
        }

        static IEnumerable<int> GetMaxIndex(IList<double> lst, double perc)
        {
            if (lst == null)
                throw new ArgumentNullException();
            if (lst.Count <= 0)
                yield break;
            for (int k = 0; k < lst.Count; k++)
                if (Math.Abs(lst[k] - perc) < DiffEqual)
                    yield return k;
        }

        public Processor GetEqual(ProcessorContainer prc)
        {
            if (prc == null)
                throw new ArgumentNullException();
            if (prc.Width > Width || prc.Height > Height)
                throw new ArgumentException();
            if (prc.Count <= 0)
                throw new ArgumentException();
            WriteLog("Обработка начата");
            Processor proc = new Processor(Width, Height, Tag);
            //ParallelLoopResult pty = Parallel.For(0, Height, y1 =>
            for (int y1 = 0; y1 < Height; y1++)
            {
                try
                {
                    //ParallelLoopResult ptx = Parallel.For(0, Width, x1 =>
                    for (int x1 = 0; x1 < Width; x1++)
                    {
                        try
                        {
                            ConcurrentDictionary<int, int[,]> procPercent = new ConcurrentDictionary<int, int[,]>();
                            //ParallelLoopResult pr1 = Parallel.For(0, prc.Count, j =>
                            for (int j = 0; j < prc.Count; j++)
                            {
                                try
                                {
                                    Processor ps = prc[j];
                                    int[,] pc = new int[ps.Width, ps.Height];
                                    if (ps.Width > Width - x1 || ps.Height > Height - y1)
                                        continue;//return;
                                    for (int y = 0, yy = y1; y < prc.Height; y++, yy++)
                                        for (int x = 0, xx = x1; x < prc.Width; x++, xx++)
                                        {
                                            ProcClass tpps = ps[x, y], curp = this[xx, yy];
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
                            }//);
                             //if (!pr1.IsCompleted)
                             //   throw new Exception($"Ошибка при выполнении цикла обработки карт ({nameof(pr1)})");
                            Processor pr = new Processor(Width, Height, Tag);
                            //ParallelLoopResult pr2 = Parallel.For(0, Height - prc.Height, y =>
                            for (int y = 0; y <= Height - prc.Height; y++)
                            {
                                try
                                {
                                    //ParallelLoopResult pr3 = Parallel.For(0, Width - prc.Width, x =>
                                    for (int x = 0; x <= Width - prc.Width; x++)
                                    {
                                        try
                                        {
                                            double[] mas = new double[prc.Count];
                                            for (int k = 0; k < prc.Count; k++)
                                            {
                                                for (int y2 = y, yp = y + prc[k].Height; y2 < yp; y2++)
                                                    for (int x2 = x, xp = x + prc[k].Width; x2 < xp; x2++)
                                                        if (GetMinIndex(procPercent, x2, y2, k))
                                                            mas[k]++;
                                                mas[k] /= prc[k].Length;
                                            }
                                            ProcClass pc = pr[x, y];
                                            pc.CurrentSignValue = this[x1 + x, y1 + y].CurrentSignValue;
                                            double db = mas.Max();
                                            pc.Percent = db;
                                            foreach (int i in GetMaxIndex(mas, db))
                                                pc.Processors.Add(prc[i]);
                                        }
                                        catch (Exception ex)
                                        {
                                            WriteLog(ex.Message);
                                        }
                                    }//);
                                     // if (!pr3.IsCompleted)
                                     //     throw new Exception($"Ошибка при выполнении цикла обработки карт ({nameof(pr3)})");
                                }
                                catch (Exception ex)
                                {
                                    WriteLog(ex.Message);
                                }
                            }//);
                             // if (!pr2.IsCompleted)
                             //     throw new Exception($"Ошибка при выполнении цикла обработки карт ({nameof(pr2)})");
                            proc[x1, y1].Processors.Add(pr);
                        }
                        catch (Exception ex)
                        {
                            WriteLog(ex.Message);
                        }
                    }//);
                     //if (!ptx.IsCompleted)
                     //   throw new Exception($"Ошибка при выполнении цикла обработки карт ({nameof(ptx)})");
                }
                catch (Exception ex)
                {
                    WriteLog(ex.Message);
                }
            }//);
             // if (!pty.IsCompleted)
             //     throw new Exception($"Ошибка при выполнении цикла обработки карт ({nameof(pty)})");
            WriteLog("Обработка успешно завершена");
            return proc;
        }
    }
}