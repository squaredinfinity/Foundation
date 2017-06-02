using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SquaredInfinity.Threading;
using System.Threading.Tasks;
using System.Threading;
using SquaredInfinity.Threading.Locks;
using System.Collections.Generic;
using System.Diagnostics;

namespace Threading.Locks.UnitTests
{
    [TestClass]
    public class AsyncReaderWriterLock__General
    {
        IReadOnlyList<LockType> GetLockTypes() => new[] { LockType.Read, LockType.Write };
        IReadOnlyList<LockRecursionPolicy> GetRecursionPolicies() => new[] { LockRecursionPolicy.NoRecursion, LockRecursionPolicy.SupportsRecursion };

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
            try { l.AcquireReadLock(new CancellationToken(canceled: true)); }
            catch (OperationCanceledException) { got_exception = true; }
            if (!got_exception) Assert.Fail("cancellation exception not thrown");

            got_exception = false;
            try { l.AcquireWriteLock(new CancellationToken(canceled: true)); }
            catch (OperationCanceledException) { got_exception = true; }
            if (!got_exception) Assert.Fail("cancellation exception not thrown");

            got_exception = false;
            try { await l.AcquireReadLockAsync(new CancellationToken(canceled: true)); }
            catch (OperationCanceledException) { got_exception = true; }
            if (!got_exception) Assert.Fail("cancellation exception not thrown");

            got_exception = false;
            try { await l.AcquireWriteLockAsync(new CancellationToken(canceled: true)); }
            catch (OperationCanceledException) { got_exception = true; }
            if (!got_exception) Assert.Fail("cancellation exception not thrown");
        }

        [TestMethod]
        public void writer_held__next_writer_on_different_thread_fails()
        {
            foreach (var rp in GetRecursionPolicies())
            {
                var l = new AsyncReaderWriterLock(rp);

                using (var a1 = l.AcquireWriteLock())
                {
                    Assert.IsTrue(a1.IsLockHeld);

                    var r = Task.Factory.StartNew(() => l.AcquireWriteLock(25)).Result;
                    Assert.IsFalse(r.IsLockHeld);

                    r = Task.Factory.StartNew(async () => await l.AcquireWriteLockAsync(25)).Result.Result;
                    Assert.IsFalse(r.IsLockHeld);
                }
            }
        }

        [TestMethod]
        public void writer_finishes_all_waiting_readers_get_locks()
        {
            foreach (var rp in GetRecursionPolicies())
            {
                var l = new AsyncReaderWriterLock(rp);

                var a = l.AcquireWriteLock();

                var cd = new CountdownEvent(2);

                Task.Factory.StartNew(() =>
                {
                    using (l.AcquireReadLock())
                    {
                        cd.Signal();
                    }
                });

                Task.Factory.StartNew(async () =>
                {
                    using (await l.AcquireReadLockAsync())
                    {
                        cd.Signal();
                    }
                });

                if (cd.Wait(25))
                    Assert.Fail("Waiters were allowed in");

                a.Dispose();

                if (!cd.Wait(25))
                    Assert.Fail("Waiters were not allowed in");

                if (cd.CurrentCount != 0)
                    Assert.Fail("Not all Waiters were allowed in");
            }
        }

