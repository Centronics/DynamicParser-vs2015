using System;
using System.Collections.Generic;
using System.Linq;

namespace DynamicParser
{
    /// <summary>
    /// Представляет найденную подстроку и позицию её местонахождения.
    /// </summary>
    public struct FindString
    {
        /// <summary>
        /// Найденная подстрока.
        /// </summary>
        public readonly string CurrentString;

        /// <summary>
        /// Позиция, на которой находится подстрока.
        /// </summary>
        public readonly int Position;

        /// <summary>
        /// Инициализирует структуру <see cref="FindString" /> исходными значениями.
        /// </summary>
        /// <param name="str">Подстрока.</param>
        /// <param name="position">Позиция подстроки.</param>
        public FindString(string str, int position)
        {
            CurrentString = str.ToUpper();
            Position = position;
        }

        /// <summary>
        /// Получает значение, определяющее сходство между указанной строкой и текущей.
        /// </summary>
        /// <param name="str">Сравниваемая строка.</param>
        /// <returns>Возвращает значение true в случае сходства строк, false в противном случае.</returns>
        public bool GetStringEqual(string str)
        {
            if (string.IsNullOrEmpty(str))
                return false;
            if (str.Length != CurrentString.Length)
                return false;
            str = str.ToUpper();
            List<int> lstCompare = new List<int>(str.Length);
            foreach (char ch in str)
            {
                int count = GetCount(ch);
                if (count <= 0)
                    return false;
                lstCompare.Add(count);
            }
            return CurrentString.Length == lstCompare.Sum();
        }

        /// <summary>
        /// Возвращает количество раз, которое встречается искомый символ в строке или ноль в случае отсутствия такового.
        /// </summary>
        /// <param name="ch">Искомый символ.</param>
        /// <returns>Возвращает количество раз, которое встречается искомый символ в строке или ноль в случае отсутствия такового.</returns>
        int GetCount(char ch)
        {
            return CurrentString.Count(c => c == ch);
        }
    }

    /// <summary>
    /// Производит поиск заданного Tag в строке и возвращает варианты возможных соответствий.
    /// </summary>
    public class TagSearcher
    {
        /// <summary>
        /// Анализируемая строка.
        /// </summary>
        readonly string _currentStr;

        /// <summary>
        /// Задаёт анализируемую строку.
        /// </summary>
        /// <param name="str">Анализируемая строка.</param>
        public TagSearcher(string str)
        {
            if (str == null)
                throw new ArgumentNullException(nameof(str), $"{nameof(TagSearcher)}: Задана пустая строка (null).");
            if (str == string.Empty)
                throw new ArgumentException($"{nameof(TagSearcher)}: Задана пустая строка.", nameof(str));
            _currentStr = str;
        }

        /// <summary>
        /// Проверяет, является эквивалентом указанная строка по отношению к текущей или нет.
        /// </summary>
        /// <param name="tag">Проверяемая строка.</param>
        /// <returns>Возвращает структуру <see cref="FindString" /> в случае, когда указанная подстрока найдена, в противном случае null.</returns>
        public IEnumerable<FindString> IsEqual(string tag)
        {
            if (tag == null)
                throw new ArgumentNullException(nameof(tag), $"{nameof(IsEqual)}: Задана пустая строка (null).");
            if (tag == string.Empty)
                throw new ArgumentException($"{nameof(IsEqual)}: Задана пустая строка.", nameof(tag));
            foreach (FindString fs in Find(tag))
                yield return fs;
        }

        /// <summary>
        /// Выполняет поиск подстроки в строке и возвращает <see cref="FindString" /> в случае соответствия строк.
        /// </summary>
        /// <param name="tag">Строка, поиск которой необходимо выполнить.</param>
        /// <returns>Возвращает <see cref="FindString" /> в случае соответствия строк.</returns>
        IEnumerable<FindString> Find(string tag)
        {
            if (tag == null)
                throw new ArgumentNullException(nameof(tag), $"{nameof(Find)}: Задана пустая строка (null).");
            if (tag == string.Empty)
                throw new ArgumentException($"{nameof(Find)}: Задана пустая строка.", nameof(tag));
            foreach (FindString fs in GetStringChunk(tag))
                if (fs.GetStringEqual(tag))
                    yield return fs;
        }

        /// <summary>
        /// Получает части строки, равные по длине искомой строки.
        /// </summary>
        /// <param name="str">Искомая строка.</param>
        /// <returns>Возвращает <see cref="FindString" /> для строк, которые равны по длине искомой строке.</returns>
        IEnumerable<FindString> GetStringChunk(string str)
        {
            if (str == null)
                throw new ArgumentNullException(nameof(str), $"{nameof(GetStringChunk)}: Попытка получить часть пустой строки (null).");
            if (str == string.Empty)
                throw new ArgumentException($"{nameof(GetStringChunk)}: Попытка получить часть пустой строки.", nameof(str));
            if (str.Length != _currentStr.Length)
                throw new ArgumentException($"{nameof(GetStringChunk)}: Длина искомой строки ({str.Length}) не равна длине исходной ({_currentStr.Length}).",
                    nameof(str));
            for (int k = 0, max = _currentStr.Length - str.Length, sl = str.Length; k <= max; k++)
            {
                FindString fs = new FindString(_currentStr.Substring(k, sl), k);
                yield return fs;
            }
        }
    }
}