using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SquaredInfinity.Threading;
using System.Threading.Tasks;
using System.Threading;
using SquaredInfinity.Threading.Locks;

namespace Threading.Locks.UnitTests
{
	[TestClass]
    public class AsyncReaderWriterLock__NoRecursion
    {
        [TestMethod]
        public async Task MyTestMethod()
        {
            var l1 = new AsyncReaderWriterLock(recursionPolicy: LockRecursionPolicy.NoRecursion);
            var l2 = new AsyncReaderWriterLock(recursionPolicy: LockRecursionPolicy.NoRecursion);
            var l3 = new AsyncReaderWriterLock(recursionPolicy: LockRecursionPolicy.NoRecursion);
            var l4 = new AsyncReaderWriterLock(recursionPolicy: LockRecursionPolicy.NoRecursion);

            var all_acquisitions = await AsyncLock.AcquireReadLockAsync(l1, l2, l3, l4);


            Assert.IsTrue(all_acquisitions.IsLockHeld);

            Assert.AreEqual(-1, l1.WriteOwnerThreadId);
            Assert.AreEqual(-1, l2.WriteOwnerThreadId);
            Assert.AreEqual(-1, l3.WriteOwnerThreadId);
            Assert.AreEqual(-1, l4.WriteOwnerThreadId);

            Assert.IsTrue(l1.ReadOwnerThreadIds.Contains(System.Environment.CurrentManagedThreadId));
            Assert.IsTrue(l2.ReadOwnerThreadIds.Contains(System.Environment.CurrentManagedThreadId));
            Assert.IsTrue(l3.ReadOwnerThreadIds.Contains(System.Environment.CurrentManagedThreadId));
            Assert.IsTrue(l4.ReadOwnerThreadIds.Contains(System.Environment.CurrentManagedThreadId));

            all_acquisitions.Dispose();

            Assert.IsFalse(all_acquisitions.IsLockHeld);

            Assert.AreEqual(-1, l1.WriteOwnerThreadId);
            Assert.AreEqual(-1, l2.WriteOwnerThreadId);
            Assert.AreEqual(-1, l3.WriteOwnerThreadId);
            Assert.AreEqual(-1, l4.WriteOwnerThreadId);

            Assert.AreEqual(0, l1.ReadOwnerThreadIds.Count);
            Assert.AreEqual(0, l2.ReadOwnerThreadIds.Count);
            Assert.AreEqual(0, l3.ReadOwnerThreadIds.Count);
            Assert.AreEqual(0, l4.ReadOwnerThreadIds.Count);
        }
    }
}