        [TestMethod]
        public void writer_owns_lock__reader_or_writer_waits__reader_or_writer_gets_lock()
        {
            // Writer owns a lock
            // there is a waiting reader or writer
            // once Writer releases the lock, waiting reader or writer gets it

            foreach (var lt in GetLockTypes())
            {
                foreach (var rp in GetRecursionPolicies())
                {
                    Trace.WriteLine($"TESTING: {lt}, {rp}");

                    var l = new AsyncReaderWriterLock(rp);

                    var aw = l.AcquireWriteLock();

                    var task =
                    Task.Factory.StartNew(() =>
                    {
                        if (lt == LockType.Read)
                        {
                            using (var ar = l.AcquireReadLock())
                            {
                                Assert.IsTrue(ar.IsLockHeld);
                                Assert.IsTrue(l.IsReadLockHeld);
                                Assert.IsTrue(l.ReadOwnerThreadIds.Contains(Environment.CurrentManagedThreadId));
                                Assert.AreEqual(ReaderWriterLockState.Read, l.State);

                                Assert.IsFalse(l.IsWriteLockHeld);
                                Assert.AreEqual(-1, l.WriteOwnerThreadId);
                            }
                        }
                        else
                        if (lt == LockType.Write)
                        {
                            using (var ar = l.AcquireWriteLock())
                            {
                                Assert.IsTrue(ar.IsLockHeld);
                                Assert.IsTrue(l.IsWriteLockHeld);
                                Assert.IsTrue(l.WriteOwnerThreadId == Environment.CurrentManagedThreadId);
                                Assert.AreEqual(ReaderWriterLockState.Write, l.State);

                                Assert.IsFalse(l.IsReadLockHeld);
                                Assert.AreEqual(0, l.ReadOwnerThreadIds.Count);
                            }
                        }
                        else
                        {
                            throw new NotImplementedException();
                        }
                    });

                    // wait so that read lock can have time to be queued
                    Thread.Sleep(25);

                    // release write lock so read lock can be acquired
                    aw.Dispose();

                    task.Wait();
                }
            }
        }

        [TestMethod]
        public void writer_owns_lock__async_reader_or_writer_waits__reader_or_writer_gets_lock()
        {
            // when waiting reader is finally given a lock
            // correct thread should be reported as read_owner

            foreach (var lt in GetLockTypes())
            {
                foreach (var rp in GetRecursionPolicies())
                {
                    var l = new AsyncReaderWriterLock(rp);

                    var aw = l.AcquireWriteLock();

                    var task =
                    Task.Factory.StartNew(async () =>
                    {
                        if (lt == LockType.Read)
                        {
                            using (var ar = await l.AcquireReadLockAsync())
                            {
                                Assert.IsTrue(ar.IsLockHeld);
                                Assert.IsTrue(l.IsReadLockHeld);
                                Assert.IsTrue(l.ReadOwnerThreadIds.Contains(Environment.CurrentManagedThreadId));
                                Assert.AreEqual(ReaderWriterLockState.Read, l.State);

                                Assert.IsFalse(l.IsWriteLockHeld);
                                Assert.AreEqual(-1, l.WriteOwnerThreadId);
                            }
                        }
                        else
                        if (lt == LockType.Write)
                        {
                            using (var ar = await l.AcquireWriteLockAsync())
                            {
                                Assert.IsTrue(ar.IsLockHeld);
                                Assert.IsTrue(l.IsWriteLockHeld);
                                Assert.IsTrue(l.WriteOwnerThreadId == Environment.CurrentManagedThreadId);
                                Assert.AreEqual(ReaderWriterLockState.Write, l.State);

                                Assert.IsFalse(l.IsReadLockHeld);
                                Assert.AreEqual(0, l.ReadOwnerThreadIds.Count);
                            }
                        }
                        else
                        {
                            throw new NotImplementedException();
                        }
                    });
                    
                    // release write lock so read lock can be acquired
                    aw.Dispose();

                    task.Wait();
                }
            }
        }

        #region No Lock Held, Lock Acquired On Caller Thread

        [TestMethod]
        public void no_lock_held_lock_acquisition_on_caller_thread()
        {
            foreach (var lt in new[] { LockType.Read, LockType.Write })
            {
                foreach (var rp in new[] { LockRecursionPolicy.NoRecursion, LockRecursionPolicy.SupportsRecursion })
                {
                    var l1 = new AsyncReaderWriterLock(rp);

                    var tid = System.Environment.CurrentManagedThreadId;

                    if (lt == LockType.Read)
                    {
                        // this should finish right away and synchronously
                        // because no other locks are held
                        var a = l1.AcquireReadLock();
                        Assert.IsTrue(a.IsLockHeld);
                        Assert.IsTrue(l1.ReadOwnerThreadIds.Contains(tid));
                    }
                    else if (lt == LockType.Write)
                    {
                        // this should finish right away and synchronously
                        // because no other locks are held
                        var a = l1.AcquireWriteLock();
                        Assert.IsTrue(a.IsLockHeld);
                        Assert.AreEqual(tid, l1.WriteOwnerThreadId);
                    }
                    else
                        Assert.Fail();
                }
            }
        }

