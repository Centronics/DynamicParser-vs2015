using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;

namespace DynamicParser
{
    /// <summary>
    ///     Содержит карты, отобранные по проценту соответствия в заданной точке.
    /// </summary>
    public struct ProcPerc
    {
        /// <summary>
        ///     Процент соответствия.
        /// </summary>
        public double Percent;

        /// <summary>
        ///     Карты.
        /// </summary>
        public Processor[] Procs;

        /// <summary>
        ///     Точка.
        /// </summary>
        public Point Position;
    }

    /// <summary>
    ///     Содержит информацию о выбранной карте.
    /// </summary>
    public struct Reg
    {
        /// <summary>
        ///     Инициализирует структуру с заданной позицией.
        /// </summary>
        /// <param name="position">Позиция выбранной карты.</param>
        public Reg(Point position)
        {
            Position = position;
            Percent = 0.0;
            SelectedProcessor = null;
        }

        /// <summary>
        ///     Процент соответствия выбранной карты.
        /// </summary>
        public double Percent;

        /// <summary>
        ///     Выбранная карта.
        /// </summary>
        public Processor SelectedProcessor;

        /// <summary>
        ///     Координаты выбранной карты.
        /// </summary>
        public Point Position { get; }
    }

    /// <summary>
    ///     Отражает статус конкретного <see cref="Region" /> при проверке на совместимость с другим.
    /// </summary>
    public enum RegionStatus
    {
        /// <summary>
        ///     Проверка прошла успешно.
        /// </summary>
        Ok,

        /// <summary>
        ///     Указанный регион равен null.
        /// </summary>
        Null,

        /// <summary>
        ///     Указанный регион шире текущего.
        /// </summary>
        WidthBig,

        /// <summary>
        ///     Указанный регион выше текущего.
        /// </summary>
        HeightBig
    }

    /// <summary>
    ///     Хранит информацию о соответствии карт в конкретной точке.
    /// </summary>
    public sealed class SearchResults
    {
        /// <summary>
        ///     Разница между процентами соответствия, позволяющая считать их равными.
        /// </summary>
        const double DiffEqual = 0.01;

        /// <summary>
        ///     Хранит информацию о картах и проценте их соответствия в данной точке.
        /// </summary>
        readonly ProcPerc[,] _coords;

        /// <summary>
        ///     Инициализирует экземпляр с заданными параметрами ширины и высоты.
        /// </summary>
        /// <param name="width">Ширина.</param>
        /// <param name="height">Высота.</param>
        /// <param name="mapWidth">Ширина обрабатываемых карт.</param>
        /// <param name="mapHeight">Высота обрабатываемых карт.</param>
        public SearchResults(int width, int height, int mapWidth, int mapHeight)
        {
            if (width <= 0)
                throw new ArgumentException($"{nameof(SearchResults)}: Ширина указана некорректно ({width}).",
                    nameof(width));
            if (height <= 0)
                throw new ArgumentException($"{nameof(SearchResults)}: Высота указана некорректно ({height}).",
                    nameof(height));
            if (mapWidth <= 0)
                throw new ArgumentException($"{nameof(SearchResults)}: Ширина карт указана некорректно ({mapWidth}).",
                    nameof(mapWidth));
            if (mapHeight <= 0)
                throw new ArgumentException($"{nameof(SearchResults)}: Высота карт указана некорректно ({mapHeight}).",
                    nameof(mapHeight));
            if (mapWidth > width)
                throw new ArgumentException(
                    $"{nameof(SearchResults)}: Ширина обрабатываемых карт не может превышать ширину карты ({mapWidth} > {width}).",
                    nameof(mapWidth));
            if (mapHeight > height)
                throw new ArgumentException(
                    $"{nameof(SearchResults)}: Высота обрабатываемых карт не может превышать высоту карты ({mapHeight} > {height}).",
                    nameof(mapHeight));
            MapWidth = mapWidth;
            MapHeight = mapHeight;
            _coords = new ProcPerc[width, height];
        }

        /// <summary>
        ///     Ширина.
        /// </summary>
        public int Width => _coords.GetLength(0);

        /// <summary>
        ///     Высота.
        /// </summary>
        public int Height => _coords.GetLength(1);

        /// <summary>
        ///     Размер текущей карты.
        /// </summary>
        public Size ResultSize => new Size(Width, Height);

