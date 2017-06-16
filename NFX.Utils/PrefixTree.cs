using System.Collections;
using System.Collections.Generic;
using NFX.ApplicationModel.Pile;
using NFX.ServiceModel;
using System.Linq;
using System;

namespace NFX.Utils
{
    public class PrefixTree<T>: DisposableObject, IEnumerable<KeyValuePair<string, T>>
    {
        #region CONSTS

        public static readonly int SIZE = 2;
        
        #endregion
        #region .ctor

        public PrefixTree(IPile pile)
        {
            m_Pile = pile;
            var tmp = newPrefixTreeNode();
            m_Root = m_Pile.Put(tmp);
        }

        protected override void Destructor()
        {
            var test = m_Pile as DefaultPile;
            if (test != null && test.Status == ControlStatus.Active) clearAll();
            base.Destructor();
        }

        #endregion

        #region Fields

        private IPile m_Pile;
        private PilePointer m_Root;
        private int m_Count;

        #endregion

        #region Properties
        
        public T this[string key] 
        {
            get
            {
                return find(key);
            }
            set { setValue(key, value); }
        }
        
        public IEnumerable<string> Keys { get { return makeKeys(); }}
        
        public int Count {get { return GetCount(); }}

        #endregion

        #region Public

        public bool Remove(string key)
        {
            bool result = false;
            PilePointer currentPP = findPP(key);
            if (currentPP != PilePointer.Invalid)
            {
                PrefixTreeNode current = (PrefixTreeNode) m_Pile.Get(currentPP);
                m_Pile.Delete(current.ValuePP);
                current.ValuePP = PilePointer.Invalid;
                m_Pile.Put(currentPP, current);
                m_Count--;
            }

            return result;
        }

        public KeyValuePair<string, T>[] getKeyValuePairs()
        {
            KeyValuePair<string, T>[] result = new KeyValuePair<string, T>[Count];
            var i = 0;
            getNextValuePairs(ref result, m_Root, i, "");
            return result;
        }
        
        private void getNextValuePairs(ref KeyValuePair<string, T>[] result, PilePointer pp, int i, string strKey)
        {
            if (pp == PilePointer.Invalid) return;
            var current = (PrefixTreeNode) m_Pile.Get(pp);
            if (current.ValuePP != PilePointer.Invalid) result[i++] = new KeyValuePair<string, T>(strKey, (T)m_Pile.Get(current.ValuePP));
            foreach(Entry entry in current.Children)
            {
                if (entry.ValuePP != PilePointer.Invalid)
                {
                    getNextValuePairs(ref result, entry.ValuePP, i, strKey + entry.Key);
                }
                else
                {
                    break;
                }
            }
        }

        public IEnumerator<KeyValuePair<string, T>> GetEnumerator()
        {
            throw new System.NotImplementedException();
            // return new PrefixTreeEnumerator<T>(this);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            throw new System.NotImplementedException();
            // return GetEnumerator();
        }
        #endregion

        #region Protected

        #endregion

        #region .pvt

        private PrefixTreeNode newPrefixTreeNode()
        {
            return new PrefixTreeNode()
            {
                ValuePP = PilePointer.Invalid,
                Children = Enumerable.Repeat<Entry>(new Entry() { Key = '\0', ValuePP = PilePointer.Invalid }, SIZE).ToArray<Entry>()
            };
        }

        private T find(string key)
        {
            T result = default(T);
            PilePointer currentPP = findPP(key);
            if (currentPP != PilePointer.Invalid)
            {
                PrefixTreeNode current = (PrefixTreeNode)m_Pile.Get(currentPP);
                result = current.ValuePP != PilePointer.Invalid ? (T) m_Pile.Get(current.ValuePP) : default(T); 
            }
            return result;
        }

        private PilePointer findPP(string Key)
        {
            char[] keys = Key.ToCharArray();
            string strKey = "";
            PilePointer currentPP = m_Root;
            PrefixTreeNode current = (PrefixTreeNode)m_Pile.Get(currentPP);
            bool contains = false;
            foreach (char charKey in keys)
            {
                strKey += charKey;
                int i = 0;
                contains = false;
                for (i = 0; i < current.Children.Length; i++)
                {
                    if (current.Children[i].Key > charKey) break;
                    if (current.Children[i].Key == charKey)
                    {
                        contains = true;
                        break;
                    }
                }
                if (i >= current.Children.Length) break;
                currentPP = current.Children[i].ValuePP;
                current = (PrefixTreeNode)m_Pile.Get(currentPP);
            }
            return contains ? currentPP : PilePointer.Invalid;
        }

