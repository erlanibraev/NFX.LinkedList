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

        public static readonly int SIZE = 20;
        
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
        
        public IEnumerable<string> Keys { get { return getKeys(); }}
        
        public int Count {get { return GetCount(); }}

        #endregion

        #region Public

        public bool Remove(string key)
        {
            bool result = false;
            PilePointer currentPP = findPP(key);
            if (currentPP != default(PilePointer))
            {
                PrefixTreeNode current = (PrefixTreeNode) m_Pile.Get(currentPP);
                m_Pile.Delete(current.ValuePP);
                current.ValuePP = default(PilePointer);
                m_Pile.Put(currentPP, current);
                m_Count--;
            }

            return result;
        }

        public IEnumerable<KeyValuePair<string, T>> getKeyValuePairs()
        {
            return getNextValuePairs( m_Root, "");;
        }
        
        private IEnumerable<KeyValuePair<string, T>> getNextValuePairs(PilePointer pp, string strKey)
        {
            if (pp == default(PilePointer)) yield break;
            var current = (PrefixTreeNode) m_Pile.Get(pp);
            if (current.ValuePP != default(PilePointer)) yield return new KeyValuePair<string, T>(strKey, (T)m_Pile.Get(current.ValuePP));
            foreach(Entry entry in current.Children)
            {
                if (entry.ValuePP != default(PilePointer))
                {
                    foreach (var nextValuePair in getNextValuePairs(entry.ValuePP, strKey + entry.Key))
                    {
                        yield return nextValuePair;
                    }
                }
                else
                {
                    yield break;
                }
            }
        }

        public void Put(string key, T value)
        {
            putValue(m_Root, key, value, "", 0);
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
                ValuePP = default(PilePointer),
                Children = Enumerable.Repeat<Entry>(new Entry(), SIZE).ToArray<Entry>()
            };
        }

        private T find(string key)
        {
            T result = default(T);
            PilePointer currentPP = findPP(key);
            if (currentPP != default(PilePointer))
            {
                PrefixTreeNode current = (PrefixTreeNode)m_Pile.Get(currentPP);
                result = current.ValuePP != default(PilePointer) ? (T) m_Pile.Get(current.ValuePP) : default(T); 
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
            return contains ? currentPP : default(PilePointer);
        }

        private void putValue(PilePointer currentPP, string key, T value, string strKey, int i)
        {
            if (i > key.Length) throw new IndexOutOfRangeException();
            if (currentPP == default(PilePointer)) throw new IndexOutOfRangeException();
            var current = (PrefixTreeNode)m_Pile.Get(currentPP);
            if (key == strKey)
            {
                if (current.ValuePP != default(PilePointer))
                {
                    m_Pile.Put(current.ValuePP, value);
                }
                else
                {
                    current.ValuePP = m_Pile.Put(value);
                    m_Pile.Put(currentPP, current);
                    m_Count++;
                }
                return;
            }
            for (int j = 0; j < current.Children.Length; j++)
            {
                if (current.Children[j].ValuePP == default(PilePointer))
                {
                    var tmp = newPrefixTreeNode();
                    Entry entry = new Entry() { Key = key[i], ValuePP = m_Pile.Put(tmp) };
                    insertEntry(j, ref current.Children, entry);
                    m_Pile.Put(currentPP, current);
                }
                else if (current.Children[j].Key > key[i])
                {
                    var tmp = newPrefixTreeNode();
                    Entry entry = new Entry() { Key = key[i], ValuePP = m_Pile.Put(tmp) };
                    insertEntry(j, ref current.Children, entry);
                    m_Pile.Put(currentPP, current);
                }
                if (current.Children[j].Key == key[i])
                {
                    putValue(current.Children[j].ValuePP, key, value, strKey + current.Children[j].Key, i + 1);
                    break;
                }
            }
        }

        private void setValue(string key, T value)
        {
            char[] keys = key.ToCharArray();
            string strKey = "";
            PilePointer currentPP = m_Root;
            PrefixTreeNode current = (PrefixTreeNode)m_Pile.Get(currentPP);
            foreach(char charKey in keys)
            {
                strKey += charKey;
                int i = 0;
                for(i = 0; i < current.Children.Length; i++)
                {
                    if (current.Children[i].ValuePP == default(PilePointer))
                    {
                        var tmp = newPrefixTreeNode();
                        Entry entry = new Entry() { Key = charKey, ValuePP = m_Pile.Put(tmp) };
                        insertEntry(i, ref current.Children, entry);
                        m_Pile.Put(currentPP, current);
                    }
                    else if (current.Children[i].Key > charKey)
                    {
                        var tmp = newPrefixTreeNode();
                        Entry entry = new Entry() { Key = charKey, ValuePP = m_Pile.Put(tmp) };
                        insertEntry(i, ref current.Children, entry);
                        m_Pile.Put(currentPP, current);
                    }
                    if (current.Children[i].Key == charKey)
                    {
                        break;
                    }
                }
                currentPP = current.Children[i].ValuePP;
                current = (PrefixTreeNode) m_Pile.Get(currentPP);
            }
            if (current.ValuePP != default(PilePointer))
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

        private void insertEntry(int i, ref Entry[] Children, Entry entry)
        {
            int start = Children.Length - 1;
            if (Children[start].ValuePP != default(PilePointer))
            {
                Array.Resize(ref Children, Children.Length + SIZE);
            }
            for(int j = start; i < j; j--)
            {
                Children[j] = Children[j - 1];
            }
            Children[i] = entry;
        }

        private void clearAll()
        {
            clearItem(m_Root);
            m_Root = default(PilePointer);
        }

        private void clearItem(PilePointer pp)
        {
            if (pp == default(PilePointer)) return;
            PrefixTreeNode current = (PrefixTreeNode)m_Pile.Get(pp);
            foreach(Entry entry in current.Children)
            {
                if (entry.ValuePP != default(PilePointer))
                {
                    clearItem(entry.ValuePP);
                }
            }
            if (current.ValuePP != default(PilePointer)) m_Pile.Delete(current.ValuePP);
            m_Pile.Delete(pp);
        }

        private int GetCount()
        {
            return m_Count;
        }

        private string[] getKeys()
        {
            string[] result = new string[m_Count];
            var i = 0;
            getNextKey(ref result, m_Root, ref i, "");
            return result;
        }

        private void getNextKey(ref string[] result, PilePointer pp, ref int i, string strKey)
        {
            if (pp == default(PilePointer)) return;
            var current = (PrefixTreeNode)m_Pile.Get(pp);
            if (current.ValuePP != default(PilePointer)) result[i++] = strKey;
            foreach (Entry entry in current.Children)
            {
                if (entry.ValuePP != default(PilePointer))
                {
                    getNextKey(ref result, entry.ValuePP, ref i, strKey + entry.Key);
                }
                else
                {
                    break;
                }
            }
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


}