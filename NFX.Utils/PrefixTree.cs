using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NFX.ApplicationModel.Pile;
using NFX.ServiceModel;

namespace NFX.Utils
{
    public class PrefixTree<T>: DisposableObject, IEnumerable<T>
    {
        #region .ctor

        public PrefixTree(IPile pile)
        {
            m_Pile = pile;
            m_Comparer = EqualityComparer<string>.Default;
            m_Root = newPrefixTreeNode(PilePointer.Invalid, "");
            m_Keys = null;
        }
        
        public PrefixTree(IPile pile, IEqualityComparer<string> comparer)
        {
            m_Pile = pile;
            m_Comparer = comparer;
            m_Root = newPrefixTreeNode(PilePointer.Invalid, "");
            m_Keys = null;
        }

        #endregion

        #region Fields

        private IPile m_Pile;
        private IEqualityComparer<string> m_Comparer;
        private PrefixTreeNode m_Root;
        private LinkedList<string> m_Keys;

        #endregion

        #region Properties
        
        public T this[string key] {
            get
            {
                return find(key);
            }
            set { setValue(key, value); }
        }
        
        public LinkedList<string> Keys { get { return makeKeys(); }}

        #endregion

        #region Public

        public bool Remove(string key)
        {
            bool result = false;
            char[] keys = key.ToCharArray();
            PrefixTreeNode current = m_Root;
            string strKey = "";
            foreach (var charKey in keys)
            {
                strKey += charKey;
                if (current.Children.Keys.Contains(strKey, m_Comparer))
                {
                    var prev = current;
                    current = (PrefixTreeNode) m_Pile.Get(current.Children[strKey]);
                    if (m_Comparer.Equals(current.Key, key) )
                    {
                        if (current.Value != PilePointer.Invalid)
                        {
                            m_Pile.Delete(current.Value);
                            current.Value = PilePointer.Invalid;
                            if (current.Children.Count == 0)
                            {
                                m_Pile.Delete(current.Self);
                                prev.Children.Remove(key);
                                m_Pile.Put(prev.Self, prev);
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
        }

        public IEnumerator<T> GetEnumerator()
        {
            return new PrefixTreeEnumerator<T>(this);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
        #endregion

        #region Protected

        #endregion
        
        #region .pvt

        private T find(string key)
        {
            char[] keys = key.ToCharArray();
            PrefixTreeNode current = m_Root;
            PilePointer ppResult = PilePointer.Invalid;
            string strKey = "";
            foreach (var charKey in keys)
            {
                strKey += charKey;
                if (current.Children.Keys.Contains(strKey, m_Comparer))
                {
                    current = (PrefixTreeNode) m_Pile.Get(current.Children[strKey]);
                    if (current.Key == key)
                    {
                        if (current.Value != PilePointer.Invalid)
                        {
                            ppResult = current.Value;    
                        }
                        break;
                    }
                }
                else
                {
                    break;
                }
                
            }
            return ppResult != PilePointer.Invalid ? (T) m_Pile.Get(ppResult) : default(T);
        }

        private void setValue(string key, T value)
        {
            char[] keys = key.ToCharArray();
            PrefixTreeNode current = m_Root;
            string strKey = "";
            foreach(char charKey in keys)
            {
                strKey += charKey;
                if (!current.Children.Keys.Contains(strKey, m_Comparer))
                {
                    PrefixTreeNode child = newPrefixTreeNode(current.Self, strKey);
                    current.Children[strKey] = child.Self;
                    m_Pile.Put(current.Self, current);
                }
                current = (PrefixTreeNode) m_Pile.Get(current.Children[strKey]);
            }
            if (current.Value != PilePointer.Invalid)
            {
                m_Pile.Put(current.Value, value);
            }
            else
            {
                current.Value = m_Pile.Put(value);
            }
            m_Pile.Put(current.Self, current);
        }

        private PrefixTreeNode newPrefixTreeNode(PilePointer parent, string key)
        {
            PrefixTreeNode result = new PrefixTreeNode()
            {
                Self = PilePointer.Invalid,
                Value = PilePointer.Invalid,
                Parent = parent,
                Children = new Dictionary<string, PilePointer>(),
                Key = key
            };
            result.Self = m_Pile.Put(result);
            m_Pile.Put(result.Self, result);
            return result;
        }

        protected override void Destructor()
        {
            var test = m_Pile as DefaultPile;
            if(test != null && test.Status == ControlStatus.Active) clearAll();
            base.Destructor();
        }

        private void clearAll()
        {
            // todo Алгоритм рекурсивный. Надо подумать, как его сделать не рекурсивным...
            if(m_Keys != null) m_Keys.Clear();
            clearItem(m_Root.Self);        
        }

        private void clearItem(PilePointer itemPP)
        {
            PrefixTreeNode item = (PrefixTreeNode) m_Pile.Get(itemPP);
            foreach (var childPP in item.Children.Values)
            {
                clearItem(childPP);
            }
            if (item.Value != PilePointer.Invalid) m_Pile.Delete(item.Value);
            m_Pile.Delete(item.Self);
        }
        
        private LinkedList<string> makeKeys()
        {
            if(m_Keys != null) m_Keys.Clear();
            LinkedList<string> result = new LinkedList<string>(m_Pile);
            var stack = new List<PrefixTreeNode>();
            stack.Add(m_Root);
            while (stack.Count > 0)
            {
                var current = stack[0];
                stack.RemoveAt(0);
                foreach (var childKey in current.Children.Keys)
                {
                    PrefixTreeNode child = (PrefixTreeNode) m_Pile.Get(current.Children[childKey]);
                    stack.Add(child);
                    if(child.Value != PilePointer.Invalid) result.AddLast(childKey);
                }                
            }
            m_Keys = result;
            return result;
        }

        #endregion

        internal struct PrefixTreeNode
        {
            internal PilePointer Self;
            internal PilePointer Value;
            internal string Key;
            internal PilePointer Parent;
            internal Dictionary<string, PilePointer> Children;

            public override string ToString()
            {
                return "self = {0}; value = {1}; key = {2}; parent={3}; Children = {4}".Args(Self.ToString(), Value.ToString(), Key, Parent.ToString(), Children != null ? Children.ToString() : "");
            }
        }
    }

    internal class PrefixTreeEnumerator<T> : IEnumerator<T>
    {
        
        public PrefixTreeEnumerator(PrefixTree<T> prefixTree)
        {
            m_PrefixTree = prefixTree;
            m_Keys = prefixTree.Keys;
            m_Current = m_Keys.First;
            first = true;
        }

        private PrefixTree<T> m_PrefixTree;
        private LinkedList<string> m_Keys;
        private LinkedListNode<string> m_Current;
        private bool first;

        public void Dispose()
        {
            m_Keys.Clear();
            m_Keys = null;
        }

        public bool MoveNext()
        {
            if (!first)
            {
                m_Current = m_Current.Next;
            }
            else
            {
                first = false;
            }
            return m_Current != null;
        }

        public void Reset()
        {
            m_Current = m_Keys.First;
            first = true;
        }

        public T Current
        {
            get
            {
                return m_Current != null ? m_PrefixTree[m_Current.Value] : default(T);
            }
        }

        object IEnumerator.Current
        {
            get { return Current; }
        }
    }
}