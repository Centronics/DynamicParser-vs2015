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
        readonly List<string> _words = new List<string>();

        /// <summary>
        /// Получает количество слов в коллекции.
        /// </summary>
        public int Count { get; }

        /// <summary>
        /// Инициализирует текущий экземпляр коллекцией слов.
        /// Выравнивает (синхронизирует) наличие всех слов в указанных коллекциях, в одной коллекции.
        /// Возвращает список строк, в котором отсутствуют дублирующиеся и пустые значения.
        /// Отсев производится без учёта регистра.
        /// </summary>
        /// <param name="strs">Коллекция слов.</param>
        public WordSearcher(IEnumerable<IList<string>> strs)
        {
            if (strs == null)
                throw new ArgumentNullException(nameof(strs), $"{nameof(WordSearcher)}: Массив слов равен null.");
            int count = 0;
            foreach (IList<string> lst in strs)
            {
                if (lst == null || lst.Count <= 0)
                    continue;
                foreach (string str in lst)
                {
                    if (string.IsNullOrEmpty(str))
                        continue;
                    if (_words.Any(s => string.Compare(s, str, StringComparison.OrdinalIgnoreCase) == 0))
                        continue;
                    _words.Add(str);
                }
                count++;
            }
            Count = count;
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
                if (count[k] >= _words.Count - 1) continue;
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
                sb.Append(_words[count[k]][0]);
            return sb.ToString();
        }
    }
}