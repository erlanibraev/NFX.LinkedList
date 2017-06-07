using System;
using NFX.ApplicationModel.Pile;
using NUnit.Framework;
using System.Collections.Generic;

namespace NFX.Utils
{
    [TestFixture]
    public class LinkedListNodeTest
    {

        public IPile Pile { get; set; }

        [Test]
        public void EmptyTest()
        {
            using (var m_pile = new DefaultPile() {AllocMode = AllocationMode.FavorSpeed})
            {
                m_pile.Start();
                var test = m_pile.Put("TEST");
                Console.WriteLine(test);
                Console.WriteLine(m_pile.Get(test));
                
                m_pile.WaitForCompleteStop();
            }
            
            Assert.True(true);
        }

        [Test]
        public void crazyTest()
        {
            using(var m_pile = new DefaultPile())
            {
                m_pile.Start();

                Pile = m_pile;
                var list = new List<LinkedListNode<int>>();

                for (var i=0; i< 10; i++)
                {
                    list.Add(new LinkedListNode<int>(Pile, i));
                }

                foreach(var item in list)
                {
                    Console.WriteLine(item.Value);
                }
            }
        }

        [Test]
        public void SimpleTest()
        {

            using (var m_pile = new DefaultPile())
            {
                m_pile.Start();
                var test = new LinkedListNode<int>(m_pile, 11);
                var pp1 = m_pile.Put(11);
                // var pp = m_pile.Put(test);
                // Console.WriteLine(pp);
                Console.WriteLine(test.Value);
                Assert.AreEqual(test.Value, 11);
                
                m_pile.WaitForCompleteStop();
            }
        }

        [Test]
        public void SimpleNullTest()
        {
            using (var m_pile = new DefaultPile() {AllocMode = AllocationMode.FavorSpeed}) {
                m_pile.Start();
                var test = new LinkedListNode<string>(m_pile, null);
                Assert.IsNull(test.Value);
            }
        }

        [Test]
        public void SimpleValueNullTest()
        {
            using (var m_pile = new DefaultPile() {AllocMode = AllocationMode.FavorSpeed})
            {
                m_pile.Start();
                var test = new LinkedListNode<string>(m_pile, "TEST");
                Assert.AreEqual(test.Value, "TEST");
                test.Value = null;
                Assert.IsNull(test.Value);
            }
        }



    }
}