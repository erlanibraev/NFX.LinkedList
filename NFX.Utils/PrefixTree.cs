using System;
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
            m_Root = newPrefixTreeNode(PilePointer.Invalid, "").Self;
            m_Keys = null;
        }
        
        public PrefixTree(IPile pile, IEqualityComparer<string> comparer)
        {
            m_Pile = pile;
            m_Comparer = comparer;
            m_Root = newPrefixTreeNode(PilePointer.Invalid, "").Self;
            m_Keys = null;
        }

        #endregion

        #region Fields

        private IPile m_Pile;
        private IEqualityComparer<string> m_Comparer;
        private PilePointer m_Root;
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
            PrefixTreeNode current = (PrefixTreeNode) m_Pile.Get(m_Root);
            PilePointer ppResult = PilePointer.Invalid;
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
            PrefixTreeNode current = (PrefixTreeNode) m_Pile.Get(m_Root);
            string strKey = "";
            foreach(char charKey in keys)
            {
                strKey += charKey;
                var node = current.Children != PilePointer.Invalid ? new LinkedListNode<PrefixTreeNode>(m_Pile, current.Children, null) : null;
                bool contains = false;
                LinkedListNode<PrefixTreeNode> prev = null;
                while (node != null)
                {
                    if (m_Comparer.Equals(node.Value.Key, strKey))
                    {
                        contains = true;
                        break;
                    }
                    prev = node;
                    node = node.Next;
                }
                
                if (!contains)
                {
                    PrefixTreeNode child = newPrefixTreeNode(current.Self, strKey);
                    node = new LinkedListNode<PrefixTreeNode>(m_Pile);
                    node.Node.ValuePP = child.Self;
                    if (prev != null)
                    {
                        node.Node.PreviousPP = prev.Node.SelfPP;
                        prev.Node.NextPP = node.Node.SelfPP;
                        m_Pile.Put(prev.Node.SelfPP, prev.Node);
                    }
                    else
                    {
                        current.Children = node.Node.SelfPP;
                    }
                    
                    m_Pile.Put(node.Node.SelfPP, node.Node);
                    m_Pile.Put(current.Self, current);
                }
                current = node.Value;
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
                Children = PilePointer.Invalid,
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
            clearItem(m_Root);        
        }

        private void clearItem(PilePointer itemPP)
        {
            if(itemPP == PilePointer.Invalid) return;
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
        }
        
        private LinkedList<string> makeKeys()
        {
            if(m_Keys != null) m_Keys.Clear();
            LinkedList<string> result = new LinkedList<string>(m_Pile);
            var stack = new List<PrefixTreeNode>();
            stack.Add((PrefixTreeNode) m_Pile.Get(m_Root));
            while (stack.Count > 0)
            {
                var current = stack[0];
                stack.RemoveAt(0);
                var node = current.Children != PilePointer.Invalid ? new LinkedListNode<PrefixTreeNode>(m_Pile, current.Children, null) : null;
                while(node != null)
                {
                    PrefixTreeNode child = (PrefixTreeNode) m_Pile.Get(node.Value.Self);
                    stack.Add(child);
                    if(child.Value != PilePointer.Invalid) result.AddLast(node.Value.Key);
                    node = node.Next;
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
            internal PilePointer Children;

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