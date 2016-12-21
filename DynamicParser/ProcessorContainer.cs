using System;
using System.Collections.Generic;
using System.Linq;

namespace DynamicParser
{
    /// <summary>
    /// Содержит массив карт для поиска. Проверяет их на то, чтобы все они были одинакового размера.
    /// </summary>
    public sealed class ProcessorContainer
    {
        /// <summary>
        /// Содержит добавленные карты.
        /// </summary>
        readonly List<Processor> _lstProcs = new List<Processor>();

        /// <summary>
        /// Получает карту по индексу.
        /// </summary>
        /// <param name="index">Индекс карты.</param>
        /// <returns>Возвращает карту по индексу.</returns>
        public Processor this[int index] => _lstProcs[index];

        /// <summary>
        /// Получает количество добавленных карт.
        /// </summary>
        public int Count => _lstProcs.Count;

        /// <summary>
        /// Ширина. Вычисляется по первой добавленной карте.
        /// </summary>
        public int Width { get; }

        /// <summary>
        /// Высота. Вычисляется по первой добавленной карте.
        /// </summary>
        public int Height { get; }

        /// <summary>
        /// Инициализирует новый экземпляр хранилища, добавляя в него указанные карты и проверяя их на то, чтобы они были одного размера.
        /// </summary>
        /// <param name="first">Первая карта. По ней будут сверяться размеры всех остальных карт.</param>
        /// <param name="processors">Добавляемые карты.</param>
        public ProcessorContainer(Processor first, params Processor[] processors)
        {
            if (first == null)
                throw new ArgumentNullException(nameof(first), $"{nameof(ProcessorContainer)}: {nameof(first)} = null.");
            if (first.Width <= 0)
                throw new ArgumentException($"{nameof(ProcessorContainer)}: Первая карта не может быть нулевой ширины ({first.Width}).", nameof(first.Width));
            if (first.Height <= 0)
                throw new ArgumentException($"{nameof(ProcessorContainer)}: Первая карта не может быть нулевой высоты ({first.Height}).", nameof(first.Height));
            if (processors == null)
                return;
            if (!InOneSize(first.Width, first.Height, processors))
                throw new ArgumentException($"{nameof(ProcessorContainer)}: Обнаружены карты различных размеров.", nameof(processors));
            if (!InOneTag(processors, first.Tag))
                throw new ArgumentException($"{nameof(ProcessorContainer)}: Карты с одинаковыми Tag не могут быть в одном списке.", nameof(processors));
            _lstProcs.Add(first);
            Width = first.Width;
            Height = first.Height;
            foreach (Processor proc in processors)
            {
                if (proc == null)
                    continue;
                _lstProcs.Add(proc);
            }
        }

        /// <summary>
        /// Добавляет карту в хранилище.
        /// </summary>
        /// <param name="proc">Добавляемая карта.</param>
        public void Add(Processor proc)
        {
            if (proc == null)
                throw new ArgumentNullException(nameof(proc), $"{nameof(Add)}: {nameof(proc)} = null.");
            if (proc.Length <= 0)
                throw new ArgumentException($"{nameof(Add)}: Длина добавляемой карты не может быть равна нулю ({proc.Length}).", nameof(proc));
            if (proc.Width != Width)
                throw new ArgumentException($"{nameof(Add)}: Ширина добавляемой карты должна совпадать с шириной хранилища ({proc.Width} и {Width}).", nameof(proc));
            if (proc.Height != Height)
                throw new ArgumentException($"{nameof(Add)}: Высота добавляемой карты должна совпадать с высотой хранилища ({proc.Height} и {Height}).", nameof(proc));
            if (ContainsTag(proc.Tag))
                throw new ArgumentException($"{nameof(Add)}: Попытка добавить карту, значение свойства Tag которой уже существует в списке.", nameof(proc));
            _lstProcs.Add(proc);
        }

        /// <summary>
        /// Добавляет коллекцию карт в хранилище.
        /// </summary>
        /// <param name="procs">Добавляемая коллекция карт.</param>
        public void AddRange(params Processor[] procs)
        {
            if (procs == null)
                throw new ArgumentNullException(nameof(procs), $"{nameof(AddRange)}: Попытка добавить коллекцию карт, равную null.");
            if (!InOneSize(Width, Height, procs))
                throw new ArgumentException($"{nameof(AddRange)}: Размеры добавляемых карт различаются.", nameof(procs));
            if (!InOneTag(procs))
                throw new ArgumentException($"{nameof(AddRange)}: Карты с одинаковыми Tag не могут быть в одном списке.", nameof(procs));
            foreach (Processor pr in procs)
                if (pr != null)
                    Add(pr);
        }

