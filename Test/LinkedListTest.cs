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
            Assert.NotNull(test);
            Assert.IsNull(test.First);
            Assert.IsNull(test.Last);
        }

        [Test]
        public void SimpleValueTest()
        {
            const int testValue = 11;
            var test = new LinkedList<int>(m_pile, testValue);
            Assert.NotNull(test);
            Assert.NotNull(test.First);
            Assert.NotNull(test.Last);
            Assert.AreEqual(test.First.Value, testValue);
            Assert.AreEqual(test.Last.Value, testValue);
            Assert.AreEqual(test.First.Value, test.Last.Value);
        }

        [Test]
        public void SimpleAddFirstTest()
        {
            const int testValue = 11;
            var test = new LinkedList<int>(m_pile);
            test.AddFirst(testValue);
            Assert.NotNull(test);
            Assert.NotNull(test.First);
            Assert.AreEqual(test.First.Value, testValue);
            Assert.AreEqual(test.Last.Value, testValue);
        }

        [Test]
        public void SimpleValyeAddFirstTest()
        {
            const int testValue = 11;
            var test = new LinkedList<int>(m_pile, testValue);
            test.AddFirst(testValue - 1);
            Assert.NotNull(test);
            Assert.NotNull(test.First);
            Assert.NotNull(test.First.Next);
            Assert.NotNull(test.Last);
            Assert.NotNull(test.Last.Previous);
            Assert.AreEqual(test.First.Value, testValue - 1);
            Assert.AreEqual(test.Last.Value, testValue);
            Assert.AreEqual(test.First.Next.Value, testValue);
        }

        [Test]
        public void SimpleAddLastTest()
        {
            const int testValue = 11;
            var test = new LinkedList<int>(m_pile);
            test.AddLast(testValue);
            Assert.NotNull(test);
            Assert.NotNull(test.First);
            Assert.NotNull(test.Last);
            Assert.AreEqual(test.Last.Value, testValue);
            Assert.AreEqual(test.First.Value, testValue);
        }

        [Test]
        public void SimpleValueAddLastTest()
        {
            const int testValue = 11;
            var test = new LinkedList<int>(m_pile, testValue);
            test.AddLast(testValue + 1);
            Assert.NotNull(test);
            Assert.NotNull(test.First);
            Assert.NotNull(test.First.Next);
            Assert.NotNull(test.Last);
            Assert.NotNull(test.Last.Previous);
            Assert.AreEqual(test.First.Value, testValue);
            Assert.AreEqual(test.Last.Value, testValue + 1);
            Assert.AreEqual(test.Last.Previous.Value, testValue);
        }

        [Test]
        public void SimpleRemoveTest()
        {
            const int testValue = 11;
            var test = new LinkedList<int>(m_pile, testValue);
            Assert.NotNull(test);
            Assert.NotNull(test.First);
            Assert.AreEqual(test.First.Value, testValue);
            test.Remove(test.First);
            Assert.IsNull(test.First);
            Assert.IsNull(test.Last);
        }

        [Test]
        public void SimpleRemoveFirstTest()
        {
            const int testValue = 11;
            var test = new LinkedList<int>(m_pile, testValue);
            var next = test.AddAfter(test.First, testValue + 1);
            Assert.AreEqual(test.First.Value, testValue);
            Assert.AreEqual(test.First.Next.Value, testValue +1);
            test.Remove(test.First);
            Assert.AreEqual(test.First.Value, testValue + 1);
            Assert.AreEqual(test.First, test.Last);
            Assert.IsNull(test.First.Next);
        }

        [Test]
        public void SimpleRemoveLastTest()
        {
            const int testValue = 11;
            var test = new LinkedList<int>(m_pile, testValue);
            var next = test.AddAfter(test.First, testValue + 1);
            Assert.AreEqual(test.First.Value, testValue);
            Assert.AreEqual(test.First.Next.Value, testValue + 1);
            test.Remove(test.Last);
            Assert.AreEqual(test.Last.Value, testValue);
            Assert.AreEqual(test.First, test.Last);
            Assert.IsNull(test.Last.Previous);
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
                Assert.AreEqual(current.Value, j);
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
                    Assert.AreEqual(current.Value, j);
                    current = current.Next;
                }
                j++;
            }
            
        }
    }
}