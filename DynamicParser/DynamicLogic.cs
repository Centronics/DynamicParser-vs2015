using DynamicProcessor;
using System.Collections.Generic;
using System;
using System.IO;

namespace DynamicParser
{
    /// <summary>
    /// Представляет хранилище с координатами фрагментов сканируемого изображения и списками выходных знаков после их разбора.
    /// </summary>
    public class DynamicAssigment
    {
        public List<List<SignValue>> Objects;

        public List<SignValue> Img1;
        /// <summary>
        /// Список знаков разобранного фрагмента, относящегося к второму изображению.
        /// </summary>
        public List<SignValue> Img2;
        /// <summary>
        /// Список знаков разобранного фрагмента, относящегося к третьему изображению.
        /// </summary>
        public List<SignValue> img3;

        public List<Map> ConverterList;

        /// <summary>
        /// Находит наиболее подходящие друг к другу изображения, сравнивая знаки, содержащиеся в списках, и вычисляя, какое изображение соответствует более всего,
        /// т.е. имеет наименьшую разницу в знаках и встречающееся более всего раз. Его номер возвращается. Количество раз, которое оно встретилось,
        /// возвращается в переменную "count".
        /// </summary>
        /// <param name="assigment">.</param>
        /// <param name="count">Степень соответствия. Чем больше, тем лучше.</param>
        /// <returns>Возвращает номер наиболее подходящего изображения.</returns>
        public byte GetBySign(DynamicAssigment assigment, out int count)
        {
            count = 0;
            if (Img1 == null && Img2 == null && img3 == null)
                return 0;
            int i1 = 0, i2 = 0, i3 = 0;
            for (int k = 0; k < Map.AllMax; k++)
            {
                SignValue sv1 = SignValue.MaxValue, sv2 = SignValue.MaxValue, sv3 = SignValue.MaxValue;
                if (Img1 != null)
                    sv1 = Img1[k] - lst1[k];
                if (Img2 != null)
                    sv2 = Img2[k] - lst2[k];
                if (img3 != null)
                    sv3 = img3[k] - lst3[3];
                if (sv1 < sv2 && sv1 < sv3)
                {
                    i1++;
                    continue;
                }
                if (sv2 < sv1 && sv2 < sv3)
                {
                    i2++;
                    continue;
                }
                i3++;
            }
            if (i1 > i2 && i1 > i3)
            {
                count = i1;
                return 1;
            }
            if (i2 > i1 && i2 > i3)
            {
                count = i2;
                return 2;
            }
            count = i3;
            return 3;
        }

        /// <summary>
        /// Находит наиболее соответствующие друг другу изображения: фрагмент исследуемого изображения и входное изображение.
        /// </summary>
        /// <param name="img">Список знаков, полученных в результате разбора фрагментов сканируемого изображения.</param>
        /// <param name="sv1">Список знаков, полученных в результате разбора первого изображения.</param>
        /// <param name="sv2">Список знаков, полученных в результате разбора второго изображения.</param>
        /// <param name="sv3">Список знаков, полученных в результате разбора третьего изображения.</param>
        /// <param name="im1">Результат поиска первого изображения на сканируемом.</param>
        /// <param name="im2">Результат поиска второго изображения на сканируемом.</param>
        /// <param name="im3">Результат поиска третьего изображения на сканируемом.</param>
        public void FindMin(DynamicAssigment assign, out Images? im1, out Images? im2, out Images? im3)
        {
            im1 = im2 = im3 = null;
            for (int k = 0, s1 = 0, s2 = 0, s3 = 0; k < img.Count; k++)
            {
                int count;
                switch (img[k].GetBySign(sv1, sv2, sv3, out count))
                {
                    case 1:
                        if (count <= s1)
                            break;
                        s1 = count;
                        im1 = img[k];
                        break;
                    case 2:
                        if (count <= s2)
                            break;
                        s2 = count;
                        im2 = img[k];
                        break;
                    case 3:
                        if (count <= s3)
                            break;
                        s3 = count;
                        im3 = img[k];
                        break;
                    default:
                        throw new Exception("Неизвестный тип объекта");
                }
            }
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