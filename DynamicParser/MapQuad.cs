using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using DynamicProcessor;

namespace DynamicParser
{
    public class Context
    {
        readonly Bitmap _btmMain;

        public Context(Bitmap btm)
        {
            if (btm == null)
                throw new ArgumentNullException();
            if (btm.Width <= 0)
                throw new ArgumentException();
            if (btm.Height <= 0)
                throw new ArgumentException();
            _btmMain = btm;
        }

        /// <summary>
        ///     Создаёт карту, полученную в результате прогона карты указанного изображения по ряду знаков, количество которых
        ///     зависит от размера изображения.
        /// </summary>
        /// <param name="target">Разбираемое изображение.</param>
        /// <returns>Возвращает карту, полученную в результате прогона карты изображения по ряду знаков.</returns>
        static SignValue?[,] GetMap(Bitmap target)
        {
            if (target == null)
                throw new ArgumentNullException();
            if (target.Width <= 0)
                throw new ArgumentException();
            if (target.Height <= 0)
                throw new ArgumentException();
            SignValue?[,] lst = new SignValue?[target.Width, target.Height];
            for (int y = 0; y < target.Height; y++)
                for (int x = 0; x < target.Width; x++)
                    lst[x, y] = new SignValue(target.GetPixel(x, y));
            return lst;
        }

        static bool SizeEqual(IList<Bitmap> bitmaps)
        {
            if (bitmaps == null)
                throw new ArgumentNullException();
            if (bitmaps.Count == 0)
                throw new ArgumentException();
            int w = bitmaps[0].Width, h = bitmaps[1].Height;
            return !bitmaps.Any(btm => btm.Width != w || btm.Height != h);
        }

        static bool IsAllow(int x, int y, SignValue?[,] diff, int w, int h)
        {
            int wd = diff.GetLength(0), hd = diff.GetLength(1);
            if (x + w > wd)
                return false;
            if (y + h > hd)
                return false;
            for (; y < hd; y++)
                for (; x < wd; x++)
                    if (diff[x, y] == null)
                        return false;
            return true;
        }

        static void DontAllow(int x, int y, SignValue?[,] diff, int w, int h)
        {
            int wd = x + w, hd = y + h;
            if (wd > diff.GetLength(0))
                return;
            if (hd > diff.GetLength(1))
                return;
            for (; y < hd; y++)
                for (; x < wd; x++)
                    diff[x, y] = null;
        }

        public IEnumerable<ReturnStruct> Find(List<Bitmap> bitSubject)
        {
            if (!SizeEqual(bitSubject))
                throw new ArgumentException();
            List<SignValue?[,]> lstSub = new List<SignValue?[,]>(bitSubject.Count);
            lstSub.AddRange(bitSubject.Select(GetMap));
            SignValue?[,] lstMain = GetMap(_btmMain);
            Difference?[,] lstWorkArray = new Difference?[bitSubject[0].Width, bitSubject[0].Height];
            while (true)
            {
                for (int y = 0; y < _btmMain.Height; y++)
                    for (int x = 0; x < _btmMain.Width; x++)
                        for (int k = 0; k < bitSubject.Count; k++)
                        {
                            if (bitSubject[k].Height < _btmMain.Height - bitSubject[k].Height)
                                break;
                            if (bitSubject[k].Width < _btmMain.Width - bitSubject[k].Width)
                                break;
                            if (!IsAllow(x, y, lstMain, bitSubject[k].Width, bitSubject[k].Height))
                                break;
                            ToArray(lstWorkArray, lstMain, lstSub[k], x, y, k);
                        }
                NumberCount nc = GetCount(lstWorkArray);
                if (nc.Number == null)
                    yield break;
                DontAllow(nc.X, nc.Y, lstMain, bitSubject[0].Width, bitSubject[0].Height);
                Bitmap btm = GetCurrentBitmap(_btmMain, nc.X, nc.Y, bitSubject[nc.Number.Value].Width, bitSubject[nc.Number.Value].Height);
                ReturnStruct rs = new ReturnStruct
                {
                    Bitmap = btm,
                    Original = bitSubject[nc.Number.Value],
                    X = nc.X,
                    Y = nc.Y
                };
                yield return rs;
            }
        }

