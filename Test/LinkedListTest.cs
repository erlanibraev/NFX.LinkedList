using System.Runtime.Remoting;
using NUnit.Framework;

namespace NFX.Utils
{
    [TestFixture]
    public class LinkedListTest
    {

        [Test]
        public void SimpleTest()
        {
            var test = new LinkedList<int>();
            Assert.NotNull(test);
            Assert.IsNull(test.First);
            Assert.IsNull(test.Last);
        }

        [Test]
        public void SimpleValueTest()
        {
            const int testValue = 11;
            var test = new LinkedList<int>(testValue);
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
            var test = new LinkedList<int>();
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
            var test = new LinkedList<int>(testValue);
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
            var test = new LinkedList<int>();
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
            var test = new LinkedList<int>(testValue);
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
            var test = new LinkedList<int>(testValue);
            Assert.NotNull(test);
            Assert.NotNull(test.First);
            Assert.AreEqual(test.First.Value, testValue);
            test.Remove(test.First);
            Assert.IsNull(test.First);
            Assert.IsNull(test.Last);
        }
    }
}