using System.Collections.Generic;
using System;

namespace SearchEngine
{
    public class Trie
    {
        private Node _root;

        public int Size { get; set; }

        public Trie()
        {
            _root = new Node(null, ' ', null, 0);
        }

        public Node TraverseFromRoot(string key)
        {
            Node currentNode = _root;
            Node result = currentNode;
            foreach (char letter in key)
            {
                currentNode = currentNode.GetChildByKey(letter);
                if (currentNode == null)
                {
                    break;
                }
                result = currentNode;
            }

            return result;
        }

        public void Insert(string key, string data)
        {
            Node current = TraverseFromRoot(key);

            // If it still does not reach the end of key
            // add middle nodes
            for (int i = current.Depth; i < key.Length; i++)
            {
                var child = new Node(current, key[i], null, current.Depth + 1);
                current.Children.Add(key[i], child);
                current = child;
                Size += 1;
            }

            // add data for last node
            current.Add(data);
        }

        public bool ContainsKey(string key)
        {
            Node prefix = TraverseFromRoot(key);
            return prefix != null && prefix.Depth == key.Length && prefix.ContainsData();
        }

        public string[] QueryFromNode(string key)
        {
            if (ContainsKey(key))
            {
                Node prefix = TraverseFromRoot(key);
                return prefix.GetData();
            }

            return new string[] { };
        }

        public HashSet<string> QueryFromChildrenNodes(string key)
        {
            Node prefix = TraverseFromRoot(key);
            HashSet<string> toReturn = new HashSet<string>();

            foreach (var childKey in prefix.Children.Keys)
            {
                string[] currentChildrenData = prefix.Children[childKey].GetData();
                for (int j = 0; j < currentChildrenData.Length; j++)
                {
                    toReturn.Add(currentChildrenData[j]);

                }
            }

            return toReturn;
        }

        public HashSet<string> QueryDeep(string key)
        {
            Node prefix = TraverseFromRoot(key);
            return QueryDeepFromNode(prefix, new HashSet<string>());
        }

        public HashSet<string> QueryDeepFromNode(Node prefix, HashSet<string> searchData)
        {
            foreach (string dataChild in prefix.GetData())
            {
                searchData.Add(dataChild);
            }

            foreach (var key in prefix.Children.Keys)
            {
                QueryDeepFromNode(prefix.Children[key], searchData);
            }

            return searchData;
        }

        public bool Remove(string key)
        {
            if (ContainsKey(key))
            {
                Node prefix = TraverseFromRoot(key);

                while (prefix.IsLeaf())
                {
                    Node parent = prefix.Parent;
                    parent.DeleteChildByKey(prefix.Key);
                    Size -= 1;
                    prefix = parent;
                }

                return true;
            }
            else
            {
                return false;
            }
        }

        public void Flush()
        {
            _root = new Node(null, ' ', null, 0);
            Size = 0;
            GC.Collect();
        }

        public void BatchInsert(List<KeyValuePair<string, string>> items)
        {
            foreach(KeyValuePair<string, string> item in items)
            {
                Insert(item.Key, item.Value);
            }
        }
    }
}