        static void ToArray(Difference?[,] lstDiff, SignValue?[,] masMain, SignValue?[,] masSubject, int sx, int sy, int number)
        {
            for (int y = sy, py = 0, ly = masSubject.GetLength(1); y < ly; y++)
                for (int x = sx, px = 0, lx = masSubject.GetLength(0); x < lx; x++)
                {
                    SignValue? sv = masMain[x++, y++] - masSubject[px, py];
                    if (!lstDiff[px, py].HasValue)
                    {
                        Debug.Assert(sv != null, "sv != null");
                        lstDiff[px, py] = new Difference
                        {
                            Diff = sv.Value,
                            LstNumber = new List<int> { number }
                        };
                        continue;
                    }
                    if (lstDiff[px, py].Value.Diff > sv)
                    {
                        lstDiff[px, py] = new Difference { Diff = sv.Value, LstNumber = lstDiff[px, py].Value.LstNumber };
                        lstDiff[px, py].Value.LstNumber.Clear();
                        continue;
                    }
                    if (lstDiff[px, py].Value.Diff == sv)
                        lstDiff[px, py].Value.LstNumber.Add(number);
                }
        }

        static NumberCount GetCount(Difference?[,] lstDiff)
        {
            SortedDictionary<int, NumberCount> dic =
                new SortedDictionary<int, NumberCount>();
            for (int y = 0, ly = lstDiff.GetLength(1); y < ly; y++)
                for (int x = 0, lx = lstDiff.GetLength(0); x < lx; x++)
                {
                    Difference? difference = lstDiff[x, y];
                    if ((difference == null) || (difference.Value.LstNumber == null)) continue;
                    Difference? o = lstDiff[x, y];
                    if (o != null)
                        foreach (int num in o.Value.LstNumber)
                            if (!dic.ContainsKey(num))
                                dic[num] = new NumberCount
                                {
                                    Number = num,
                                    X = x,
                                    Y = y
                                };
                            else
                            {
                                NumberCount dif = dic[num];
                                dif.Count++;
                                dic[num] = dif;
                            }
                    lstDiff[x, y] = null;
                }
            uint max = 0;
            int maxnum = -1;
            foreach (KeyValuePair<int, NumberCount> pair in dic)
                if (pair.Value.Count >= max)
                {
                    max = pair.Value.Count;
                    maxnum = pair.Key;
                }
            if (maxnum < 0)
                throw new Exception($"{nameof(GetCount)}: Не могу найти подходящий элемент.");
            return dic[maxnum];
        }

        /// <summary>
        ///     Копирует указанную часть изображения в отдельное изображение.
        ///     Если по ширине достигнут конец, то переход на новую строку осуществляется автоматически.
        /// </summary>
        /// <param name="target">Изображение, копию участка которого требуется снять.</param>
        /// <param name="x">Стартовый X.</param>
        /// <param name="y">Стартовый Y.</param>
        /// <param name="wid">Требуемая ширина.</param>
        /// <param name="hei">Требуемая высота.</param>
        /// <returns>Возвращает изображение, представляющее собой указанную часть исходного изображения.</returns>
        static Bitmap GetCurrentBitmap(Bitmap target, int x, int y, int wid, int hei)
        {
            int mx = x + wid, my = y + hei;
            if (mx > target.Width)
            {
                if (my > target.Height)
                    return null;
                x = 0;
                mx = wid;
                y++;
                my++;
            }
            if (my > target.Height)
                return null;
            Bitmap ret = new Bitmap(wid, hei);
            int sy = y;
            for (int xr = 0; x < mx; x++, xr++)
            {
                for (int yr = 0; y < my; y++, yr++)
                    ret.SetPixel(xr, yr, target.GetPixel(x, y));
                y = sy;
            }
            return ret;
        }

        struct ContextLine
        {
            readonly List<SignValue> _signList;
            SignValue? _midSign;

            public SignValue this[int index] => _signList[index];

            public ContextLine(List<SignValue> signList)
            {
                if (signList == null)
                    throw new ArgumentNullException();
                if (signList.Count <= 0)
                    throw new ArgumentException();
                _signList = signList;
                _midSign = null;
            }

            public SignValue MidSign
            {
                get
                {
                    if (_midSign != null)
                        return _midSign.Value;
                    if (_signList == null)
                        throw new Exception();
                    if (_signList.Count <= 0)
                        throw new Exception();
                    SignValue? svs = _signList.Aggregate<SignValue, SignValue?>(null, (current, sv) => current?.Average(sv) ?? sv);
                    if (svs == null)
                        throw new Exception();
                    _midSign = svs;
                    return svs.Value;
                }
            }
        }

        struct Difference
        {
            public List<int> LstNumber;
            public SignValue Diff;
        }

        struct NumberCount
        {
            public int? Number;
            public uint Count;
            public int X, Y;
        }

        public struct ReturnStruct
        {
            public Bitmap Bitmap, Original;
            public int X, Y;
        }
    }
}