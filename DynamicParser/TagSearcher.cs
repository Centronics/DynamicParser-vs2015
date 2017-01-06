using System;
using System.Collections.Generic;
using System.Linq;

namespace DynamicParser
{
    /// <summary>
    /// Представляет найденную подстроку и позицию её местонахождения.
    /// </summary>
    public sealed class FindString
    {
        /// <summary>
        /// Найденная подстрока.
        /// </summary>
        public string CurrentString { get; }

        /// <summary>
        /// Позиция, на которой находится подстрока.
        /// </summary>
        public int Position { get; }

        /// <summary>
        /// Счётчик количества каждой буквы в сравниваемой строке.
        /// </summary>
        readonly Dictionary<char, int> _dicCurrent;

        /// <summary>
        /// Инициализирует структуру <see cref="FindString" /> исходными значениями.
        /// </summary>
        /// <param name="str">Подстрока.</param>
        /// <param name="position">Позиция подстроки.</param>
        public FindString(string str, int position)
        {
            CurrentString = str.ToUpper();
            Position = position;
            _dicCurrent = GetCount(CurrentString);
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
            Dictionary<char, int> dicCompare = GetCount(str);
            foreach (char ch in _dicCurrent.Keys)
            {
                int val;
                if (!dicCompare.TryGetValue(ch, out val)) return false;
                if (_dicCurrent[ch] == val)
                    continue;
                return false;
            }
            return true;
        }

        /// <summary>
        /// Получает счётчики количества попаданий каждой буквы.
        /// Буква является ключом в словаре, значение является количеством попаданий буквы, находящейся на этой позиции.
        /// </summary>
        /// <param name="str">Проверяемая строка.</param>
        /// <returns>Возвращает счётчики количества попаданий каждой буквы.</returns>
        Dictionary<char, int> GetCount(string str)
        {
            str = str.ToUpper();
            Dictionary<char, int> dic = new Dictionary<char, int>(str.Length);
            foreach (char t in str)
            {
                int cou = GetCount(t);
                int val;
                if (dic.TryGetValue(t, out val))
                    dic[t] = val + cou;
                else
                    dic[t] = cou;
            }
            return dic;
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
        /// Возвращает все варианты эквивалентных строк.
        /// </summary>
        /// <param name="tag">Проверяемая строка.</param>
        /// <returns>Возвращает структуры <see cref="FindString" /> в случае, когда требуемые подстроки найдены, в противном случае null.</returns>
        public IEnumerable<FindString> FindEqual(string tag)
        {
            if (tag == null)
                throw new ArgumentNullException(nameof(tag), $"{nameof(FindEqual)}: Задана пустая строка (null).");
            if (tag == string.Empty)
                throw new ArgumentException($"{nameof(FindEqual)}: Задана пустая строка.", nameof(tag));
            foreach (FindString fs in Find(tag))
                yield return fs;
        }

        /// <summary>
        /// Проверяет, является эквивалентом указанная строка по отношению к текущей или нет.
        /// </summary>
        /// <param name="tag">Проверяемая строка.</param>
        /// <returns>Возвращает значение true в случае, когда указанная подстрока найдена, в противном случае false.</returns>
        public bool IsEqual(string tag)
        {
            if (tag == null)
                throw new ArgumentNullException(nameof(tag), $"{nameof(IsEqual)}: Задана пустая строка (null).");
            if (tag == string.Empty)
                throw new ArgumentException($"{nameof(IsEqual)}: Задана пустая строка.", nameof(tag));
            return FindEqual(tag).Any();
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