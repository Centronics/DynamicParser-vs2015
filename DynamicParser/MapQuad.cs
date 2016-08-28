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
            public int Number;
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

        static NumberCount Find(Bitmap btmMain, List<Bitmap> bitSubject)
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
            return GetCount(lstWorkArray);
        }

        static void ToArray(Difference?[,] lstDiff, SignValue[,] masMain, SignValue[,] masSubject, int sx, int sy, int number)
        {
            for (int y = sy, _y = 0, ly = masSubject.GetLength(1); y < ly; y++)
                for (int x = sx, _x = 0, lx = masSubject.GetLength(0); x < lx; x++)
                {
                    SignValue sv = masMain[x++, y++] - masSubject[_x, _y];
                    if (!lstDiff[_x, _y].HasValue)
                    {
                        lstDiff[_x, _y] = new Difference { Diff = sv, Number = number };
                        continue;
                    }
                    if (lstDiff[_x, _y].Value.Diff > sv)
                        lstDiff[_x, _y] = new Difference { Diff = sv, Number = number };
                }
        }

        static NumberCount GetCount(Difference?[,] lstDiff)
        {
            SortedDictionary<int, NumberCount> dic = new SortedDictionary<int, NumberCount>();
            for (int y = 0, ly = lstDiff.GetLength(1); y < ly; y++)
                for (int x = 0, lx = lstDiff.GetLength(0); x < lx; x++)
                    if (lstDiff[x, y].Value.Number != null)
                    {
                        if (!dic.ContainsKey(lstDiff[x, y].Value.Number))
                        {
                            NumberCount nc = new NumberCount();
                            nc.Number = lstDiff[x, y].Value.Number;
                            nc.X = x;
                            nc.Y = y;
                            dic[lstDiff[x, y].Value.Number] = nc;
                        }
                        else
                        {
                            NumberCount dif = dic[lstDiff[x, y].Value.Number];
                            dif.Count++;
                            dic[lstDiff[x, y].Value.Number] = dif;
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