        /// <summary>
        ///     Ширина обрабатываемых карт.
        /// </summary>
        public int MapWidth { get; }

        /// <summary>
        ///     Высота обрабатываемых карт.
        /// </summary>
        public int MapHeight { get; }

        /// <summary>
        ///     Размер обрабатываемых карт.
        /// </summary>
        public Size MapSize => new Size(MapWidth, MapHeight);

        /// <summary>
        ///     Получает или задаёт информацию о картах в данной точке.
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
        ///     Ищет возможность связывания указанных слов с полями <see cref="Processor.Tag" /> найденных карт.
        ///     Равносилен вызову метода <see cref="FindRelation(string,int,int)" /> для каждого слова. Оптимизирован для
        ///     многопоточного исполнения.
        ///     Возвращает список слов, с которыми удалось установить связь, или null в случае ошибки или пустого исходного
        ///     массива.
        /// </summary>
        /// <param name="words">Искомые слова.</param>
        /// <param name="startIndex">Индекс, начиная с которого будет сформирована строка названия карты.</param>
        /// <param name="count">Количество символов для выборки из названия карты, оно должно быть кратно длине искомого слова.</param>
        /// <returns>Возвращает список слов, с которыми удалось установить связь.</returns>
        public ConcurrentBag<string> FindRelation(int startIndex, int count, params string[] words)
        {
            return FindRelation((IList<string>) words, startIndex, count);
        }

        /// <summary>
        ///     Ищет возможность связывания указанных слов с полями <see cref="Processor.Tag" /> найденных карт.
        ///     Равносилен вызову метода <see cref="FindRelation(string,int,int)" /> для каждого слова. Оптимизирован для
        ///     многопоточного исполнения.
        ///     Возвращает список слов, с которыми удалось установить связь, или null в случае ошибки или пустого исходного
        ///     массива.
        /// </summary>
        /// <param name="words">Искомые слова.</param>
        /// <param name="startIndex">Индекс, начиная с которого будет сформирована строка названия карты.</param>
        /// <param name="count">Количество символов для выборки из названия карты, оно должно быть кратно длине искомого слова.</param>
        /// <returns>Возвращает список слов, с которыми удалось установить связь.</returns>
        public ConcurrentBag<string> FindRelation(ICollection words, int startIndex = 0, int count = 1)
        {
            if (words == null)
                throw new ArgumentNullException(nameof(words), $@"{nameof(FindRelation)}: Коллекция отсутствует (null).");
            if (startIndex < 0)
                throw new ArgumentOutOfRangeException(nameof(startIndex),
                    $"{nameof(FindRelation)}: Индекс вышел за допустимые пределы ({startIndex}).");
            if (count <= 0)
                throw new ArgumentOutOfRangeException(nameof(count),
                    $@"{nameof(FindRelation)}: Количество символов для выборки из названия карты меньше или равно нолю ({count}).");
            if (words.Count <= 0)
                return null;
            List<string> lst = new List<string>(words.Count);
            lst.AddRange(from string s in words where !string.IsNullOrWhiteSpace(s) select s);
            return FindRelation((IList<string>) lst, startIndex, count);
        }

