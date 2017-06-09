using System.Collections;
using System.Collections.Generic;
using NFX.ApplicationModel.Pile;

namespace NFX.Utils
{
    public class PrefixTree<T> : IEnumerable<T>
    {
        #region .ctor

        public PrefixTree(IPile pile)
        {
            m_Pile = pile;
            m_Comparer = Comparer<string>.Default;
        }
        
        public PrefixTree(IPile pile, Comparer<string> comparer)
        {
            m_Pile = pile;
            m_Comparer = comparer;
        }
        
        #endregion

        #region Fields

        private IPile m_Pile;
        private Comparer<string> m_Comparer;
        private PilePointer m_Root;

        #endregion

        #region Properties

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
        
        #endregion
    }
}