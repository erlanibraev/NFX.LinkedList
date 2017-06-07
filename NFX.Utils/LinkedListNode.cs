using NFX.ApplicationModel.Pile;

namespace NFX.Utils
{
    public class LinkedListNode<T>
    {
        private IPile m_pile { get; set; }
        protected internal NodeData Node { get; set; } = new NodeData();
        protected internal LinkedList<T> List;

        #region .ctor

        public LinkedListNode(IPile pile, PilePointer pp_self)
        {
            m_pile = pile;
            Node = m_pile.Get(pp_self) as NodeData;
        }


        public LinkedListNode(IPile pile)
        {
            m_pile = pile as IPile;
            Node.m_pp_self = pile.Put(Node);
            pile.Put(Node.m_pp_self, Node);

        }

        public LinkedListNode(IPile pile, T value)
        {
            m_pile = pile as IPile;
            if (value != null) Node.m_pp_value = m_pile.Put(value);
            Node.m_pp_self = m_pile.Put(Node);
            m_pile.Put(Node.m_pp_self, Node);
        }

        #endregion

        #region Properties

        public T Value
        {
            get => (T) (Node.m_pp_value != PilePointer.Invalid ? m_pile.Get(Node.m_pp_value) : default(T));
            set
            {
                if (Node.m_pp_value != PilePointer.Invalid)
                {
                    m_pile.Delete(Node.m_pp_value);    
                }
                if (value != null)
                {
                    Node.m_pp_value = m_pile.Put(value);
                    m_pile.Put(Node.m_pp_self, Node);
                }
                else
                {
                    Node.m_pp_value = PilePointer.Invalid;
                }
            }
            
        }

        public LinkedListNode<T> Next => Node.m_pp_next != PilePointer.Invalid ? new LinkedListNode<T>(m_pile, Node.m_pp_next) : null;

        public LinkedListNode<T> Previous => Node.m_pp_previous != PilePointer.Invalid ? new LinkedListNode<T>(m_pile, Node.m_pp_previous) : null;

        #endregion

        public override bool Equals(object obj)
        {
            bool result = false;
            if (obj?.GetType() == typeof(LinkedListNode<T>))
            {
                LinkedListNode<T> data = obj as LinkedListNode<T>;
                result = Node.Equals(data.Node);
            }
            return result;
        }

        public override int GetHashCode()
        {
            return Node.GetHashCode();
        }
    }

    public class NodeData
    {
        protected internal PilePointer m_pp_self { get; set; } = PilePointer.Invalid;
        protected internal PilePointer m_pp_value { get; set; } = PilePointer.Invalid;
        protected internal PilePointer m_pp_next { get; set; } = PilePointer.Invalid;
        protected internal PilePointer m_pp_previous { get; set; } = PilePointer.Invalid;

        public override bool Equals(object obj)
        {
            bool result = false;
            if (obj?.GetType() == typeof(NodeData))
            {
                NodeData nodeData = obj as NodeData;
                result = m_pp_self == nodeData.m_pp_self;
            }
            return result;
        }

        public override int GetHashCode()
        {
            return m_pp_self.GetHashCode() + m_pp_value.GetHashCode() + m_pp_next.GetHashCode() +
                   m_pp_previous.GetHashCode();
        }
    }
}