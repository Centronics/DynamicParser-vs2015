using DynamicProcessor;
using System;
using System.Collections.Generic;
using System.Drawing;

namespace DynamicParser
{
    public class Context
    {
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

        /// <summary>
        /// Создаёт карту, полученную в результате прогона карты указанного изображения по ряду знаков, количество которых зависит от размера изображения.
        /// </summary>
        /// <param name="target">Разбираемое изображение.</param>
        /// <returns>Возвращает карту, полученную в результате прогона карты изображения по ряду знаков.</returns>
        static SignValue[,] GetMap(Bitmap target)
        {
            if (target == null)
                throw new ArgumentNullException();
            if (target.Width <= 0)
                throw new ArgumentException();
            if (target.Height <= 0)
                throw new ArgumentException();
            SignValue[,] lst = new SignValue[target.Width, target.Height];
            for (int y = 0; y < target.Height; y++)
                for (int x = 0; x < target.Width; x++)
                    lst[x, y] = new SignValue(target.GetPixel(x, y));
            return lst;
        }

        public static Bitmap Find(Bitmap btmMain, List<Bitmap> bitSubject)
        {
            List<SignValue[,]> lstSub = new List<SignValue[,]>(bitSubject.Count);
            foreach (Bitmap btm in bitSubject)
                lstSub.Add(GetMap(btm));
            SignValue[,] lstDiff = GetMap(btmMain);
            Difference?[,] lstWorkArray = new Difference?[bitSubject[0].Width, bitSubject[0].Height];
            for (int y = 0; y < btmMain.Height; y++)
                for (int x = 0; x < btmMain.Width; x++)
                    for (int k = 0; k < bitSubject.Count; k++)
                    {
                        if (bitSubject[k].Height < (btmMain.Height - bitSubject[k].Height))
                            break;
                        if (bitSubject[k].Width < (btmMain.Width - bitSubject[k].Width))
                            break;
                        ToArray(lstWorkArray, lstDiff, lstSub[k], x, y, k);
                    }
            NumberCount nc = GetCount(lstWorkArray);
            if (nc.Number == null)
                throw new Exception("Find: Не могу найти подходящий образ");
            return GetCurrentBitmap(btmMain, nc.X, nc.Y, bitSubject[nc.Number.Value].Width, bitSubject[nc.Number.Value].Height);
        }

        static void ToArray(Difference?[,] lstDiff, SignValue[,] masMain, SignValue[,] masSubject, int sx, int sy, int number)
        {
            for (int y = sy, py = 0, ly = masSubject.GetLength(1); y < ly; y++)
                for (int x = sx, px = 0, lx = masSubject.GetLength(0); x < lx; x++)
                {
                    SignValue sv = masMain[x++, y++] - masSubject[px, py];
                    if (!lstDiff[px, py].HasValue)
                    {
                        lstDiff[px, py] = new Difference { Diff = sv, LstNumber = new List<int> { number } };
                        continue;
                    }
                    if (lstDiff[px, py].Value.Diff > sv)
                    {
                        lstDiff[px, py] = new Difference { Diff = sv, LstNumber = lstDiff[px, py].Value.LstNumber };
                        lstDiff[px, py].Value.LstNumber.Clear();
                    }
                    else
                        if (lstDiff[px, py].Value.Diff == sv)
                            lstDiff[px, py].Value.LstNumber.Add(number);
                }
        }

        static NumberCount GetCount(Difference?[,] lstDiff)
        {
            SortedDictionary<int, NumberCount> dic = new SortedDictionary<int, NumberCount>();
            for (int y = 0, ly = lstDiff.GetLength(1); y < ly; y++)
                for (int x = 0, lx = lstDiff.GetLength(0); x < lx; x++)
                {
                    Difference? difference = lstDiff[x, y];
                    if (difference == null || difference.Value.LstNumber == null) continue;
                    Difference? o = lstDiff[x, y];
                    if (o != null)
                        foreach (int num in o.Value.LstNumber)
                            if (!dic.ContainsKey(num))
                            {
                                NumberCount nc = new NumberCount
                                {
                                    Number = num,
                                    X = x,
                                    Y = y
                                };
                                dic[num] = nc;
                            }
                            else
                            {
                                NumberCount dif = dic[num];
                                dif.Count++;
                                dic[num] = dif;
                            }
                    lstDiff[x, y] = null;
                }
            uint max = 0; int maxnum = -1;
            foreach (KeyValuePair<int, NumberCount> pair in dic)
                if (pair.Value.Count >= max)
                {
                    max = pair.Value.Count;
                    maxnum = pair.Key;
                }
            if (maxnum < 0)
                throw new Exception("GetCount: Не могу найти подходящий элемент.");
            return dic[maxnum];
        }

        /// <summary>
        /// Копирует указанную часть изображения в отдельное изображение.
        /// Если по ширине достигнут конец, то переход на новую строку осуществляется автоматически.
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
    }
}