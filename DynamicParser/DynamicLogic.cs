using DynamicProcessor;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DynamicParser
{
    public struct Assigned
    {
        public int X, Y;
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

        public Map GetQuadMap()
        {
            return GetQuadMap(0, 0, Mx, My);
        }

        public Map GetQuadMap(int x, int y, int sx, int sy)
        {
            int lx = x + sx, ly = y + sy;
            if (x >= Mx || y >= My || x < 0 || y < 0 || sx < 0 || sy < 0 || lx > Mx || ly > My)
                return null;
            List<SignValue> lst = new List<SignValue>();
            for (; y < ly; y++)
                for (int px = x; px < lx; px++)
                    lst.Add(GetSign(px, y));
            return GetMap(lst);
        }

        public Assigned GetAllQuad(int sx, int sy, Map cmp)
        {
            if (sx > Mx || sy > My)
                throw new ArgumentException(string.Format("GetAllQuad: некорректные параметры: sx = {0}, Mx = {1}, sy = {2}, My = {3}", sx, Mx, sy, My));
            int mx = (Mx - sx) + 1, my = (My - sy) + 1;
            Compared?[] masAssigned = new Compared?[Map.AllMax];
            Map mapTested = MapTest(cmp, true);
            for (int y = 0; y < my; y++)
                for (int x = 0; x < mx; x++)
                    Compare(MapTest(GetQuadMap(x, y, sx, sy), false), mapTested, masAssigned, x, y);
            return Equal(masAssigned);
        }

        static void Compare(Map Signs, Map mapTested, Compared?[] assigned, int x, int y)
        {
            if (mapTested == null)
                throw new ArgumentNullException("dmc", "Compare: dmc не может быть null");
            if (mapTested.Count != Map.AllMax)
                throw new ArgumentException("Compare: Сопоставляемый объект не проходил диагностику", "mt");
            if (Signs == null)
                throw new ArgumentNullException("Dictionary", "Compare: Dictionary не может быть null");
            if (Signs.Count != Map.AllMax)
                throw new ArgumentException("Compare: Сопоставляемый объект не проходил диагностику", "Signs");
            if (assigned == null)
                throw new ArgumentNullException("assigned", "Compare: Список сопоставляемых объектов должен быть указан");
            if (assigned.Length != Map.AllMax)
                throw new ArgumentException("Compare: Массив сопоставляемых знаков не может быть длины, отличной от Map.AllMax", "assigned");
            for (int k = 0; k < Signs.Count; k++)
            {
                SignValue sv = Signs[k].Sign - mapTested[k].Sign;
                if (assigned[k] == null)
                {
                    assigned[k] = new Compared { Difference = sv, X = x, Y = y };
                    continue;
                }
                if (assigned[k].Value.Difference < sv)
                    continue;
                assigned[k] = new Compared { Difference = sv, X = x, Y = y };
            }
        }

        static Assigned Equal(Compared?[] assigned)
        {
            if (assigned == null)
                throw new ArgumentException("Equal: assigned = null");
            if (assigned.Length != Map.AllMax)
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

        static Map MapTest(Map map, bool clone)
        {
            if (map == null)
                throw new ArgumentNullException("map", "Compare: map не может быть null");
            if (map.Count <= 0)
                throw new ArgumentException("Compare: Сопоставляемый объект не проходил диагностику", "map");
            Map svMap = new Map();
            Processor ce = new Processor(clone ? (Map)map.Clone() : map);
            for (int k = 0, plus = SignValue.MaxValue.Value / Map.AllMax, p = 0; p < Map.AllMax; k += plus, p++)
                svMap.Add(new MapObject { Sign = ce.Run(new SignValue(k)).Value });
            return svMap;
        }

        /// <summary>
        /// Создаёт карту, полученную в результате прогона карты указанного изображения по ряду знаков, количество которых зависит от размера изображения.
        /// </summary>
        /// <param name="target">Разбираемое изображение.</param>
        /// <returns>Возвращает карту, полученную в результате прогона карты изображения по ряду знаков.</returns>
        static Map GetMap(List<SignValue> lst)
        {
            if (lst == null)
                return null;
            Map map = new Map();
            GetSign(lst).ForEach(it => map.Add(new MapObject { Sign = it }));
            map.ObjectNumeration();
            return map;
        }

        /// <summary>
        /// Преобразует изображение в список знаков, размера, меньшего или равного Map.AllMax, чтобы уместить их на карту.
        /// При этом, каждый знак, добавляемый в список, проходит прогон по определённому знаку.
        /// Количество знаков для прогонов зависит от размера изображения.
        /// </summary>
        /// <param name="target">Преобразуемое изображение.</param>
        /// <returns>Возвращает список знаков, размера, меньшего или равного Map.AllMax.</returns>
        static List<SignValue> GetSign(List<SignValue> lst)
        {
            int plus = SignValue.MaxValue.Value / Map.AllMax; SignValue sv = SignValue.MinValue;
            while (lst.Count > Map.AllMax)
            {
                List<SignValue> nlst = new List<SignValue>();
                int k = 0; SignValue? val = null;
                while ((val = Parse(lst, ref k, sv)) != null)
                    nlst.Add(val.Value);
                sv = new SignValue(sv + plus);
                lst = nlst;
            }
            return lst;
        }

        /// <summary>
        /// Сжимает указанный список знаков по две позиции за каждый раз.
        /// </summary>
        /// <param name="target">Преобразуемый список.</param>
        /// <param name="k">Стартовая позиция для преобразования.</param>
        /// <param name="sv">Знак для преобразования.</param>
        /// <returns>Возвращает знак после преобразования или null в случае окончания операции.</returns>
        static SignValue? Parse(List<SignValue> target, ref int k, SignValue sv)
        {
            Map map = new Map();
            int mx = (target.Count % 2 != 0) ? target.Count - 1 : target.Count;
            for (int x = 0; x < 2 && k < mx; x++, k++)
                map.Add(new MapObject { Sign = target[k] });
            Processor ce = new Processor(map);
            return ce.Run(sv);
        }
    }
}