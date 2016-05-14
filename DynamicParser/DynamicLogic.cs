using DynamicProcessor;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DynamicParser
{
    /// <summary>
    /// Представляет хранилище с координатами фрагментов сканируемого изображения и списками выходных знаков после их разбора.
    /// </summary>
    public class DynamicAssigment
    {
        public List<Map> ResearchList { get; set; }

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

        public SignValue? Convert(SignValue sv, int count)
        {
            if (ResearchList == null)
                return null;
            if (ResearchList.Count <= 0)
                return null;
            SignValue? res = null;
            for (int k = 0; k < count; k++)
                foreach (Map map in ResearchList)
                {
                    if (map == null)
                        continue;
                    if (map.Count <= 0)
                        continue;
                    SignValue? sres = (new Processor(map)).Run(sv);
                    res = (res == null) ? sres.Value : res.Value.Average(sres.Value);
                }
            return res;
        }

        /// <summary>
        /// Преобразует изображение в список знаков, размера, меньшего или равного Map.AllMax, чтобы уместить их на карту.
        /// При этом, каждый знак, добавляемый в список, проходит прогон по определённому знаку.
        /// Количество знаков для прогонов зависит от размера изображения.
        /// </summary>
        /// <param name="target">Преобразуемое изображение.</param>
        /// <returns>Возвращает список знаков, размера, меньшего или равного Map.AllMax.</returns>
        public static List<Map> GetMaps(List<SignValue> lst, SignValue? sv, int count)
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
                    map.Add(new MapObject { Sign = (sv == null) ? lst[n] : sv.Value.Average(lst[n]) });
                lstMap.Add(map);
            }
            return lstMap;
        }
    }
}