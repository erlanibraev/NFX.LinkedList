using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
            using (var test = new PrefixTree<int>(m_Pile))
            {
                Aver.IsNotNull(test);
            }
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
            using(var test = new PrefixTree<int>(m_Pile)) {
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

        [Test]
        public void SimpleOrdinalSetTest()
        {
            using (var test = new PrefixTree<int>(m_Pile))
            {
                for (int i = 12; i >= 0; i--)
                {
                    test["{0}".Args(i)] = i;
                }
                for(int i=0; i< 12; i++)
                {
                    Console.Write("{0}".Args(i));
                    Console.Write(" = ");
                    Console.WriteLine(test["{0}".Args(i)]);
                    Aver.AreEqual(test["{0}".Args(i)], i);
                }
            }
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
                var keys = new List<string>(test.Keys);
                
                foreach (var key in keys)
                {
                    Console.WriteLine(key);
                }
                Console.WriteLine(" --- ");
                for (int i = 0; i < 25; i++)
                {
                    Console.WriteLine("a--{0}".Args(i));
                    Aver.IsTrue(keys.Contains("a--{0}".Args(i)));
                }
            }
            Console.WriteLine(" --- ");
            Console.WriteLine(m_Pile.ObjectCount);            
        }

        [Test]
        public void SimpleCountTest()
        {
            using (var test = new PrefixTree<int>(m_Pile))
            {
                const int COUNT = 1000;
                for (int i = 0; i < COUNT; i++)
                {
                    test["{0}".Args(i)] = i;
                }
                Aver.AreEqual(test.Count, COUNT);
                var rnd = new Random();
                int[] delete = {rnd.Next(COUNT), rnd.Next(COUNT)};
                Array.Sort(delete) ;
                for (int i = delete[0]; i < delete[1]; i++)
                {
                    test.Remove("{0}".Args(i));
                }
                int deleted = delete[1] - delete[0];
                Aver.AreEqual(test.Count, COUNT - deleted);

                test.Remove("{0}".Args(COUNT - 1));
                Aver.AreEqual(test.Count, COUNT - deleted - 1);
                
            }
        }
        
        [Test]
        public void SimpleEnumaratorTest()
        {
/*
            using (var test = new PrefixTree<int>(m_Pile))
            {
                const int COUNT = 100;
                Console.WriteLine(DateTime.Now);
                for (int i = 0; i < COUNT; i++)
                {
                    test[Guid.NewGuid().ToString()] = i;
                }

                Console.WriteLine(DateTime.Now);
                Console.WriteLine(test.Keys.Count);
                Console.WriteLine(DateTime.Now);
                Console.WriteLine(" --- ");
                Aver.AreEqual(test.Keys.Count, COUNT);

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
*/
        }

        [TestCase(10000)]
        [TestCase(100000)]
        [TestCase(1000000)]
        [TestCase(10000000)]
        public void SimpleMassivTest(int count)
        {
            using (var test = new PrefixTree<int>(m_Pile))
            {
                String lastKey = "";
                Console.WriteLine(DateTime.Now);
                int max = 0;
                for (int i = 0; i < count; i++)
                {
                    int ms = DateTime.Now.Millisecond; 
                    string key = Guid.NewGuid().ToString();
                    // key = "TEST KEY {0}".Args(i);
                    key = key.Substring(0, 3)+" "+i.ToString();
                    test[key] = i;
                    lastKey = key;
                    int me = DateTime.Now.Millisecond;
                    if (me - ms < max) max = me - ms;
                }
                Console.WriteLine("\t"+max);
                
                Console.WriteLine(DateTime.Now);
                Console.WriteLine(m_Pile.ObjectCount);
                Console.WriteLine(DateTime.Now);
                List<string> keys = new List<string>(test.Keys);
                Console.WriteLine(keys.Count);
                Console.WriteLine(DateTime.Now);
                
                foreach (var key in keys)
                {
                    //Console.Write(key+"; ");
                    lastKey = key;
                }
                Console.WriteLine();
                Console.WriteLine(DateTime.Now);
                Console.WriteLine(keys.Find(s => s == lastKey));
                Console.WriteLine(DateTime.Now);
                Console.WriteLine(test[lastKey]);
                Console.WriteLine(DateTime.Now);
            }
            Console.WriteLine(m_Pile.ObjectCount);
            Console.WriteLine(DateTime.Now);
        }

        [TestCase(1000000)]
        public void SimpleMassivWithoutDestryTest(int count)
        {
            var test = new PrefixTree<int>(m_Pile);
            String lastKey = "";
            Console.WriteLine(DateTime.Now);
            for (int i = 0; i < count; i++)
            {
                int m = 0;
                if(i == count -1 ) m = DateTime.Now.Millisecond; 
                string key = Guid.NewGuid().ToString();
                // key = "TEST KEY {0}".Args(i);
                key = key.Substring(0, 3)+" "+i.ToString();
                test[key] = i;
                lastKey = key;
                if(i == count -1 ) Console.WriteLine("\t"+(DateTime.Now.Millisecond - m).ToString()); 

            }
            Console.WriteLine(DateTime.Now);
            Console.WriteLine(test[lastKey]);
            Console.WriteLine(DateTime.Now);
        }

        [TestCase(10000)]
        [TestCase(100000)]
        [TestCase(1000000)]
        [TestCase(10000000)]
        public void getKeyValuePairsTest(int count)
        {
            using(var test = new PrefixTree<int>(m_Pile))
            {
                Console.WriteLine(DateTime.Now);
                for (int i=0; i< count; i++)
                {
                    test[FID.Generate().ToString()] = i;
                }
                Console.WriteLine(DateTime.Now);
                List<KeyValuePair<string, int>> keyValuePairs = new List<KeyValuePair<string, int>>(test.getKeyValuePairs());
                Console.WriteLine(keyValuePairs.Count);
                Console.WriteLine(DateTime.Now);
                Console.WriteLine(new List<String>(test.Keys).Count);
                Console.WriteLine(DateTime.Now);
            }
            Console.WriteLine(m_Pile.ObjectCount);
            Console.WriteLine(DateTime.Now);
        }

        [TestCase(5)]
        [TestCase(10000)]
        [TestCase(100000)]
        [TestCase(1000000)]
        [TestCase(10000000)]
        public void PutValueSimpleTest(int count)
        {
            string[] keys = new string[count];
            var rnd = new Random();
            DateTime start;
            DateTime end;
            using (var test = new PrefixTree<int>(m_Pile))
            {
                start = DateTime.Now;
                Console.WriteLine(start);
                for (int i = 0; i < count; i++)
                {
                    keys[i] = FID.Generate().ToString();
                    test.Put(keys[i], i);
                }
                end = DateTime.Now;
                Console.WriteLine(end);
                Console.WriteLine(end.Subtract(start));
                if (count < 11)
                {
                    foreach (string key in test.Keys)
                    {
                        Console.Write(key);
                        Console.Write(" = ");
                        Console.WriteLine(test[key]);
                    }
                    Console.WriteLine(DateTime.Now);
                    Console.WriteLine(test[keys[rnd.Next(count)]]);
                    Console.WriteLine(DateTime.Now);
                }
            }
            using (var test1 = new PrefixTree<int>(m_Pile))
            {
                start = DateTime.Now;
                Console.WriteLine(start);
                for (int i = 0; i < count; i++)
                {
                    try
                    {
                        test1[keys[i]] = i;
                    }
                    catch (Exception ex)
                    {
                        Console.Write("Error: ");
                        Console.WriteLine(keys[i]);
                        Console.WriteLine(test1.Count);
                    }

                }
                end = DateTime.Now;
                Console.WriteLine(end);
                Console.WriteLine(end.Subtract(start));
                Console.WriteLine(test1[keys[rnd.Next(count)]]);
                Console.WriteLine(DateTime.Now);
            }
            start = DateTime.Now;
            Console.WriteLine(start);
            Dictionary<string, int> test2 = new Dictionary<string, int>();
            for (int i = 0; i < count; i++)
            {
                try
                {
                    test2[keys[i]] = i;
                }
                catch (Exception ex)
                {
                    Console.Write("Error: ");
                    Console.WriteLine(keys[i]);
                    Console.WriteLine(test2.Count);
                }

            }
            end = DateTime.Now;
            Console.WriteLine(end);
            Console.WriteLine(end.Subtract(start));
            Console.WriteLine(DateTime.Now);
            Console.WriteLine(test2[keys[rnd.Next(count)]]);
            Console.WriteLine(DateTime.Now);
            start = DateTime.Now;
            Console.WriteLine(start);
            var test3 = new SortedDictionary<string, int>(test2);
            end = DateTime.Now;
            Console.WriteLine(end);
            Console.WriteLine(test3[keys[rnd.Next(count)]]);
            Console.WriteLine(end.Subtract(start));
        }

    }
}