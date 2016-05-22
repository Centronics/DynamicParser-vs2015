using DynamicProcessor;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DynamicParser
{
    public struct Assigned
    {
        public int X, Y, Equal;
    }

    public class MapQuad
    {
        struct Compared
        {
            public int X, Y;
            public SignValue Difference;
        }

        public int Mx { get; private set; }
        public int My { get; private set; }
        public List<SignValue> Signs { get; private set; }

        public MapQuad(List<SignValue> lst, int mx, int my)
        {
            bool lstOk = lst != null;
            if (lstOk)
                lstOk = lst.Count > 0;
            if (mx <= 0 || my <= 0 || !lstOk)
                throw new ArgumentException(string.Format("MapQuad: некорректные значения аргументов: x = {0}, y = {1}, lst = {2}",
                    mx, my, lst == null ? "null" : lst.Count.ToString()));
            if (lst.Count != mx * my)
                throw new ArgumentException(string.Format("MapQuad: длина списка знаков не равна (mx * my): {0} против {1}", lst.Count, mx * my));
            Mx = mx;
            My = my;
            Signs = lst;
        }

        public SignValue GetSign(int x, int y)
        {
            if (x < 0 || y < 0 || x >= Mx || y >= My)
                throw new ArgumentException(string.Format("GetSign: некорректные параметры для получения знака: x = {0}, y = {1}", x, y));
            return Signs[(y * Mx) + x];
        }

        public MapQuad GetQuad(int x, int y, int sx, int sy)
        {
            int lx = x + sx, ly = y + sy;
            if (x >= Mx || y >= My || x < 0 || y < 0 || sx < 0 || sy < 0 || lx > Mx || ly > My)
                return null;
            List<SignValue> lst = new List<SignValue>();
            for (; y < ly; y++)
                for (int px = x; px < lx; px++)
                    lst.Add(GetSign(px, y));
            return new MapQuad(lst, sx, sy);
        }

        public Assigned GetAllQuad(int sx, int sy, List<SignValue> signs, int partSize)
        {
            if (sx > Mx || sy > My)
                throw new ArgumentException(string.Format("GetAllQuad: некорректные параметры: sx = {0}, Mx = {1}, sy = {2}, My = {3}", sx, Mx, sy, My));
            if (signs == null)
                throw new ArgumentNullException("signs", "GetAllQuad: Знаки для разбора должны быть указаны (null)");
            if (signs.Count <= 0)
                throw new ArgumentException("GetAllQuad: Знаки для разбора должны быть указаны (0)", "signs");
            if (partSize <= 0)
                throw new ArgumentException(string.Format("GetAllQuad: Размер диагностируемой части должен быть больше нуля ({0})", partSize), "partSize");
            int mx = (Mx - sx) + 1, my = (My - sy) + 1;
            Compared?[] masAssigned = new Compared?[signs.Count];
            for (int y = 0; y < my; y++)
                for (int x = 0; x < mx; x++)
                    Compare(new MapTester(GetQuad(x, y, sx, sy).Signs, partSize, signs), masAssigned, x, y);
            return Equal(masAssigned);
        }

        void Compare(MapTester mt, Compared?[] assigned, int x, int y)
        {
            if (mt == null)
                throw new ArgumentNullException("dmc", "Compare: dmc не может быть null");
            if (Signs == null)
                throw new ArgumentNullException("Dictionary", "Compare: Dictionary не может быть null");
            if (mt.ResultList == null)
                throw new ArgumentNullException("dmc", "Compare: Dictionary не может быть null");
            if (Signs.Count != mt.ResultList.Count)
                throw new ArgumentException("dmc", "Compare: Невозможно сопоставить объекты, которые прогонялись с различным количеством знаков");
            if (mt.ResultList.Count <= 0)
                throw new ArgumentException("dmc", "Compare: dmc не проходил диагностику");
            if (Signs.Count <= 0)
                throw new ArgumentException("Compare: Сопоставляемый объект не проходил диагностику");
            if (assigned == null)
                throw new ArgumentNullException("assigned", "Compare: Список сопоставляемых объектов должен быть указан");
            for (int k = 0; k < Signs.Count; k++)
            {
                SignValue sv = Signs.ElementAt(k) - mt.ResultList.ElementAt(k);
                if (assigned[k] == null)
                {
                    assigned[k] = new Compared { Difference = sv, X = x, Y = y };
                    continue;
                }
                if (assigned[k].Value.Difference > sv)
                    continue;
                assigned[k] = new Compared { Difference = sv, X = x, Y = y };
            }
        }

        static Assigned Equal(Compared?[] assigned)
        {
            if (assigned == null)
                throw new ArgumentException("Equal: assigned = null");
            if (assigned.Length <= 0)
                throw new ArgumentException("Equal: Список сопоставляемых объектов должен содержать хотя бы один элемент", "assigned");
            List<int> lstMax = new List<int>(assigned.Length);
            for (int k = 0, count = 0; k < assigned.Length; k++, count = 0)
            {
                for (int n = 0; n < assigned.Length; n++)
                    if (assigned[k].Value.X == assigned[n].Value.X && assigned[k].Value.Y == assigned[n].Value.Y)
                        count++;
                lstMax.Add(count);
            }
            int maxNum = -1;
            for (int k = 0, max = 0; k < lstMax.Count; k++)
                if (lstMax[k] >= max)
                {
                    max = lstMax[k];
                    maxNum = k;
                }
            return new Assigned { X = assigned[maxNum].Value.X, Y = assigned[maxNum].Value.Y };
        }
    }

    public class MapTester
    {
        public List<SignValue> ResultList { get; private set; }

        /// <summary>
        /// Преобразует изображение в список знаков, размера, меньшего или равного Map.AllMax, чтобы уместить их на карту.
        /// При этом, каждый знак, добавляемый в список, проходит прогон по определённому знаку.
        /// Количество знаков для прогонов зависит от размера изображения.
        /// </summary>
        /// <param name="target">Преобразуемое изображение.</param>
        /// <returns>Возвращает список знаков, размера, меньшего или равного Map.AllMax.</returns>
        public MapTester(List<SignValue> lst, int partSize, List<SignValue> lstConvert)
        {
            if (partSize <= 0)
                throw new ArgumentException("Количество объектов, добавляемых на карту, не может быть меньше или равно нулю", "count");
            if (partSize > Map.AllMax)
                throw new ArgumentException("Количество объектов, добавляемых на карту, не может быть больше Map.Allmax", "count");
            if (lst == null)
                throw new ArgumentNullException("lst", "Список знаков не может быть null");
            if (lst.Count <= 0)
                throw new ArgumentNullException("lst", "Список знаков не может быть пустым");
            if (lstConvert == null)
                throw new ArgumentNullException("lstConvert", "Список знаков для прогона карт должен быть указан");
            if (lstConvert.Count <= 0)
                throw new ArgumentException("Список знаков для прогона карт должен содержать хотя бы один знак", "lstConvert");
            Map map = new Map();
            Processor proc = new Processor(map);
            foreach (SignValue sv in lstConvert)
            {
                SignValue? res = null;
                for (int n = 0; n < lst.Count; )
                {
                    for (int k = 0; k < partSize; k++, n++)
                    {
                        if (n >= lst.Count)
                            break;
                        map.Add(new MapObject { Sign = lst[n] });
                    }
                    SignValue? sres = proc.Run(sv);
                    res = (res == null) ? sres.Value : res.Value.Average(sres.Value);
                    map.Clear();
                }
                ResultList.Add(res.Value);
            }
        }
    }
}