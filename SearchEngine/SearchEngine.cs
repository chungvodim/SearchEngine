using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace SearchEngine
{
    public class SearchEngine
    {
        private readonly Trie _trie = new Trie();
        public Trie Trie
        {
            get => _trie;
        }

        private int _count = 0;
        private bool _debug = false;
        private bool _orderFixed = true;
        private readonly bool _normalize = true;
        private readonly Regex _rgx;
        private readonly int _memoryLimit = 0;

        public SearchEngine(bool debug = false,
            bool normalize = true,
            bool orderFixed = true,
            string normalizePattern = "[^a-zA-Z0-9 -]", int memoryLimit = 0)
        {
            _debug = debug;
            _normalize = normalize;
            _orderFixed = orderFixed;
            _rgx = new Regex(normalizePattern);
            _memoryLimit = memoryLimit;
        }

        public bool Debug { get => _debug; set => _debug = value; }
        public bool OrderFixed { get => _orderFixed; set => _orderFixed = value; }
        public int Count { get => _count; }

        private bool Insert(string key, string resourceName)
        {
            if (_memoryLimit == 0 || GC.GetTotalMemory(false) < _memoryLimit)
            {
                string wordToInsert = CleanWord(key);

                if (_debug && Count % 50000 == 0)
                {
                    Console.WriteLine($"Batch {Count} with total {_trie.Size} nodes with memory size of {GC.GetTotalMemory(false)} bytes ");
                }

                if (_orderFixed)
                {
                    _count++;
                    _trie.Insert(wordToInsert, resourceName);
                }
                else
                {
                    var wordsToInsert = GenerateAllPosibleWordsFromOrgin(wordToInsert);
                    foreach (var word in wordsToInsert)
                    {
                        _count++;
                        _trie.Insert(word, resourceName);
                    }
                }

                return true;
            }
            else
            {
                return false;
            }
        }

        private List<string> GenerateAllPosibleWordsFromOrgin(string set)
        {
            return GenerateAllPosibleWords(set, string.Empty);
        }

        private List<string> GenerateAllPosibleWords(string set, string prefix)
        {
            if (set.Length == 0)
            {
                return new List<string>() { prefix };
            }

            var result = new List<string>();

            for (int i = 0; i < set.Length; i++)
            {
                var newPrefix = prefix + set[i];
                result.AddRange(GenerateAllPosibleWords(set.Remove(i, 1), newPrefix));
            }

            return result;
        }

        public void InsertResource(string resourceName, string content)
        {
            string[] contentWords = content.Split(' ');

            foreach (string word in contentWords)
            {
                Insert(word, resourceName);
            }
        }

        public string Echo(string text)
        {
            return text;
        }

        private string CleanWord(string word)
        {
            string wordToReturn;
            if (_normalize)
            {
                wordToReturn = _rgx.Replace(word, "");
                wordToReturn = wordToReturn.Trim();
                wordToReturn = wordToReturn.ToLower();
            }
            else
            {
                wordToReturn = word;
            }

            return wordToReturn;
        }

        public List<string> Get(string key)
        {
            string wordToSearch = CleanWord(key);

            List<string> toReturn = new List<string>();

            string[] data = _trie.QueryFromNode(wordToSearch);

            foreach (string element in data)
            {
                toReturn.Add(element);
            }

            return toReturn;
        }

        public IEnumerable<string> QueryDeep(string key)
        {
            string wordToSearch = CleanWord(key);
            return _trie.QueryDeep(wordToSearch);
        }

        public IEnumerable<string> QueryShallow(string key)
        {
            string wordToSearch = CleanWord(key);
            return _trie.QueryFromChildrenNodes(wordToSearch);
        }

        public bool Remove(string key)
        {
            return _trie.Remove(key);
        }

        public void Flush()
        {
            _count = 0;
            _trie.Flush();
        }
    }
}