        /// <summary>
        ///     Ищет возможность связывания указанных слов с полями <see cref="Processor.Tag" /> найденных карт.
        ///     Равносилен вызову метода <see cref="FindRelation(string,int,int)" /> для каждого слова. Оптимизирован для
        ///     многопоточного исполнения.
        ///     Возвращает список слов, с которыми удалось установить связь, или null в случае ошибки или пустого исходного
        ///     массива.
        /// </summary>
        /// <param name="words">Искомые слова.</param>
        /// <param name="startIndex">Индекс, начиная с которого будет сформирована строка названия карты.</param>
        /// <param name="count">Количество символов для выборки из названия карты, оно должно быть кратно длине искомого слова.</param>
        /// <returns>Возвращает список слов, с которыми удалось установить связь.</returns>
        public ConcurrentBag<string> FindRelation(IList<string> words, int startIndex = 0, int count = 1)
        {
            if (words == null)
                throw new ArgumentNullException(nameof(words),
                    $"{nameof(FindRelation)}: Искомые слова отсутствуют (null).");
            if (startIndex < 0)
                throw new ArgumentOutOfRangeException(nameof(startIndex),
                    $"{nameof(FindRelation)}: Индекс вышел за допустимые пределы ({startIndex}).");
            if (count <= 0)
                throw new ArgumentOutOfRangeException(nameof(count),
                    $@"{nameof(FindRelation)}: Количество символов для выборки из названия карты меньше или равно нолю ({count}).");
            if (words.Count <= 0)
                return null;
            ConcurrentBag<string> result = new ConcurrentBag<string>();
            if (words.Count == 1)
            {
                string str = words[0];
                if (string.IsNullOrEmpty(str)) return null;
                if (!FindRelation(str, startIndex, count)) return null;
                result.Add(str);
                return result;
            }
            bool exThrown = false, exStopping = false;
            string exString = string.Empty, exStoppingString = string.Empty;
            Parallel.For(0, words.Count, (i, state) =>
            {
                try
                {
                    string str = words[i];
                    if (string.IsNullOrEmpty(str))
                        return;
                    if (FindRelation(str, startIndex, count))
                        result.Add(str);
                }
                catch (Exception ex)
                {
                    try
                    {
                        exThrown = true;
                        exString = ex.Message;
                        state.Stop();
                    }
                    catch (Exception ex1)
                    {
                        exStopping = true;
                        exStoppingString = ex1.Message;
                    }
                }
            });
            if (exThrown)
                throw new Exception(exStopping ? $"{exString}{Environment.NewLine}{exStoppingString}" : exString);
            return result;
        }

        /// <summary>
        ///     Ищет возможность связывания указанного слова с полями <see cref="Processor.Tag" /> найденных карт.
        ///     Иными словами, отвечает на вопрос: "можно ли из имеющихся найденных карт составить искомое слово?".
        ///     Возвращает значение true в случае нахождения слова, в противном случае - false.
        /// </summary>
        /// <param name="word">Искомое слово.</param>
        /// <param name="startIndex">Индекс, начиная с которого будет сформирована строка названия карты.</param>
        /// <param name="count">Количество символов для выборки из названия карты, оно должно быть кратно длине искомого слова.</param>
        /// <returns>Возвращает значение true в случае нахождения связи, в противном случае - false.</returns>
        public bool FindRelation(string word, int startIndex = 0, int count = 1)
        {
            if (word == null)
                throw new ArgumentNullException(nameof(word),
                    $"{nameof(FindRelation)}: Искомое слово отсутствует (null).");
            if (startIndex < 0)
                throw new ArgumentOutOfRangeException(nameof(startIndex),
                    $"{nameof(FindRelation)}: Индекс вышел за допустимые пределы ({startIndex}).");
            if (count <= 0)
                throw new ArgumentOutOfRangeException(nameof(count),
                    $@"{nameof(FindRelation)}: Количество символов для выборки из названия карты меньше или равно нолю ({count}).");
            if (word.Length <= 0)
                return false;
            int mod = word.Length % count;
            if (mod != 0)
                throw new ArgumentException(
                    $@"{nameof(FindRelation)}: Количество символов для выборки должно быть кратно длине искомого слова: слово: {
                        word}, длина: {word.Length}, количество символов для выборки: {count}, остаток от деления: {mod}.",
                    nameof(count));
            List<Reg> lst = new List<Reg>();
            foreach (string str in GetWord(word, count))
            {
                List<Reg> lstReg = FindSymbols(str, startIndex);
                if (lstReg != null && lstReg.Count > 0)
                    lst.AddRange(lstReg);
            }
            return FindWord(lst, startIndex, word, count);
        }

        /// <summary>
        ///     Получает части слова указанной длины.
        /// </summary>
        /// <param name="word">Искомое слово.</param>
        /// <param name="length">Требуемое количество букв в подстроке.</param>
        /// <returns>Возвращает части слова указанной длины.</returns>
        static IEnumerable<string> GetWord(string word, int length)
        {
            for (int k = 0, max = word.Length - length; k <= max; k += length)
                yield return word.Substring(k, length);
        }

