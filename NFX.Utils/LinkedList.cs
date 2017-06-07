using System;
using System.Collections;
using System.Collections.Generic;
using NFX.ApplicationModel.Pile;

namespace NFX.Utils
{
    public class LinkedList<T> : IEnumerable<T>
    {

        public LinkedList(IPile pile)
        {
            m_Pile = pile;
        }

        public LinkedList(IPile pile, T value)
        {
            m_Pile = pile;
            var item = new LinkedListNode<T>(m_Pile, value);
            item.List = this;
            m_First = item;
            m_Last = item;
        }

        private readonly IPile m_Pile;
        private LinkedListNode<T> m_First;
        private LinkedListNode<T> m_Last;

        public LinkedListNode<T> First { get { return m_First; }}

        public LinkedListNode<T> Last { get { return m_Last; }}


        public LinkedListNode<T> AddFirst(T value)
        {
            var result = new LinkedListNode<T>(m_Pile, value);
            result.List = this;
            if (First != null)
            {
                AddBefore(First, result);
            }
            else
            {
                addFirstLastOnly(result);
            }
            return result;
        }

        public LinkedListNode<T> AddLast(T value)
        {
            var result = new LinkedListNode<T>(m_Pile, value);
            result.List = this;
            if (Last != null)
            {
                AddAfter(Last, result);
            }
            else
            {
                addFirstLastOnly(result);
            }
            
            return result;
        }

        public LinkedListNode<T> AddBefore(LinkedListNode<T> node, T value)
        {
            var result = new LinkedListNode<T>(m_Pile, value);
            result.List = this;
            AddBefore(node, result);
            return result;
        }
        
        public void AddBefore(LinkedListNode<T> node, LinkedListNode<T> newNode)
        {
            if(newNode == null || node == null ) throw new ArgumentNullException();
            if (newNode.Next != null || newNode.Previous != null)
                throw new InvalidOperationException();
            if(node.List != this || newNode.List != this) throw new InvalidOperationException();
            
            var tmp_prev = node.Node.PreviousPP;
            node.Node.PreviousPP = newNode.Node.SelfPP;
            newNode.Node.NextPP = node.Node.SelfPP;
            newNode.Node.PreviousPP = tmp_prev;
            if (First.Node.SelfPP.Equals(node.Node.SelfPP))
            {
                m_First = newNode;
            }
            m_Pile.Put(node.Node.SelfPP, node.Node);
            m_Pile.Put(newNode.Node.SelfPP, newNode.Node);
        }

        public LinkedListNode<T> AddAfter(LinkedListNode<T> node, T value)
        {
            var result = new LinkedListNode<T>(m_Pile, value);
            result.List = this;
            AddAfter(node, result);
            return result;
        }

        public void AddAfter(LinkedListNode<T> node, LinkedListNode<T> newNode)
        {
            if(newNode == null || node == null ) throw new ArgumentNullException();
            if (newNode.Node.NextPP != PilePointer.Invalid || newNode.Node.PreviousPP != PilePointer.Invalid)
                throw new InvalidOperationException();
            if(node.List != this || newNode.List != this) throw new InvalidOperationException();
            
            var tmpPrev = node.Node.NextPP;
            node.Node.NextPP = newNode.Node.SelfPP;
            newNode.Node.PreviousPP = node.Node.SelfPP;
            newNode.Node.NextPP = tmpPrev;
            if (Last.Node.SelfPP.Equals(node.Node.SelfPP))
            {
                m_Last = newNode;
            }
            m_Pile.Put(node.Node.SelfPP, node.Node);
            m_Pile.Put(newNode.Node.SelfPP, newNode.Node);
        }

        public void Remove(LinkedListNode<T> node)
        {
            var current = First;
            while (current != null)
            {
                if (current.Equals(node))
                {
                    NodeData prevous = current.Node.PreviousPP != PilePointer.Invalid ? m_Pile.Get(current.Node.PreviousPP) as NodeData : null;
                    NodeData next = current.Node.NextPP != PilePointer.Invalid ? m_Pile.Get(current.Node.NextPP) as NodeData : null;
                    NodeData self = current.Node;

                    if (prevous == null && next == null)
                    {
                        if (current.Equals(First) && current.Equals(Last))
                        {
                            m_Pile.Delete(self.SelfPP);
                            m_First = null;
                            m_Last = null;
                        }
                        else
                        {
                            throw new ArgumentOutOfRangeException();
                        }
                    }
                    else if (prevous == null && next != null)
                    {
                        if (current.Equals(First))
                        {
                            m_Pile.Delete(self.SelfPP);
                            next.PreviousPP = PilePointer.Invalid;
                            m_Pile.Put(next.SelfPP, next);
                            m_First = new LinkedListNode<T>(m_Pile, next.SelfPP) {List = this};
                        }
                        else
                        {
                            throw new ArgumentOutOfRangeException();
                        }
                    } 
                    else if (prevous != null && next == null)
                    {
                        if(current.Equals(Last)) {
                            m_Pile.Delete(self.SelfPP);
                            prevous.NextPP = PilePointer.Invalid;
                            m_Pile.Put(prevous.SelfPP, prevous);
                            m_Last = new LinkedListNode<T>(m_Pile, prevous.SelfPP) {List = this};
                        }
                        else
                        {
                            throw new ArgumentOutOfRangeException();
                        }
                    }
                    else
                    {
                        prevous.NextPP = self.NextPP;
                        next.PreviousPP = self.PreviousPP;
                        m_Pile.Delete(self.SelfPP);
                        m_Pile.Put(next.SelfPP, next);
                        m_Pile.Put(prevous.SelfPP, prevous);
                    }
                    
                    break;
                }
                current = current.Next;
            }    
        }

        public IEnumerator<T> GetEnumerator()
        {
            var result = new LinkedListEnumerator<T>(this);
            return result;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
        
        private void addFirstLastOnly(LinkedListNode<T> node)
        {
            m_First = node;
            m_Last = node;
        }
    }

    internal class  LinkedListEnumerator<T> : IEnumerator<T>
    {

        private readonly LinkedList<T> List;
        private LinkedListNode<T> CurrentNode;
        private bool First = true;

        public LinkedListEnumerator(LinkedList<T> list)
        {
            List = list;
            CurrentNode = list.First;
        }

        public void Dispose()
        {
            
            // throw new System.NotImplementedException();
        }

        public bool MoveNext()
        {
            if (!First)
            {
                CurrentNode = CurrentNode != null ? CurrentNode.Next : null;
            }
            else
            {
                First = false;
            }
           
            return CurrentNode != null;
        }

        public void Reset()
        {
            CurrentNode = List.First;
            First = true;
        }

        object IEnumerator.Current => Current;

        public T Current => CurrentNode != null ? CurrentNode.Value : default(T);
    }
}