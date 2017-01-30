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
            AgrumentAssertions(processors);
            Width = processors[0].Width;
            Height = processors[0].Height;
            AddProcessors(processors);
        }

        /// <summary>
        /// Инициализирует новый экземпляр хранилища, добавляя в него указанные карты и проверяя их на то, чтобы они были одного размера.
        /// </summary>
        /// <param name="processors">Добавляемые карты.</param>
        public ProcessorContainer(params Processor[] processors)
        {
            AgrumentAssertions(processors);
            Width = processors[0].Width;
            Height = processors[0].Height;
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
        void AgrumentAssertions(IList<Processor> processors)
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
                throw new ArgumentException($"{nameof(ProcessorContainer)}: Карты с одинаковыми Tag не могут быть в одном списке.", nameof(processors));
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
                throw new ArgumentException($"{nameof(Add)}: Длина добавляемой карты не может быть равна нолю ({proc.Length}).", nameof(proc));
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
            if (IsNull(procs))
                throw new ArgumentNullException(nameof(procs), $"{nameof(AddRange)}: Все элементы коллекции процессоров равны null.");
            if (IsEquals(procs))
                throw new ArgumentException($"{nameof(AddRange)}: Обнаружены ссылки, указывающие на одни и те же карты.", nameof(procs));
            if (!InOneSize(procs))
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
            if (IsNull(procs))
                throw new ArgumentNullException(nameof(procs), $"{nameof(AddRange)}: Все элементы коллекции процессоров равны null.");
            if (IsEquals(procs))
                throw new ArgumentException($"{nameof(AddRange)}: Обнаружены ссылки, указывающие на одни и те же карты.", nameof(procs));
            if (!InOneSize(procs))
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
            int width = processors[0].Width;
            int height = processors[0].Height;
            return processors.Count <= 0 || processors.All(pr => pr == null || pr.Width == width && pr.Height == height);
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
        /// Проверяет присутствие карт с одинаковыми свойствами Tag в указанном списке.
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