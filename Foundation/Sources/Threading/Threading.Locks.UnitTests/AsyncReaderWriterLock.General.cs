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
    public class AsyncReaderWriterLock__General
    {
        [TestMethod]
        public async Task lock_acquired_and_released__state_changes()
        {
            var l = new AsyncReaderWriterLock();

            Assert.AreEqual(ReaderWriterLockState.NoLock, l.State);

            // write
            using (l.AcquireWriteLock())
                Assert.AreEqual(ReaderWriterLockState.Write, l.State);

            Assert.AreEqual(ReaderWriterLockState.NoLock, l.State);

            // write async
            using (await l.AcquireWriteLockAsync())
                Assert.AreEqual(ReaderWriterLockState.Write, l.State);

            Assert.AreEqual(ReaderWriterLockState.NoLock, l.State);

            // read
            using (l.AcquireReadLock())
                Assert.AreEqual(ReaderWriterLockState.Read, l.State);

            Assert.AreEqual(ReaderWriterLockState.NoLock, l.State);

            // read async
            using (await l.AcquireReadLockAsync())
                Assert.AreEqual(ReaderWriterLockState.Read, l.State);

            Assert.AreEqual(ReaderWriterLockState.NoLock, l.State);
        }

        [TestMethod]
        public async Task cancellation_throws_operation_canceled_exception()
        {
            var l = new AsyncReaderWriterLock();

            bool got_exception = false;
            try { l.AcquireReadLock(new CancellationTokenSource(0).Token); }
            catch (OperationCanceledException) { got_exception = true; }
            if (!got_exception) Assert.Fail("cancellation exception not thrown");

            got_exception = false;
            try { l.AcquireWriteLock(new CancellationTokenSource(0).Token); }
            catch (OperationCanceledException) { got_exception = true; }
            if (!got_exception) Assert.Fail("cancellation exception not thrown");

            got_exception = false;
            try { await l.AcquireReadLockAsync(new CancellationTokenSource(0).Token); }
            catch (OperationCanceledException) { got_exception = true; }
            if (!got_exception) Assert.Fail("cancellation exception not thrown");

            got_exception = false;
            try { await l.AcquireWriteLockAsync(new CancellationTokenSource(0).Token); }
            catch (OperationCanceledException) { got_exception = true; }
            if (!got_exception) Assert.Fail("cancellation exception not thrown");
        }

        #region Can Acquire Lock

        [TestMethod]
        public void can_acquire_lock()
        {
            foreach (var lt in new[] { LockType.Read, LockType.Write })
            {
                foreach (var rp in new[] { LockRecursionPolicy.NoRecursion, LockRecursionPolicy.SupportsRecursion })
                {
                    var l1 = new AsyncReaderWriterLock(rp);
                    var l2 = new AsyncReaderWriterLock(rp);
                    var l3 = new AsyncReaderWriterLock(rp);
                    var l4 = new AsyncReaderWriterLock(rp);

                    var all_acquisitions = (ILockAcquisition)null;

                    if (lt == LockType.Read)
                        all_acquisitions = AsyncLock.AcquireReadLock(l1, l2, l3, l4);
                    else if (lt == LockType.Write)
                        all_acquisitions = AsyncLock.AcquireWriteLock(l1, l2, l3, l4);
                    else
                        Assert.Fail();


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

        [TestMethod]
        public async Task can_acquire_lock_async()
        {
            foreach (var lt in new[] { LockType.Read, LockType.Write })
            {
                foreach (var rp in new[] { LockRecursionPolicy.NoRecursion, LockRecursionPolicy.SupportsRecursion })
                {
                    var l1 = new AsyncReaderWriterLock(rp);
                    var l2 = new AsyncReaderWriterLock(rp);
                    var l3 = new AsyncReaderWriterLock(rp);
                    var l4 = new AsyncReaderWriterLock(rp);

                    var all_acquisitions = (ILockAcquisition)null;

                    if (lt == LockType.Read)
                        all_acquisitions = await AsyncLock.AcquireReadLockAsync(l1, l2, l3, l4);
                    else if (lt == LockType.Write)
                        all_acquisitions = await AsyncLock.AcquireWriteLockAsync(l1, l2, l3, l4);
                    else
                        Assert.Fail();

                    Assert.IsTrue(all_acquisitions.IsLockHeld);

                    Assert.AreEqual(-1, l1.WriteOwnerThreadId);
                    Assert.AreEqual(-1, l2.WriteOwnerThreadId);
                    Assert.AreEqual(-1, l3.WriteOwnerThreadId);
                    Assert.AreEqual(-1, l4.WriteOwnerThreadId);

                    Assert.IsTrue(l1.ReadOwnerThreadIds.Contains(Environment.CurrentManagedThreadId));
                    Assert.IsTrue(l2.ReadOwnerThreadIds.Contains(Environment.CurrentManagedThreadId));
                    Assert.IsTrue(l3.ReadOwnerThreadIds.Contains(Environment.CurrentManagedThreadId));
                    Assert.IsTrue(l4.ReadOwnerThreadIds.Contains(Environment.CurrentManagedThreadId));

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

        #endregion



        
        #region Lock Acquisition on Caller Thread

        [TestMethod]
        public void lock_acquisition_on_caller_thread()
        {
            foreach (var rp in new[] { LockRecursionPolicy.NoRecursion, LockRecursionPolicy.SupportsRecursion })
            {
                var l1 = new AsyncReaderWriterLock(recursionPolicy: LockRecursionPolicy.NoRecursion);

                var a = l1.AcquireReadLock();

                Assert.IsTrue(a.IsLockHeld);

                Assert.IsTrue(l1.ReadOwnerThreadIds.Contains(System.Environment.CurrentManagedThreadId));
            }
        }

        #endregion
    }
}
