using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using DynamicProcessor;

namespace DynamicParser
{
    /// <summary>
    /// Процессор, выполняющий поиск соответствующих указанных карт на рабочем поле.
    /// Работает по принципу поисковика наиболее подходящей карты для каждого места на рабочем поле.
    /// Поиск производится по всему полю.
    /// </summary>
    public sealed class Processor
    {
        /// <summary>
        /// Разница между процентами соответствия объектов, которая позволяет считать их равными.
        /// </summary>
        const double DiffEqual = 0.01;

        /// <summary>
        /// Рабочее поле, на котором производится поиск требумых данных.
        /// </summary>
        readonly SignValue[,] _bitmap;

        /// <summary>
        /// Название текущей карты.
        /// </summary>
        public string Tag { get; }

        /// <summary>
        /// Ширина.
        /// </summary>
        public int Width => _bitmap.GetLength(0);

        /// <summary>
        /// Высота.
        /// </summary>
        public int Height => _bitmap.GetLength(1);

        /// <summary>
        /// Длина. Вычисляется как: Width * Height.
        /// </summary>
        public int Length => Width * Height;

        /// <summary>
        /// Получает размер карты.
        /// </summary>
        public Size Size => new Size(Width, Height);

        /// <summary>
        /// Извлекает объект рабочего поля.
        /// </summary>
        /// <param name="x">Координата X.</param>
        /// <param name="y">Координата Y.</param>
        /// <returns>Возвращает запрашиваемый объект рабочего поля.</returns>
        public SignValue this[int x, int y] => _bitmap[x, y];

        /// <summary>
        /// Генерирует регион, равный по размеру текущему рабочему полю.
        /// </summary>
        public Region CurrentRegion => new Region(Width, Height);

        /// <summary>
        /// Генерирует поле для поиска обозначенных регионов на текущем рабочем поле.
        /// </summary>
        public Attacher CurrentAttacher => new Attacher(Width, Height);

        /// <summary>
        /// Предназначен для загрузки изображения как рабочего поля.
        /// </summary>
        /// <param name="btm">Загружаемое изображение.</param>
        /// <param name="tag">Название изображения.</param>
        public Processor(Bitmap btm, string tag)
        {
            if (btm == null)
                throw new ArgumentNullException(nameof(btm), $"{nameof(Processor)}: Изображение не может быть равно null.");
            if (btm.Width <= 0)
                throw new ArgumentException($"{nameof(Processor)}: Ширина изображения не может быть меньше или равна нолю ({btm.Width}).", nameof(btm));
            if (btm.Height <= 0)
                throw new ArgumentException($"{nameof(Processor)}: Высота изображения не может быть меньше или равна нолю ({btm.Height}).", nameof(btm));
            if (string.IsNullOrWhiteSpace(tag))
                throw new ArgumentNullException(nameof(tag), $"{nameof(Processor)}: {nameof(tag)} не может быть равен null.");
            _bitmap = new SignValue[btm.Width, btm.Height];
            //btm.Save(@"D:\tmp.bmp");
            using (FileStream fs = new FileStream(@"D:\tmp.bin", FileMode.Create))
            {
                for (int y = 0; y < btm.Height; y++)
                for (int x = 0; x < btm.Width; x++)
                {
                    Color col = btm.GetPixel(x, y);
                    fs.Write(BitConverter.GetBytes(col.ToArgb()), 0, 4);
                    _bitmap[x, y] = new SignValue(col);
                }
            }

            using (FileStream fs = new FileStream(@"D:\tmp.bin", FileMode.Create))
            {
                for (int y = 0; y < btm.Height; y++)
                    for (int x = 0; x < btm.Width; x++)
                    {
                        fs.Write(BitConverter.GetBytes(_bitmap[x,y].Value),0,4);
                    }
            }

            Tag = tag.Trim();
        }

