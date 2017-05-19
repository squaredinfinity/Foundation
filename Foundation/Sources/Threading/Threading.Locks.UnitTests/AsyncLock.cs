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


        [TestMethod]
        public async Task not_reentrant__can_not_be_acquired_again_even_with_continue_on_captured_true()
        {
            var al = new AsyncLock(supportsReentrancy: false);

            using (var l = await al.AcqureWriteLockAsync(continueOnCapturedContext: false))
            {
                Assert.IsTrue(l.IsLockHeld);

                var l2 = al.AcqureWriteLockAsync(TimeSpan.FromMilliseconds(25), continueOnCapturedContext: true).Result;

                Assert.IsFalse(l2.IsLockHeld);
            }
        }

        [TestMethod]
        public async Task reentrant__can_be_acquired_again_even_with_continue_on_captured_false()
        {
            var al = new AsyncLock(supportsReentrancy:true);

            using (var l = await al.AcqureWriteLockAsync(continueOnCapturedContext: false))
            {
                Assert.IsTrue(l.IsLockHeld);

                var l2 = al.AcqureWriteLockAsync(TimeSpan.FromMilliseconds(25), continueOnCapturedContext: false).Result;

                Assert.IsTrue(l2.IsLockHeld);
            }
        }

        [TestMethod]
        public async Task reentrant2__can_be_acquired_again_even_with_continue_on_captured_false()
        {
            // l1 acquires lock
            // l2 fails after timeout

            var al = new AsyncLock(supportsReentrancy: true);

            var are = new AutoResetEvent(false);

            var l2 = Task.Run(() =>
            {
                are.WaitOne();
                return al.AcqureWriteLockAsync(100, continueOnCapturedContext: true);
            });

            var l1 = Task.Run(() =>
            {
                var t = al.AcqureWriteLockAsync(continueOnCapturedContext: true);
                are.Set();
                return t;
            });

            Task.WhenAll(l1, l2).Wait();

            Assert.IsTrue(l1.Result.IsLockHeld);
            Assert.IsFalse(l2.Result.IsLockHeld);
        }
    }
}
