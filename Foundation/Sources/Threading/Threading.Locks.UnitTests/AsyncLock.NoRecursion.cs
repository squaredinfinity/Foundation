using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SquaredInfinity.Threading;
using System.Threading.Tasks;
using System.Threading;
using SquaredInfinity.Threading.Locks;

namespace Threading.Locks.UnitTests
{
    [TestClass]
    public partial class AsyncLock__NoRecursion
    {
        [TestMethod]
        public async Task can_not_be_acquired_again_from_same_thread()
        {
            //  No Recursion
            //  Trying to acquire lock twice on a thread that already owns it will fail with exception

            var al = new AsyncLock(recursionPolicy: LockRecursionPolicy.NoRecursion);

            using (var l = al.AcquireWriteLock())
            {
                Assert.IsTrue(l.IsLockHeld);
                Assert.AreEqual(System.Environment.CurrentManagedThreadId, al.WriteOwnerThreadId);

                var ok = false;
                //try
                //{
                //    var l2 = al.AcquireWriteLock();
                //}
                //catch(LockRecursionException) { ok = true; }
                //Assert.IsTrue(ok, "Exception has not been thrown");

                ok = false;
                try
                {
                    var l2 = await al.AcquireWriteLockAsync();
                }
                catch (LockRecursionException) { ok = true; }
                Assert.IsTrue(ok, "Exception has not been thrown");
            }
        }

        [TestMethod]
        public async Task can_not_be_acquired_again_from_different_thread()
        {
            //  No Recursion
            //  Trying to acquire already-held lock on different thread will not work
            //  This isn't a true recursion scenario, just a confirmation of expected behavior

            var al = new AsyncLock(recursionPolicy: LockRecursionPolicy.NoRecursion);

            using (var l = al.AcquireWriteLock())
            {
                Assert.IsTrue(l.IsLockHeld);
                Assert.AreEqual(System.Environment.CurrentManagedThreadId, al.WriteOwnerThreadId);

                var l2 = Task.Run(() =>
                {
                    // this should fail to acquire lock
                    return al.AcquireWriteLock(new SyncOptions(25));
                }).Result;

                Assert.IsFalse(l2.IsLockHeld);
            }
        }
    }
}
