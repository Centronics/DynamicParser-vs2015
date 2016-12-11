﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace DynamicParser
{
    /// <summary>
    /// Содержит информацию о точке и сопоставленными с ней картами.
    /// </summary>
    public sealed class Attach
    {
        /// <summary>
        /// Содержит информацию о точке и сопоставленных с ней картах.
        /// </summary>
        public struct Proc
        {
            /// <summary>
            /// Точка.
            /// </summary>
            public Point Place;

            /// <summary>
            /// Сопоставленные карты.
            /// </summary>
            public IEnumerable<Processor> Procs;
        }

        /// <summary>
        /// Точка.
        /// </summary>
        public Point Point { get; set; }

        /// <summary>
        /// Сопоставленные карты.
        /// </summary>
        public List<Reg> Regs { get; set; }

        /// <summary>
        /// Получает список карт из списка сопоставленных карт.
        /// </summary>
        public IEnumerable<Processor> Processors => (from rg in Regs from proc in rg.Procs select proc).ToList();

        /// <summary>
        /// Получает уникальные по полю Tag карты.
        /// </summary>
        public Proc Unique => new Proc { Place = Point, Procs = UniqueMas };

        /// <summary>
        /// Получает уникальные по полю Tag карты.
        /// </summary>
        IEnumerable<Processor> UniqueMas
        {
            get
            {
                if (Regs == null)
                    yield break;
                IEnumerable<Processor> lst = Processors;
                List<Processor> uni = new List<Processor>();
                foreach (Processor pr in lst)
                    if (!Inclusive(uni, pr.Tag))
                    {
                        uni.Add(pr);
                        yield return pr;
                    }
            }
        }

        /// <summary>
        /// Проверяет, хранится ли в указанном списке карта с указанным значением поля Tag. Сравнение происходит с обрезанием пробелов и без учёта регистра.
        /// </summary>
        /// <param name="lst">Список карт для поиска.</param>
        /// <param name="str">Искомое значение поля Tag.</param>
        /// <returns>Если карта с указанным значением поля Tag хранится в указанном списке, возвращается true, иначе false.</returns>
        static bool Inclusive(IEnumerable<Processor> lst, string str)
        {
            return lst != null && lst.Any(s => string.Compare(s.Tag?.Trim() ?? string.Empty, str.Trim(), StringComparison.OrdinalIgnoreCase) == 0);
        }
    }

    /// <summary>
    /// Представляет собой полотно, состоящее из точек, которые отображаются на регион.
    /// </summary>
    public sealed class Attacher
    {
        /// <summary>
        /// Содержит информацию о точках.
        /// </summary>
        readonly List<Attach> _attaches = new List<Attach>();

        /// <summary>
        /// Ширина.
        /// </summary>
        public int Width { get; }

        /// <summary>
        /// Высота.
        /// </summary>
        public int Height { get; }

        /// <summary>
        /// Перечисляет добавленные точки и информацию о сопоставленных с ними картах.
        /// </summary>
        public IEnumerable<Attach> Attaches => _attaches;

        /// <summary>
        /// Перечисляет добавленные точки.
        /// </summary>
        public IEnumerable<Point> Points => _attaches.Select(att => att.Point);

        /// <summary>
        /// Инициализирует класс с указанными шириной и высотой.
        /// </summary>
        /// <param name="width">Ширина.</param>
        /// <param name="height">Высота.</param>
        public Attacher(int width, int height)
        {
            if (width <= 0)
                throw new ArgumentException($"{nameof(Attacher)}: Ширина не может быть меньше или равна нулю ({width}).", nameof(width));
            if (height <= 0)
                throw new ArgumentException($"{nameof(Attacher)}: Высота не может быть меньше или равна нулю ({height}).", nameof(height));
            Width = width;
            Height = height;
        }

        /// <summary>
        /// Выясняет, попадает ли указанная точка на указанную область.
        /// </summary>
        /// <param name="pt">Точка.</param>
        /// <param name="rect">Область.</param>
        /// <returns>В случае попадания возвращает true, в противном случае false.</returns>
        public static bool IsConflict(Point pt, Rectangle rect)
        {
            return pt.X >= rect.X && pt.X <= rect.Right && pt.Y >= rect.Y && pt.Y <= rect.Bottom;
        }

        /// <summary>
        /// Выясняет, указывают ли две или более точки на проверяемую область.
        /// </summary>
        /// <param name="rect">Проверяемая область.</param>
        /// <returns>Если две или более точки указывают на одну и ту же проверяемую область, возвращается true, в противном случае false.</returns>
        public bool IsConflict(Rectangle rect)
        {
            bool one = false;
            foreach (Point pt in Points)
                if (IsConflict(pt, rect))
                    if (one)
                        return true;
                    else
                        one = true;
            return false;
        }

        /// <summary>
        /// Выясняет, указывают ли две или более точки на одну и ту же область.
        /// </summary>
        /// <param name="region">Проверяемый регион.</param>
        /// <returns>В случае, когда конфликт обнаружен, возвращается true, в противном случае false.</returns>
        public bool IsConflict(Region region)
        {
            if (region == null)
                throw new ArgumentNullException(nameof(region), $"{nameof(IsConflict)}: {nameof(region)} = null");
            return region.Rectangles.Any(IsConflict);
        }

        /// <summary>
        /// Накладывает маску на указанный регион, записывая результат в текущий экземпляр.
        /// </summary>
        /// <param name="region">Регион, на который необходимо наложить маску.</param>
        public void SetMask(Region region)
        {
            if (IsConflict(region))
                throw new ArgumentException($"{nameof(SetMask)}: Найдено две или более точек, указывающих на один и тот же регион.", nameof(region));
            foreach (Attach att in Attaches)
                att.Regs = region[att.Point]?.Register;
        }

        /// <summary>
        /// Проверяет, содержится ли указанная точка в списке.
        /// </summary>
        /// <param name="point">Проверяемая точка.</param>
        /// <returns>Если в списке содержится указанная точка, возвращается true, в противном случае false.</returns>
        public bool Contains(Point point)
        {
            return _attaches.Contains(new Attach { Point = point });
        }

        /// <summary>
        /// Добавляет точку в список.
        /// </summary>
        /// <param name="point">Добавляемая точка.</param>
        public void Add(Point point)
        {
            if (Contains(point))
                throw new ArgumentException($"{nameof(Add)}: Указанная точка уже содержится в списке x = {point.X}, y = {point.Y}.", nameof(point));
            _attaches.Add(new Attach { Point = point });
        }

        /// <summary>
        /// Добавляет точку в список.
        /// </summary>
        /// <param name="x">Координата X.</param>
        /// <param name="y">Координата Y.</param>
        public void Add(int x, int y)
        {
            Add(new Point(x, y));
        }
    }
}