using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SquaredInfinity.Threading;
using System.Threading.Tasks;
using System.Threading;
using SquaredInfinity.Threading.Locks;

namespace Threading.Locks.UnitTests
{
    [TestClass]
    public class AsyncLockTests
    {
        [TestMethod]
        public async Task only_one_thread_allowed_to_hold_lock()
        {
            var al = new AsyncLock();

            using (var l = await al.AcqureWriteLockAsync(continueOnCapturedContext: false))
            {
                Assert.IsTrue(l.IsLockHeld);

                var l2 = await al.AcqureWriteLockAsync(TimeSpan.FromMilliseconds(25), continueOnCapturedContext: false);

                Assert.IsFalse(l2.IsLockHeld);
            }
        }
    }
}
