using System;
using NFX.ApplicationModel.Pile;
using NUnit.Framework;

namespace NFX.Utils
{
    [TestFixture]
    public class PrefixTreeTest
    {
        private DefaultPile m_Pile = new DefaultPile(){AllocMode = AllocationMode.FavorSpeed};

        [SetUp]
        public void SetUp()
        {
            m_Pile.Start();
        }

        [TearDown]
        public void TearDown()
        {
            m_Pile.WaitForCompleteStop();
        }

        [Test]
        public void SimpleTest()
        {
            var test = new PrefixTree<int>(m_Pile);
            Aver.IsNotNull(test);
        }

        [Test]
        public void SimpleDestructorTest()
        {
            using (var test = new PrefixTree<string>(m_Pile))
            {
                for (int i = 0; i < 20; i++)
                {
                    test["a--{0}".Args(i)] = Guid.NewGuid().ToString();
                }
                
                Console.WriteLine(m_Pile.ObjectCount);
                Aver.AreNotEqual(m_Pile.ObjectCount, 0);
            }
            Console.WriteLine(m_Pile.ObjectCount);
            Aver.AreEqual(m_Pile.ObjectCount, 0);
        }

        [Test]
        public void SimpleSetTest()
        {
            var test = new PrefixTree<int>(m_Pile);
            test["1"] = 1;
            Console.WriteLine(test["1"]);
            test["12"] = 12;
            Console.WriteLine(test["12"]);
            test["22"] = 22;    
            Console.WriteLine(test["22"]);
            Console.WriteLine(test["2"]);
            Aver.AreEqual(test["1"], 1);
            Aver.AreEqual(test["12"], 12);
            Aver.AreEqual(test["22"], 22);
            Aver.AreEqual(test["2"], 0);
            Aver.AreEqual(test["0"], 0);

            test["12"] = 21;
            Aver.AreEqual(test["12"], 21);
        }
    }
}