using DynamicProcessor;
using System;
using System.Collections.Generic;

namespace DynamicParser
{
    public class MapContainer
    {
        struct Compared
        {
            public int N;
            public SignValue Difference;
        }

        List<Map> _maps;
        List<List<SignValue>> _lists = new List<List<SignValue>>();

        public MapContainer(int ctxLen, List<Map> svs)
        {
            if (ctxLen <= 0)
                throw new ArgumentException(string.Format("MapContainer: Параметр длины контекста должен быть больше или равен нулю: {0}", ctxLen), "ctxLen");
            if (svs.Count <= 0)
                throw new ArgumentException(string.Format("MapContainer: Параметр разрядности контекста должен быть больше или равен нулю: {0}", svs.Count), "svs");
            foreach (Map map in svs)
                if (map.Count != ctxLen)
                    throw new ArgumentException(string.Format("MapContainer: Размеры карт для прогона не соответствуют заявленным: {0} против {1}", map.Count, ctxLen), "svs");
            _maps = svs;
        }

        public int Update(SignValue sv)
        {
            List<SignValue> mapTested = new List<SignValue>(_maps.Count);
            foreach (Map map in _maps)
                mapTested.Add((new Processor(map)).Run(sv).Value);
            try
            {
                if (_lists.Count <= 0)
                    return -1;
                Compared?[] assigned = new Compared?[_maps.Count];
                for (int n = 0; n < _lists.Count; n++)
                    Compare(_lists[n], mapTested, assigned, n);
                return Equal(assigned);
            }
            finally
            {
                _lists.Add(mapTested);
            }
        }

        static void Compare(List<SignValue> signs, List<SignValue> mapTested, Compared?[] assigned, int n)
        {
            if (mapTested == null)
                throw new ArgumentNullException("mapTested", "Compare: mapTested не может быть null");
            if (signs == null)
                throw new ArgumentNullException("Dictionary", "Compare: Dictionary не может быть null");
            if (signs.Count != mapTested.Count)
                throw new ArgumentException("Compare: Сопоставляемый объект не проходил диагностику", "Signs");
            if (assigned == null)
                throw new ArgumentNullException("assigned", "Compare: Список сопоставляемых объектов должен быть указан");
            if (assigned.Length != mapTested.Count)
                throw new ArgumentException("Compare: Массив сопоставляемых знаков не может быть длины, отличной от Map.AllMax", "assigned");
            for (int k = 0; k < signs.Count; k++)
            {
                SignValue sv = signs[k] - mapTested[k];
                if (assigned[k] == null)
                {
                    assigned[k] = new Compared { Difference = sv, N = n };
                    continue;
                }
                if (assigned[k].Value.Difference < sv)
                    continue;
                assigned[k] = new Compared { Difference = sv, N = n };
            }
        }

        static int Equal(Compared?[] assigned)
        {
            if (assigned == null)
                throw new ArgumentException("Equal: assigned = null");
            if (assigned.Length != Map.AllMax)
                throw new ArgumentException("Equal: Список сопоставляемых объектов должен содержать хотя бы один элемент", "assigned");
            List<int> lstMax = new List<int>(assigned.Length);
            for (int k = 0, count = 0; k < assigned.Length; k++, count = 0)
            {
                for (int n = 0; n < assigned.Length; n++)
                    if (assigned[k].Value.N == assigned[n].Value.N && assigned[k].Value.Y == assigned[n].Value.Y)
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
            return assigned[maxNum].Value.N;
        }
    }

    public struct Assigned
    {
        public int X, Y, Width, Height, MaxNumber;
        public MapQuad Mapq;
    }

    public class MapQuad
    {


        public int Mx { get; private set; }
        public int My { get; private set; }
        public List<SignValue> Signs { get; private set; }
        public List<MapQuad> MapLayer { get; private set; }
        public object AssignedObject { get; set; }

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
            MapLayer = new List<MapQuad>();
        }

        public void GetAllQuad(int sx, int sy, MapQuad cmps)
        {
            if (sx > Mx || sy > My || sx <= 0 || sy <= 0)
                throw new ArgumentException(string.Format("GetAllQuad: некорректные параметры: sx = {0}, Mx = {1}, sy = {2}, My = {3}", sx, Mx, sy, My));
            if (cmps == null)
                throw new ArgumentNullException("cmps", "GetAllQuad: Сопоставляемые карты должны быть указаны (null)");
            if (cmps.MapLayer.Count <= 0)
                throw new ArgumentException("GetAllQuad: Сопоставляемые карты должны быть указаны (Count = 0)", "cmps");
            if (cmps.Mx != Mx)
                throw new ArgumentException(string.Format("GetAllQuad: Сопоставляемые карты должны быть одинаковых размеров основных карт (Mx): {0} против {1}",
                    cmps.Mx, Mx), "cmps");
            if (cmps.My != My)
                throw new ArgumentException(string.Format("GetAllQuad: Сопоставляемые карты должны быть одинаковых размеров основных карт (My): {0} против {1}",
                    cmps.My, My), "cmps");
            if ((Mx % sx) != 0)
                throw new ArgumentException("GetAllQuad: Размеры загружаемых карт должны быть кратны размерам основной карты", "sx");
            if ((My % sy) != 0)
                throw new ArgumentException("GetAllQuad: Размеры загружаемых карт должны быть кратны размерам основной карты", "sy");
            foreach (MapQuad mq in cmps.MapLayer)
            {
                if (mq == null)
                    throw new ArgumentNullException("cmps", "GetAllQuad: Массив не должен содержать пустые карты");
                if (mq.Mx != sx || mq.My != sy)
                    throw new ArgumentOutOfRangeException("cmps", "GetAllQuad: Анализируемые карты не могут иметь различные размеры");
            }
            for (int k = 0; k < cmps.MapLayer.Count; k++)
            {
                Compared?[] comp = new Compared?[Signs.Count / (cmps.Mx * cmps.My)];

            }
        }


    }
}