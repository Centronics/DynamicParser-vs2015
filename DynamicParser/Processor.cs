using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
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

        static bool GetMinIndex(IDictionary<int, SignValue[,]> db, int x, int y, int number)
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
                SignValue[,] mas = db[key];
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
            SearchResults sr = new SearchResults(Width, Height);
            string errString = string.Empty;
            Parallel.For(0, Height, y1 =>
            {
                try
                {
                    Parallel.For(0, Width, x1 =>
                    {
                        try
                        {
                            ConcurrentDictionary<int, SignValue[,]> procPercent = new ConcurrentDictionary<int, SignValue[,]>();
                            Parallel.For(0, prc.Count, j =>
                            {
                                try
                                {
                                    Processor ps = prc[j];
                                    SignValue[,] pc = new SignValue[ps.Width, ps.Height];
                                    if (ps.Width > Width - x1 || ps.Height > Height - y1)
                                        return;
                                    Parallel.For(0, prc.Height, y =>
                                    {
                                        try
                                        {
                                            Parallel.For(0, prc.Width, x =>
                                            {
                                                try
                                                {
                                                    pc[x, y] = ps[x, y] - this[x + x1, y + y1];
                                                }
                                                catch (Exception ex)
                                                {
                                                    errString = ex.Message;
                                                }
                                            });
                                        }
                                        catch (Exception ex)
                                        {
                                            errString = ex.Message;
                                        }
                                    });
                                    procPercent[j] = pc;
                                }
                                catch (Exception ex)
                                {
                                    errString = ex.Message;
                                }
                            });
                            if (procPercent.Count <= 0)
                                return;
                            double[] mas = new double[prc.Count];
                            Parallel.For(0, prc.Count, k =>
                            {
                                try
                                {
                                    Parallel.For(0, prc.Height, y2 =>
                                    {
                                        try
                                        {
                                            Parallel.For(0, prc.Width, x2 =>
                                            {
                                                try
                                                {
                                                    if (GetMinIndex(procPercent, x2, y2, k))
                                                        mas[k]++;
                                                }
                                                catch (Exception ex)
                                                {
                                                    errString = ex.Message;
                                                }
                                            });
                                        }
                                        catch (Exception ex)
                                        {
                                            errString = ex.Message;
                                        }
                                    });
                                    mas[k] /= prc[k].Length;
                                }
                                catch (Exception ex)
                                {
                                    errString = ex.Message;
                                }
                            });
                            double db = mas.Max();
                            sr[x1, y1] = new ProcPerc
                            {
                                Procs = GetMaxIndex(mas, db).Select(i => prc[i]).ToArray(),
                                Percent = db
                            };
                        }
                        catch (Exception ex)
                        {
                            errString = ex.Message;
                        }
                    });
                }
                catch (Exception ex)
                {
                    errString = ex.Message;
                }
            });
            if (!string.IsNullOrEmpty(errString))
                throw new Exception(errString);
            return sr;
        }
    }
}