        /// <summary>
        ///     Получает значение true в случае нахождения искомого слова, в противном случае - false.
        /// </summary>
        /// <param name="regs">Список обрабатываемых карт.</param>
        /// <param name="startIndex">Индекс, начиная с которого будет сформирована строка названия карты.</param>
        /// <param name="word">Искомое слово.</param>
        /// <param name="selectCount">
        ///     Количество символов, которое необходимо выбрать из названия карты для поиска требуемого
        ///     слова.
        /// </param>
        /// <returns>Возвращает <see cref="WordSearcher" />, который позволяет выполнить поиск требуемого слова.</returns>
        bool FindWord(IList<Reg> regs, int startIndex, string word, int selectCount)
        {
            if (regs == null)
                throw new ArgumentNullException(nameof(regs),
                    $"{nameof(FindWord)}: Список обрабатываемых карт равен null.");
            if (startIndex < 0)
                throw new ArgumentOutOfRangeException(nameof(startIndex),
                    $"{nameof(FindWord)}: Индекс вышел за допустимые пределы ({startIndex}).");
            if (selectCount <= 0)
                throw new ArgumentOutOfRangeException(nameof(selectCount), $@"{nameof(FindWord)
                    }: Количество символов, которое необходимо выбрать из названия карты, должно быть больше ноля ({selectCount}).");
            int sCount = word.Length / selectCount;
            if (sCount <= 0)
                throw new ArgumentOutOfRangeException(nameof(word),
                    $@"{nameof(FindWord)}: Количество символов для выборки из названия карты меньше или равно нолю ({sCount}).");
            if (regs.Count <= 0)
                return false;
            int[] counting = new int[sCount];
            Reg[] regsCounting = new Reg[sCount];
            Region region = new Region(Width, Height);
            for (int counter = sCount - 1; counter >= 0;)
            {
                bool result = true;
                for (int k = 0; k < counting.Length; k++)
                    regsCounting[k] = regs[counting[k]];
                foreach (Reg pp in regsCounting)
                {
                    if (region.Contains(pp.SelectedProcessor.GetProcessorName(startIndex, selectCount), startIndex))
                        continue;
                    Rectangle rect = new Rectangle(pp.Position, MapSize);
                    if (region.IsConflict(rect))
                    {
                        result = false;
                        break;
                    }
                    region.Add(rect, pp.SelectedProcessor, pp.Percent);
                }
                if (result)
                    if (GetStringFromRegion(region, startIndex, selectCount)?.IsEqual(word) ?? false)
                        return true;
                if ((counter = ChangeCount(counting, regs.Count)) < 0)
                    return false;
                region.Clear();
            }
            return false;
        }

        /// <summary>
        ///     Генерирует <see cref="WordSearcher" /> из <see cref="Processor.GetProcessorName" />.
        /// </summary>
        /// <param name="region"><see cref="Region" />, из которого требуется получить <see cref="WordSearcher" />.</param>
        /// <param name="startIndex">Индекс, начиная с которого будет сформирована строка названия карты.</param>
        /// <param name="count">Количество символов в строке <see cref="Processor.Tag" /> карты.</param>
        /// <returns>
        ///     Возвращает <see cref="WordSearcher" /> из первых букв названия (<see cref="Processor.Tag" />) объектов
        ///     <see cref="Region" />.
        /// </returns>
        static WordSearcher GetStringFromRegion(Region region, int startIndex, int count)
        {
            if (region == null)
                throw new ArgumentNullException(nameof(region), $"{nameof(GetStringFromRegion)}: Регион равен null.");
            if (count <= 0)
                throw new ArgumentOutOfRangeException(nameof(count),
                    $@"{nameof(GetStringFromRegion)}: Количество символов для выборки из названия карты меньше или равно нолю ({count}).");
            if (startIndex < 0)
                throw new ArgumentOutOfRangeException(nameof(startIndex),
                    $"{nameof(GetStringFromRegion)}: Индекс вышел за допустимые пределы ({startIndex}).");
            string[] lstWords =
                region.Elements.Select(
                        registered => registered.Register.SelectedProcessor.GetProcessorName(startIndex, count)).
                    Where(procName => !string.IsNullOrEmpty(procName)).ToArray();
            return lstWords.Length <= 0 ? null : new WordSearcher(lstWords);
        }