        /// <summary>
        /// Загружает указанную карту, создавая внутреннюю её копию.
        /// </summary>
        /// <param name="btm">Загружаемая карта.</param>
        /// <param name="tag">Название карты.</param>
        public Processor(SignValue[,] btm, string tag)
        {
            if (btm == null)
                throw new ArgumentNullException(nameof(btm), $"{nameof(Processor)}: Загружаемая карта не может быть равна null.");
            int w = btm.GetLength(0), h = btm.GetLength(1);
            if (w <= 0)
                throw new ArgumentException($"{nameof(Processor)}: Ширина загружаемой карты не может быть меньше или равна нолю ({w}).", nameof(btm));
            if (h <= 0)
                throw new ArgumentException($"{nameof(Processor)}: Высота загружаемой карты не может быть меньше или равна нолю ({h}).", nameof(btm));
            if (string.IsNullOrWhiteSpace(tag))
                throw new ArgumentNullException(nameof(tag), $"{nameof(Processor)}: {nameof(tag)} не может быть равен null.");
            _bitmap = new SignValue[w, h];
            for (int y = 0; y < h; y++)
                for (int x = 0; x < w; x++)
                    _bitmap[x, y] = btm[x, y];
            Tag = tag;
        }

        /// <summary>
        /// Загружает указанный массив, создавая внутреннюю его копию.
        /// </summary>
        /// <param name="btm">Загружаемый массив.</param>
        /// <param name="tag">Название карты.</param>
        public Processor(SignValue[] btm, string tag)
        {
            if (btm == null)
                throw new ArgumentNullException(nameof(btm), $"{nameof(Processor)}: Загружаемая карта не может быть равна null.");
            if (btm.Length <= 0)
                throw new ArgumentException($"{nameof(Processor)}: Ширина загружаемой карты не может быть меньше или равна нолю ({btm.Length}).", nameof(btm));
            if (string.IsNullOrWhiteSpace(tag))
                throw new ArgumentNullException(nameof(tag), $"{nameof(Processor)}: {nameof(tag)} не может быть равен null.");
            _bitmap = new SignValue[btm.Length, 1];
            for (int x = 0; x < btm.Length; x++)
                _bitmap[x, 0] = btm[x];
            Tag = tag;
        }

        /// <summary>
        /// Получает строковое представление текущего экземпляра, которое является значением свойства Tag.
        /// </summary>
        /// <returns>Возвращает строковое представление текущего экземпляра, которое является значением свойства Tag.</returns>
        public override string ToString()
        {
            return Tag;
        }

        /// <summary>
        /// Подтверждает, является ли карта с указанным индексом наиболее подходящей в данной точке.
        /// </summary>
        /// <param name="db">Массив карт для поиска.</param>
        /// <param name="x">Координата X.</param>
        /// <param name="y">Координата Y.</param>
        /// <param name="number">Индекс карты, которую требуется проверить.</param>
        /// <returns>Возвращает true в случае, если number входит в список наиболее подходящих карт.</returns>
        static bool GetMinIndex(IDictionary<int, SignValue[,]> db, int x, int y, int number)
        {
            if (x < 0)
                throw new ArgumentException($"{nameof(GetMinIndex)}: Координата x меньше ноля ({x}).", nameof(x));
            if (y < 0)
                throw new ArgumentException($"{nameof(GetMinIndex)}: Координата y меньше ноля ({y}).", nameof(y));
            if (number < 0)
                throw new ArgumentException($"{nameof(GetMinIndex)}: Индекс проверяемой карты меньше ноля ({number}).", nameof(number));
            if (db == null)
                throw new ArgumentNullException(nameof(db), $"{nameof(GetMinIndex)}: Массив карт для поиска равен null.");
            if (db.Count <= 0)
                throw new ArgumentException($"{nameof(GetMinIndex)}: Длина массива карт для поиска равна нолю ({db.Count}).", nameof(db));
            SignValue ind = SignValue.MaxValue;
            int n = -1;
            foreach (int key in db.Keys)
            {
                SignValue[,] mas = db[key];
                if (ind < mas[x, y]) continue;
                ind = mas[x, y];
                n = key;
            }
            return n >= 0 && db.Keys.Where(key => ind == db[key][x, y]).ToArray().Contains(number);
        }

        /// <summary>
        /// Находит все карты, которые имеют тот же процент соответствия, что и заданный.
        /// </summary>
        /// <param name="lst">Список процентного соответствия карт.</param>
        /// <param name="perc">Искомый процент соответствия.</param>
        /// <returns>Возвращает индексы карт, которые имеют тот же процент соответствия.
        /// Индексы соответствуют поисковому массиву.</returns>
        static IEnumerable<int> GetMaxIndex(IList<double> lst, double perc)
        {
            if (lst == null)
                throw new ArgumentNullException(nameof(lst), $"{nameof(GetMaxIndex)}: Список процентного соответствия карт равен null.");
            if (lst.Count <= 0)
                yield break;
            for (int k = 0; k < lst.Count; k++)
                if (Math.Abs(lst[k] - perc) < DiffEqual)
                    yield return k;
        }