        private void setValue(string key, T value)
        {
            char[] keys = key.ToCharArray();
            string strKey = "";
            PilePointer currentPP = m_Root;
            PrefixTreeNode current = (PrefixTreeNode)m_Pile.Get(currentPP);
            bool contains = false;
            foreach(char charKey in keys)
            {
                strKey += charKey;
                int i = 0;
                contains = false;
                for(i = 0; i < current.Children.Length; i++)
                {
                    if (current.Children[i].Key > charKey || current.Children[i].ValuePP == PilePointer.Invalid) break;
                    if (current.Children[i].Key == charKey)
                    {
                        contains = true;
                        break;
                    }
                }
                if(!contains)
                {
                    var tmp = newPrefixTreeNode();
                    if (i < current.Children.Length)
                    {
                        if(current.Children[i].ValuePP == PilePointer.Invalid)
                        {
                            current.Children[i].Key = charKey;
                            current.Children[i].ValuePP = m_Pile.Put(tmp);
                            m_Pile.Put(currentPP, current);
                        }
                        else
                        {
                            Entry entry = new Entry() { Key = charKey, ValuePP = m_Pile.Put(tmp) };
                            current.Children = insertEntry(i, current.Children, entry);
                            m_Pile.Put(currentPP, current);
                        }
                    } 
                    else
                    {
                        Array.Resize(ref current.Children, i + SIZE);
                        for(var j = i; j < current.Children.Length; j++)
                        {
                            current.Children[j].Key = '\0';
                            current.Children[j].ValuePP = PilePointer.Invalid;
                        }
                        current.Children[i].Key = charKey;
                        current.Children[i].ValuePP = m_Pile.Put(tmp);
                        m_Pile.Put(currentPP, current);
                    }
                }
                currentPP = current.Children[i].ValuePP;
                current = (PrefixTreeNode) m_Pile.Get(currentPP);
            }
            if (current.ValuePP != PilePointer.Invalid)
            {
                m_Pile.Put(current.ValuePP, value);
            }
            else
            {
                current.ValuePP = m_Pile.Put(value);
                m_Pile.Put(currentPP, current);
                m_Count++;
            }
        }

        private Entry[] insertEntry(int i, Entry[] Children, Entry entry)
        {
            Entry[] newArray = new Entry[Children.Length + 1];
            Array.Copy(Children, 0, newArray, 0, i);
            newArray[i] = entry;
            Array.Copy(Children, i, newArray, i+1, Children.Length - i);
            return newArray;
        }

        private void clearAll()
        {
            clearItem(m_Root);
            m_Root = PilePointer.Invalid;
        }

        private void clearItem(PilePointer pp)
        {
            if (pp == PilePointer.Invalid) return;
            PrefixTreeNode current = (PrefixTreeNode)m_Pile.Get(pp);
            foreach(Entry entry in current.Children)
            {
                if (entry.ValuePP != PilePointer.Invalid)
                {
                    clearItem(entry.ValuePP);
                }
            }
            if (current.ValuePP != PilePointer.Invalid) m_Pile.Delete(current.ValuePP);
            m_Pile.Delete(pp);
        }

        private IEnumerable<string> makeKeys()
        {
            return new Keys(m_Root, m_Pile);
        }

        private int GetCount()
        {
            return m_Count;
        }

        #endregion




    }

    internal class PrefixTreeEnumerator<T> : IEnumerator<T>
    {
        
        public PrefixTreeEnumerator(PrefixTree<T> prefixTree)
        {
            m_PrefixTree = prefixTree;
            first = true;
        }

        private PrefixTree<T> m_PrefixTree;
        private bool first;

        public void Dispose()
        {
        }

        public bool MoveNext()
        {
            throw new System.NotImplementedException();
        }

        public void Reset()
        {
            throw new System.NotImplementedException();
        }

        public T Current
        {
            get
            {
                throw new System.NotImplementedException();
            }
        }

        object IEnumerator.Current
        {
            get { return Current; }
        }
    }
    
    internal struct Entry
    {
        internal char Key;
        internal PilePointer ValuePP;
    }
    
    internal struct PrefixTreeNode
    {
        internal PilePointer ValuePP;
        internal Entry[] Children;

    }
    
    internal class Keys : IEnumerable<string>
    {
        public Keys(PilePointer root, IPile pile)
        {
            m_Root = root;
            m_Pile = pile;
        }

        private PilePointer m_Root;
        private IPile m_Pile;

        public IEnumerator<string> GetEnumerator()
        {
            return new Keys.KeysEnumenator(m_Root, m_Pile);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        internal class KeysEnumenator : IEnumerator<string>
        {

            public KeysEnumenator(PilePointer root, IPile pile)
            {
                m_Root = root;
                m_Pile = pile;
                Reset();
            }
            
            private PilePointer m_Root;
            private IPile m_Pile;
            private List<KeyValuePair<string, PilePointer>> stack;
            private int i;
            private PrefixTreeNode currentNode;
            private string strKey;

            private string currentKey;

            public void Dispose()
            {
                // throw new NotImplementedException();
            }

            public bool MoveNext()
            {
                currentKey = "";
                while (stack.Any())
                {
                    if(i < 0) {
                        var currentPP = stack[0].Value;
                        strKey = stack[0].Key;
                        stack.RemoveAt(0);
                        currentNode = (PrefixTreeNode) m_Pile.Get(currentPP);
                        for (int j = 0; j < currentNode.Children.Length; j++)
                        {
                            if(currentNode.Children[j].ValuePP != PilePointer.Invalid) 
                                stack.Add(new KeyValuePair<string, PilePointer>(strKey + currentNode.Children[j].Key, (currentNode.Children[j].ValuePP)));
                        }
                        i = 0;
                    }
                    var find = false;
                    while (i < currentNode.Children.Length)
                    {
                        Entry entry = currentNode.Children[i];
                        i++;
                        if (entry.ValuePP != PilePointer.Invalid)
                        {
                            PrefixTreeNode node = (PrefixTreeNode) m_Pile.Get(entry.ValuePP);
                            if (node.ValuePP != PilePointer.Invalid)
                            {
                                currentKey = strKey + entry.Key;
                                find = true;
                                break;
                            }
                        }
                    }
                    if (find) break;
                    i = -1;
                }
                
                return !currentKey.IsNullOrEmpty();
            }

            public void Reset()
            {
                stack = new List<KeyValuePair<string, PilePointer>>() {new KeyValuePair<string, PilePointer>("", m_Root)};
                i = -1;
            }

            public string Current
            {
                get
                {
                    return currentKey; 
                    
                }
            }

            object IEnumerator.Current
            {
                get { return Current; }
            }
        }
    }
}