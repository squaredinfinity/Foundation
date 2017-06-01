using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SquaredInfinity.Threading;
using System.Threading.Tasks;
using System.Threading;
using SquaredInfinity.Threading.Locks;
using System.Collections.Generic;

namespace Threading.Locks.UnitTests
{
    [TestClass]
    public partial class AsyncLock__General
    {
        [TestMethod]
        public async Task write_owner_is_set()
        {
            var al = new AsyncLock();
            var l = await al.AcquireWriteLockAsync();

            Assert.AreEqual(System.Environment.CurrentManagedThreadId, al.WriteOwnerThreadId);

            l.Dispose();

            Assert.AreEqual(-1, al.WriteOwnerThreadId);
        }

        [TestMethod]
        public async Task only_one_thread_allowed_to_hold_lock()
        {
            var al = new AsyncLock();

            using (var l = await al.AcquireWriteLockAsync())
            {
                Assert.IsTrue(l.IsLockHeld);

                var l2 = Task.Factory.StartNew(() => al.AcquireWriteLockAsync(new AsyncOptions(25)).Result).Result;

                Assert.IsFalse(l2.IsLockHeld);
            }
        }

        [TestMethod]
        public async Task acquire_multiple_locks_asynchronously()
        {
            var l1 = new AsyncLock();
            var l2 = new AsyncLock();
            var l3 = new AsyncLock();
            var l4 = new AsyncLock();

            var all_acquisitions =
                await
                AsyncLock.AcquireWriteLockAsync(l1, l2, l3, l4);

            Assert.IsTrue(all_acquisitions.IsLockHeld);

            Assert.AreEqual(System.Environment.CurrentManagedThreadId, l1.WriteOwnerThreadId);
            Assert.AreEqual(System.Environment.CurrentManagedThreadId, l2.WriteOwnerThreadId);
            Assert.AreEqual(System.Environment.CurrentManagedThreadId, l3.WriteOwnerThreadId);
            Assert.AreEqual(System.Environment.CurrentManagedThreadId, l4.WriteOwnerThreadId);

            all_acquisitions.Dispose();

            Assert.IsFalse(all_acquisitions.IsLockHeld);

            Assert.AreEqual(-1, l1.WriteOwnerThreadId);
            Assert.AreEqual(-1, l2.WriteOwnerThreadId);
            Assert.AreEqual(-1, l3.WriteOwnerThreadId);
            Assert.AreEqual(-1, l4.WriteOwnerThreadId);
        }

        [TestMethod]
        public async Task can_have_async_code_in_critical_section()
        {
            var l = new AsyncLock();

            using (var a = await l.AcquireWriteLockAsync())
            {
                Assert.IsTrue(a.IsLockHeld);

                await Task.Delay(25);

                Assert.IsTrue(a.IsLockHeld);
                Assert.AreNotEqual(-1, l.WriteOwnerThreadId);
                Assert.AreNotEqual(System.Environment.CurrentManagedThreadId, l.WriteOwnerThreadId);
            }
        }
    }
}
