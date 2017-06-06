using System;
using NFX.Utils;
using NUnit.Framework;

namespace NFX.Utils
{
    [TestFixture]
    public class LinkedListNodeTest
    {
        [Test]
        public void EmptyTest()
        {
            Assert.True(true);
        }

        [Test]
        public void SimpleTest()
        {
            var test = new LinkedListNode<int>(11);
            Console.WriteLine(test.Value);            
            Assert.AreEqual(test.Value, 11);
        }

        [Test]
        public void SimpleNullTest()
        {
            var test = new LinkedListNode<string>(null);
            Assert.IsNull(test.Value);
        }

        [Test]
        public void SimpleValueNullTest()
        {
            var test = new LinkedListNode<string>("TEST");
            Assert.AreEqual(test.Value, "TEST");
            test.Value = null;
            Assert.IsNull(test.Value);
        }

    }
}