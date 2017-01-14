using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace DynamicParser
{
    /// <summary>
    /// Содержит карты, отобранные по проценту соответствия в заданной точке.
    /// </summary>
    public struct ProcPerc
    {
        /// <summary>
        /// Процент соответствия.
        /// </summary>
        public double Percent;

        /// <summary>
        /// Карты.
        /// </summary>
        public Processor[] Procs;
    }

    /// <summary>
    /// Содержит карты, отобранные по проценту соответствия в указанной области.
    /// </summary>
    public struct Reg
    {
        /// <summary>
        /// Точка.
        /// </summary>
        public Point Position;

        /// <summary>
        /// Карты, отобранные по проценту соответствия.
        /// </summary>
        public Processor[] Procs;

        /// <summary>
        /// Процент соответствия.
        /// </summary>
        public double Percent;
    }

    /// <summary>
    /// Отражает статус конкретного региона при проверке на совместимость с другим.
    /// </summary>
    public enum RegionStatus
    {
        /// <summary>
        /// Проверка прошла успешно.
        /// </summary>
        Ok,
        /// <summary>
        /// Указанный регион равен null.
        /// </summary>
        Null,
        /// <summary>
        /// Указанный регион шире существующего.
        /// </summary>
        WidthBig,
        /// <summary>
        /// Указанный регион выше существующего.
        /// </summary>
        HeightBig
    }

    /// <summary>
    /// Хранит информацию о соответствии карт в конкретной точке.
    /// </summary>
    public sealed class SearchResults
    {
        /// <summary>
        /// Разница между процентами соответствия, позволяющая считать их равными.
        /// </summary>
        const double DiffEqual = 0.01;

        /// <summary>
        /// Хранит информацию о картах и проценте их соответствия в данной точке.
        /// </summary>
        readonly ProcPerc[,] _coords;

        /// <summary>
        /// Ширина.
        /// </summary>
        public int Width => _coords.GetLength(0);

        /// <summary>
        /// Высота.
        /// </summary>
        public int Height => _coords.GetLength(1);

        /// <summary>
        /// Ширина карт, которые проходили обработку.
        /// </summary>
        public int MapWidth { get; }

        /// <summary>
        /// Высота карт, которые проходили обработку.
        /// </summary>
        public int MapHeight { get; }

        /// <summary>
        /// Инициализирует экземпляр с заданными параметрами ширины и высоты.
        /// </summary>
        /// <param name="width">Ширина.</param>
        /// <param name="height">Высота.</param>
        /// <param name="mapWidth">Ширина карт, которые проходили обработку.</param>
        /// <param name="mapHeight">Высота карт, которые проходили обработку.</param>
        public SearchResults(int width, int height, int mapWidth, int mapHeight)
        {
            if (width <= 0)
                throw new ArgumentException($"{nameof(SearchResults)}: Ширина указана некорректно ({width}).", nameof(width));
            if (height <= 0)
                throw new ArgumentException($"{nameof(SearchResults)}: Высота указана некорректно ({height}).", nameof(height));
            if (mapWidth <= 0)
                throw new ArgumentException($"{nameof(SearchResults)}: Ширина карт указана некорректно ({mapWidth}).", nameof(mapWidth));
            if (mapHeight <= 0)
                throw new ArgumentException($"{nameof(SearchResults)}: Высота карт указана некорректно ({mapHeight}).", nameof(mapHeight));
            MapWidth = mapWidth;
            MapHeight = mapHeight;
            _coords = new ProcPerc[width, height];
        }

        /// <summary>
        /// Ищет возможность связывания указанных слов с полями Tag найденных карт.
        /// Иными словами, отвечает на вопрос: "можно ли из имеющихся найденных карт составить искомые слова при условии отсутствия пересечений между ними?".
        /// Если хотя бы одно слово отсутствует, возвращается значение false.
        /// </summary>
        /// <param name="words">Искомые слова.</param>
        /// <returns>Возвращает значение true в случае нахождения связи, в противном случае - false.</returns>
        public bool FindRelation(params string[] words)
        {
            return FindRelation((IEnumerable<string>)words);
        }

        /// <summary>
        /// Ищет возможность связывания указанных слов с полями Tag найденных карт.
        /// Иными словами, отвечает на вопрос: "можно ли из имеющихся найденных карт составить искомые слова при условии отсутствия пересечений между ними?".
        /// Если хотя бы одно слово отсутствует, возвращается значение false.
        /// </summary>
        /// <param name="words">Искомые слова.</param>
        /// <returns>Возвращает значение true в случае нахождения связи, в противном случае - false.</returns>
        public bool FindRelation(IEnumerable<string> words)
        {
            Region region = new Region(Width, Height);
            List<Processor>[,] points = new List<Processor>[Width, Height];
            foreach (string word in words)
            {
                if (string.IsNullOrEmpty(word))
                    continue;
                int[] count = new int[word.Length];
                List<IList<Reg>> lstReg = word.Select(ch => FindSymbols(ch, points)).Cast<IList<Reg>>().ToList();
            }
            return true;
        }

        /// <summary>
        /// Находит объекты в результатах поиска, поле Tag которых соответствует указанному символу по своему первому символу и помечает его обработанным.
        /// </summary>
        /// <param name="symbol">Искомый символ.</param>
        /// <param name="points">Массив данных, содержащий информацию об обработанных объектах.</param>
        /// <returns>Возвращает информацию о найденных объектах.</returns>
        List<Reg> FindSymbols(char symbol, List<Processor>[,] points)
        {
            symbol = char.ToUpper(symbol);
            List<Reg> lstRegs = new List<Reg>();
            for (int y = 0; y < Height; y++)
                for (int x = 0; x < Width; x++)
                {
                    Processor[] procs = this[x, y].Procs;
                    if (procs == null)
                        continue;
                    List<Processor> processors = new List<Processor>();
                    foreach (Processor pr in procs)
                    {
                        if (pr == null || char.ToUpper(pr.Tag[0]) != symbol ||
                            points[x, y] != null && points[x, y].Where(prc => prc != null).Any(prc => char.ToUpper(prc.Tag[0]) == symbol))
                            continue;
                        if (points[x, y] == null)
                            points[x, y] = new List<Processor>();
                        processors.Add(pr);
                        points[x, y].Add(pr);
                    }
                    lstRegs.Add(new Reg
                    {
                        Percent = this[x, y].Percent,
                        Position = new Point(x, y),
                        Procs = processors.ToArray()
                    });
                }
            return lstRegs;
        }

        /// <summary>
        /// Увеличивает значение старших разрядов счётчика букв, если это возможно.
        /// Если увеличение было произведено, возвращается номер позиции, на которой произошло изменение, в противном случае -1.
        /// </summary>
        /// <param name="count">Массив-счётчик.</param>
        /// <param name="lstReg"></param>
        /// <returns>Возвращается номер позиции, на которой произошло изменение, в противном случае -1.</returns>
        int ChangeCount(IList<int> count, IList<IList<Reg>> lstReg)
        {
            for (int k = count.Count - 1; k >= 0; k--)
            {
                if (count[k] >= lstReg[k].Count - 1) continue;
                count[k]++;
                for (int x = k + 1; x < count.Count; x++)
                    count[x] = 0;
                return k;
            }
            return -1;
        }

        /// <summary>
        /// Генерирует слово из частей, содержащихся в коллекции, основываясь на данных счётчиков.
        /// </summary>
        /// <param name="count">Данные счётчиков по каждому слову.</param>
        /// <returns>Возвращает слово из частей, содержащихся в коллекции.</returns>
        string GetWord(IList<int> count)
        {
            if (count == null)
                throw new ArgumentNullException(nameof(count), $"{nameof(GetWord)}: Массив данных равен null.");
            if (count.Count != Count)
                throw new ArgumentException($"{nameof(GetWord)}: Длина массива данных должна совпадать с количеством хранимых слов.", nameof(count));
            StringBuilder sb = new StringBuilder();
            for (int k = 0; k < Count; k++)
                sb.Append(this[k][count[k]]);
            return sb.ToString();
        }

        /// <summary>
        /// Получает или задаёт информацию о картах в данной точке.
        /// </summary>
        /// <param name="x">Координата X.</param>
        /// <param name="y">Координата Y.</param>
        /// <returns>Возвращает информацию о картах в данной точке.</returns>
        public ProcPerc this[int x, int y]
        {
            get { return _coords[x, y]; }
            set { _coords[x, y] = value; }
        }

        /// <summary>
        /// Выполняет поиск наиболее подходящих карт в указанной области.
        /// </summary>
        /// <param name="rect">Указанная область.</param>
        /// <returns>Возвращает список наиболее подходящих карт в указанной области.</returns>
        List<Reg> Find(Rectangle rect)
        {
            if (rect.Width > Width)
                throw new ArgumentException($"{nameof(Find)}: Указанная область шире, чем текущая.", nameof(rect.Width));
            if (rect.Height > Height)
                throw new ArgumentException($"{nameof(Find)}: Указанная область выше, чем текущая.", nameof(rect.Height));
            if (rect.Right >= Width || rect.Height >= Height || rect.Width <= 0 || rect.Height <= 0 || rect.X < 0 || rect.Y < 0)
                return null;
            double max = -1.0;
            for (int y = rect.Y; y < rect.Bottom; y++)
                for (int x = rect.X; x < rect.Right; x++)
                {
                    if (max < _coords[x, y].Percent)
                        max = _coords[x, y].Percent;
                    if (max >= 1.0)
                        goto exit;
                }
            if (max <= 0)
                return null;
            exit: List<Reg> procs = new List<Reg>();
            for (int y = rect.Y; y < rect.Bottom; y++)
                for (int x = rect.X; x < rect.Right; x++)
                {
                    ProcPerc pp = _coords[x, y];
                    if (Math.Abs(pp.Percent - max) <= DiffEqual)
                        procs.Add(new Reg { Position = new Point(x, y), Procs = pp.Procs, Percent = pp.Percent });
                }
            return procs;
        }

        /// <summary>
        /// Определяет, есть ли какие-либо конфликты между заданным регионом и текущим.
        /// </summary>
        /// <param name="region">Регион, относительно которого происходит проверка.</param>
        /// <returns>Возвращает OK в случае отсутствия конфликтов.</returns>
        public RegionStatus RegionCorrect(Region region)
        {
            if (region == null)
                return RegionStatus.Null;
            if (region.Width > Width)
                return RegionStatus.WidthBig;
            return region.Height > Height ? RegionStatus.HeightBig : RegionStatus.Ok;
        }

        /// <summary>
        /// Заполняет указанный Region найденными наиболее подходящими картами в соответствии с расположением областей в указанном регионе.
        /// </summary>
        /// <param name="region">Регион для заполнения.</param>
        /// <returns>Возвращает OK в случае отсутствия конфликтов. Если результат не равен OK, то состояние region не изменяется.</returns>
        public RegionStatus FindRegion(Region region)
        {
            RegionStatus rs = RegionCorrect(region);
            if (rs != RegionStatus.Ok)
                return rs;
            foreach (Registered reg in region.Elements)
                reg.Register = Find(reg.Region);
            return RegionStatus.Ok;
        }

        /// <summary>
        /// Накладывает одни результаты поиска карт на другие и вычисляет, какие карты соответствуют друг другу в конкретной точке более всего.
        /// Карты должны быть одного размера.
        /// </summary>
        /// <param name="srs">Результаты поиска.</param>
        /// <returns>Возвращает обобщённые результаты поиска.</returns>
        public static SearchResults Combine(IList<SearchResults> srs)
        {
            if (srs == null)
                throw new ArgumentNullException(nameof(srs), $"{nameof(Combine)}: Коллекция не может быть равна null.");
            if (srs.Count <= 0)
                throw new ArgumentException($"{nameof(Combine)}: В коллекции должен быть хотя бы один элемент.", nameof(srs));
            if (!InOneSize(srs))
                throw new ArgumentException($"{nameof(Combine)}: Результаты поиска должны совпадать по размерам.", nameof(srs));
            int width = srs[0].Width;
            int height = srs[0].Height;
            SearchResults sr = new SearchResults(width, height, srs[0].MapWidth, srs[0].MapHeight);
            for (int y = 0; y < height; y++)
                for (int x = 0; x < width; x++)
                {
                    double? pp = null;
                    foreach (SearchResults searchResults in srs)
                    {
                        if (searchResults == null)
                            continue;
                        ProcPerc pp1 = searchResults[x, y];
                        if (pp == null || pp1.Percent >= pp.Value)
                            pp = pp1.Percent;
                    }
                    if (pp == null)
                        continue;
                    List<Processor> lstProcessors = new List<Processor>();
                    foreach (SearchResults searchResults in srs)
                    {
                        ProcPerc pp1 = searchResults[x, y];
                        if (pp1.Procs != null && pp1.Procs.Length > 0)
                            lstProcessors.AddRange(pp1.Procs);
                    }
                    sr[x, y] = new ProcPerc { Percent = pp.Value, Procs = lstProcessors.ToArray() };
                }
            return sr;
        }

        /// <summary>
        /// Накладывает одни результаты поиска карт на другие и вычисляет, какие карты соответствуют друг другу в конкретной точке более всего.
        /// Карты должны быть одного размера.
        /// </summary>
        /// <param name="srs">Результаты поиска.</param>
        /// <returns>Возвращает обобщённые результаты поиска.</returns>
        public static SearchResults Combine(params SearchResults[] srs)
        {
            return Combine((IList<SearchResults>)srs);
        }

        /// <summary>
        /// Проверяет, соответствуют ли все результаты поиска одному размеру, включая размеры поисковых карт.
        /// </summary>
        /// <returns>Возвращает true в случае соответствия, иначе false.</returns>
        public static bool InOneSize(IList<SearchResults> srs)
        {
            if (srs == null)
                throw new ArgumentNullException(nameof(srs), $"{nameof(InOneSize)}: Коллекция не может быть равна null.");
            if (srs.Count <= 0)
                return true;
            int width = srs[0].Width, mapWidth = srs[0].MapWidth;
            int height = srs[0].Height, mapHeight = srs[0].MapHeight;
            return srs.All(sr => sr.Width == width && sr.Height == height) && srs.All(sr => sr.MapWidth == mapWidth && sr.MapHeight == mapHeight);
        }
    }
}