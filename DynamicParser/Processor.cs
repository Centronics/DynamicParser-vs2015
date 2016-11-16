using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading.Tasks;
using System.Collections.Concurrent;
using System.Linq;
using DynamicProcessor;

namespace DynamicParser
{
    public enum ProcType
    {
        None,
        ProcList,
        Sign,
        Error
    }

    public sealed class ProcClass
    {
        public ConcurrentDictionary<int, SignValue> Procs { get; } = new ConcurrentDictionary<int, SignValue>();
        public List<Processor> CurrentProcessors { get; } = new List<Processor>();
        readonly SignValue? _currentSignValue;

        public ProcClass(SignValue? sv = null)
        {
            _currentSignValue = sv;
        }

        public void Recognize(ProcClass pc1, ProcClass pc2)
        {
            if (pc1 == null)
                throw new ArgumentNullException();
            if (pc2 == null)
                throw new ArgumentNullException();
            for (int k = 0; k < pc1.CurrentProcessors.Count; k++)
            {
                Processor proc = pc1.CurrentProcessors[k];
                foreach (Processor t in pc2.CurrentProcessors)
                    proc.Add(t);
                CurrentProcessors.Add(proc.GetEqual());
            }
        }

        public ProcType Type
        {
            get
            {
                if (CurrentProcessors.Count > 0 && _currentSignValue != null)
                    return ProcType.Error;
                if (_currentSignValue != null)
                    return ProcType.Sign;
                return CurrentProcessors.Count > 0 ? ProcType.ProcList : ProcType.None;
            }
        }

        public SignValue Value
        {
            get
            {
                if (Type != ProcType.Sign)
                    throw new ArgumentException($"Знак должен быть указан ({Type})", nameof(Value));
                if (_currentSignValue == null)
                    throw new ArgumentException("Знак должен быть указан (null)", nameof(Value));
                return _currentSignValue.Value;
            }
        }

        public int MaxEqualIndex
        {
            get
            {
                if (Procs.IsEmpty)
                    return -1;
                return Procs.Min().Key;
            }
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
                            Processor proc = new Processor(MaxWidth, MaxHeight);
                            ParallelLoopResult pr1 = Parallel.For(0, _lstProcs.Count, j =>
                            {
                                try
                                {
                                    Processor ps = _lstProcs[j];
                                    if (ps.Width < Width - x1 || ps.Height < Height - y1)
                                        return;
                                    for (int y = 0; y < ps.Height; y++, y1++)
                                        for (int x = 0; x < ps.Width; x++, x1++)
                                        {
                                            ProcClass tp = proc._bitmap[x, y], tpps = ps._bitmap[x, y], curp = _bitmap[x1, y1];
                                            if (tp == null)
                                                throw new ArgumentException($"{nameof(GetEqual)}: Элемент проверяемой карты равен null", nameof(tp));
                                            if (tpps == null)
                                                throw new ArgumentException($"{nameof(GetEqual)}: Элемент проверяющей карты равен null", nameof(tpps));
                                            if (curp == null)
                                                throw new ArgumentException($"{nameof(GetEqual)}: Элемент текущей карты равен null", nameof(curp));
                                            if (tpps.Type == ProcType.Sign && curp.Type == ProcType.Sign)
                                            {
                                                tp.Procs[j] = curp.Value - tpps.Value;
                                                continue;
                                            }
                                            if (tpps.Type == ProcType.ProcList && curp.Type == ProcType.ProcList)
                                            {
                                                tp.Recognize(tpps, curp);
                                                continue;
                                            }
                                            if (tpps.Type == ProcType.Error || curp.Type == ProcType.Error)
                                                throw new ArgumentException();
                                        }
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
                                            Dictionary<int, double> dct = new Dictionary<int, double>();
                                            for (int k = 0; k < _lstProcs.Count; k++)
                                            {
                                                if (proc.Width - x < _lstProcs[k].Width || proc.Height - y < _lstProcs[k].Height)
                                                    continue;
                                                for (int y2 = y, y3 = 0, yp = y2 + proc.Height; y2 < yp; y2++, y3++)
                                                    for (int x2 = x, x3 = 0, xp = x2 + proc.Width; x2 < xp; x2++, x3++)
                                                    {
                                                        int index = proc._bitmap[x3, y3].MaxEqualIndex;
                                                        if (index < 0)
                                                            continue;
                                                        if (!dct.ContainsKey(index))
                                                            dct[index] = 1;
                                                        else
                                                            dct[index]++;
                                                    }
                                                dct[k] = dct[k] / Convert.ToDouble(_lstProcs[k].Length);
                                            }
                                            if (dct.Count <= 0)
                                                throw new Exception();
                                            KeyValuePair<int, double> db = dct.Max();
                                            IEnumerable<int> lst = from svv in dct where Math.Abs(svv.Value - db.Value) < 0.0000000000000001 select svv.Key;
                                            ProcClass pc = _bitmap[x, y];
                                            foreach (int i in lst)
                                                pc.CurrentProcessors.Add(_lstProcs[i]);
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
            return processor;
        }
    }
}