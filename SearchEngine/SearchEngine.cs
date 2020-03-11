using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace SearchEngine
{
    public class SearchEngine
    {
        private readonly Trie _trie = new Trie();
        private readonly bool _normalize;
        private readonly Regex _rgx;
        private readonly int _memoryLimit;

        public SearchEngine(bool debug = false,
            bool normalize = true,
            bool orderFixed = true,
            int numberOfPermutation = 2,
            string normalizePattern = "[^a-zA-Z0-9 -]", int memoryLimit = 0)
        {
            Debug = debug;
            _normalize = normalize;
            OrderFixed = orderFixed;
            NumberOfPermutation = numberOfPermutation;
            _rgx = new Regex(normalizePattern);
            _memoryLimit = memoryLimit;
        }

        public bool Debug { get; set; }
        public bool OrderFixed { get; set; }
        public int NumberOfPermutation { get; set; }
        public int Count { get; private set; }
        public int TrieSize => _trie.Size;

        private bool Insert(string key, string resourceName)
        {
            if (_memoryLimit == 0 || GC.GetTotalMemory(false) < _memoryLimit)
            {
                string wordToInsert = CleanWord(key);

                if (Debug && Count % 50000 == 0)
                {
                    Console.WriteLine($"Batch {Count} with total {_trie.Size} nodes with memory size of {GC.GetTotalMemory(false)} bytes ");
                }

                if (OrderFixed)
                {
                    Count++;
                    Console.WriteLine($"Insert key {wordToInsert} for resource {resourceName}");
                    _trie.Insert(wordToInsert, resourceName);
                }
                else
                {
                    var wordsToInsert = GenerateAllPosibleWordsFromOrgin(wordToInsert);
                    foreach (var word in wordsToInsert)
                    {
                        Count++;
                        Console.WriteLine($"Insert key {word} for resource {resourceName}");
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
            if(NumberOfPermutation == 0)
            {
                return GenerateAllPermutationWords(set, string.Empty);
            }
            else
            {
                return GeneratePermutationWordsWithLimit(set, string.Empty);
            }
        }

        private List<string> GenerateAllPermutationWords(string set, string prefix)
        {
            if (set.Length == 0)
            {
                return new List<string>() { prefix };
            }

            var result = new List<string>();

            for (int i = 0; i < set.Length; i++)
            {
                var newPrefix = prefix + set[i];
                result.AddRange(GenerateAllPermutationWords(set.Remove(i, 1), newPrefix));
            }

            return result;
        }

        private List<string> GeneratePermutationWordsWithLimit(string set, string prefix)
        {
            if (prefix.Length == NumberOfPermutation)
            {
                for (int i = 0; i < set.Length; i++)
                {
                    if (set[i] == ' ')
                    {
                        set = set.Set(i, prefix[0]);
                        prefix = prefix.Substring(1);
                    }
                }
                return new List<string>() { set };
            }

            var result = new List<string>();

            for (int i = 0; i < set.Length; i++)
            {
                if (set[i] == ' ') continue;
                var newPrefix = prefix + set[i];
                result.AddRange(GeneratePermutationWordsWithLimit(set.Set(i, ' '), newPrefix));
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
            Count = 0;
            _trie.Flush();
        }
    }
}
