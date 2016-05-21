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

    public class Mapping
    {
        public DynamicMapCreator[,] Assigment { get; set; }

        public Mapping()
        {
            Assigment = new DynamicMapCreator[0, 0];
        }

        int Equal(List<List<SignValue>> lstSv, out int equal)
        {
            List<int> lstMax = new List<int>(lstSv.Count);
            for (int k = 0, count = 0; k < lstSv.Count; k++)
            {
                for (int n = 0; n < lstSv.Count; n++)
                    if (lstSv[k] == lstSv[n])
                        count++;
                lstMax.Add(count);
                count = 0;
            }
            int maxSv = 0, maxNum = 0;
            for (int k = 0; k < lstMax.Count; k++)
                if (lstMax[k] >= maxSv)
                {
                    maxSv = lstMax[k];
                    maxNum = k;
                }
            equal = maxSv;
            return maxNum;
        }

        public Assigned Compare(Mapping dmc)
        {
            if (Assigment == null)
                throw new ArgumentException("Mapping.Compare: Assigment не может быть пустым");
            if (Assigment.Length <= 0)
                throw new ArgumentException("Mapping.Compare: Количество прогонов в основном объекте должно быть больше нуля (Count = 0)", "dmc");
            if (dmc == null)
                throw new ArgumentNullException("dmc", "Mapping.Compare: Assigment не может быть пустым (null)");
            if (dmc.Assigment.Length != 1)
                throw new ArgumentException("Mapping.Compare: Количество прогонов в сопоставляемом объекте должно быть равно одному (Count = 1)", "dmc");
            List<List<SignValue>> lst = new List<List<SignValue>>();
            int mx = Assigment.GetLength(0);
            for (int y = 0, my = Assigment.GetLength(1); y < my; y++)
                for (int x = 0; x < mx; x++)
                    lst.Add(Assigment[x, y].Compare(dmc.Assigment[0, 0]));
            int eq;
            int num = Equal(lst, out eq);
            return new Assigned
            {
                Equal = eq,
                X = num % mx,
                Y = num / mx
            };
        }
    }

    public class MapCube
    {
        public MapQuad[,] Cube { get; set; }
        public DynamicAssigment[,] Assign { get; set; }

        public MapCube()
        {
            Cube = new MapQuad[0, 0];
            Assign = new DynamicAssigment[0, 0];
        }

        public Mapping Mapping(List<SignValue> signs)
        {
            DynamicMapCreator[,] assigment = new DynamicMapCreator[Cube.GetLength(0), Cube.GetLength(1)];
            Assign = new DynamicAssigment[Cube.GetLength(0), Cube.GetLength(1)];
            for (int y = 0, my = Cube.GetLength(1); y < my; y++)
                for (int x = 0, mx = Cube.GetLength(0); x < mx; x++)
                    assigment[x, y] = (Assign[x, y] = new DynamicAssigment(Cube[x, y].Signs, 2)).ConvertRange(signs);
            return new Mapping { Assigment = assigment };
        }
    }

    public class MapQuad
    {
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

        public MapCube Cube
        {
            get
            {
                return GetAllQuad(Mx, My);
            }
        }

        public SignValue GetSign(int x, int y)
        {
            if (x < 0 || y < 0 || x >= Mx || y >= My)
                throw new ArgumentException(string.Format("GetSign: некорректные параметры для получения знака: x = {0}, y = {1}", x, y));
            return Signs[(y * Mx) + x];
        }

        MapQuad GetQuad(int x, int y, int sx, int sy)
        {
            if (x >= Mx || y >= My || x < 0 || y < 0 || sx < 0 || sy < 0 || x + sx >= Mx || y + sy >= My)
                throw new ArgumentException(string.Format(@"Некорректные параметры получения квадратной карты: x = {0}, y = {1}, sx = {2}, sy = {3},
Mx = {4}, My = {5}, Signs.Count = {6}", x, y, sx, sy, Mx, My, Signs.Count.ToString()));
            List<SignValue> lst = new List<SignValue>();
            for (; y < sy; y++)
                for (; x < sx; x++)
                    lst.Add(GetSign(x, y));
            return new MapQuad(lst, sx, sy);
        }

        public MapCube GetAllQuad(int sx, int sy)
        {
            if (sx > Mx || sy > My)
                throw new ArgumentException(string.Format("GetAllQuad: некорректные параметры: sx = {0}, Mx = {1}, sy = {2}, My = {3}", sx, Mx, sy, My));
            int mx = (Mx - sx) + 1, my = (My - sy) + 1;
            MapQuad[,] mas = new MapQuad[mx, my];
            for (int y = 0; y < my; y++)
                for (int x = 0; x < mx; x++)
                    mas[x, y] = GetQuad(x, y, sx, sy);
            return new MapCube { Cube = mas };
        }
    }

    public class DynamicMapCreator
    {
        public SortedDictionary<SignValue, DynamicAssigment> Dictionary { get; set; }

        public DynamicMapCreator()
        {
            Dictionary = new SortedDictionary<SignValue, DynamicAssigment>();
        }

        public List<SignValue> Compare(DynamicMapCreator dmc)
        {
            if (dmc == null)
                throw new ArgumentNullException("dmc", "Compare: dmc не может быть null");
            if (Dictionary == null)
                throw new ArgumentNullException("Dictionary", "Compare: Dictionary не может быть null");
            if (dmc.Dictionary == null)
                throw new ArgumentNullException("dmc", "Compare: Dictionary не может быть null");
            if (Dictionary.Count != dmc.Dictionary.Count)
                throw new ArgumentException("dmc", "Compare: Невозможно сопоставить объекты, которые прогонялись с различным количеством знаков");
            if (dmc.Dictionary.Count <= 0)
                throw new ArgumentException("dmc", "Compare: dmc не проходил диагностику");
            if (Dictionary.Count <= 0)
                throw new ArgumentException("Compare: Сопоставляемый объект не проходил диагностику");
            List<SignValue> lstSv = new List<SignValue>(dmc.Dictionary.Count);
            for (int k = 0; k < Dictionary.Count; k++)
                lstSv.Add(Dictionary.ElementAt(k).Value.ConvertedSign.Value - dmc.Dictionary.ElementAt(k).Value.ConvertedSign.Value);
            return lstSv;
        }
    }

    /// <summary>
    /// Представляет хранилище с координатами фрагментов сканируемого изображения и списками выходных знаков после их разбора.
    /// </summary>
    public class DynamicAssigment : ICloneable
    {
        public List<Map> ResearchList { get; set; }
        public SignValue? ConvertedSign { get; private set; }

        public DynamicAssigment()
        {
            ResearchList = new List<Map>();
        }

        /// <summary>
        /// Преобразует изображение в список знаков, размера, меньшего или равного Map.AllMax, чтобы уместить их на карту.
        /// При этом, каждый знак, добавляемый в список, проходит прогон по определённому знаку.
        /// Количество знаков для прогонов зависит от размера изображения.
        /// </summary>
        /// <param name="target">Преобразуемое изображение.</param>
        /// <returns>Возвращает список знаков, размера, меньшего или равного Map.AllMax.</returns>
        public DynamicAssigment(List<SignValue> lst, int count)
        {
            if (count <= 0)
                throw new ArgumentException("Количество объектов, добавляемых на карту, не может быть меньше или равно нулю", "count");
            if (count > Map.AllMax)
                throw new ArgumentException("Количество объектов, добавляемых на карту, не может быть больше Map.Allmax", "count");
            if (lst == null)
                throw new ArgumentNullException("lst", "Список знаков не может быть null");
            if (lst.Count <= 0)
                throw new ArgumentNullException("lst", "Список знаков не может быть пустым");
            List<Map> lstMap = new List<Map>();
            for (int n = 0; n < lst.Count; )
            {
                Map map = new Map();
                for (int k = 0; k < count; k++, n++)
                    map.Add(new MapObject { Sign = lst[n] });
                lstMap.Add(map);
            }
            ResearchList = lstMap;
        }

        public SignValue? Convert(SignValue sv)
        {
            if (ResearchList == null)
                return null;
            if (ResearchList.Count <= 0)
                return null;
            SignValue? res = null;
            foreach (Map map in ResearchList)
            {
                if (map == null)
                    continue;
                if (map.Count <= 0)
                    continue;
                SignValue? sres = (new Processor(map)).Run(sv);
                res = (res == null) ? sres.Value : res.Value.Average(sres.Value);
            }
            ConvertedSign = res;
            return res;
        }

        public DynamicMapCreator ConvertRange(List<SignValue> lst)
        {
            if (lst == null)
                throw new ArgumentNullException("lst", "Список знаков для прогона должен быть указан");
            if (lst.Count <= 0)
                return null;
            SortedDictionary<SignValue, DynamicAssigment> dic = new SortedDictionary<SignValue, DynamicAssigment>();
            foreach (SignValue sv in lst)
            {
                DynamicAssigment d = (DynamicAssigment)Clone();
                d.Convert(sv);
                dic.Add(sv, d);
            }
            return new DynamicMapCreator { Dictionary = dic };
        }

        public object Clone()
        {
            if (ResearchList == null)
                return null;
            if (ResearchList.Count <= 0)
                return new DynamicAssigment();
            List<Map> lst = new List<Map>();
            foreach (Map map in ResearchList)
            {
                if (map == null)
                    lst.Add(null);
                lst.Add((Map)map.Clone());
            }
            return new DynamicAssigment { ResearchList = lst };
        }
    }
}