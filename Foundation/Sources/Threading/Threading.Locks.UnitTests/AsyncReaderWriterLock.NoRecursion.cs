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
        [ExpectedException(typeof(LockRecursionException))]
        public async Task writer_held__same_thread_can_not_get_reader()
        {
            var l = new AsyncReaderWriterLock(recursionPolicy: LockRecursionPolicy.NoRecursion);

            using (var a1 = l.AcquireWriteLock())
            {
                Assert.IsTrue(a1.IsLockHeld);

                l.AcquireReadLock();
            }
        }
    }
}