        /// <summary>
        ///     Находит карты в результатах поиска, поля <see cref="Processor.Tag" /> которых по указанной позиции соответствуют
        ///     указанной строке.
        /// </summary>
        /// <param name="procName">Искомая строка.</param>
        /// <param name="startIndex">Индекс, начиная с которого будет сформирована строка названия карты.</param>
        /// <returns>Возвращает информацию о найденных картах.</returns>
        List<Reg> FindSymbols(string procName, int startIndex)
        {
            if (procName == null)
                throw new ArgumentNullException(nameof(procName), $"{nameof(FindSymbols)}: Искомая строка равна null.");
            if (procName == string.Empty)
                throw new ArgumentException(
                    $"{nameof(FindSymbols)}: Искомая строка должна состоять хотя бы из одного символа.",
                    nameof(procName));
            if (startIndex < 0)
                throw new ArgumentOutOfRangeException(nameof(startIndex),
                    $"{nameof(FindSymbols)}: Индекс вышел за допустимые пределы ({startIndex}).");
            List<Reg> lstRegs = new List<Reg>();
            for (int y = 0; y < Height; y++)
            for (int x = 0; x < Width; x++)
            {
                Processor[] processors =
                    this[x, y].Procs?.Where(pr => pr != null && pr.IsProcessorName(procName, startIndex)).ToArray();
                if ((processors?.Length ?? 0) <= 0)
                    continue;
                double percent = this[x, y].Percent;
                Point point = new Point(x, y);
                lstRegs.AddRange(from pr in processors
                    where pr != null
                    select new Reg(point)
                    {
                        SelectedProcessor = pr,
                        Percent = percent
                    });
            }
            return lstRegs;
        }

        /// <summary>
        ///     Увеличивает значение старших разрядов счётчика букв, если это возможно.
        ///     Если увеличение было произведено, возвращается номер позиции, на которой произошло изменение, в противном случае
        ///     -1.
        /// </summary>
        /// <param name="count">Массив-счётчик.</param>
        /// <param name="maxCount">Максимальное значение счётчика.</param>
        /// <returns>Возвращается номер позиции, на которой произошло изменение, в противном случае -1.</returns>
        static int ChangeCount(IList<int> count, int maxCount)
        {
            if (count == null)
                throw new ArgumentNullException(nameof(count),
                    $"{nameof(ChangeCount)}: Массив-счётчик не указан или его длина некорректна (null).");
            if (count.Count <= 0)
                throw new ArgumentException(
                    $"{nameof(ChangeCount)}: Длина массива-счётчика должна быть больше ноля ({count.Count}).",
                    nameof(count));
            if (maxCount <= 0)
                throw new ArgumentOutOfRangeException(nameof(maxCount),
                    $@"{nameof(ChangeCount)}: Максимальное значение счётчика меньше или равно нолю ({maxCount
                        }).");
            for (int k = count.Count - 1, mc = maxCount - 1; k >= 0; k--)
            {
                if (count[k] >= mc) continue;
                count[k]++;
                for (int x = k + 1; x < count.Count; x++)
                    count[x] = 0;
                return k;
            }
            return -1;
        }

        /// <summary>
        ///     Выполняет поиск наиболее подходящих карт в указанной области.
        /// </summary>
        /// <param name="rect">Область поиска карт.</param>
        /// <returns>Возвращает список наиболее подходящих карт в указанной области.</returns>
        public List<ProcPerc> Find(Rectangle rect)
        {
            if (rect.Width > Width)
                throw new ArgumentException($"{nameof(Find)}: Указанная область шире, чем текущая.", nameof(rect.Width));
            if (rect.Height > Height)
                throw new ArgumentException($"{nameof(Find)}: Указанная область выше, чем текущая.", nameof(rect.Height));
            if (rect.Right > Width || rect.Bottom > Height || rect.Width <= 0 || rect.Height <= 0 || rect.X < 0 ||
                rect.Y < 0)
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
            exit:
            List<ProcPerc> procs = new List<ProcPerc>();
            for (int y = rect.Y; y < rect.Bottom; y++)
            for (int x = rect.X; x < rect.Right; x++)
            {
                ProcPerc pp = _coords[x, y];
                if (Math.Abs(pp.Percent - max) <= DiffEqual)
                    procs.Add(pp);
            }
            return procs;
        }

        /// <summary>
        ///     Определяет, есть ли какие-либо конфликты между заданным <see cref="Region" /> и текущим.
        /// </summary>
        /// <param name="region">Проверяемый <see cref="Region" />.</param>
        /// <returns>Возвращает <see cref="RegionStatus.Ok" /> в случае отсутствия конфликтов.</returns>
        public RegionStatus RegionCorrect(Region region)
        {
            if (region == null)
                return RegionStatus.Null;
            if (region.Width > Width)
                return RegionStatus.WidthBig;
            return region.Height > Height ? RegionStatus.HeightBig : RegionStatus.Ok;
        }
    }
}