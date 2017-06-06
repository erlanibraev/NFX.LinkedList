﻿using System;
using System.Collections;
using System.Collections.Generic;
using NFX.ApplicationModel.Pile;

namespace NFX.Utils
{
    public class LinkedList<T> : IEnumerable<T>, IEnumerable
    {
        protected internal PilePointer m_pp_first { get; set; } = PilePointer.Invalid;
        protected internal PilePointer m_pp_self { get; set; } = PilePointer.Invalid;
        protected internal PilePointer m_pp_last { get; set; } = PilePointer.Invalid;
        
        
        public LinkedList()
        {
            m_pp_self = PileSinglton.Pile.Put(this);
            PileSinglton.Pile.Put(m_pp_self, this);
        }

        public LinkedList(T value)
        {
            var item = new LinkedListNode<T>(value);
            m_pp_first = item.m_pp_self;
            m_pp_last = item.m_pp_self;
            m_pp_self = PileSinglton.Pile.Put(this);
            PileSinglton.Pile.Put(m_pp_self, this);
            item.m_pp_list = m_pp_self;
            PileSinglton.Pile.Put(item.m_pp_self, item);
        }

        public LinkedListNode<T> First => (LinkedListNode<T>) (m_pp_first != PilePointer.Invalid
            ? PileSinglton.Pile.Get(m_pp_first)
            : null);
        
        public LinkedListNode<T> Last => (LinkedListNode<T>) (m_pp_last != PilePointer.Invalid
            ? PileSinglton.Pile.Get(m_pp_last)
            : null);


        public LinkedListNode<T> AddFirst(T value)
        {
            var result = new LinkedListNode<T>(value);
            result.m_pp_list = m_pp_self;
            if (m_pp_first != PilePointer.Invalid)
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
            var result = new LinkedListNode<T>(value);
            result.m_pp_list = m_pp_self;
            if (m_pp_last != PilePointer.Invalid)
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
            var result = new LinkedListNode<T>(value);
            result.m_pp_list = m_pp_self;
            AddBefore(node, result);
            return result;
        }
        
        public void AddBefore(LinkedListNode<T> node, LinkedListNode<T> newNode)
        {
            if(newNode == null || node == null ) throw new ArgumentNullException();
            if (newNode.m_pp_next != PilePointer.Invalid || newNode.m_pp_previous != PilePointer.Invalid)
                throw new InvalidOperationException();
            if(node.m_pp_list != m_pp_self || newNode.m_pp_list != m_pp_self) throw new InvalidOperationException();
            
            var tmp_prev = node.m_pp_previous;
            node.m_pp_previous = newNode.m_pp_self;
            newNode.m_pp_next = node.m_pp_self;
            newNode.m_pp_previous = tmp_prev;
            if (m_pp_first.Equals(node.m_pp_self))
            {
                m_pp_first = newNode.m_pp_self;
                PileSinglton.Pile.Put(m_pp_self, this);
            }
            PileSinglton.Pile.Put(node.m_pp_self, node);
            PileSinglton.Pile.Put(newNode.m_pp_self, newNode);
        }

        public LinkedListNode<T> AddAfter(LinkedListNode<T> node, T value)
        {
            var result = new LinkedListNode<T>(value);
            result.m_pp_list = m_pp_self;
            AddAfter(node, result);
            return result;
        }

        public void AddAfter(LinkedListNode<T> node, LinkedListNode<T> newNode)
        {
            if(newNode == null || node == null ) throw new ArgumentNullException();
            if (newNode.m_pp_next != PilePointer.Invalid || newNode.m_pp_previous != PilePointer.Invalid)
                throw new InvalidOperationException();
            if(node.m_pp_list != m_pp_self || newNode.m_pp_list != m_pp_self) throw new InvalidOperationException();
            
            var tmp_prev = node.m_pp_next;
            node.m_pp_next = newNode.m_pp_self;
            newNode.m_pp_previous = node.m_pp_self;
            newNode.m_pp_next = tmp_prev;
            if (m_pp_last.Equals(node.m_pp_self))
            {
                m_pp_last = newNode.m_pp_self;
                PileSinglton.Pile.Put(m_pp_self, this);
            }
            PileSinglton.Pile.Put(node.m_pp_self, node);
            PileSinglton.Pile.Put(newNode.m_pp_self, newNode);
        }

        public void Remove(LinkedListNode<T> node)
        {
            throw new NotImplementedException();    
        }

        private void addFirstLastOnly(LinkedListNode<T> node)
        {
            m_pp_first = node.m_pp_self;
            m_pp_last = node.m_pp_self;
            PileSinglton.Pile.Put(m_pp_self, this);
            PileSinglton.Pile.Put(node.m_pp_self, node);            
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