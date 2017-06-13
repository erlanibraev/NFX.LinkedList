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
            /*
            bool result = false;
            char[] keys = key.ToCharArray();
            PrefixTreeNode current = (PrefixTreeNode) m_Pile.Get(m_Root);
            string strKey = "";
            foreach (var charKey in keys)
            {
                strKey += charKey;
                var node = current.Children != PilePointer.Invalid ? new LinkedListNode<PrefixTreeNode>(m_Pile, current.Children, null) : null;
                bool contains = false;
                while (node != null)
                {
                    if (m_Comparer.Equals(node.Value.Key, strKey))
                    {
                        contains = true;
                        break;
                    }
                    node = node.Next;
                }
                if (contains)
                {
                    current = node.Value;
                    if (m_Comparer.Equals(current.Key, key) )
                    {
                        if (current.Value != PilePointer.Invalid)
                        {
                            m_Pile.Delete(current.Value);
                            current.Value = PilePointer.Invalid;
                            if (current.Children == PilePointer.Invalid)
                            {
                                m_Pile.Delete(current.Self);
                                var prev = node.Previous;
                                var next = node.Next;
                                if (prev != null)
                                {
                                    prev.Node.NextPP = next != null ? next.Node.SelfPP : PilePointer.Invalid;
                                    m_Pile.Put(prev.Node.SelfPP, prev.Node);
                                }

                                if (next != null)
                                {
                                    next.Node.PreviousPP = prev != null ? prev.Node.SelfPP : PilePointer.Invalid;
                                    m_Pile.Put(next.Node.SelfPP, next.Node);
                                }
                                m_Pile.Delete(node.Node.SelfPP);
                            }
                            else
                            {
                                m_Pile.Put(current.Self, current);
                            }
                            result = true;
                        }
                        break;
                    }
                }
                else
                {
                    break;
                }
            }
            return result;
            */
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
            return; // todo Сделать!!!!
            var test = m_Pile as DefaultPile;
            if (test != null && test.Status == ControlStatus.Active) clearAll();
            base.Destructor();
        }
        #endregion

        #region .pvt

        private T find(string key)
        {
            char[] keys = key.ToCharArray();
            string strKey = "";
            PrefixTreeNode current = (PrefixTreeNode) m_Pile.Get(m_Root);
            bool contains = false;
            foreach(char charKey in keys)
            {
                strKey += charKey;
                int i = 0;
                contains = false;
                for(i = 0; i < current.Children.Length; i++)
                {
                    if (current.Children[i].Key > charKey) break;
                    if (current.Children[i].Key == charKey)
                    {
                        contains = true;
                        break;
                    }
                }
                if (!contains || i >= current.Children.Length)
                {
                    break;
                }
                current = (PrefixTreeNode) m_Pile.Get(current.Children[i].ValuePP);
            }
            return contains && current.ValuePP != PilePointer.Invalid ? (T) m_Pile.Get(current.ValuePP) : default(T);
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

        private PrefixTreeNode newPrefixTreeNode()
        {
            PrefixTreeNode result = new PrefixTreeNode()
            {
                ValuePP = PilePointer.Invalid,
                Children = Enumerable.Repeat(new Entry() { Key='\0', ValuePP = PilePointer.Invalid}, SIZE).ToArray()
            };
            return result;
        }

        private void clearAll()
        {
            // todo Алгоритм рекурсивный. Надо подумать, как его сделать не рекурсивным...
            throw new System.NotImplementedException();
            clearItem(m_Root);        
        }

        private void clearItem(PilePointer itemPP)
        {
            throw new System.NotImplementedException();
            /*
            if (itemPP == PilePointer.Invalid) return;
            PrefixTreeNode item = (PrefixTreeNode) m_Pile.Get(itemPP);
            NodeData node = (NodeData) (item.Children != PilePointer.Invalid ? m_Pile.Get(item.Children) : null);
            while(node != null)
            {
                clearItem(node.ValuePP);
                m_Pile.Delete(node.SelfPP);
                node = node.NextPP != PilePointer.Invalid ? (NodeData) m_Pile.Get(node.NextPP) : null;
            }
            if (item.Value != PilePointer.Invalid) m_Pile.Delete(item.Value);
            m_Pile.Delete(item.Self);
            */
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