        /// <summary>
        /// Добавляет коллекцию карт в хранилище.
        /// </summary>
        /// <param name="procs">Добавляемая коллекция карт.</param>
        public void AddRange(IList<Processor> procs)
        {
            if (procs == null)
                throw new ArgumentNullException(nameof(procs), $"{nameof(AddRange)}: Попытка добавить коллекцию карт, равную null.");
            if (!InOneSize(Width, Height, procs))
                throw new ArgumentException($"{nameof(AddRange)}: Размеры добавляемых карт различаются.", nameof(procs));
            if (!InOneTag(procs))
                throw new ArgumentException($"{nameof(AddRange)}: Обнаружены карты, совпадающие по свойству Tag.", nameof(procs));
            foreach (Processor pr in procs)
                if (pr != null)
                    Add(pr);
        }

        /// <summary>
        /// Проверяет, присутствуют ли одинаковые ссылки на объекты.
        /// </summary>
        /// <param name="processors">Проверяемый массив.</param>
        /// <returns>В случае, когда обнаружены совпадающе ссылки, возвращает true, иначе false.</returns>
        public static bool IsEquals(IList<Processor> processors)
        {
            if (processors == null)
                return false;
            // ReSharper disable once LoopCanBeConvertedToQuery
            // ReSharper disable once ForCanBeConvertedToForeach
            for (int k = 0; k < processors.Count; k++)
            {
                object refer = processors[k];
                uint count = 0;
                if (!processors.All(pr =>
                 {
                     if (pr == refer)
                         count++;
                     return count <= 1;
                 })) return true;
            }
            return false;
        }

        /// <summary>
        /// Проверяет, все ли указанные карты одного размера, также она проверяет присутствие ссылок на один и тот же объект.
        /// Если они будут обнаружены, функция породит исключение ArgumentException.
        /// </summary>
        /// <param name="width">Ширина, с которой необходимо соответствие.</param>
        /// <param name="height">Высота, с которой необходимо соответствие.</param>
        /// <param name="processors">Список карт, с которыми идёт сопоставление.</param>
        /// <returns>Возвращает true в случае, если все ли указанные карты одного размера, в противном случае false.</returns>
        public static bool InOneSize(int width, int height, IList<Processor> processors)
        {
            if (width <= 0)
                throw new ArgumentException($"{nameof(InOneSize)}: Ширина не может быть меньше или равна нулю ({nameof(width)}).", nameof(width));
            if (height <= 0)
                throw new ArgumentException($"{nameof(InOneSize)}: Высота не может быть меньше или равна нулю ({nameof(height)}).", nameof(height));
            if (processors == null)
                throw new ArgumentNullException(nameof(processors), $"{nameof(InOneSize)}: Массив сопоставляемых карт равен null.");
            if (IsEquals(processors))
                throw new ArgumentException($"{nameof(InOneSize)}: Обнаружены ссылки, указывающие на одни и те же карты.", nameof(processors));
            return processors.Count <= 0 || processors.All(pr => pr == null || pr.Width == width && pr.Height == height);
        }

        /// <summary>
        /// Проверяет присутствие карт с одинаковыми свойствами Tag в указанном списке.
        /// Соответствие tagex, также считается повторением.
        /// </summary>
        /// <param name="tags">Проверяемый список.</param>
        /// <param name="tagex">Tag, равенство которому будет приводить к тому, что функция вернёт false.</param>
        /// <returns>Возвращает true в случае, когда повторяющиеся значения не встречались, в противном случае false.</returns>
        public static bool InOneTag(IList<Processor> tags, string tagex)
        {
            if ((tags?.Count ?? 0) <= 0 || tags == null || string.IsNullOrEmpty(tagex))
                return true;
            // ReSharper disable once LoopCanBeConvertedToQuery
            foreach (Processor tag in tags)
            {
                if (tag == null)
                    continue;
                if (Attach.TagStringCompare(tag.Tag, tagex))
                    return false;
                uint count = 0;
                if (!tags.All(pr =>
                {
                    if (pr == null)
                        return true;
                    if (Attach.TagStringCompare(pr.Tag, tag.Tag))
                        count++;
                    return count <= 1;
                })) return false;
            }
            return true;
        }

        /// <summary>
        /// Проверяет присутствие карт с одинаковыми свойствами Tag в указанном списке.
        /// </summary>
        /// <param name="tags">Проверяемый список.</param>
        /// <returns>Возвращает true в случае, когда повторяющиеся значения не встречались, в противном случае false.</returns>
        public static bool InOneTag(IList<Processor> tags)
        {
            if ((tags?.Count ?? 0) <= 0 || tags == null)
                return true;
            // ReSharper disable once LoopCanBeConvertedToQuery
            foreach (Processor tag in tags)
            {
                uint count = 0;
                if (!tags.All(pr =>
                {
                    if (Attach.TagStringCompare(pr.Tag, tag.Tag))
                        count++;
                    return count <= 1;
                })) return false;
            }
            return true;
        }

        /// <summary>
        /// Определяет, существует ли в текущем списке карта с таким же тегом или нет.
        /// </summary>
        /// <param name="tag">Искомый тег.</param>
        /// <returns>Возвращает true в случае, если карта найдена, в противном случае false.</returns>
        public bool ContainsTag(string tag)
        {
            return !string.IsNullOrWhiteSpace(tag) && _lstProcs.Any(pr => Attach.TagStringCompare(pr.Tag, tag));
        }
    }
}