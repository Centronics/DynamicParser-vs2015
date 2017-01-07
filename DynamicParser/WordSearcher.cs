using System;
using System.Collections.Generic;
using System.Linq;
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
        readonly List<List<string>> _words = new List<List<string>>();

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
                List<string> lsts = new List<string>(str.Count);
                lsts.AddRange(str.Where(s => !string.IsNullOrEmpty(s)));
                if (lsts.Count <= 0)
                    continue;
                _words.Add(lsts);
            }
            if (Count <= 0)
                throw new ArgumentException($"{nameof(WordSearcher)}: Массив слов пустой ({Count}).", nameof(strs));
        }

        /// <summary>
        /// Выполняет проверку соответствия слов, содержащихся в текущем экземпляре, с заданным словом.
        /// </summary>
        /// <param name="word">Проверяемое слово.</param>
        /// <returns>Возвращает значение true в случае, если соответствие обнаружено, в противном случае - false.</returns>
        public bool IsEqual(string word)
        {
            if (string.IsNullOrEmpty(word))
                return false;
            if (word.Length != Count)
                throw new ArgumentException($@"{nameof(IsEqual)}: Длины сопоставляемых слов должны быть равны: проверяемое слово длиной {word.Length
                    } сопоставляется со словом длиной {Count}.", nameof(word));
            TagSearcher ts = new TagSearcher(word);
            int[] count = new int[Count];
            for (int counter = Count - 1; counter >= 0;)
            {
                if (ts.IsEqual(GetWord(count)))
                    return true;
                if ((counter = ChangeCount(count)) < 0)
                    return false;
            }
            return false;
        }

        /// <summary>
        /// Увеличивает значение старших разрядов счётчика букв, если это возможно.
        /// Если увеличение было произведено, возвращается номер позиции, на которой произошло изменение, в противном случае -1.
        /// </summary>
        /// <param name="count">Массив-счётчик.</param>
        /// <returns>Возвращается номер позиции, на которой произошло изменение, в противном случае -1.</returns>
        int ChangeCount(int[] count)
        {
            if (count == null || count.Length != Count)
                throw new ArgumentException($"{nameof(ChangeCount)}: Массив-счётчик не указан или его длина некорректна ({count?.Length}).", nameof(count));
            for (int k = Count - 1; k >= 0; k--)
            {
                if (count[k] >= this[k].Count - 1) continue;
                count[k]++;
                for (int x = k + 1; x < count.Length; x++)
                    count[x] = 0;
                return k;
            }
            return -1;
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