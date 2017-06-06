using NFX.ApplicationModel.Pile;

namespace NFX.Utils
{
    public class PileSinglton
    {
        private static DefaultPile m_Pile = null;

        public static IPile Pile
        {
            get
            {
                if (m_Pile == null)
                {
                    m_Pile = new DefaultPile {AllocMode = AllocationMode.FavorSpeed};
                    m_Pile.Start();
                }
                return m_Pile;
            }
        }

        public static void Done()
        {
            m_Pile.WaitForCompleteStop();
        }
    }
}