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
        public async Task can_acquire_read_lock_async()
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

        [TestMethod]
        public void can_acquire_read_lock()
        {
            var l1 = new AsyncReaderWriterLock(recursionPolicy: LockRecursionPolicy.NoRecursion);
            var l2 = new AsyncReaderWriterLock(recursionPolicy: LockRecursionPolicy.NoRecursion);
            var l3 = new AsyncReaderWriterLock(recursionPolicy: LockRecursionPolicy.NoRecursion);
            var l4 = new AsyncReaderWriterLock(recursionPolicy: LockRecursionPolicy.NoRecursion);

            var all_acquisitions = AsyncLock.AcquireReadLock(l1, l2, l3, l4);


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

        [TestMethod]
        public void can_acquire_write_lock()
        {
            var l1 = new AsyncReaderWriterLock(recursionPolicy: LockRecursionPolicy.NoRecursion);

            var a = l1.AcquireWriteLock();

            Assert.IsTrue(a.IsLockHeld);

            Assert.AreEqual(Environment.CurrentManagedThreadId, l1.WriteOwnerThreadId);

            Assert.AreEqual(0, l1.ReadOwnerThreadIds.Count);

            a.Dispose();

            Assert.IsFalse(a.IsLockHeld);

            Assert.AreEqual(-1, l1.WriteOwnerThreadId);

            Assert.AreEqual(0, l1.ReadOwnerThreadIds.Count);
        }

        [TestMethod]
        public async Task can_acquire_write_lock_async()
        {
            var l1 = new AsyncReaderWriterLock(recursionPolicy: LockRecursionPolicy.NoRecursion);

            var a = await l1.AcquireWriteLockAsync();

            Assert.IsTrue(a.IsLockHeld);

            Assert.AreEqual(Environment.CurrentManagedThreadId, l1.WriteOwnerThreadId);

            Assert.AreEqual(0, l1.ReadOwnerThreadIds.Count);

            a.Dispose();

            Assert.IsFalse(a.IsLockHeld);

            Assert.AreEqual(-1, l1.WriteOwnerThreadId);

            Assert.AreEqual(0, l1.ReadOwnerThreadIds.Count);
        }

        [TestMethod]
        public void lock_acquisition_on_caller_thread()
        {
            var l1 = new AsyncReaderWriterLock(recursionPolicy: LockRecursionPolicy.NoRecursion);

            var a = l1.AcquireReadLock();

            Assert.IsTrue(a.IsLockHeld);

            Assert.IsTrue(l1.ReadOwnerThreadIds.Contains(System.Environment.CurrentManagedThreadId));
        }

        [TestMethod]
        public async Task no_lock_held_async_lock_acquisition_on_caller_thread()
        {
            var l1 = new AsyncReaderWriterLock(recursionPolicy: LockRecursionPolicy.NoRecursion);

            var tid = System.Environment.CurrentManagedThreadId;

            // this should finish right away and synchronously
            // because no other locks are held
            var a = await l1.AcquireReadLockAsync();

            Assert.IsTrue(a.IsLockHeld);
            Assert.IsTrue(l1.ReadOwnerThreadIds.Contains(tid));
        }


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

        [TestMethod]
        public async Task writer_held__readers_wait()
        {
            var l = new AsyncReaderWriterLock(recursionPolicy: LockRecursionPolicy.NoRecursion);

            using (var a1 = l.AcquireWriteLock())
            {
                Assert.IsTrue(a1.IsLockHeld);

                var r = Task.Factory.StartNew(() => l.AcquireReadLock(new SyncOptions(25))).Result;

                Assert.IsFalse(r.IsLockHeld);
            }
        }

        [TestMethod]
        public async Task writer_held__next_writer_on_different_thread_fails()
        {
            var l = new AsyncReaderWriterLock(recursionPolicy: LockRecursionPolicy.NoRecursion);

            using (var a1 = l.AcquireWriteLock())
            {
                Assert.IsTrue(a1.IsLockHeld);

                var r = Task.Factory.StartNew(() => l.AcquireWriteLock(new SyncOptions(25))).Result;

                Assert.IsFalse(r.IsLockHeld);
            }
        }

        [TestMethod]
        public void writer_finishes_all_waiting_readers_get_locks()
        {
            var l = new AsyncReaderWriterLock(recursionPolicy: LockRecursionPolicy.NoRecursion);

            var a = l.AcquireWriteLock();

            var cd = new CountdownEvent(2);

            Task.Factory.StartNew(() =>
            {
                using (l.AcquireReadLock())
                {
                    cd.Signal();
                }
            });

            Task.Factory.StartNew(() =>
            {
                using (l.AcquireReadLock())
                {
                    cd.Signal();
                }
            });

            if (cd.Wait(25))
                Assert.Fail("Waiters were allowed in");

            a.Dispose();

            if (!cd.Wait(25))
                Assert.Fail("Waiters were not allowed in");

            if(cd.CurrentCount != 0)
                Assert.Fail("Not all Waiters were allowed in");

        }
    }
}
