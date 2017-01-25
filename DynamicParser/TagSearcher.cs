using System;
using System.Collections.Generic;
using System.Linq;

namespace DynamicParser
{
    /// <summary>
    /// Сравнивает две строки по количеству встречающихся букв в каждой строке.
    /// </summary>
    public sealed class TagSearcher
    {
        /// <summary>
        /// Исходная строка.
        /// </summary>
        public string SourceString { get; }

        /// <summary>
        /// Счётчик количества каждой буквы в исходной строке.
        /// </summary>
        readonly Dictionary<char, int> _dicCurrent;

        /// <summary>
        /// Инициализирует класс <see cref="TagSearcher" /> исходной строкой.
        /// </summary>
        /// <param name="str">Строка, с которой будет производиться сравнение.</param>
        public TagSearcher(string str)
        {
            if (str == null)
                throw new ArgumentNullException(nameof(str), $"{nameof(TagSearcher)}: Подстрока пустая (null).");
            if (str == string.Empty)
                throw new ArgumentException($"{nameof(TagSearcher)}: Подстрока не может быть пустой.", nameof(str));
            _dicCurrent = GetCount(SourceString = str.ToUpper());
        }

        /// <summary>
        /// Получает значение, определяющее сходство между указанной строкой и текущей.
        /// </summary>
        /// <param name="str">Сравниваемая строка.</param>
        /// <returns>Возвращает значение true в случае сходства строк, false в противном случае.</returns>
        public bool IsEqual(string str)
        {
            if (string.IsNullOrEmpty(str))
                return false;
            if (str.Length != SourceString.Length)
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
            return SourceString.Count(c => c == ch);
        }
    }
}