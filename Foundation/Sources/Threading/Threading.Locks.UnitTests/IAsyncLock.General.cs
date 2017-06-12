using System;
using System.Linq;
using SquaredInfinity.Extensions;
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
    public partial class IAsyncLock__General
    {
        [TestMethod]
        public void no_lock_held__sync_lock_acquisition_on_caller_thread()
        {
            foreach (var s in _lock_test_setup.AllAsyncTestSetups.Where(x => !x.IsAsync))
            {
                var l1 = s.GetLock();

                // this should finish right away and synchronously
                // because no other locks are held
                var a = l1.AcquireLock(s.LockType);
                Assert.IsTrue(a.IsLockHeld);
                Assert.AreEqual(s.ExpectedLockState, l1.State);
                Assert.IsTrue(l1.ReadOwners.Where(x => x.CorrelationToken == ThreadCorrelationToken.FromCurrentThread).Any());
            }
        }

        [TestMethod]
        public async Task acquire_lock()
        {
            foreach (var s in _lock_test_setup.AllTestSetups)
            {
                Trace.WriteLine($"EXECUTING: {s.ToString()}");

                if (s.RecursionPolicy == LockRecursionPolicy.SupportsRecursion)
                {
                    if (s.IsAsync)
                    {
                        var l = s.GetLock();

                        var a1 = l.AcquireLock(s.LockType);

                        Assert.IsTrue(a1.IsLockHeld);

                        var a2 = await l.AcquireLockAsync(s.LockType, a1.CorrelationToken);

                        Assert.IsTrue(a1.IsLockHeld);
                        Assert.IsTrue(a2.IsLockHeld);

                        a2.Dispose();

                        Assert.IsTrue(a1.IsLockHeld);
                        Assert.IsFalse(a2.IsLockHeld);

                        a1.Dispose();

                        Assert.IsFalse(a1.IsLockHeld);
                    }
                    else
                    {
                        var l = s.GetLock();
                        using (var a = l.AcquireLock(s.LockType))
                        {
                            Assert.AreEqual(s.ExpectedLockState, l.State);

                            Assert.IsTrue(a.IsLockHeld);
                        }

                        Assert.AreEqual(LockState.NoLock, l.State);
                    }
                }
                else
                {
                    if (s.IsAsync)
                    {
                        var l = s.GetLock();
                        using (var a = await l.AcquireLockAsync(s.LockType))
                        {
                            Assert.AreEqual(s.ExpectedLockState, l.State);

                            Assert.IsTrue(a.IsLockHeld);
                        }

                        Assert.AreEqual(LockState.NoLock, l.State);
                    }
                    else
                    {
                        var l = s.GetLock();
                        using (var a = l.AcquireLock(s.LockType))
                        {
                            Assert.AreEqual(s.ExpectedLockState, l.State);

                            Assert.IsTrue(a.IsLockHeld);
                        }

                        Assert.AreEqual(LockState.NoLock, l.State);
                    }
                }
            }
        }

        [TestMethod]
        public async Task reports_owner()
        {
            foreach (var s in _lock_test_setup.AllTestSetups)
            {
                Trace.WriteLine($"TESTING: {s.ToString()}");

                if (s.RecursionPolicy == LockRecursionPolicy.SupportsRecursion)
                {
                    if (s.IsAsync)
                    {
                        var l = s.GetLock();
                        using (await l.AcquireLockAsync(s.LockType))
                        {
                            // async lock creates random correlation token
                            // we cannot know exactly what it is, but we can check if it's there
                            Assert.AreEqual(1, l.AllOwners.Single().AcquisitionCount);
                            Assert.IsNotNull(l.AllOwners.Single().CorrelationToken);
                        }

                        Assert.AreEqual(0, l.AllOwners.Count);
                    }
                    else
                    {
                        var l = s.GetLock();
                        using (l.AcquireLock(s.LockType))
                        {
                            Assert.AreEqual(ThreadCorrelationToken.FromCurrentThread, l.AllOwners.Single().CorrelationToken);
                        }

                        Assert.AreEqual(0,l.AllOwners.Count);
                    }
                }
                else
                {
                    if (s.IsAsync)
                    {
                        var l = s.GetLock();
                        using (await l.AcquireLockAsync(s.LockType))
                        {
                            // async lock creates random correlation token
                            // we cannot know exactly what it is, but we can check if it's there
                            Assert.AreEqual(1, l.AllOwners.Single().AcquisitionCount);
                            Assert.IsNotNull(l.AllOwners.Single().CorrelationToken);
                        }

                        Assert.AreEqual(0, l.AllOwners.Count);
                    }
                    else
                    {
                        var l = s.GetLock();
                        using (l.AcquireLock(s.LockType))
                        {
                            Assert.AreEqual(ThreadCorrelationToken.FromCurrentThread, l.AllOwners.Single().CorrelationToken);
                        }

                        Assert.AreEqual(0, l.AllOwners.Count);
                    }
                }
            }
        }

        [TestMethod]
        public async Task release_lock()
        {
            foreach (var s in _lock_test_setup.AllTestSetups)
            {
                Trace.WriteLine($"TESTING: {s.ToString()}");

                if (s.RecursionPolicy == LockRecursionPolicy.SupportsRecursion)
                {
                    if (s.IsAsync)
                    {
                        var al = s.GetLock();
                        var l = await al.AcquireLockAsync(s.LockType);

                        Assert.AreEqual(s.ExpectedLockState, al.State);

                        l.Dispose();

                        Assert.AreEqual(LockState.NoLock, al.State);
                    }
                    else
                    {
                        var al = s.GetLock();
                        var l = al.AcquireLock(s.LockType);
                        Assert.AreEqual(s.ExpectedLockState, al.State);
                        l.Dispose();
                        Assert.AreEqual(LockState.NoLock, al.State);
                    }
                }
                else
                {
                    if (s.IsAsync)
                    {
                        var al = s.GetLock();
                        var l = await al.AcquireLockAsync(s.LockType);
                        Assert.AreEqual(s.ExpectedLockState, al.State);
                        l.Dispose();
                        Assert.AreEqual(LockState.NoLock, al.State);
                    }
                    else
                    {
                        var al = s.GetLock();
                        var l = al.AcquireLock(s.LockType);
                        Assert.AreEqual(s.ExpectedLockState, al.State);
                        l.Dispose();
                        Assert.AreEqual(LockState.NoLock, al.State);
                    }
                }
            }
        }

        [TestMethod]
        public async Task only_one_thread_allowed_to_hold_lock()
        {
            foreach (var s in _lock_test_setup.AllTestSetups)
            {
                Trace.WriteLine($"TESTING: {s.ToString()}");

                if (s.RecursionPolicy == LockRecursionPolicy.SupportsRecursion)
                {
                    if (s.IsAsync)
                    {
                        var al = s.GetLock();
                        var l1 = await al.AcquireLockAsync(s.LockType);
                        Assert.AreEqual(s.ExpectedLockState, al.State);

                        var l2 = Task.Factory.StartNewThread(async () => await al.AcquireWriteLockAsync(new AsyncOptions(50))).Result.Result;

                        Assert.IsFalse(l2.IsLockHeld);
                    }
                    else
                    {
                        var al = s.GetLock();
                        var l = al.AcquireLock(s.LockType);
                        Assert.AreEqual(s.ExpectedLockState, al.State);

                        var l2 = Task.Factory.StartNewThread(() => al.AcquireWriteLock(new SyncOptions(50))).Result;
                        Assert.IsFalse(l2.IsLockHeld);
                    }
                }
                else
                {
                    if (s.IsAsync)
                    {
                        var al = s.GetLock();
                        var l = await al.AcquireLockAsync(s.LockType);
                        Assert.AreEqual(s.ExpectedLockState, al.State);

                        var l2 = Task.Factory.StartNewThread(() => al.AcquireWriteLock(new SyncOptions(50))).Result;
                        Assert.IsFalse(l2.IsLockHeld);
                    }
                    else
                    {
                        var al = s.GetLock();
                        var l = al.AcquireLock(s.LockType);
                        Assert.AreEqual(s.ExpectedLockState, al.State);

                        var l2 = Task.Factory.StartNewThread(() => al.AcquireWriteLock(new SyncOptions(50))).Result;
                        Assert.IsFalse(l2.IsLockHeld);
                    }
                }
            }
        }

        [TestMethod]
        public async Task allows_async_code_in_critical_section()
        {
            foreach (var s in _lock_test_setup.AllTestSetups)
            {                
                Trace.WriteLine($"TESTING: {s.ToString()}");

                if (s.RecursionPolicy == LockRecursionPolicy.SupportsRecursion)
                {
                    if (s.IsAsync)
                    {
                        var al = s.GetLock();

                        using (await al.AcquireLockAsync(s.LockType))
                        {
                            Assert.AreEqual(s.ExpectedLockState, al.State);

                            await Task.Delay(25);

                            Assert.AreEqual(s.ExpectedLockState, al.State);
                        }                        
                    }
                    else
                    {
                        var al = s.GetLock();
                        using (al.AcquireLock(s.LockType))
                        {
                            Assert.AreEqual(s.ExpectedLockState, al.State);

                            await Task.Delay(25);

                            Assert.AreEqual(s.ExpectedLockState, al.State);
                        }
                    }
                }
                else
                {
                    if (s.IsAsync)
                    {
                        var al = s.GetLock();
                        using (await al.AcquireLockAsync(s.LockType))
                        {
                            Assert.AreEqual(s.ExpectedLockState, al.State, s.ToString());

                            await Task.Delay(25);

                            Assert.AreEqual(s.ExpectedLockState, al.State, s.ToString());
                        }
                    }
                    else
                    {
                        var al = s.GetLock();
                        using (al.AcquireLock(s.LockType))
                        {
                            Assert.AreEqual(s.ExpectedLockState, al.State);

                            await Task.Delay(25);

                            Assert.AreEqual(s.ExpectedLockState, al.State);
                        }
                    }
                }
            }
        }

        [TestMethod]
        public async Task different_thread_tries_to_acquire_taken_write_lock()
        {
            foreach (var s in _lock_test_setup.AllTestSetups)
            {
                Trace.WriteLine($"TESTING: {s.ToString()}");

                if (s.RecursionPolicy == LockRecursionPolicy.SupportsRecursion)
                {
                    if (s.IsAsync)
                    {
                        var al = s.GetLock();
                        using (await al.AcquireLockAsync(s.LockType))
                        {
                            Assert.AreEqual(s.ExpectedLockState, al.State);

                            var l2 = Task.Factory.StartNewThread(() => al.AcquireLock(s.LockType, new SyncOptions(50))).Result;

                            if (al is AsyncLock)
                            {
                                Assert.IsFalse(l2.IsLockHeld, s.ToString()); // can't hold another write
                            }
                            else
                            {
                                if (s.LockType == LockType.Read)
                                    Assert.IsTrue(l2.IsLockHeld, s.ToString()); // can hold another read
                                else
                                    Assert.IsFalse(l2.IsLockHeld, s.ToString()); // but can't hold another write
                            }

                            Assert.AreEqual(s.ExpectedLockState, al.State);
                        }
                    }
                    else
                    {
                        var al = s.GetLock();
                        using (al.AcquireLock(s.LockType))
                        {
                            Assert.AreEqual(s.ExpectedLockState, al.State);

                            var l2 = Task.Factory.StartNewThread(() => al.AcquireLock(s.LockType, new SyncOptions(50))).Result;

                            if (al is AsyncLock)
                            {
                                Assert.IsFalse(l2.IsLockHeld, s.ToString()); // can't hold another write
                            }
                            else
                            {
                                if (s.LockType == LockType.Read)
                                    Assert.IsTrue(l2.IsLockHeld, s.ToString()); // can hold another read
                                else
                                    Assert.IsFalse(l2.IsLockHeld, s.ToString()); // but can't hold another write
                            }

                            Assert.AreEqual(s.ExpectedLockState, al.State);
                        }
                    }
                }
                else
                {
                    if (s.IsAsync)
                    {
                        var al = s.GetLock();
                        using (await al.AcquireLockAsync(s.LockType))
                        {
                            Assert.AreEqual(s.ExpectedLockState, al.State);

                            var l2 = await Task.Factory.StartNewThread(async () => await al.AcquireLockAsync(s.LockType, new AsyncOptions(50))).Result;

                            if (al is AsyncLock)
                            {
                                Assert.IsFalse(l2.IsLockHeld, s.ToString()); // can't hold another write
                            }
                            else
                            {
                                if (s.LockType == LockType.Read)
                                    Assert.IsTrue(l2.IsLockHeld, s.ToString()); // can hold another read
                                else
                                    Assert.IsFalse(l2.IsLockHeld, s.ToString()); // but can't hold another write
                            }

                            Assert.AreEqual(s.ExpectedLockState, al.State);

                            l2.Dispose();
                        }

                        Assert.AreEqual(LockState.NoLock, al.State);
                    }
                    else
                    {
                        var al = s.GetLock();
                        using (al.AcquireLock(s.LockType))
                        {
                            Assert.AreEqual(s.ExpectedLockState, al.State);

                            var l2 = await Task.Factory.StartNewThread(() => al.AcquireLock(s.LockType, new SyncOptions(50)));

                            if (al is AsyncLock)
                            {
                                Assert.IsFalse(l2.IsLockHeld, s.ToString()); // can't hold another write
                            }
                            else
                            {
                                if (s.LockType == LockType.Read)
                                    Assert.IsTrue(l2.IsLockHeld, s.ToString()); // can hold another read
                                else
                                    Assert.IsFalse(l2.IsLockHeld, s.ToString()); // but can't hold another write
                            }

                            Assert.AreEqual(s.ExpectedLockState, al.State);
                        }
                    }
                }
            }
        }

        [TestMethod]
        public async Task same_thread_tries_to_acquire_taken_lock()
        {
            foreach (var s in _lock_test_setup.AllTestSetups)
            {
                Trace.WriteLine($"TESTING: {s.ToString()}"); 

                if (s.RecursionPolicy == LockRecursionPolicy.SupportsRecursion)
                {
                    if (s.IsAsync)
                    {
                        var al = s.GetLock();
                        using (await al.AcquireLockAsync(s.LockType, AsyncOptions.OnCapturedContext))
                        {
                            Assert.AreEqual(s.ExpectedLockState, al.State);

                            var l2 = await al.AcquireLockAsync(s.LockType, new AsyncOptions(50, true));

                            if (al is AsyncLock)
                            {
                                Assert.IsFalse(l2.IsLockHeld, s.ToString()); // can't hold another write
                            }
                            else
                            {
                                if (s.LockType == LockType.Read)
                                    Assert.IsTrue(l2.IsLockHeld, s.ToString()); // can hold another read
                                else
                                    Assert.IsFalse(l2.IsLockHeld, s.ToString()); // but can't hold another write
                            }

                            Assert.AreEqual(s.ExpectedLockState, al.State);
                        }
                    }
                    else
                    {
                        var al = s.GetLock();
                        using (al.AcquireLock(s.LockType))
                        {
                            Assert.AreEqual(s.ExpectedLockState, al.State);

                            var l2 = al.AcquireLock(s.LockType, new SyncOptions(50));

                            // recursive lock granted
                            Assert.IsTrue(l2.IsLockHeld, s.ToString());
                            Assert.AreEqual(s.ExpectedLockState, al.State);
                        }
                    }
                }
                else
                {
                    if (s.IsAsync)
                    {
                        // Async Locks are not tied to a specific thread
                        // so they do not check for recursion
                        // recursion only applies to synchronous locks

                        var al = s.GetLock();
                        using (await al.AcquireLockAsync(s.LockType, AsyncOptions.OnCapturedContext))
                        {
                            Assert.AreEqual(s.ExpectedLockState, al.State);

                            var l2 = await al.AcquireLockAsync(s.LockType, new AsyncOptions(50, true));

                            if (al is AsyncLock)
                            {
                                Assert.IsFalse(l2.IsLockHeld, s.ToString()); // can't hold another write
                            }
                            else
                            {
                                if (s.LockType == LockType.Read)
                                    Assert.IsTrue(l2.IsLockHeld, s.ToString()); // can hold another read
                                else
                                    Assert.IsFalse(l2.IsLockHeld, s.ToString()); // but can't hold another write
                            }

                            Assert.AreEqual(s.ExpectedLockState, al.State);
                        }
                    }
                    else
                    {
                        var al = s.GetLock();
                        using (al.AcquireLock(s.LockType))
                        {
                            Assert.AreEqual(s.ExpectedLockState, al.State);

                            Trace.WriteLine(Environment.CurrentManagedThreadId);
                            
                            try
                            {
                                al.AcquireLock(s.LockType, new SyncOptions(50));
                                Assert.Fail("should have thrown");
                            }
                            catch(LockRecursionException) { }
                        }
                    }
                }
            }
        }
    }
}
