using System;
using System.Collections.Generic;
using System.Runtime.Remoting;
using NFX.ApplicationModel.Pile;
using NUnit.Framework;

namespace NFX.Utils
{
    [TestFixture]
    public class LinkedListTest
    {

        public DefaultPile m_pile = new DefaultPile() {AllocMode = AllocationMode.ReuseSpace};
        
        [SetUp]
        public void setUp()
        {
            m_pile.Start();
        }

        [TearDown]
        public void TearDown()
        {
            m_pile.WaitForCompleteStop();
        }
        [Test]
        public void SimpleTest()
        {
            var test = new LinkedList<int>(m_pile);
            Aver.IsNotNull(test);
            Aver.IsNull(test.First);
            Aver.IsNull(test.Last);
        }

        [Test]
        public void SimpleValueTest()
        {
            const int testValue = 11;
            var test = new LinkedList<int>(m_pile, testValue);
            
            Aver.IsNotNull(test);
            Aver.IsNotNull(test.First);
            Aver.IsNotNull(test.Last);
            Aver.AreEqual(test.First.Value, testValue);
            Aver.AreEqual(test.Last.Value, testValue);
            Aver.AreEqual(test.First.Value, test.Last.Value);
        }

        [Test]
        public void SimpleAddFirstTest()
        {
            const int testValue = 11;
            var test = new LinkedList<int>(m_pile);
            test.AddFirst(testValue);
            Aver.IsNotNull(test);
            Aver.IsNotNull(test.First);
            Aver.AreEqual(test.First.Value, testValue);
            Aver.AreEqual(test.Last.Value, testValue);
        }

        [Test]
        public void SimpleValyeAddFirstTest()
        {
            const int testValue = 11;
            var test = new LinkedList<int>(m_pile, testValue);
            test.AddFirst(testValue - 1);
            Aver.IsNotNull(test);
            Aver.IsNotNull(test.First);
            Aver.IsNotNull(test.First.Next);
            Aver.IsNotNull(test.Last);
            Aver.IsNotNull(test.Last.Previous);
            Aver.AreEqual(test.First.Value, testValue - 1);
            Aver.AreEqual(test.Last.Value, testValue);
            Aver.AreEqual(test.First.Next.Value, testValue);
        }

        [Test]
        public void SimpleAddLastTest()
        {
            const int testValue = 11;
            var test = new LinkedList<int>(m_pile);
            test.AddLast(testValue);
            Aver.IsNotNull(test);
            Aver.IsNotNull(test.First);
            Aver.IsNotNull(test.Last);
            Aver.AreEqual(test.Last.Value, testValue);
            Aver.AreEqual(test.First.Value, testValue);
        }

        [Test]
        public void SimpleValueAddLastTest()
        {
            const int testValue = 11;
            var test = new LinkedList<int>(m_pile, testValue);
            test.AddLast(testValue + 1);
            Aver.IsNotNull(test);
            Aver.IsNotNull(test.First);
            Aver.IsNotNull(test.First.Next);
            Aver.IsNotNull(test.Last);
            Aver.IsNotNull(test.Last.Previous);
            Aver.AreEqual(test.First.Value, testValue);
            Aver.AreEqual(test.Last.Value, testValue + 1);
            Aver.AreEqual(test.Last.Previous.Value, testValue);
        }

        [Test]
        public void SimpleRemoveTest()
        {
            const int testValue = 11;
            var test = new LinkedList<int>(m_pile, testValue);
            Aver.IsNotNull(test);
            Aver.IsNotNull(test.First);
            Aver.AreEqual(test.First.Value, testValue);
            test.Remove(test.First);
            Aver.IsNull(test.First);
            Aver.IsNull(test.Last);
        }

        [Test]
        public void SimpleRemoveFirstTest()
        {
            const int testValue = 11;
            var test = new LinkedList<int>(m_pile, testValue);
            var next = test.AddAfter(test.First, testValue + 1);
            Aver.AreEqual(test.First.Value, testValue);
            Aver.AreEqual(test.First.Next.Value, testValue +1);
            test.Remove(test.First);
            Aver.AreEqual(test.First.Value, testValue + 1);
            Aver.IsTrue(test.First.Equals(test.Last));
            Aver.IsNull(test.First.Next);
        }

        [Test]
        public void SimpleRemoveLastTest()
        {
            const int testValue = 11;
            var test = new LinkedList<int>(m_pile, testValue);
            var next = test.AddAfter(test.First, testValue + 1);
            Aver.AreEqual(test.First.Value, testValue);
            Aver.AreEqual(test.First.Next.Value, testValue + 1);
            test.Remove(test.Last);
            Aver.AreEqual(test.Last.Value, testValue);
            Aver.IsTrue(test.First.Equals(test.Last));
            Aver.IsNull(test.Last.Previous);
        }

        [Test]
        public void RemoveTest()
        {
            var test = new LinkedList<int>(m_pile, 0);
            LinkedListNode<int> forDelete = null;
            for (var i = 1; i < 10; i++)
            {
                var item = test.AddLast(i);
                if (i == 5)
                {
                    forDelete = item;
                } 
            }
            var j = 0;
            var current = test.First;
            while (current != null)
            {
                Console.WriteLine(j);
                Aver.AreEqual(current.Value, j);
                current = current.Next;
                j++;
            }
            test.Remove(forDelete);
            current = test.First;
            j = 0;
            while (current != null)
            {
                if (j != 5)
                {
                    Console.WriteLine(j);
                    Aver.AreEqual(current.Value, j);
                    current = current.Next;
                }
                j++;
            }
            
        }

        [Test]
        public void ForEachTest()
        {
            var test = new LinkedList<int>(m_pile, 0);
            for (var i = 1; i < 10; i++)
            {
                test.AddLast(i);
            }
            var j = 0;
            foreach (var item in test)
            {
                Aver.AreEqual(item, j);
                j++;
            }
        }
    }
}