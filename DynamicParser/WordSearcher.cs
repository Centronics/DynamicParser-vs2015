using System;
using System.Collections.Generic;
using System.Text;

namespace DynamicParser
{
    /// <summary>
    /// Выполняет поиск соответствия слова указанной строке, состоящей из массива слов.
    /// </summary>
    public class WordSearcher
    {
        /// <summary>
        /// Содержит коллекцию слов.
        /// </summary>
        readonly List<IList<string>> _words = new List<IList<string>>();

        /// <summary>
        /// Получает количество слов в коллекции.
        /// </summary>
        public int Count => _words.Count;

        /// <summary>
        /// Получает слово по индексу.
        /// </summary>
        /// <param name="index">Индекс слова.</param>
        /// <returns>Возвращает слово по индексу.</returns>
        public IList<string> this[int index] => _words[index];

        /// <summary>
        /// Инициализирует текущий экземпляр коллекцией слов.
        /// </summary>
        /// <param name="strs">Коллекция слов.</param>
        public WordSearcher(IEnumerable<IList<string>> strs)
        {
            if (strs == null)
                throw new ArgumentNullException(nameof(strs), $"{nameof(WordSearcher)}: Массив слов равен null.");
            foreach (IList<string> str in strs)
            {
                if (str == null || str.Count <= 0)
                    continue;
                _words.Add(str);
            }
            if (Count <= 0)
                throw new ArgumentException($"{nameof(WordSearcher)}: Массив слов пустой ({Count}).", nameof(strs));
        }

        /// <summary>
        /// Выполняет проверку соответствия слов, содержащихся в текущем экземпляре, с заданным словом.
        /// </summary>
        /// <param name="word">Проверяемое слово.</param>
        /// <returns>Возвращает значение true в случае, если соответствие обнаружено, в противном случае - false.</returns>
        public bool FindWord(string word)
        {
            if (string.IsNullOrEmpty(word))
                return false;
            TagSearcher ts = new TagSearcher(word);
            int[] count = new int[Count];
            for (int counter = Count - 1; counter >= 0;)
            {
                for (int k = 0; k < this[counter][k].Length; k++)
                {
                    count[counter] = k;
                    if (ts.IsEqual(GetWord(count)))
                        return true;
                }
                for (int k = counter + 1; k < count.Length; k++)
                    count[k] = 0;
                count[counter]++;
                if (count[counter] <= this[counter].Count) continue;
                count[counter] = 0;
                counter--;
            }
            return true;
        }

        /// <summary>
        /// Генерирует слово из частей, содержащихся в коллекции, основываясь на данных счётчиков.
        /// </summary>
        /// <param name="count">Данные счётчиков по каждому слову.</param>
        /// <returns>Возвращает слово из частей, содержащихся в коллекции.</returns>
        string GetWord(IList<int> count)
        {
            if (count == null)
                throw new ArgumentNullException(nameof(count), $"{nameof(GetWord)}: Массив данных равен null.");
            if (count.Count != Count)
                throw new ArgumentException($"{nameof(GetWord)}: Длина массива данных должна совпадать с количеством хранимых слов.", nameof(count));
            StringBuilder sb = new StringBuilder();
            for (int k = 0; k < Count; k++)
                sb.Append(this[k][count[k]]);
            return sb.ToString();
        }
    }
}