using System.Collections;
using System.Collections.Generic;
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
            m_Comparer = Comparer<string>.Default;
            m_Root = newPrefixTreeNode(PilePointer.Invalid, default(char));
        }
        
        public PrefixTree(IPile pile, Comparer<string> comparer)
        {
            m_Pile = pile;
            m_Comparer = comparer;
            m_Root = newPrefixTreeNode(PilePointer.Invalid, default(char));
        }
        
        
        #endregion

        #region Fields

        private IPile m_Pile;
        private Comparer<string> m_Comparer;
        private PrefixTreeNode m_Root;

        #endregion

        #region Properties
        
        public T this[string key] {
            get
            {
                return find(key);
            }
            set { setValue(key, value); }
        }
        

        #endregion

        #region Public
        
        public IEnumerator<T> GetEnumerator()
        {
            throw new System.NotImplementedException();
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
            foreach (var charKey in keys)
            {
                if (current.Children.ContainsKey(charKey))
                {
                    current = (PrefixTreeNode) m_Pile.Get(current.Children[charKey]);
                    if (current.Value != PilePointer.Invalid)
                    {
                        ppResult = current.Value;    
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
            foreach(char charKey in keys)
            {
                if (!current.Children.ContainsKey(charKey))
                {
                    PrefixTreeNode child = newPrefixTreeNode(current.Self, charKey);
                    current.Children[charKey] = child.Self;
                    m_Pile.Put(current.Self, current);
                }
                current = (PrefixTreeNode) m_Pile.Get(current.Children[charKey]);
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

        private PrefixTreeNode newPrefixTreeNode(PilePointer parent, char key)
        {
            PrefixTreeNode result = new PrefixTreeNode()
            {
                Self = PilePointer.Invalid,
                Value = PilePointer.Invalid,
                Parent = parent,
                Children = new Dictionary<char, PilePointer>(),
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

        #endregion

        internal struct PrefixTreeNode
        {
            internal PilePointer Self;
            internal PilePointer Value;
            internal char Key;
            internal PilePointer Parent;
            internal Dictionary<char, PilePointer> Children;

            public override string ToString()
            {
                return "self = {0}; value = {1}; key = {2}; parent={3}; Children = {4}".Args(Self.ToString(), Value.ToString(), Key, Parent.ToString(), Children != null ? Children.ToString() : "");
            }
        }
    }
}