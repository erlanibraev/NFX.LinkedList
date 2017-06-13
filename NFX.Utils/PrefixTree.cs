using System.Collections;
using System.Collections.Generic;
using NFX.ApplicationModel.Pile;
using NFX.ServiceModel;
using System.Linq;
using System;

namespace NFX.Utils
{
    public class PrefixTree<T>: DisposableObject, IEnumerable<T>
    {
        public static readonly int SIZE = 10;

        #region .ctor

        public PrefixTree(IPile pile)
        {
            m_Pile = pile;
            var tmp = newPrefixTreeNode();
            m_Root = m_Pile.Put(tmp);
        }
        
        #endregion

        #region Fields

        private IPile m_Pile;
        private PilePointer m_Root;

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
        
        public ICollection<string> Keys { get { return makeKeys(); }}

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
            }

            return result;
            throw new System.NotImplementedException();
        }

        public IEnumerator<T> GetEnumerator()
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
        protected override void Destructor()
        {
            var test = m_Pile as DefaultPile;
            if (test != null && test.Status == ControlStatus.Active) clearAll();
            base.Destructor();
        }
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
                for(i = 0; i < current.Children.Length; i ++)
                {
                    if (current.Children[i].Key > charKey || current.Children[i].ValuePP == PilePointer.Invalid) break;
                    if(current.Children[i].Key == charKey)
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
                            List<Entry> tmpList = current.Children.ToList<Entry>();
                            Entry entry = new Entry() { Key = charKey, ValuePP = m_Pile.Put(tmp) };
                            tmpList.Insert(i, entry);
                            current.Children = tmpList.ToArray<Entry>();
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
            }
        }

        private void clearAll()
        {
            var stack = new List<PilePointer> {m_Root};
            while (stack.Any())
            {
                var currentPP = stack[0];
                var current = (PrefixTreeNode) m_Pile.Get(currentPP);
                foreach (var entry in current.Children)
                {
                    if(entry.ValuePP != PilePointer.Invalid) stack.Add(entry.ValuePP);
                }
                if (current.ValuePP != PilePointer.Invalid) m_Pile.Delete(current.ValuePP);
                if (currentPP != PilePointer.Invalid)
                {
                    m_Pile.Delete(currentPP);
                    stack.Remove(currentPP);
                }
            }                    
        }

        private ICollection<string> makeKeys()
        {
            throw new System.NotImplementedException();
        }

        #endregion

        internal struct PrefixTreeNode
        {
            internal PilePointer ValuePP;
            internal Entry[] Children;

        }

        internal struct Entry
        {
            internal char Key;
            internal PilePointer ValuePP;
        }
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
}