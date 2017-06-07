using System;
using System.Collections;
using System.Collections.Generic;
using NFX.ApplicationModel.Pile;

namespace NFX.Utils
{
    public class LinkedList<T> : IEnumerable<T>, IEnumerable
    {

        private readonly IPile m_pile;
        private LinkedListNode<T> m_first;
        private LinkedListNode<T> m_last;
        
        public LinkedList(IPile pile)
        {
            m_pile = pile;
        }

        public LinkedList(IPile pile, T value)
        {
            m_pile = pile;
            var item = new LinkedListNode<T>(m_pile, value);
            item.List = this;
            m_first = item;
            m_last = item;
        }

        public LinkedListNode<T> First => m_first;

        public LinkedListNode<T> Last => m_last;


        public LinkedListNode<T> AddFirst(T value)
        {
            var result = new LinkedListNode<T>(m_pile, value);
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
            var result = new LinkedListNode<T>(m_pile, value);
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
            var result = new LinkedListNode<T>(m_pile, value);
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
            
            var tmp_prev = node.Node.m_pp_previous;
            node.Node.m_pp_previous = newNode.Node.m_pp_self;
            newNode.Node.m_pp_next = node.Node.m_pp_self;
            newNode.Node.m_pp_previous = tmp_prev;
            if (First.Node.m_pp_self.Equals(node.Node.m_pp_self))
            {
                m_first = newNode;
            }
            m_pile.Put(node.Node.m_pp_self, node.Node);
            m_pile.Put(newNode.Node.m_pp_self, newNode.Node);
        }

        public LinkedListNode<T> AddAfter(LinkedListNode<T> node, T value)
        {
            var result = new LinkedListNode<T>(m_pile, value);
            result.List = this;
            AddAfter(node, result);
            return result;
        }

        public void AddAfter(LinkedListNode<T> node, LinkedListNode<T> newNode)
        {
            if(newNode == null || node == null ) throw new ArgumentNullException();
            if (newNode.Node.m_pp_next != PilePointer.Invalid || newNode.Node.m_pp_previous != PilePointer.Invalid)
                throw new InvalidOperationException();
            if(node.List != this || newNode.List != this) throw new InvalidOperationException();
            
            var tmp_prev = node.Node.m_pp_next;
            node.Node.m_pp_next = newNode.Node.m_pp_self;
            newNode.Node.m_pp_previous = node.Node.m_pp_self;
            newNode.Node.m_pp_next = tmp_prev;
            if (Last.Node.m_pp_self.Equals(node.Node.m_pp_self))
            {
                m_last = newNode;
            }
            m_pile.Put(node.Node.m_pp_self, node.Node);
            m_pile.Put(newNode.Node.m_pp_self, newNode.Node);
        }

        public void Remove(LinkedListNode<T> node)
        {
            var current = First;
            while (current != null)
            {
                if (current.Equals(node))
                {
                    NodeData m_prevous = current.Node.m_pp_previous != PilePointer.Invalid ? m_pile.Get(current.Node.m_pp_previous) as NodeData : null;
                    NodeData m_next = current.Node.m_pp_next != PilePointer.Invalid ? m_pile.Get(current.Node.m_pp_next) as NodeData : null;
                    NodeData m_self = current.Node;

                    if (m_prevous == null && m_next == null)
                    {
                        if (current.Equals(First) && current.Equals(Last))
                        {
                            m_pile.Delete(m_self.m_pp_self);
                            m_first = null;
                            m_last = null;
                        }
                        else
                        {
                            throw new ArgumentOutOfRangeException();
                        }
                    }
                    else if (m_prevous == null && m_next != null)
                    {
                        if (current.Equals(First))
                        {
                            m_pile.Delete(m_self.m_pp_self);
                            m_next.m_pp_previous = PilePointer.Invalid;
                            m_pile.Put(m_next.m_pp_self, m_next);
                            m_first = new LinkedListNode<T>(m_pile, m_next.m_pp_self) {List = this};
                        }
                        else
                        {
                            throw new ArgumentOutOfRangeException();
                        }
                    } 
                    else if (m_prevous != null && m_next == null)
                    {
                        if(current.Equals(Last)) {
                            m_pile.Delete(m_self.m_pp_self);
                            m_prevous.m_pp_next = PilePointer.Invalid;
                            m_pile.Put(m_prevous.m_pp_self, m_prevous);
                            m_last = new LinkedListNode<T>(m_pile, m_prevous.m_pp_self) {List = this};
                        }
                        else
                        {
                            throw new ArgumentOutOfRangeException();
                        }
                    }
                    else
                    {
                        m_prevous.m_pp_next = m_self.m_pp_next;
                        m_next.m_pp_previous = m_self.m_pp_previous;
                        m_pile.Delete(m_self.m_pp_self);
                        m_pile.Put(m_next.m_pp_self, m_next);
                        m_pile.Put(m_prevous.m_pp_self, m_prevous);
                    }
                    
                    break;
                }
                current = current.Next;
            }    
        }

        private void addFirstLastOnly(LinkedListNode<T> node)
        {
            m_first = node;
            m_last = node;
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
    }

    internal class  LinkedListEnumerator<T> : IEnumerator<T>
    {

        private readonly LinkedList<T> m_list;
        private LinkedListNode<T> m_current_node;

        public LinkedListEnumerator(LinkedList<T> list)
        {
            m_list = list;
            m_current_node = list.First;
        }

        public void Dispose()
        {
            throw new System.NotImplementedException();
        }

        public bool MoveNext()
        {
            m_current_node = m_current_node?.Next;
            return m_current_node != null;
        }

        public void Reset()
        {
            m_current_node = m_list.First;
        }

        object IEnumerator.Current => Current;

        public T Current => m_current_node != null ? m_current_node.Value : default(T);
    }
}