        /// <summary>
        /// Находит соответствие между заданными картами и рабочей областью, производя поиск по ней.
        /// </summary>
        /// <param name="processors">Все остальные карты. Может быть null.</param>
        /// <returns>Возвращает SearchResults, содержащий поле с результатами поиска.</returns>
        public SearchResults GetEqual(params Processor[] processors)
        {
            return GetEqual(new ProcessorContainer(processors));
        }

        /// <summary>
        /// Находит соответствие между заданным массивом карт и рабочей областью в многопоточном режиме.
        /// </summary>
        /// <param name="processors">Массив искомых карт.</param>
        /// <returns>Возвращает массив SearchResults, в котором номер карты в исходном массиве соответствует её номеру в массиве результата.</returns>
        public SearchResults[] GetEqual(ICollection<ProcessorContainer> processors)
        {
            if (processors == null)
                throw new ArgumentNullException();
            if (processors.Count <= 0)
                throw new ArgumentException();
            SearchResults[] sr = new SearchResults[processors.Count];
            string errString = string.Empty;
            bool exThrown = false;
            Parallel.For(0, processors.Count, (k, state) =>
            {
                try
                {
                    sr[k] = GetEqual(processors.ElementAt(k));
                }
                catch (Exception ex)
                {
                    try
                    {
                        state.Stop();
                        errString = ex.Message;
                        exThrown = true;
                    }
                    catch
                    {
                        //ignored
                    }
                }
            });
            if (exThrown)
                throw new Exception(errString);
            return sr;
        }

        /// <summary>
        /// Находит соответствие между заданным массивом карт и рабочей областью в многопоточном режиме.
        /// </summary>
        /// <param name="pc">Массив искомых карт.</param>
        /// <returns>Возвращает массив SearchResults, в котором номер карты в исходном массиве соответствует её номеру в массиве результата.</returns>
        public SearchResults[] GetEqual(params ProcessorContainer[] pc)
        {
            return GetEqual((ICollection<ProcessorContainer>)pc);
        }

