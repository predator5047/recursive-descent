using NUnit.Framework;
using Parser;

namespace ParserTests
{
    [TestFixture]
    public class PQueueTests
    {
        private PQueue<int> sut;

        [SetUp]
        public void Setup()
        {
            sut = new PQueue<int>();
        }

        [Test]
        public void CanGetLookAhead()
        {
            sut.Enqueue(2);
            sut.Enqueue(4);
            sut.Enqueue(6);
            sut.Enqueue(8);

            Assert.AreEqual(2, sut.Peek());
            Assert.AreEqual(4, sut.PeekNext());
        }

        [Test]
        public void CanGetDeeperLookAhead()
        {
            sut.Enqueue(2);
            sut.Enqueue(4);
            sut.Enqueue(6);
            sut.Enqueue(8);
            sut.Enqueue(10);
            sut.Enqueue(12);

            Assert.AreEqual(6, sut.PeekN(2));
            Assert.AreEqual(8, sut.PeekN(3));
        }

        [Test]
        public void CanEnqueueBeginning()
        {
            sut.Enqueue(2);
            sut.Enqueue(4);
            sut.Enqueue(10);
            sut.Dequeue();
            sut.EnqueueBeginning(2);

            Assert.AreEqual(2, sut.Peek());
        }

        [Test]
        [ExpectedException]
        public void ThrowsExceptionIfPeekIsTooDeep()
        {
            sut.Enqueue(2);
            sut.PeekNext();
        }
    }
}