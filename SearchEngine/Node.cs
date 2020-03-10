using System.Collections;
using System.Collections.Generic;

namespace SearchEngine
{
    public class Node : ICollection<string>
    {
        private HashSet<string> _data = new HashSet<string>();
        internal Node Parent { get; set; }
        public char Key { get; set; }
        public Dictionary<char, Node> Children { get; set; } = new Dictionary<char, Node>();
        public int Depth { get; set; }
        public int Count => _data.Count;
        public bool IsReadOnly => false;

        public Node(Node parent, char key, string data = null, int depth = 0)
        {
            Parent = parent;
            Key = key;
            if (data != null)
            {
                _data.Add(data);
            }
            Depth = depth;
        }

        public Node GetChildByKey(char key)
        {
            return Children.TryGetValue(key, out var child) ? child : null;
        }

        public bool DeleteChildByKey(char key)
        {
            return Children.Remove(key);
        }

        public bool IsLeaf()
        {
            return Children.Count == 0 && Parent != null;
        }

        public bool ContainsData()
        {
            return _data != null;
        }

        public bool ContainsData(string dataToCompare)
        {
            return _data.Contains(dataToCompare);
        }

        public void Add(string item)
        {
            _data.Add(item);
        }

        public void Clear()
        {
            _data.Clear();
        }

        /// <summary>
        /// Return a clone of data
        /// </summary>
        /// <returns></returns>
        public string[] GetData()
        {
            string[] dataToReturn = new string[_data.Count];
            _data.CopyTo(dataToReturn);
            return dataToReturn;
        }

        public bool Contains(string item)
        {
            return _data.Contains(item);
        }

        public void CopyTo(string[] array, int arrayIndex)
        {
            _data.CopyTo(array, arrayIndex);
        }

        public bool Remove(string item)
        {
            return _data.Remove(item);
        }

        public IEnumerator<string> GetEnumerator()
        {
            return _data.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _data.GetEnumerator();
        }
    }
}
