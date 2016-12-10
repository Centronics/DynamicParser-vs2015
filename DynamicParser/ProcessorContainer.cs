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
                throw new ArgumentNullException(nameof(first), $"{nameof(ProcessorContainer)}: {nameof(first)} = null");
            if (first.Length <= 0)
                throw new ArgumentException($"{nameof(ProcessorContainer)}: Первая карта не может быть нулевой длины ({first.Length}).", nameof(first));
            if (processors == null)
                return;
            if (!InOneSize(first, processors))
                throw new ArgumentException($"{nameof(ProcessorContainer)}: Обнаружены карты различных размеров.", nameof(processors));
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
                throw new ArgumentNullException(nameof(proc), $"{nameof(Add)}: {nameof(proc)} = null");
            if (proc.Length <= 0)
                throw new ArgumentException($"{nameof(Add)}: Длина добавляемой карты не может быть равна нулю ({proc.Length}).", nameof(proc));
            if (proc.Width != Width)
                throw new ArgumentException($"{nameof(Add)}: Ширина добавляемой карты должна совпадать с шириной хранилища ({proc.Width} и {Width}).", nameof(proc));
            if (proc.Height != Height)
                throw new ArgumentException($"{nameof(Add)}: Высота добавляемой карты должна совпадать с высотой хранилища ({proc.Height} и {Height}).", nameof(proc));
            _lstProcs.Add(proc);
        }

        /// <summary>
        /// Проверяет, все ли указанные карты одного размера.
        /// </summary>
        /// <param name="proc">Карта-образец для сравнения.</param>
        /// <param name="processors">Список карт, с которыми идёт сопоставление.</param>
        /// <returns>Возвращает true в случае, если все ли указанные карты одного размера, в противном случае false.</returns>
        public static bool InOneSize(Processor proc, Processor[] processors)
        {
            if (proc == null)
                return false;
            return processors != null && processors.All(pr => pr?.Width == proc.Width && pr.Height == proc.Height);
        }
    }
}