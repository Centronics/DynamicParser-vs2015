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
        /// <param name="processors">Добавляемые карты.</param>
        public ProcessorContainer(IList<Processor> processors)
        {
            Processor processor = ArgumentAssertions(processors);
            Width = processor.Width;
            Height = processor.Height;
            AddProcessors(processors);
        }

        /// <summary>
        /// Инициализирует новый экземпляр хранилища, добавляя в него указанные карты и проверяя их на то, чтобы они были одного размера.
        /// </summary>
        /// <param name="processors">Добавляемые карты.</param>
        public ProcessorContainer(params Processor[] processors)
        {
            Processor processor = ArgumentAssertions(processors);
            Width = processor.Width;
            Height = processor.Height;
            AddProcessors(processors);
        }

        /// <summary>
        /// Добавляет карты в коллекцию.
        /// </summary>
        /// <param name="processors">Добавляемые карты.</param>
        void AddProcessors(IEnumerable<Processor> processors)
        {
            _lstProcs.AddRange(processors.Where(proc => proc != null));
        }

        /// <summary>
        /// Выдаёт исключения в случае обнаружения каких-либо ошибок.
        /// </summary>
        /// <param name="processors">Добавляемые карты.</param>
        Processor ArgumentAssertions(IList<Processor> processors)
        {
            if (processors == null)
                throw new ArgumentNullException(nameof(processors), $"{nameof(ProcessorContainer)}: Коллекция карт не может быть равна null.");
            if (processors.Count <= 0)
                throw new ArgumentException($"{nameof(ProcessorContainer)}: В коллекции должен быть хотя бы один элемент.", nameof(processors));
            if (IsNull(processors))
                throw new ArgumentNullException(nameof(processors), $"{nameof(ProcessorContainer)}: Все элементы коллекции процессоров равны null.");
            if (IsEquals(processors))
                throw new ArgumentException($"{nameof(ProcessorContainer)}: Обнаружены ссылки, указывающие на одни и те же карты.", nameof(processors));
            if (!InOneSize(processors))
                throw new ArgumentException($"{nameof(ProcessorContainer)}: Обнаружены карты различных размеров.", nameof(processors));
            if (!InOneTag(processors))
                throw new ArgumentException($"{nameof(ProcessorContainer)}: Карты с одинаковыми {nameof(Processor.Tag)} не могут быть в одном списке.", nameof(processors));
            return FirstInArray(processors);
        }

        /// <summary>
        /// Добавляет карту в хранилище.
        /// </summary>
        /// <param name="proc">Добавляемая карта.</param>
        public void Add(Processor proc)
        {
            if (proc == null)
                throw new ArgumentNullException(nameof(proc), $"{nameof(Add)}: {nameof(proc)} = null.");
            if (proc.Width != Width)
                throw new ArgumentException($"{nameof(Add)}: Ширина добавляемой карты должна совпадать с шириной хранилища ({proc.Width} и {Width}).", nameof(proc));
            if (proc.Height != Height)
                throw new ArgumentException($"{nameof(Add)}: Высота добавляемой карты должна совпадать с высотой хранилища ({proc.Height} и {Height}).", nameof(proc));
            if (ContainsTag(proc.Tag))
                throw new ArgumentException($"{nameof(Add)}: Попытка добавить карту, значение свойства {nameof(Processor.Tag)} которой уже существует в списке.", nameof(proc));
            _lstProcs.Add(proc);
        }

        /// <summary>
        /// Добавляет коллекцию карт в хранилище.
        /// </summary>
        /// <param name="procs">Добавляемая коллекция карт.</param>
        public void AddRange(params Processor[] procs)
        {
            ArgumentAssertions(procs);
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
            ArgumentAssertions(procs);
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
                Processor prc = processors[k];
                if (prc == null)
                    continue;
                object refer = prc;
                uint count = 0;
                if (!processors.All(pr =>
                {
                    if (pr == null)
                        return true;
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
        /// <param name="processors">Список карт, с которыми идёт сопоставление.</param>
        /// <returns>Возвращает true в случае, если все ли указанные карты одного размера, в противном случае false.</returns>
        public static bool InOneSize(IList<Processor> processors)
        {
            if (processors == null || processors.Count <= 0)
                return true;
            Processor processor = FirstInArray(processors);
            if (processor == null)
                return true;
            int width = processor.Width;
            int height = processor.Height;
            return processors.Count <= 0 || processors.All(pr => pr == null || pr.Width == width && pr.Height == height);
        }

        /// <summary>
        /// Находит первый процессор в коллекции, который не равен null.
        /// </summary>
        /// <param name="processors">Коллекция процессоров, в которой производится поиск.</param>
        /// <returns>Возвращает первый процессор в коллекции, который не равен null или null, если таких процессоров в коллекции нет.</returns>
        static Processor FirstInArray(ICollection<Processor> processors)
        {
            if (processors == null || processors.Count <= 0)
                return null;
            return processors.FirstOrDefault(pr => pr != null);
        }

        /// <summary>
        /// Проверяет, все ли элементы коллекции равны null или отсутствуют.
        /// </summary>
        /// <param name="collection">Проверяемая коллекция.</param>
        /// <returns>Возвращает true в случае, если все элементы коллекции равны null или отсутствуют.</returns>
        static bool IsNull(ICollection<Processor> collection)
        {
            if (collection == null)
                throw new ArgumentNullException(nameof(collection), $"{nameof(IsNull)}: Проверяемая коллекция не может быть равна null.");
            return collection.Count <= 0 || collection.All(pr => pr == null);
        }

        /// <summary>
        /// Проверяет присутствие карт с одинаковыми свойствами <see cref="Processor.Tag"/> в указанном списке.
        /// </summary>
        /// <param name="tags">Проверяемый список.</param>
        /// <returns>Возвращает true в случае, когда повторяющиеся значения не встречались, в противном случае false.</returns>
        public static bool InOneTag(ICollection<Processor> tags)
        {
            if (tags == null || tags.Count <= 1)
                return true;
            // ReSharper disable once LoopCanBeConvertedToQuery
            foreach (Processor tag in tags)
            {
                if (tag == null)
                    continue;
                uint count = 0;
                if (!tags.All(pr =>
                {
                    if (pr == null)
                        return true;
                    if (TagStringCompare(pr.Tag, tag.Tag))
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
            return !string.IsNullOrWhiteSpace(tag) && _lstProcs.Any(pr => TagStringCompare(pr.Tag, tag));
        }

        /// <summary>
        /// Сравнивает строки по правилам сравнения свойства <see cref="Processor.Tag"/>.
        /// С обрезанием пробелов и без учёта регистра.
        /// </summary>
        /// <param name="tag">Строка, которую надо сравнить.</param>
        /// <param name="cmp">Строка, с которой надо сравнить.</param>
        /// <returns>Если строки равны, возвращает true, в противном случае false.</returns>
        public static bool TagStringCompare(string tag, string cmp)
        {
            if (tag == null)
                throw new ArgumentNullException(nameof(tag), $"{nameof(TagStringCompare)}: Свойство {nameof(Processor.Tag)} не может быть равно null.");
            if (tag == string.Empty)
                throw new ArgumentException($"{nameof(TagStringCompare)}: Свойство {nameof(Processor.Tag)} не может быть равно {nameof(string.Empty)}.",
                    nameof(tag));
            if (cmp == null)
                throw new ArgumentNullException(nameof(cmp), $"{nameof(TagStringCompare)}: Свойство {nameof(Processor.Tag)} не может быть равно null.");
            if (cmp == string.Empty)
                throw new ArgumentException($"{nameof(TagStringCompare)}: Свойство {nameof(Processor.Tag)} не может быть равно {nameof(string.Empty)}.",
                    nameof(cmp));
            return string.Compare(tag.Trim(), cmp.Trim(), StringComparison.OrdinalIgnoreCase) == 0;
        }
    }
}