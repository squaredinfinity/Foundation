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

        [TestMethod]
        void writer_owns_lock__reader_or_writer_waits__reader_or_writer_gets_lock()
        {
            // Writer owns a lock
            // there is a waiting reader or writer
            // once Writer releases the lock, waiting reader or writer gets it

            foreach (var lock_type in new[] { LockType.Read, LockType.Write })
            {
                var l = new AsyncReaderWriterLock(recursionPolicy: LockRecursionPolicy.NoRecursion);

                var aw = l.AcquireWriteLock();

                var task =
                Task.Factory.StartNew(() =>
                {
                    if (lock_type == LockType.Read)
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
                    if(lock_type == LockType.Write)
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

        [TestMethod]
        public void writer_owns_lock__async_reader_or_writer_waits__reader_or_writer_gets_lock()
        {
            // when waiting reader is finally given a lock
            // correct thread should be reported as read_owner

            foreach (var lock_type in new[] { LockType.Read, LockType.Write })
            {
                var l = new AsyncReaderWriterLock(recursionPolicy: LockRecursionPolicy.NoRecursion);

                var aw = l.AcquireWriteLock();

                var task =
                Task.Factory.StartNew(async () =>
                {
                    if (lock_type == LockType.Read)
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
                    if (lock_type == LockType.Write)
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

                // wait so that read lock can have time to be queued
                Thread.Sleep(25);

                // release write lock so read lock can be acquired
                aw.Dispose();

                task.Wait();
            }
        }
    }
}