        /// <summary>
        /// Производит поиск указанных карт на рабочей области и возвращает результат в виде класса SearchResults,
        /// содержащего поле с результатами поиска.
        /// </summary>
        /// <param name="prc">Массив карт для поиска.</param>
        /// <returns>Возвращает SearchResults, содержащий поле с результатами поиска.</returns>
        public SearchResults GetEqual(ProcessorContainer prc)
        {
            if (prc == null)
                throw new ArgumentNullException(nameof(prc), $"{nameof(GetEqual)}: Массив карт для поиска равен null.");
            if (prc.Width > Width)
                throw new ArgumentException($"{nameof(GetEqual)}: Карты для поиска превышают существующее поле по ширине ({prc.Width} > {Width}).", nameof(prc));
            if (prc.Height > Height)
                throw new ArgumentException($"{nameof(GetEqual)}: Карты для поиска превышают существующее поле по высоте ({prc.Height} > {Height}).", nameof(prc));
            if (prc.Count <= 0)
                throw new ArgumentException($"{nameof(GetEqual)}: Массив карт для поиска ничего не содержит.", nameof(prc));
            SearchResults sr = new SearchResults(Width, Height, prc.Width, prc.Height);
            string errString = string.Empty;
            bool exThrown = false;
            Parallel.For(0, Height, (y1, stateHeightMain) =>
            {
                try
                {
                    Parallel.For(0, Width, (x1, stateWidthMain) =>
                    {
                        try
                        {
                            ConcurrentDictionary<int, SignValue[,]> procPercent = new ConcurrentDictionary<int, SignValue[,]>();
                            Parallel.For(0, prc.Count, (j, stateCountMap) =>
                            {
                                try
                                {
                                    Processor ps = prc[j];
                                    if (ps.Width > Width - x1 || ps.Height > Height - y1)
                                        return;
                                    SignValue[,] pc = new SignValue[ps.Width, ps.Height];
                                    Parallel.For(0, prc.Height, (y, stateCountY) =>
                                    {
                                        try
                                        {
                                            if (stateHeightMain.IsStopped || stateWidthMain.IsStopped || stateCountMap.IsStopped || stateCountY.IsStopped)
                                                return;
                                            Parallel.For(0, prc.Width, (x, stateCountX) =>
                                            {
                                                try
                                                {
                                                    pc[x, y] = ps[x, y] - this[x + x1, y + y1];
                                                }
                                                catch (Exception ex)
                                                {
                                                    try
                                                    {
                                                        errString = ex.Message;
                                                        exThrown = true;
                                                        stateCountX.Stop();
                                                        stateCountY.Stop();
                                                        stateCountMap.Stop();
                                                        stateWidthMain.Stop();
                                                    }
                                                    catch
                                                    {
                                                        //ignored
                                                    }
                                                }
                                            });
                                        }
                                        catch (Exception ex)
                                        {
                                            try
                                            {
                                                errString = ex.Message;
                                                exThrown = true;
                                                stateCountY.Stop();
                                                stateCountMap.Stop();
                                                stateWidthMain.Stop();
                                            }
                                            catch
                                            {
                                                //ignored
                                            }
                                        }
                                    });
                                    procPercent[j] = pc;
                                }
                                catch (Exception ex)
                                {
                                    try
                                    {
                                        errString = ex.Message;
                                        exThrown = true;
                                        stateCountMap.Stop();
                                        stateWidthMain.Stop();
                                    }
                                    catch
                                    {
                                        //ignored
                                    }
                                }
                            });
                            if (procPercent.Count <= 0 || stateHeightMain.IsStopped || stateWidthMain.IsStopped)
                                return;
                            double[] mas = new double[prc.Count];
                            Parallel.For(0, prc.Count, (k, stateCountMap) =>
                            {
                                try
                                {
                                    if (stateHeightMain.IsStopped || stateWidthMain.IsStopped || stateCountMap.IsStopped)
                                        return;
                                    Parallel.For(0, prc.Height, (y2, stateHeightCount) =>
                                    {
                                        try
                                        {
                                            if (stateHeightMain.IsStopped || stateWidthMain.IsStopped || stateCountMap.IsStopped || stateHeightCount.IsStopped)
                                                return;
                                            Parallel.For(0, prc.Width, (x2, stateWidthCount) =>
                                            {
                                                try
                                                {
                                                    if (GetMinIndex(procPercent, x2, y2, k))
                                                        mas[k]++;
                                                }
                                                catch (Exception ex)
                                                {
                                                    try
                                                    {
                                                        errString = ex.Message;
                                                        exThrown = true;
                                                        stateWidthCount.Stop();
                                                        stateHeightCount.Stop();
                                                        stateHeightMain.Stop();
                                                        stateWidthMain.Stop();
                                                        stateCountMap.Stop();
                                                    }
                                                    catch
                                                    {
                                                        //ignored
                                                    }
                                                }
                                            });
                                        }
                                        catch (Exception ex)
                                        {
                                            try
                                            {
                                                errString = ex.Message;
                                                exThrown = true;
                                                stateHeightCount.Stop();
                                                stateCountMap.Stop();
                                                stateHeightMain.Stop();
                                                stateWidthMain.Stop();
                                            }
                                            catch
                                            {
                                                //ignored
                                            }
                                        }
                                    });
                                    mas[k] /= prc[k].Length;
                                }
                                catch (Exception ex)
                                {
                                    try
                                    {
                                        errString = ex.Message;
                                        exThrown = true;
                                        stateCountMap.Stop();
                                        stateHeightMain.Stop();
                                        stateWidthMain.Stop();
                                    }
                                    catch
                                    {
                                        //ignored
                                    }
                                }
                            });
                            if (stateHeightMain.IsStopped || stateWidthMain.IsStopped)
                                return;
                            double db = mas.Max();
                            sr[x1, y1] = new ProcPerc
                            {
                                Procs = GetMaxIndex(mas, db).Select(i => prc[i]).ToArray(),
                                Percent = db
                            };
                        }
                        catch (Exception ex)
                        {
                            try
                            {
                                errString = ex.Message;
                                exThrown = true;
                                stateHeightMain.Stop();
                                stateWidthMain.Stop();
                            }
                            catch
                            {
                                //ignored
                            }
                        }
                    });
                }
                catch (Exception ex)
                {
                    try
                    {
                        errString = ex.Message;
                        exThrown = true;
                        stateHeightMain.Stop();
                    }
                    catch
                    {
                        //ignored
                    }
                }
            });
            if (exThrown)
                throw new Exception(errString);
            return sr;
        }
    }
}