﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace DynamicParser
{
    /// <summary>
    /// Представляет область, которая не пересекается ни с какой другой.
    /// Хранит список наиболее соответствующих найденных карт.
    /// </summary>
    public sealed class Registered
    {
        /// <summary>
        /// Область, которая не пересекается ни с какой другой.
        /// </summary>
        public Rectangle Region { get; set; }

        /// <summary>
        /// Список наиболее соответствующих найденных карт.
        /// </summary>
        public List<Reg> Register { get; set; }

        /// <summary>
        /// Возвращает true в случае, если список найденных карт пуст.
        /// </summary>
        public bool IsEmpty => (Register?.Count ?? 0) <= 0;

        /// <summary>
        /// Координата X начала области.
        /// </summary>
        public int X => Region.X;

        /// <summary>
        /// Координата Y начала области.
        /// </summary>
        public int Y => Region.Y;

        /// <summary>
        /// Возвращает координату, являющуюся суммой свойств X и Width.
        /// </summary>
        public int Right => Region.Right;

        /// <summary>
        /// Возвращает координату, являющуюся суммой свойств Y и Height.
        /// </summary>
        public int Bottom => Region.Bottom;

        /// <summary>
        /// Выясняет, пересекается ли текущая область с указанной.
        /// </summary>
        /// <param name="rect">Проверяемая область.</param>
        /// <returns>Возвращает true в случае обнаружения пересечения.</returns>
        public bool IsConflict(Rectangle rect)
        {
            return rect.IntersectsWith(Region);
        }
    }

    /// <summary>
    /// Представляет регион, разделённый на области, ни одна из которых не пересекается с другой.
    /// </summary>
    public sealed class Region
    {
        /// <summary>
        /// Хранит области, индексируя их как Width * y + x.
        /// </summary>
        readonly SortedDictionary<ulong, Registered> _rects = new SortedDictionary<ulong, Registered>();

        /// <summary>
        /// Ширина региона.
        /// </summary>
        public int Width { get; }

        /// <summary>
        /// Высота региона.
        /// </summary>
        public int Height { get; }

        /// <summary>
        /// Получает коллекцию хранимых областей.
        /// </summary>
        public IEnumerable<Registered> Elements => _rects.Values;

        /// <summary>
        /// Получает количество добавленных областей.
        /// </summary>
        public int Count => _rects.Count;

        /// <summary>
        /// Получает коллекцию хранимых координат областей.
        /// </summary>
        public IEnumerable<Rectangle> Rectangles => _rects.Values.Select(reg => reg.Region);

        /// <summary>
        /// Получает область, на которую попадает указанная точка или null, если она не попадает ни на какую.
        /// </summary>
        /// <param name="pt">Заданная точка.</param>
        /// <returns>Возвращает область, на которую попадает указанная точка или null, если она не попадает ни на какую.</returns>
        public Registered this[Point pt] => this[pt.X, pt.Y];

        /// <summary>
        /// Инициализирует регион с указанными параметрами ширины и высоты.
        /// </summary>
        /// <param name="width">Ширина региона.</param>
        /// <param name="height">Высота региона.</param>
        public Region(int width, int height)
        {
            if (width <= 0)
                throw new ArgumentException($"{nameof(Region)}: Ширина не может быть меньше или равна нолю ({width}).", nameof(width));
            if (height <= 0)
                throw new ArgumentException($"{nameof(Region)}: Высота не может быть меньше или равна нолю ({height}).", nameof(height));
            Width = width;
            Height = height;
        }

        /// <summary>
        /// Вычисляет индекс по формуле Width * y + x.
        /// </summary>
        /// <param name="x">Координата X.</param>
        /// <param name="y">Координата Y.</param>
        /// <returns>Возвращает индекс по формуле Width * y + x.</returns>
        ulong GetIndex(int x, int y)
        {
            if (x < 0)
                throw new ArgumentException($"{nameof(GetIndex)}: Координата X не может быть меньше ноля ({x}).", nameof(x));
            if (y < 0)
                throw new ArgumentException($"{nameof(GetIndex)}: Координата Y не может быть меньше ноля ({y}).", nameof(y));
            if (x >= Width)
                throw new ArgumentException($"{nameof(GetIndex)}: Координата X не может быть больше или равна ширине ({x}).", nameof(x));
            if (y >= Height)
                throw new ArgumentException($"{nameof(GetIndex)}: Координата Y не может быть больше или равна высоте ({y}).", nameof(y));
            return Convert.ToUInt64(Width * y + x);
        }

        /// <summary>
        /// Получает область, на которую попадает указанная точка или null, если она не попадает ни на какую.
        /// </summary>
        /// <param name="x">Координата X.</param>
        /// <param name="y">Координата Y.</param>
        /// <returns>Возвращает область, на которую попадает указанная точка или null, если она не попадает ни на какую.</returns>
        public Registered this[int x, int y]
        {
            get
            {
                if (x < 0)
                    throw new ArgumentException($"Indexer: Координата X не может быть меньше ноля ({x}).", nameof(x));
                if (y < 0)
                    throw new ArgumentException($"Indexer: Координата Y не может быть меньше ноля ({y}).", nameof(y));
                if (x >= Width)
                    throw new ArgumentException($"Indexer: Координата X не может быть больше или равна ширине ({x}).", nameof(x));
                if (y >= Height)
                    throw new ArgumentException($"Indexer: Координата Y не может быть больше или равна высоте ({y}).", nameof(y));
                return Elements.FirstOrDefault(reg => x >= reg.X && x < reg.Right && y >= reg.Y && y < reg.Bottom);
            }
        }

        /// <summary>
        /// Определяет, перекрывается ли указанная область с какой-либо из содержащихся в объекте.
        /// </summary>
        /// <param name="rect">Проверяемая область.</param>
        /// <returns>В случае, если перекрываются, возвращает true, в противном случае false.</returns>
        public bool IsConflict(Rectangle rect)
        {
            if (rect.Right > Width || rect.Bottom > Height)
                return true;
            return _rects.Values.Any(reg => reg.IsConflict(rect));
        }

        /// <summary>
        /// Уточняет, присутствует карта с указанным названием в текущем регионе или нет.
        /// </summary>
        /// <param name="processorName">Проверяемая карта.</param>
        /// <param name="index">Индекс, с которого необходимо начать выбор подстроки названия указанной карты.</param>
        /// <returns>Возвращает значение true в случае, если указанная карта присутствует в текущем регионе, в противном случае - false.</returns>
        public bool Contains(string processorName, int index)
        {
            if (processorName == null)
                throw new ArgumentNullException(nameof(processorName), $@"{nameof(Contains)}: Проверяемая карта отсутствует (null).");
            if (index < 0)
                throw new ArgumentOutOfRangeException(nameof(index), $@"{nameof(Contains)}: Индекс подстроки названия карты недопустим: ({index}).");
            if (processorName == null)
                throw new ArgumentNullException(nameof(processorName), $@"{nameof(Contains)}: Имя проверяемой карты пустое (null).");
            if (processorName == string.Empty)
                throw new ArgumentException($"{nameof(Contains)}: Имя проверяемой карты пустое (\"\").", nameof(processorName));
            return Elements.Where(registered => registered != null && !registered.IsEmpty).Any(registered => registered.Register.Where(reg => reg.Procs != null).
                Any(reg => reg.Procs.Where(proc => proc != null).Any(proc => proc.IsProcessorName(processorName, index))));
        }

        /// <summary>
        /// Вставляет указанную область в коллекцию.
        /// </summary>
        /// <param name="rect">Вставляемая область.</param>
        public void Add(Rectangle rect)
        {
            if (rect.Right > Width)
                throw new ArgumentException($@"{nameof(Add)}: Попытка вставить элемент, конфликтующий с шириной региона (Width = {Width
                    }, {nameof(rect.Right)} = {rect.Right}).", nameof(rect));
            if (rect.Bottom > Height)
                throw new ArgumentException($@"{nameof(Add)}: Попытка вставить элемент, конфликтующий с высотой региона (Height = {Height
                    }, {nameof(rect.Bottom)} = {rect.Bottom}).", nameof(rect));
            if (IsConflict(rect))
                throw new ArgumentException($"{nameof(Add)}: Попытка вставить элемент, конфликтующий с существующими.", nameof(rect));
            _rects[GetIndex(rect.X, rect.Y)] = new Registered { Region = rect };
        }

        /// <summary>
        /// Вставляет указанную область в коллекцию.
        /// </summary>
        /// <param name="x">Координата X.</param>
        /// <param name="y">Координата Y.</param>
        /// <param name="width">Ширина.</param>
        /// <param name="height">Высота.</param>
        public void Add(int x, int y, int width, int height)
        {
            Add(new Rectangle(x, y, width, height));
        }

        /// <summary>
        /// Удаляет область по указанным координатам.
        /// </summary>
        /// <param name="x">Координата X.</param>
        /// <param name="y">Координата Y.</param>
        public void Remove(int x, int y)
        {
            _rects.Remove(GetIndex(x, y));
        }

        /// <summary>
        /// Удаляет все области из текущего объекта.
        /// </summary>
        public void Clear()
        {
            _rects.Clear();
        }
    }
}