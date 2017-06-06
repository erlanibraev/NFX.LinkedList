using System.Collections;
using System.Collections.Generic;

namespace NFX.Utils
{
    public class LinkedListEnumenator<T> : IEnumerator<T>
    {

        private LinkedList<T> m_list;
        private LinkedListNode<T> m_current_node;

        public LinkedListEnumenator(LinkedList<T> list)
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