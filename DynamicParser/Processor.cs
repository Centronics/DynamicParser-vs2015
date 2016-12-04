using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using DynamicProcessor;

namespace DynamicParser
{
    public sealed class Processor
    {
        const double DiffEqual = 0.01;

        readonly SignValue[,] _bitmap;

        public object Tag { get; }

        public int Width => _bitmap.GetLength(0);

        public int Height => _bitmap.GetLength(1);

        public int Length => Width * Height;

        public event Action<string> LogEvent;

        public SignValue this[int x, int y] => _bitmap[x, y];

        public Region CurrentRegion => new Region(Width, Height);

        public Processor(Bitmap btm, object tag)
        {
            if (btm == null)
                throw new ArgumentNullException();
            if (btm.Width <= 0)
                throw new ArgumentException();
            if (btm.Height <= 0)
                throw new ArgumentException();
            if (tag == null)
                throw new ArgumentNullException();
            _bitmap = new SignValue[btm.Width, btm.Height];
            for (int y = 0; y < btm.Height; y++)
                for (int x = 0; x < btm.Width; x++)
                    _bitmap[x, y] = new SignValue(btm.GetPixel(x, y));
            Tag = tag;
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

        bool GetMinIndex(IDictionary<int, SignValue[,]> db, int x, int y, int number)
        {
            if (x < 0 || y < 0 || number < 0)
                throw new ArgumentException();
            if (db == null)
                throw new ArgumentNullException();
            if (db.Count <= 0)
                throw new ArgumentException();
            SignValue ind = SignValue.MaxValue;
            int n = -1;
            foreach (int key in db.Keys)
            {
                try
                {
                    SignValue[,] mas = db[key];
                    try
                    {
                        if (ind < mas[x, y]) continue;
                        try
                        {
                            ind = mas[x, y];
                        }
                        catch (Exception ex)
                        {
                            WriteLog(ex.Message);
                        }
                    }
                    catch (Exception ex)
                    {
                        WriteLog(ex.Message);
                    }
                    n = key;
                }
                catch (Exception ex)
                {
                    WriteLog(ex.Message);
                }
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

        public SearchResults GetEqual(Processor first, params Processor[] processors)
        {
            return GetEqual(new ProcessorContainer(first, processors));
        }

        public SearchResults GetEqual(ProcessorContainer prc)
        {
            if (prc == null)
                throw new ArgumentNullException();
            if (prc.Width > Width || prc.Height > Height)
                throw new ArgumentException();
            if (prc.Count <= 0)
                throw new ArgumentException();
            WriteLog("Обработка начата");
            SearchResults sr = new SearchResults(Width, Height);
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
                            ConcurrentDictionary<int, SignValue[,]> procPercent = new ConcurrentDictionary<int, SignValue[,]>();
                            //ParallelLoopResult pr1 = Parallel.For(0, prc.Count, j =>
                            for (int j = 0; j < prc.Count; j++)
                            {
                                try
                                {
                                    Processor ps = prc[j];
                                    SignValue[,] pc = new SignValue[ps.Width, ps.Height];
                                    if (ps.Width > Width - x1 || ps.Height > Height - y1)
                                        continue;//return;
                                    for (int y = 0, yy = y1; y < prc.Height; y++, yy++)
                                        for (int x = 0, xx = x1; x < prc.Width; x++, xx++)
                                        {
                                            try
                                            {
                                                SignValue tpps = ps[x, y], curp = this[xx, yy];
                                                if (tpps == null)
                                                    throw new ArgumentException(
                                                        $"{nameof(GetEqual)}: Элемент проверяющей карты равен null",
                                                        nameof(tpps));
                                                if (curp == null)
                                                    throw new ArgumentException(
                                                        $"{nameof(GetEqual)}: Элемент текущей карты равен null",
                                                        nameof(curp));
                                                pc[x, y] = tpps - curp;
                                            }
                                            catch (Exception ex)
                                            {
                                                WriteLog(ex.Message);
                                            }
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
                            if (procPercent.Count <= 0)
                                continue;//return;
                            //ParallelLoopResult pr2 = Parallel.For(0, Height - prc.Height, y =>
                            //for (int y = 0; y < prc.Height; y++)
                            {
                                try
                                {
                                    //ParallelLoopResult pr3 = Parallel.For(0, Width - prc.Width, x =>
                                    //for (int x = 0; x < prc.Width; x++)
                                    {
                                        try
                                        {
                                            double[] mas = new double[prc.Count];
                                            for (int k = 0; k < prc.Count; k++)
                                            {
                                                for (int y2 = 0; y2 < prc.Height; y2++)
                                                    for (int x2 = 0; x2 < prc.Width; x2++)
                                                        if (GetMinIndex(procPercent, x2, y2, k))
                                                            mas[k]++;
                                                mas[k] /= prc[k].Length;
                                            }
                                            double db = mas.Max();
                                            sr[x1, y1] = new ProcPerc
                                            {
                                                Procs = GetMaxIndex(mas, db).Select(i => prc[i]).ToArray(),
                                                Percent = db
                                            };
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
            return sr;
        }
    }
}