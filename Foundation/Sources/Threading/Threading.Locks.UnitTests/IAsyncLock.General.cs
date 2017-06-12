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
                            if (l is AsyncLock) Assert.AreEqual((int)LockType.Write, (int)l.State);
                            else Assert.AreEqual((int)s.LockType, (int)l.State);

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
                            if (l is AsyncLock) Assert.AreEqual((int)LockType.Write, (int)l.State);
                            else Assert.AreEqual((int)s.LockType, (int)l.State);

                            Assert.IsTrue(a.IsLockHeld);
                        }

                        Assert.AreEqual(LockState.NoLock, l.State);
                    }
                    else
                    {
                        var l = s.GetLock();
                        using (var a = l.AcquireLock(s.LockType))
                        {
                            if (l is AsyncLock) Assert.AreEqual((int)LockType.Write, (int)l.State);
                            else Assert.AreEqual((int)s.LockType, (int)l.State);

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
                            Assert.AreEqual(ThreadCorrelationToken.FromCurrentThread, l.AllOwners.Single());
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
                            Assert.AreEqual(ThreadCorrelationToken.FromCurrentThread, l.AllOwners.Single());
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
                if (s.IsAsync == false) continue;
                if (s.RecursionPolicy == LockRecursionPolicy.NoRecursion) continue;
                
                Trace.WriteLine($"TESTING: {s.ToString()}");

                if (s.RecursionPolicy == LockRecursionPolicy.SupportsRecursion)
                {
                    if (s.IsAsync)
                    {
                        var al = s.GetLock();

                        using (await al.AcquireLockAsync(s.LockType))
                        {
                            Assert.AreEqual((int)s.ExpectedLockState, (int)al.State);

                            await Task.Delay(25);

                            Assert.AreEqual((int)s.ExpectedLockState, (int)al.State);
                        }                        
                    }
                    else
                    {
                        var al = new AsyncLock(s.RecursionPolicy);
                        using (al.AcquireLock(s.LockType))
                        {
                            Assert.AreEqual((int)s.ExpectedLockState, (int)al.State);

                            await Task.Delay(25);

                            Assert.AreEqual((int)s.ExpectedLockState, (int)al.State);
                        }
                    }
                }
                else
                {
                    if (s.IsAsync)
                    {
                        var al = new AsyncLock(s.RecursionPolicy);
                        using (await al.AcquireLockAsync(s.LockType))
                        {
                            Assert.AreEqual((int)s.ExpectedLockState, (int)al.State);

                            await Task.Delay(25);

                            Assert.AreEqual((int)s.ExpectedLockState, (int)al.State);
                        }
                    }
                    else
                    {
                        var al = new AsyncLock(s.RecursionPolicy);
                        using (al.AcquireLock(s.LockType))
                        {
                            Assert.AreEqual((int)s.ExpectedLockState, (int)al.State);

                            await Task.Delay(25);

                            Assert.AreEqual((int)s.ExpectedLockState, (int)al.State);
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
                            Assert.AreEqual((int)s.ExpectedLockState, (int)al.State);

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

                            Assert.AreEqual((int)s.ExpectedLockState, (int)al.State);
                        }
                    }
                    else
                    {
                        var al = s.GetLock();
                        using (al.AcquireLock(s.LockType))
                        {
                            Assert.AreEqual((int)s.ExpectedLockState, (int)al.State);

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

                            Assert.AreEqual((int)s.ExpectedLockState, (int)al.State);
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
                            Assert.AreEqual((int)s.ExpectedLockState, (int)al.State);

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

                            Assert.AreEqual((int)s.ExpectedLockState, (int)al.State);

                            l2.Dispose();
                        }

                        Assert.AreEqual(LockState.NoLock, al.State);
                    }
                    else
                    {
                        var al = s.GetLock();
                        using (al.AcquireLock(s.LockType))
                        {
                            Assert.AreEqual((int)s.ExpectedLockState, (int)al.State);

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

                            Assert.AreEqual((int)s.ExpectedLockState, (int)al.State);
                        }
                    }
                }
            }
        }

        [TestMethod]
        public async Task same_thread_tries_to_acquire_taken_write_lock()
        {
            foreach (var s in _lock_test_setup.AllTestSetups)
            {
                Trace.WriteLine($"TESTING: {s.ToString()}"); 

                if (s.RecursionPolicy == LockRecursionPolicy.SupportsRecursion)
                {
                    if (s.IsAsync)
                    {
                        Assert.Fail();
                    }
                    else
                    {
                        var al = s.GetLock();
                        using (al.AcquireLock(s.LockType))
                        {
                            Assert.AreEqual((int)s.ExpectedLockState, (int)al.State);

                            var l2 = al.AcquireLock(s.LockType, new SyncOptions(50)); ;

                            // recursive lock granted
                            Assert.IsTrue(l2.IsLockHeld, s.ToString());
                            Assert.AreEqual((int)s.ExpectedLockState, (int)al.State);
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
                            Assert.AreEqual((int)s.ExpectedLockState, (int)al.State);

                            var l2 = await al.AcquireLockAsync(s.LockType, new AsyncOptions(50));

                            // should time out no not acquire lock
                            Assert.IsFalse(l2.IsLockHeld, s.ToString());
                            Assert.AreEqual((int)s.ExpectedLockState, (int)al.State);
                        }
                    }
                    else
                    {
                        var al = s.GetLock();
                        using (al.AcquireLock(s.LockType))
                        {
                            Assert.AreEqual((int)s.ExpectedLockState, (int)al.State);

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