        [TestMethod]
        public async Task no_lock_held_async_lock_acquisition_on_caller_thread()
        {
            foreach (var lt in new[] { LockType.Read, LockType.Write })
            {
                foreach (var rp in new[] { LockRecursionPolicy.NoRecursion, LockRecursionPolicy.SupportsRecursion })
                {
                    var l1 = new AsyncReaderWriterLock(rp);

                    var tid = System.Environment.CurrentManagedThreadId;

                    if (lt == LockType.Read)
                    {
                        // this should finish right away and synchronously
                        // because no other locks are held
                        var a = await l1.AcquireReadLockAsync();
                        Assert.IsTrue(a.IsLockHeld);
                        Assert.IsTrue(l1.ReadOwnerThreadIds.Contains(tid));
                    }
                    else if (lt == LockType.Write)
                    {
                        // this should finish right away and synchronously
                        // because no other locks are held
                        var a = await l1.AcquireWriteLockAsync();
                        Assert.IsTrue(a.IsLockHeld);
                        Assert.AreEqual(tid, l1.WriteOwnerThreadId);
                    }
                    else
                        Assert.Fail();
                }
            }
        }

        #endregion

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
        public async Task can_acquire_read_lock_async()
        {
            foreach (var rp in new[] { LockRecursionPolicy.NoRecursion, LockRecursionPolicy.SupportsRecursion })
            {
                var l1 = new AsyncReaderWriterLock(rp);
                var l2 = new AsyncReaderWriterLock(rp);
                var l3 = new AsyncReaderWriterLock(rp);
                var l4 = new AsyncReaderWriterLock(rp);

                var all_acquisitions = await AsyncLock.AcquireReadLockAsync(l1, l2, l3, l4);

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

        #endregion
                
        #region Lock Acquisition on Caller Thread

        [TestMethod]
        public void lock_acquisition_on_caller_thread()
        {
            foreach (var lt in new[] { LockType.Read, LockType.Write })
            {
                foreach (var rp in new[] { LockRecursionPolicy.NoRecursion, LockRecursionPolicy.SupportsRecursion })
                {
                    var l1 = new AsyncReaderWriterLock(rp);

                    if (lt == LockType.Read)
                    {
                        var a = l1.AcquireReadLock();
                        Assert.IsTrue(a.IsLockHeld);
                        Assert.IsTrue(l1.ReadOwnerThreadIds.Contains(Environment.CurrentManagedThreadId));
                    }
                    else if (lt == LockType.Write)
                    {
                        var a = l1.AcquireWriteLock();
                        Assert.IsTrue(a.IsLockHeld);
                        Assert.AreEqual(Environment.CurrentManagedThreadId, l1.WriteOwnerThreadId);
                    }
                    else
                    {
                        Assert.Fail();
                    }
                }
            }
        }

        #endregion

        #region Write Held, Readers Wait

        [TestMethod]
        public void write_held__readers_wait()
        {
            foreach (var rp in new[] { LockRecursionPolicy.NoRecursion, LockRecursionPolicy.SupportsRecursion })
            {
                var l = new AsyncReaderWriterLock(rp);

                using (var a1 = l.AcquireWriteLock())
                {
                    Assert.IsTrue(a1.IsLockHeld);
                    var r = Task.Factory.StartNew(() => l.AcquireReadLock(25)).Result;
                    Assert.IsFalse(r.IsLockHeld);
                }
            }
        }

        [TestMethod]
        public void write_held__async_readers_wait()
        {
            foreach (var rp in new[] { LockRecursionPolicy.NoRecursion, LockRecursionPolicy.SupportsRecursion })
            {
                var l = new AsyncReaderWriterLock(rp);

                using (var a1 = l.AcquireWriteLock())
                {
                    Assert.IsTrue(a1.IsLockHeld);
                    var r = Task.Factory.StartNew(async () => await l.AcquireReadLockAsync(25)).Result.Result;
                    Assert.IsFalse(r.IsLockHeld);
                }
            }
        }

        #endregion
    }
}
