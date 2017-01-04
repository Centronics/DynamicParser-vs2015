using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DynamicParser
{
    /// <summary>
    /// Выполняет поиск соответствия слова указанной строке, состоящей из массива слов (букв).
    /// </summary>
    public class WordSearcher
    {
        /// <summary>
        /// Инициализирует текущий экземпляр коллекцией карт. Используется поле Tag.
        /// </summary>
        /// <param name="mas">Коллекция карт.</param>
        public WordSearcher(IEnumerable<Processor> mas)
        {

        }

        /// <summary>
        /// Инициализирует текущий экземпляр коллекцией слов (букв).
        /// </summary>
        /// <param name="str">Коллекция предложений.</param>
        public WordSearcher(IEnumerable<string> str)
        {

        }

        /// <summary>
        /// Выполняет проверку соответствия слов, содержащихся в текущем экземпляре, с заданным словом.
        /// </summary>
        /// <param name="word">Проверяемое слово.</param>
        /// <returns>Возвращает значение true в случае, если соответствие обнаружено, в противном случае - false.</returns>
        public bool FindWord(string word)
        {

        }
    }
}