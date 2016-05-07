using DynamicProcessor;
using System;
using System.Collections.Generic;

namespace DynamicParser
{
    public struct AssigmentResult
    {
        public int Difference;
        public int Number;

        public bool IsActual
        {
            get
            {
                return (Difference >= 0 && Number >= 0);
            }
        }
    }

    /// <summary>
    /// Представляет хранилище с координатами фрагментов сканируемого изображения и списками выходных знаков после их разбора.
    /// </summary>
    public class DynamicAssigment
    {
        public List<Map> ResearchList { get; set; }

        /// <summary>
        /// Находит наиболее подходящие друг к другу изображения, сравнивая знаки, содержащиеся в списках, и вычисляя, какое изображение соответствует более всего,
        /// т.е. имеет наименьшую разницу в знаках и встречающееся более всего раз. Его номер возвращается. Количество раз, которое оно встретилось,
        /// возвращается в переменную "count".
        /// </summary>
        /// <param name="assigment">.</param>
        /// <returns>Возвращает номер наиболее подходящего изображения.</returns>
        public AssigmentResult Assign(Map assign)
        {
            if (ResearchList == null)
                return new AssigmentResult { Difference = -1, Number = -1 };
            if (ResearchList.Count <= 0)
                return new AssigmentResult { Difference = -1, Number = -1 };
            if (assign == null)
                return new AssigmentResult { Difference = -1, Number = -1 };
            if (assign.Count <= 0)
                return new AssigmentResult { Difference = -1, Number = -1 };
            int size = ResearchList[0].Count;
            foreach (Map map in ResearchList)
                if (map.Count != size)
                    throw new ArgumentException("Карты, содержащиеся в объекте \"ResearchList\" должны быть с одним и тем же количеством объектов");
            if (assign.Count != size)
                throw new ArgumentException(
                    string.Format("Количество объектов на сопоставляемых картах должно быть одинаковым: {0} сопоставляется с {1}", assign, size));
            int diff = int.MaxValue, number = int.MaxValue;
            for (int k = 0; k < ResearchList.Count; k++)
            {
                int diffSumm = 0;
                for (int n = 0; n < ResearchList[k].Count; n++)
                    diffSumm += (ResearchList[k][n].Sign - assign[n].Sign).Value;
                if (diffSumm > diff)
                    continue;
                diff = diffSumm;
                number = k;
            }
            return new AssigmentResult { Difference = diff, Number = number };
        }

        public static Map Convert(List<Map> lstConvert)
        {
            if (lstConvert == null)
                return null;
            if (lstConvert.Count <= 0)
                return null;
            Map curMap = new Map(); SignValue? sv = null;
            for (int n = 0, plus = SignValue.MaxValue.Value / Map.AllMax, p = 0; p < Map.AllMax; n += plus, p++)
            {
                for (int k = 0; k < lstConvert.Count; k++)
                {
                    Map map = (Map)lstConvert[k].Clone();
                    Processor proc = new Processor(map);
                    SignValue? sv1 = proc.Run(new SignValue(n));
                    if (sv1 == null)
                        return null;
                    sv = (sv == null) ? sv1.Value : sv.Value.Average(sv1.Value);
                }
                curMap.Add(new MapObject { Sign = sv.Value });
                sv = null;
            }
            return curMap;
        }

        /// <summary>
        /// Осуществляет разбор карт по количеству знаков, равному Map.AllMax. Таким образом, формируются списки выходных знаков.
        /// </summary>
        /// <returns></returns>
        public static Map Combine(Map convert)
        {
            if (convert == null)
                return null;
            if (convert.Count <= 0)
                return null;
            Processor proc = new Processor(convert);
            Map map = new Map();
            for (int k = 0, plus = SignValue.MaxValue.Value / Map.AllMax, p = 0; p < Map.AllMax; k += plus, p++)
                map.Add(new MapObject { Sign = proc.Run(new SignValue(k)).Value });
            map.ObjectNumeration();
            return map;
        }
    }
}