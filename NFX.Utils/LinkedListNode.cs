using NFX.ApplicationModel.Pile;

namespace NFX.Utils
{
    public class LinkedListNode<T>
    {
        protected internal PilePointer m_pp_self { get; set; } = PilePointer.Invalid;
        protected internal PilePointer m_pp_value { get; set; } = PilePointer.Invalid;
        protected internal PilePointer m_pp_next { get; set; } = PilePointer.Invalid;
        protected internal PilePointer m_pp_previous { get; set; } = PilePointer.Invalid;
        protected internal PilePointer m_pp_list { get; set; } =PilePointer.Invalid;

        public LinkedListNode()
        {
            m_pp_self = PileSinglton.Pile.Put(this);
            PileSinglton.Pile.Put(m_pp_self, this);

        }

        public LinkedListNode(T value)
        {
            if (value != null) m_pp_value = PileSinglton.Pile.Put(value);
            m_pp_self = PileSinglton.Pile.Put(this);
            PileSinglton.Pile.Put(m_pp_self, this);

        }

        public T Value
        {
            get => (T) (m_pp_value != PilePointer.Invalid ? PileSinglton.Pile.Get(m_pp_value) : default(T));
            set
            {
                if (m_pp_value != PilePointer.Invalid)
                {
                    PileSinglton.Pile.Delete(m_pp_value);    
                }
                if (value != null)
                {
                    m_pp_value = PileSinglton.Pile.Put(value);
                    PileSinglton.Pile.Put(m_pp_self, this);
                }
                else
                {
                    m_pp_value = PilePointer.Invalid;
                }
            }
            
        }

        public LinkedListNode<T> Next => (LinkedListNode<T>) (m_pp_next != PilePointer.Invalid ? PileSinglton.Pile.Get(m_pp_next) : null);

        public LinkedListNode<T> Previous => (LinkedListNode<T>) (m_pp_previous != PilePointer.Invalid ? PileSinglton.Pile.Get(m_pp_previous) : null);
    }
}