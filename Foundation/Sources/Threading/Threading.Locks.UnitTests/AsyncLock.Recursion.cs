using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SquaredInfinity.Threading;
using System.Threading.Tasks;
using System.Threading;
using SquaredInfinity.Threading.Locks;

namespace Threading.Locks.UnitTests
{
    [TestClass]
    public partial class AsyncLock__Recursion
    {
        [TestMethod]
        public async Task can_be_acquired_again_even_with_continue_on_captured_false()
        {
            var al = new AsyncLock(recursionPolicy: LockRecursionPolicy.SupportsRecursion);

            using (var l = await al.AcquireWriteLockAsync())
            {
                Assert.IsTrue(l.IsLockHeld);

                var l2 = al.AcquireWriteLockAsync(new AsyncOptions(25, continueOnCapturedContext: false)).Result;

                Assert.IsTrue(l2.IsLockHeld);
            }
        }

        [TestMethod]
        public async Task can_not_be_acquired_again_even_with_continue_on_captured_false()
        {
            // l1 acquires lock
            // l2 fails after timeout

            var al = new AsyncLock(recursionPolicy: LockRecursionPolicy.SupportsRecursion);

            var are = new AutoResetEvent(false);

            var l2 = Task.Run(() =>
            {
                are.WaitOne();
                return al.AcquireWriteLockAsync(new AsyncOptions(100, continueOnCapturedContext: true));
            });

            var l1 = Task.Run(() =>
            {
                var t = al.AcquireWriteLockAsync(AsyncOptions.OnCapturedContext);
                are.Set();
                return t;
            });

            await Task.WhenAll(l1, l2);
            
            Assert.IsTrue(l1.Result.IsLockHeld);
            Assert.IsFalse(l2.Result.IsLockHeld);
        }
    }
}
