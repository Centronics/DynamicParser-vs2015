using DynamicProcessor;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DynamicParser
{
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

        public SignValue GetSign(int x, int y)
        {
            if (x < 0 || y < 0 || x >= Mx || y >= My)
                throw new ArgumentException(string.Format("GetSign: некорректные параметры для получения знака: x = {0}, y = {1}", x, y));
            return Signs[(y * Mx) + x];
        }

        public MapQuad GetQuad(int x, int y, int sx, int sy)
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

        public MapQuad[,] GetAllQuad(int sx, int sy)
        {
            if (sx > Mx || sy > My)
                throw new ArgumentException(string.Format("GetAllQuad: некорректные параметры: sx = {0}, Mx = {1}, sy = {2}, My = {3}", sx, Mx, sy, My));
            int mx = (Mx - sx) + 1, my = (My - sy) + 1;
            MapQuad[,] mas = new MapQuad[mx, my];
            for (int y = 0; y < my; y++)
                for (int x = 0; x < mx; x++)
                    mas[x, y] = GetQuad(x, y, sx, sy);
            return mas;
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
        /// Находит наиболее подходящие друг к другу изображения, сравнивая знаки, содержащиеся в списках, и вычисляя, какое изображение соответствует более всего,
        /// т.е. имеет наименьшую разницу в знаках и встречающееся более всего раз. Его номер возвращается. Количество раз, которое оно встретилось,
        /// возвращается в переменную "count".
        /// </summary>
        /// <param name="assigment">.</param>
        /// <returns>Возвращает номер наиболее подходящего изображения.</returns>
        public int Assign(Map assign)
        {
            if (ResearchList == null)
                return -1;
            if (ResearchList.Count <= 0)
                return -1;
            if (assign == null)
                return -1;
            if (assign.Count <= 0)
                return -1;
            int size = (ResearchList[0] == null) ? 0 : ResearchList[0].Count;
            if (size <= 0)
                throw new ArgumentException("Количество объектов на сопоставляемых картах должно быть больше нуля");
            foreach (Map map in ResearchList)
            {
                if (map == null)
                    continue;
                if (map.Count <= 0)
                    throw new ArgumentException("Количество объектов на сопоставляемых картах должно быть больше нуля");
                if (map.Count != size)
                    throw new ArgumentException("Карты, содержащиеся в объекте \"ResearchList\" должны быть с одним и тем же количеством объектов");
            }
            if (assign.Count != size)
                throw new ArgumentException(
                    string.Format("Количество объектов на сопоставляемых картах должно быть одинаковым: {0} сопоставляется с {1}", assign, size));
            List<int> lstSv = new List<int>(size);
            for (int j = 0; j < size; j++)
            {
                int diffSumm = int.MaxValue, diffNum = int.MaxValue;
                for (int k = 0; k < ResearchList.Count; k++)
                {
                    int d = (ResearchList[k][j].Sign - assign[j].Sign).Value;
                    if (d > diffSumm)
                        continue;
                    diffNum = k;
                    diffSumm = d;
                    if (diffSumm <= 0)
                        break;
                }
                lstSv.Add(diffNum);
            }
            List<int> lstMax = new List<int>(size);
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
            return lstSv[maxNum];
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

        public SortedDictionary<SignValue, DynamicAssigment> ConvertRange(List<SignValue> lst)
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
            return dic;
        }

        public static Map ToMap(SortedDictionary<SignValue, DynamicAssigment> lst)
        {
            if (lst == null)
                throw new ArgumentNullException("lst", "ToMap: lst не может быть null");
            if (lst.Count > Map.AllMax)
                throw new ArgumentException(string.Format("ToMap: Количество знаков в списке больше максимального размера карты: {0}", Map.AllMax, "lst"));
            if (lst.Count <= 0)
                return null;
            Map map = new Map();
            foreach (DynamicAssigment ds in lst.Values)
            {
                if (ds == null)
                    continue;
                if (ds.ConvertedSign == null)
                    throw new ArgumentException("ToMap: ConvertedSign должен быть указан");
                map.Add(new MapObject { Sign = ds.ConvertedSign.Value });
            }
            return map;
        }

        public static List<SignValue> GetSigns(int count)
        {
            if (count > SignValue.MaxValue.Value || count < SignValue.MinValue.Value)
                throw new ArgumentException("Параметр находится вне допустимого диапазона", "count");
            List<SignValue> lst = new List<SignValue>();
            int plus = SignValue.MaxValue.Value / count;
            for (int k = 0; k < count; k++)
                lst.Add(new SignValue(plus * k));
            return lst;
        }

        /// <summary>
        /// Преобразует изображение в список знаков, размера, меньшего или равного Map.AllMax, чтобы уместить их на карту.
        /// При этом, каждый знак, добавляемый в список, проходит прогон по определённому знаку.
        /// Количество знаков для прогонов зависит от размера изображения.
        /// </summary>
        /// <param name="target">Преобразуемое изображение.</param>
        /// <returns>Возвращает список знаков, размера, меньшего или равного Map.AllMax.</returns>
        public static DynamicAssigment GetMaps(List<SignValue> lst, int count)
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
            return new DynamicAssigment { ResearchList = lstMap };
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