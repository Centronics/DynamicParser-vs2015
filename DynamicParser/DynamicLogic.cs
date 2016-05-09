﻿using DynamicProcessor;
using System;
using System.Collections.Generic;

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

        public Map Convert()
        {
            if (ResearchList == null)
                return null;
            if (ResearchList.Count <= 0)
                return null;
            Map curMap = new Map(); SignValue? sv = null;
            for (int n = 0, plus = SignValue.MaxValue.Value / Map.AllMax, p = 0; p < Map.AllMax; n += plus, p++)
            {
                foreach (Map map in ResearchList)
                {
                    if (map == null)
                        continue;
                    if (map.Count <= 0)
                        continue;
                    Processor proc = new Processor((Map)map.Clone());
                    SignValue? sv1 = proc.Run(new SignValue(n));
                    sv = (sv == null) ? sv1.Value : sv.Value.Average(sv1.Value);
                }
                if (sv == null)
                    break;
                curMap.Add(new MapObject { Sign = sv.Value });
                sv = null;
            }
            return curMap;
        }
    }
}