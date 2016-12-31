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
            CurrentString = str;
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
            List<int> lstCompare = new List<int>(str.Length);
            // ReSharper disable once LoopCanBeConvertedToQuery
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
        /// Возвращает количество раз, которое встречается искомый символ в строке или -1 в случае отсутствия такового.
        /// </summary>
        /// <param name="ch">Искомый символ.</param>
        /// <returns>Возвращает количество раз, которое встречается искомый символ в строке или -1 в случае отсутствия такового.</returns>
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
            if (string.IsNullOrEmpty(str))
                throw new ArgumentException($"{nameof(TagSearcher)}: Задана пустая строка.", nameof(str));
            _currentStr = str.ToUpper();
        }

        /// <summary>
        /// Выполняет поиск подстроки в строке и возвращает <see cref="FindString" /> в случае соответствия строк.
        /// </summary>
        /// <param name="tag">Строка, поиск которой необходимо выполнить.</param>
        /// <returns>Возвращает <see cref="FindString" /> в случае соответствия строк.</returns>
        public IEnumerable<FindString> Find(string tag)
        {
            if (string.IsNullOrEmpty(tag))
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
            if (string.IsNullOrEmpty(str))
                throw new ArgumentException($"{nameof(GetStringChunk)}: Попытка получить часть пустой строки.", nameof(str));
            if (str.Length > _currentStr.Length)
                throw new ArgumentException($"{nameof(GetStringChunk)}: Длина искомой строки ({str.Length}) больше, чем исходной ({_currentStr.Length}).",
                    nameof(str));
            for (int k = 0, max = _currentStr.Length - str.Length; k <= max; k++)
            {
                string s = _currentStr.Substring(k, str.Length);
                FindString fs = new FindString(s, k);
                yield return fs;
            }
        }
    }
}