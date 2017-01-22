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
        /// Размер текущей карты.
        /// </summary>
        public Size ResultSize => new Size(Width, Height);

        /// <summary>
        /// Ширина карт, которые проходили обработку.
        /// </summary>
        public int MapWidth { get; }

        /// <summary>
        /// Высота карт, которые проходили обработку.
        /// </summary>
        public int MapHeight { get; }

        /// <summary>
        /// Размер карт, которые проходили обработку.
        /// </summary>
        public Size MapSize => new Size(MapWidth, MapHeight);

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
        /// Ищет возможность связывания указанных слов с полями <see cref="Processor.Tag"/> найденных карт.
        /// Иными словами, отвечает на вопрос: "можно ли из имеющихся найденных карт составить искомые слова при условии отсутствия пересечений между ними?".
        /// Если хотя бы одно слово отсутствует, возвращается значение false.
        /// </summary>
        /// <param name="words">Искомые слова.</param>
        /// <returns>Возвращает значение true в случае нахождения связи, в противном случае - false.</returns>
        public bool FindRelation(params string[] words)
        {
            return FindRelation((IList<string>)words);
        }

        /// <summary>
        /// Ищет возможность связывания указанных слов с полями <see cref="Processor.Tag"/> найденных карт.
        /// Иными словами, отвечает на вопрос: "можно ли из имеющихся найденных карт составить искомые слова при условии отсутствия пересечений между ними?".
        /// Если хотя бы одно слово отсутствует, возвращается значение false.
        /// </summary>
        /// <param name="words">Искомые слова.</param>
        /// <returns>Возвращает значение true в случае нахождения связи, в противном случае - false.</returns>
        public bool FindRelation(IList<string> words)
        {
            if (words == null)
                throw new ArgumentNullException(nameof(words), $"{nameof(FindRelation)}: Искомые слова отсутствуют (null).");
            if (words.Count <= 0)
                return false;
            if (words.Count == 1)
                return FindRelation(words[0]);
            StringBuilder sb = new StringBuilder();
            foreach (string s in words)
            {
                if (string.IsNullOrEmpty(s))
                    continue;
                sb.Append(s);
            }
            string str = sb.ToString();
            return !string.IsNullOrEmpty(str) && FindRelation(str);
        }

        /// <summary>
        /// Ищет возможность связывания указанного слова с полями <see cref="Processor.Tag"/> найденных карт.
        /// Иными словами, отвечает на вопрос: "можно ли из имеющихся найденных карт составить искомое слово?".
        /// Возвращает значение true в случае нахождения слова, в противном случае - false.
        /// </summary>
        /// <param name="word">Искомое слово.</param>
        /// <param name="startIndex">Индекс, начиная с которого будет сформирована строка названия карты.</param>
        /// <param name="length">Максимальное количество символов в строке названия карты.</param>
        /// <returns>Возвращает значение true в случае нахождения связи, в противном случае - false.</returns>
        public bool FindRelation(string word, int startIndex = 0, int length = 1)
        {
            if (word == null)
                throw new ArgumentNullException(nameof(word), $"{nameof(FindRelation)}: Искомые слова отсутствуют (null).");
            if (word.Length <= 0)
                return false;
            List<Processor>[,] points = new List<Processor>[Width, Height];
            List<List<Reg>> lst = new List<List<Reg>>();
            // ReSharper disable once LoopCanBeConvertedToQuery
            foreach (FindString ch in GetWord(word, length))
            {
                List<Reg> lstReg = FindSymbols(ch.CurrentString, points, startIndex, length);
                if (lstReg != null && lstReg.Count > 0)
                    lst.Add(lstReg);
            }
            WordSearcher ws = GetWordSearcher(lst, startIndex, length);
            return ws != null && ws.IsEqual(word);
        }

        /// <summary>
        /// Получает части слова указанной длины.
        /// </summary>
        /// <param name="word">Искомое слово.</param>
        /// <param name="length">Требуемое количество букв в подстроке.</param>
        /// <returns>Возвращает части слова указанной длины.</returns>
        static IEnumerable<FindString> GetWord(string word, int length)
        {
            for (int k = 0, max = word.Length - length; k <= max; k++)
            {
                FindString fs = new FindString(word.Substring(k, length), k, word);
                yield return fs;
            }
        }

        /// <summary>
        /// Получает <see cref="WordSearcher"/>, который позволяет выполнить поиск требуемого слова.
        /// </summary>
        /// <param name="regs">Список обрабатываемых карт.</param>
        /// <param name="startIndex">Индекс, начиная с которого будет сформирована строка названия карты.</param>
        /// <param name="length">Максимальное количество символов в строке названия карты.</param>
        /// <returns>Возвращает <see cref="WordSearcher"/>, который позволяет выполнить поиск требуемого слова.</returns>
        WordSearcher GetWordSearcher(IList<List<Reg>> regs, int startIndex, int length)
        {
            if (regs == null)
                throw new ArgumentNullException(nameof(regs), $"{nameof(GetWordSearcher)}: Список обрабатываемых карт равен null.");
            if (regs.Count <= 0)
                return null;
            int[] count = new int[regs.Count];
            Region region = new Region(Width, Height);
            for (int counter = regs.Count - 1; counter >= 0;)
            {
                List<Reg> lstReg = GetWord(count, regs);
                bool result = true;
                foreach (Reg reg in lstReg)
                {
                    Rectangle rect = new Rectangle(reg.Position, MapSize);
                    if (region.IsConflict(rect))
                    {
                        result = false;
                        break;
                    }
                    region.Add(rect);
                    region[reg.Position].Register = new List<Reg> { reg };
                }
                if (result)
                    return GetStringFromRegion(region, startIndex, length);
                if ((counter = ChangeCount(count, regs)) < 0)
                    return null;
            }
            return null;
        }

        /// <summary>
        /// Генерирует <see cref="WordSearcher"/> из <see cref="Processor.GetProcessorName"/>.
        /// </summary>
        /// <param name="region">Регион, из которого требуется получить <see cref="WordSearcher"/>.</param>
        /// <param name="startIndex">Индекс, начиная с которого будет сформирована строка названия карты.</param>
        /// <param name="length">Максимальное количество символов в строке названия карты.</param>
        /// <returns>Возвращает <see cref="WordSearcher"/> из первых букв названия (<see cref="Processor.Tag"/>) объектов региона.</returns>
        WordSearcher GetStringFromRegion(Region region, int startIndex, int length)
        {
            if (region == null)
                throw new ArgumentNullException(nameof(region), $"{nameof(GetStringFromRegion)}: Регион равен null.");
            List<List<string>> lstWords = new List<List<string>>();
            foreach (Registered registered in region.Elements)
                foreach (Reg reg in registered.Register)
                {
                    List<string> lst = new List<string>();
                    // ReSharper disable once LoopCanBeConvertedToQuery
                    foreach (Processor pr in reg.Procs)
                        lst.Add(pr.GetProcessorName(startIndex, length));
                    lstWords.Add(lst);
                }
            return new WordSearcher(lstWords);
        }

        /// <summary>
        /// Находит объекты в результатах поиска, поля <see cref="Processor.Tag"/> которых по указанной позиции соответствуют указанной строке.
        /// </summary>
        /// <param name="procName">Искомая строка.</param>
        /// <param name="points">Массив данных, содержащий информацию об обработанных объектах.</param>
        /// <param name="startIndex">Индекс, начиная с которого будет сформирована строка названия карты.</param>
        /// <param name="length">Максимальное количество символов в строке названия карты.</param>
        /// <returns>Возвращает информацию о найденных объектах.</returns>
        List<Reg> FindSymbols(string procName, List<Processor>[,] points, int startIndex, int length)
        {
            if (procName == null)
                throw new ArgumentNullException(nameof(procName), $"{nameof(FindSymbols)}: Искомая строка = null.");
            if (procName == string.Empty)
                throw new ArgumentException($"{nameof(FindSymbols)}: Искомая строка должна состоять хотя бы из одиного символа.", nameof(procName));
            if (points == null)
                throw new ArgumentNullException(nameof(points),
                    $"{nameof(FindSymbols)}: Массив данных, содержащий информацию об обработанных объектах равен null.");
            if (points.GetLength(0) != Width)
                throw new ArgumentException(
                    $@"{nameof(FindSymbols)}: Массив данных, содержащий информацию об обработанных объектах не соответствует ширине карты ({
                        points.GetLength(0)} и {Width}).");
            if (points.GetLength(1) != Height)
                throw new ArgumentException(
                    $@"{nameof(FindSymbols)}: Массив данных, содержащий информацию об обработанных объектах не соответствует высоте карты ({
                        points.GetLength(1)} и {Height}).");
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
                        if (pr == null || !pr.IsProcessorName(procName, startIndex, length))//МОЖНО СДЕЛАТЬ ВМЕСТО LENGTH procName.Length и points убрать
                            continue;
                        if (points[x, y] != null)
                        {
                            if (points[x, y].Where(prc => prc != null).Any(prc => prc.IsProcessorName(procName, startIndex, length)))
                                continue;
                            points[x, y].Add(pr);
                        }
                        else
                            points[x, y] = new List<Processor>();
                        processors.Add(pr);
                    }
                    if (processors.Count > 0)
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
        /// <param name="lstReg">Список найденных карт.</param>
        /// <returns>Возвращается номер позиции, на которой произошло изменение, в противном случае -1.</returns>
        int ChangeCount(IList<int> count, IList<List<Reg>> lstReg)
        {
            if (lstReg == null)
                throw new ArgumentNullException(nameof(lstReg), $"{nameof(ChangeCount)}:Список найденных карт равен (null).");
            if (count == null || count.Count != lstReg.Count)
                throw new ArgumentException($"{nameof(ChangeCount)}: Массив-счётчик не указан или его длина некорректна ({count?.Count}).", nameof(count));
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
        /// <param name="lstReg">Список найденных карт.</param>
        /// <returns>Возвращает слово из частей, содержащихся в коллекции.</returns>
        List<Reg> GetWord(IList<int> count, IList<List<Reg>> lstReg)
        {
            if (lstReg == null)
                throw new ArgumentNullException(nameof(lstReg), $"{nameof(GetWord)}:Список найденных карт равен (null).");
            if (count == null)
                throw new ArgumentNullException(nameof(count), $"{nameof(GetWord)}: Массив данных равен null.");
            if (count.Count != lstReg.Count)
                throw new ArgumentException($"{nameof(GetWord)}: Длина массива данных должна совпадать с количеством хранимых слов.", nameof(count));
            List<Reg> lst = new List<Reg>();
            // ReSharper disable once LoopCanBeConvertedToQuery
            for (int k = 0; k < lstReg.Count; k++)
                lst.Add(lstReg[k][count[k]]);
            return lst;
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