using System;
using System.Collections.Generic;
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

        [Test]
        public void SimpleRemoveTest()
        {
            using (var test = new PrefixTree<int>(m_Pile))
            {
                const int REMOVE_VALUE = 15;
                const int REMOVE_VALUE_1 = 1;
                const int REMOVE_VALUE_2 = 17;
                for (int i = 0; i < 20; i++)
                {
                    test["a--{0}".Args(i)] = i;
                }
                test.Remove("a--{0}".Args(REMOVE_VALUE));
                Console.WriteLine(test["a--{0}".Args(REMOVE_VALUE)]);
                
                test.Remove("a--{0}".Args(REMOVE_VALUE_1));
                Console.WriteLine(test["a--{0}".Args(REMOVE_VALUE_1)]);

                test.Remove("a--{0}".Args(REMOVE_VALUE_2));
                Console.WriteLine(test["a--{0}".Args(REMOVE_VALUE_2)]);
                
                
                Aver.AreEqual(test["a--{0}".Args(REMOVE_VALUE)], default(int));
                Aver.AreEqual(test["a--{0}".Args(REMOVE_VALUE_1)], default(int));
                Aver.AreEqual(test["a--{0}".Args(REMOVE_VALUE_2)], default(int));
                Aver.AreEqual(test["a--{0}".Args(19)],19);
                Aver.AreEqual(test["a--{0}".Args(12)],12);
            }
        }

        [Test]
        public void SimpleKeysTest()
        {
            using (var test = new PrefixTree<int>(m_Pile))
            {
                for (int i = 0; i < 25; i++)
                {
                    test["a--{0}".Args(i)] = i;
                }
                foreach (var key in test.Keys)
                {
                    Console.WriteLine(key);
                }
                Console.WriteLine(" --- ");
                for (int i = 0; i < 25; i++)
                {
                    Console.WriteLine("a--{0}".Args(i));
                    Aver.IsTrue(test.Keys.Contains("a--{0}".Args(i)));
                }
            }
        }

        [Test]
        public void SimpleEnumaratorTest()
        {
            using (var test = new PrefixTree<int>(m_Pile))
            {
                const int COUNT = 100;
                Console.WriteLine(DateTime.Now);
                for (int i = 0; i < COUNT; i++)
                {
                    test[Guid.NewGuid().ToString()] = i;
                }
/*
                Console.WriteLine(DateTime.Now);
                Console.WriteLine(test.Keys.Count);
                Console.WriteLine(DateTime.Now);
                Console.WriteLine(" --- ");
                Aver.AreEqual(test.Keys.Count, COUNT);
*/
                Console.WriteLine(DateTime.Now);
                int j = 0;
                int sum = 0;
                foreach (var item in test)
                {
                    j++;
                    sum += item;
                    // Console.WriteLine(item);
                }
                Console.WriteLine(DateTime.Now);
                Console.Write("sum = ");
                Console.WriteLine(sum);
                Console.WriteLine(" --- ");
                Console.WriteLine(j);
                Aver.AreEqual(j, COUNT);
                Console.WriteLine(" --- ");
                Console.WriteLine(m_Pile.ObjectCount);
            }
            Console.WriteLine(" --- ");
            Console.WriteLine(m_Pile.ObjectCount);
        }

        [TestCase(100000)]
        public void SimpleMassivTest(int count)
        {
            using (var test = new PrefixTree<int>(m_Pile))
            {
                Console.WriteLine(DateTime.Now);
                for (int i = 0; i < count; i++)
                {
                    string key = Guid.NewGuid().ToString();
                    // key = "TEST KEY {0}".Args(i);
                    key = key.Substring(0, 3)+" "+i.ToString();
                    test[key] = i;
                }
                Console.WriteLine(DateTime.Now);
                Console.WriteLine(m_Pile.ObjectCount);
                Console.WriteLine(DateTime.Now);
                LinkedList<string> keys = test.Keys;
                Console.WriteLine(keys.Count);
                Console.WriteLine(DateTime.Now);
                String lastKey = "";
                foreach (var key in keys)
                {
                    //Console.Write(key+"; ");
                    lastKey = key;
                }
                Console.WriteLine();
                Console.WriteLine(DateTime.Now);
                Console.WriteLine(keys.Find(lastKey).Value);
                Console.WriteLine(DateTime.Now);
            }
            Console.WriteLine(m_Pile.ObjectCount);
        }
    }
}