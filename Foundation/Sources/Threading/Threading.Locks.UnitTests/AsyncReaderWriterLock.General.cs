using System;
using SquaredInfinity.Extensions;
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

            Assert.AreEqual(LockState.NoLock, l.State);

            // write
            using (l.AcquireWriteLock())
                Assert.AreEqual(LockState.Write, l.State);

            Assert.AreEqual(LockState.NoLock, l.State);

            // write async
            using (await l.AcquireWriteLockAsync())
                Assert.AreEqual(LockState.Write, l.State);

            Assert.AreEqual(LockState.NoLock, l.State);

            // read
            using (l.AcquireReadLock())
                Assert.AreEqual(LockState.Read, l.State);

            Assert.AreEqual(LockState.NoLock, l.State);

            // read async
            using (await l.AcquireReadLockAsync())
                Assert.AreEqual(LockState.Read, l.State);

            Assert.AreEqual(LockState.NoLock, l.State);
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

                if (!cd.Wait(250))
                    Assert.Fail($"Waiters were not allowed in. Remaining waiters: {cd.CurrentCount}");

                if (cd.CurrentCount != 0)
                    Assert.Fail("Not all Waiters were allowed in");
            }
        }

        [TestMethod]
        public void writer_owns_lock__acquisition_request_waits__writer_disposed__acquisition_request_gets_lock()
        {
            // Writer (W1) owns a lock
            // there is a waiting reader or writer (A1)
            // once Writer (W1) releases the lock, waiting reader or writer (A1) gets it

            foreach (var s in _lock_test_setup.AllAsyncTestSetups.Where(x => x.GetLock() is AsyncReaderWriterLock))
            {
                Trace.WriteLine($"TESTING: {s}");

                var l = s.GetLock();
                var aw = l.AcquireWriteLock();

                bool all_ok = false;

                var task = (Task)null;

                if (s.IsAsync)
                {
                    task =
                    Task.Factory.StartNewThread(async () =>
                    {
                        using (var ar = await l.AcquireLockAsync(s.LockType, AsyncOptions.OnCapturedContext))
                        {
                            Assert.IsTrue(ar.IsLockHeld);
                            Assert.IsTrue(l.AllOwners.Any());
                            Assert.AreEqual(s.ExpectedLockState, l.State);

                            all_ok = true;
                        }
                    });
                }
                else
                {
                    task =
                    Task.Factory.StartNewThread(() =>
                    {
                        using (var ar = l.AcquireLock(s.LockType))
                        {
                            Assert.IsTrue(ar.IsLockHeld);
                            Assert.IsTrue(l.AllOwners.Where(x => x.CorrelationToken == ThreadCorrelationToken.FromCurrentThread).Any());
                            Assert.AreEqual(s.ExpectedLockState, l.State);

                            all_ok = true;
                        }
                    });
                }

                // wait so that read lock can have time to be queued
                Thread.Sleep(25);

                // release write lock so read lock can be acquired
                aw.Dispose();

                task.Wait();

                Thread.Sleep(25);

                Assert.IsTrue(all_ok);
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
                    Task.Factory.StartNew((Func<Task>)(async () =>
                    {
                        if (lt == LockType.Read)
                        {
                            using (var ar = await l.AcquireReadLockAsync())
                            {
                                Assert.IsTrue(ar.IsLockHeld);
                                Assert.IsTrue(l.ReadOwners.Where(x => x.CorrelationToken == ThreadCorrelationToken.FromCurrentThread).Any());
                                Assert.AreEqual(LockState.Read, l.State);
                                
                                Assert.AreEqual(-1, l.WriteOwner);
                            }
                        }
                        else
                        if (lt == LockType.Write)
                        {
                            using (var ar = await l.AcquireWriteLockAsync())
                            {
                                Assert.IsTrue(ar.IsLockHeld);
                                Assert.IsTrue(l.WriteOwner.CorrelationToken == ThreadCorrelationToken.FromCurrentThread);
                                Assert.AreEqual(LockState.Write, l.State);
                                
                                Assert.AreEqual(0, l.ReadOwners.Count);
                            }
                        }
                        else
                        {
                            throw new NotImplementedException();
                        }
                    }));
                    
                    // release write lock so read lock can be acquired
                    aw.Dispose();

                    task.Wait();
                }
            }
        }
    }
}
