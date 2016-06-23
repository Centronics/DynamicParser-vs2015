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
        public List<List<SignValue>> _lists { get; private set; }
        public List<SignValue> _compares { get; private set; }

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
            _lists = new List<List<SignValue>>();
            _compares = new List<SignValue>();
        }

        public bool this[SignValue sign]//должна получать массив знаков??? или должно быть соответствие по всем версиям... может, надо проверять карты целиком?
        {
            get
            {

            }
        }

        public int Update(SignValue sv)
        {
            List<SignValue> mapTested = new List<SignValue>(_maps.Count);
            foreach (Map map in _maps)
                mapTested.Add((new Processor((Map)map.Clone())).Run(sv).Value);
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

        /// <summary>
        /// Создаёт карту, полученную в результате прогона карты указанного изображения по ряду знаков, количество которых зависит от размера изображения.
        /// </summary>
        /// <param name="target">Разбираемое изображение.</param>
        /// <returns>Возвращает карту, полученную в результате прогона карты изображения по ряду знаков.</returns>
        public static Map GetMap(List<SignValue> target)
        {
            if (target == null)
                return null;
            Map map = new Map();
            GetSign(target).ForEach(it => map.Add(new MapObject { Sign = it }));
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
        static List<SignValue> GetSign(List<SignValue> target)
        {
            int plus = SignValue.MaxValue.Value / Map.AllMax; SignValue sv = SignValue.MinValue;
            while (target.Count > Map.AllMax)
            {
                List<SignValue> nlst = new List<SignValue>();
                int k = 0; SignValue? val = null;
                while ((val = Parse(target, ref k, sv)) != null)
                    nlst.Add(val.Value);
                sv = new SignValue(sv + plus);
                target = nlst;
            }
            return target;
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