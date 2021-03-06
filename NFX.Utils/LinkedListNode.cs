using System;
using NFX.ApplicationModel.Pile;

namespace NFX.Utils
{
    public class LinkedListNode<T> :IEquatable<LinkedListNode<T>>
    {
        #region .ctor

        internal LinkedListNode(IPile pile, PilePointer self, LinkedList<T> list)
        {
            m_Pile = pile;
            Node = m_Pile.Get(self) as NodeData;
            if (Node == null) throw new AccessViolationException();
            List = list;

        }

        public LinkedListNode(IPile pile)
        {
            m_Pile = pile;
            Node = new NodeData();
            Node.SelfPP = pile.Put(Node);
            pile.Put(Node.SelfPP, Node);

        }

        public LinkedListNode(IPile pile, T value)
        {
            m_Pile = pile;
            Node = new NodeData();
            if (value != null) Node.ValuePP = m_Pile.Put(value);
            Node.SelfPP = m_Pile.Put(Node);
            m_Pile.Put(Node.SelfPP, Node);
        }

        #endregion

        #region Properties

        private readonly IPile m_Pile;
        internal NodeData Node;
        internal LinkedList<T> List;

        public T Value
        {
            get { return (T) (Node.ValuePP != PilePointer.Invalid ? m_Pile.Get(Node.ValuePP) : default(T)); }
            set
            {
                if (Node.ValuePP != PilePointer.Invalid)
                {
                    m_Pile.Delete(Node.ValuePP);    
                }
                if (value != null)
                {
                    Node.ValuePP = m_Pile.Put(value);
                    m_Pile.Put(Node.SelfPP, Node);
                }
                else
                {
                    Node.ValuePP = PilePointer.Invalid;
                }
            }
        }
        
        public LinkedListNode<T> Next => Node.NextPP != PilePointer.Invalid ? new LinkedListNode<T>(m_Pile, Node.NextPP, List) : null;

        public LinkedListNode<T> Previous => Node.PreviousPP != PilePointer.Invalid ? new LinkedListNode<T>(m_Pile, Node.PreviousPP, List) : null;

        #endregion
        
        
        public bool Equals(LinkedListNode<T> other)
        {
            return !object.ReferenceEquals(other, null) && Node.Equals(other.Node);
        }

        public override bool Equals(object obj)
        {
            var tmp = obj as LinkedList<T>;
            if (tmp == null) return false;
            return Equals(tmp);
        }

        public override int GetHashCode()
        {
            return Node.GetHashCode();
        }

        public static bool operator ==(LinkedListNode<T> a, LinkedListNode<T> b)
        {
            return (!object.ReferenceEquals(a, null)  && a.Equals(b)) || (!object.ReferenceEquals(b, null)  && b.Equals(a )) || (object.ReferenceEquals(a, null) && object.ReferenceEquals(b, null));
        }

        public static bool operator !=(LinkedListNode<T> a, LinkedListNode<T> b)
        {
            return !(a == b);
        }
    }

    internal class NodeData : IEquatable<NodeData>
    {
        public PilePointer SelfPP = PilePointer.Invalid;
        public PilePointer ValuePP = PilePointer.Invalid;
        public PilePointer NextPP = PilePointer.Invalid;
        public PilePointer PreviousPP = PilePointer.Invalid;

        public bool Equals(NodeData other)
        {
            return !object.ReferenceEquals(other, null) && SelfPP == other.SelfPP;
        }

        public override bool Equals(object obj)
        {
            var nd = obj as NodeData;
            if (nd == null) return false;
            return Equals(nd);
        }

        public override int GetHashCode()
        {
            return SelfPP.GetHashCode();
        }

        public static bool operator ==(NodeData a, NodeData b)
        {
            return (!object.ReferenceEquals(a, null)  && a.Equals(b)) || (!object.ReferenceEquals(b, null)  && b.Equals(a )) || (object.ReferenceEquals(a, null) && object.ReferenceEquals(b, null));
        }

        public static bool operator !=(NodeData a, NodeData b)
        {
            return !(a == b);
        }
    }
}