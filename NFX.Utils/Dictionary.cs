using System.Collections;
using System.Collections.Generic;
using NFX.ApplicationModel.Pile;

namespace NFX.Utils
{
    public class Dictionary<TKey, TValue> : DisposableObject, IDictionary<TKey, TValue>
    {

        public Dictionary(IPile pile)
        {
            m_Pile = pile;
        }


        private IPile m_Pile;
        
        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        {
            throw new System.NotImplementedException();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void Add(KeyValuePair<TKey, TValue> item)
        {
            throw new System.NotImplementedException();
        }

        public void Clear()
        {
            throw new System.NotImplementedException();
        }

        public bool Contains(KeyValuePair<TKey, TValue> item)
        {
            throw new System.NotImplementedException();
        }

        public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
        {
            throw new System.NotImplementedException();
        }

        public bool Remove(KeyValuePair<TKey, TValue> item)
        {
            throw new System.NotImplementedException();
        }

        public int Count { get; }
        public bool IsReadOnly { get; }
        public bool ContainsKey(TKey key)
        {
            throw new System.NotImplementedException();
        }

        public void Add(TKey key, TValue value)
        {
            throw new System.NotImplementedException();
        }

        public bool Remove(TKey key)
        {
            throw new System.NotImplementedException();
        }

        public bool TryGetValue(TKey key, out TValue value)
        {
            throw new System.NotImplementedException();
        }

        public TValue this[TKey key]
        {
            get { throw new System.NotImplementedException(); }
            set { throw new System.NotImplementedException(); }
        }

        public ICollection<TKey> Keys { get; }
        public ICollection<TValue> Values { get; }

        internal struct DictionaryStruct<TKey, TValue>
        {
            internal PilePointer Self;
            internal KeyValuePair<TKey, TValue>[] Keys;
            
        }
    }
}