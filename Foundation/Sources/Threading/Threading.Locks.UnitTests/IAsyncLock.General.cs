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
                if (s.RecursionPolicy == LockRecursionPolicy.SupportsRecursion)
                {
                    if (s.IsAsync)
                    {
                        // async locks should throw on recursive locks

                        bool thrown = false;

                        try
                        {
                            var l = s.GetLock();
                            await l.AcquireLockAsync(s.LockType);

                            Assert.Fail(s.ToString());
                        }
                        catch (NotSupportedException ex)
                        {
                            thrown = true;
                        }

                        Assert.IsTrue(thrown, s.ToString());
                    }
                    else
                    {
                        var l = s.GetLock();
                        using (var a = l.AcquireLock(s.LockType))
                        {
                            if(l is AsyncLock) Assert.AreEqual((int)LockType.Write, (int)l.State);
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
        public async Task reports_owner_thread()
        {
            foreach (var s in _lock_test_setup.AllTestSetups)
            {
                Trace.WriteLine($"TESTING: {s.ToString()}");

                if (s.RecursionPolicy == LockRecursionPolicy.SupportsRecursion)
                {
                    if (s.IsAsync)
                    {
                        // ignored, async recursive throw as per acquire_lock test
                    }
                    else
                    {
                        var l = s.GetLock();
                        using (l.AcquireLock(s.LockType))
                        {
                            Assert.AreEqual(Environment.CurrentManagedThreadId, l.AllOwners.Single());
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
                            Assert.AreEqual(Environment.CurrentManagedThreadId, l.AllOwners.Single());
                        }

                        Assert.AreEqual(0, l.AllOwners.Count);
                    }
                    else
                    {
                        var l = s.GetLock();
                        using (l.AcquireLock(s.LockType))
                        {
                            Assert.AreEqual(Environment.CurrentManagedThreadId, l.AllOwners.Single());
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
                if (s.RecursionPolicy == LockRecursionPolicy.SupportsRecursion)
                {
                    if (s.IsAsync)
                    {
                        // ignored, async recursive throw as per acquire_lock test
                    }
                    else
                    {
                        var al = new AsyncLock(s.RecursionPolicy);
                        var l = al.AcquireLock(s.LockType);
                        Assert.AreEqual(LockState.Write, al.State);
                        l.Dispose();
                        Assert.AreEqual(LockState.NoLock, al.State);
                    }
                }
                else
                {
                    if (s.IsAsync)
                    {
                        var al = new AsyncLock(s.RecursionPolicy);
                        var l = await al.AcquireLockAsync(s.LockType);
                        Assert.AreEqual(LockState.Write, al.State);
                        l.Dispose();
                        Assert.AreEqual(LockState.NoLock, al.State);
                    }
                    else
                    {
                        var al = new AsyncLock(s.RecursionPolicy);
                        var l = al.AcquireLock(s.LockType);
                        Assert.AreEqual(LockState.Write, al.State);
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
                if (s.RecursionPolicy == LockRecursionPolicy.SupportsRecursion)
                {
                    if (s.IsAsync)
                    {
                        // ignored, async recursive throw as per acquire_lock test
                    }
                    else
                    {
                        var al = new AsyncLock(s.RecursionPolicy);
                        var l = al.AcquireLock(s.LockType);
                        Assert.AreEqual(LockState.Write, al.State);

                        var l2 = Task.Factory.StartNewThread(() => al.AcquireWriteLock(new SyncOptions(50))).Result;
                        Assert.IsFalse(l2.IsLockHeld);
                    }
                }
                else
                {
                    if (s.IsAsync)
                    {
                        var al = new AsyncLock(s.RecursionPolicy);
                        var l = await al.AcquireLockAsync(s.LockType);
                        Assert.AreEqual(LockState.Write, al.State);

                        var l2 = Task.Factory.StartNewThread(() => al.AcquireWriteLock(new SyncOptions(50))).Result;
                        Assert.IsFalse(l2.IsLockHeld);
                    }
                    else
                    {
                        var al = new AsyncLock(s.RecursionPolicy);
                        var l = al.AcquireLock(s.LockType);
                        Assert.AreEqual(LockState.Write, al.State);

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
                if (s.RecursionPolicy == LockRecursionPolicy.SupportsRecursion)
                {
                    if (s.IsAsync)
                    {
                        // ignored, async recursive throw as per acquire_lock test
                    }
                    else
                    {
                        var al = new AsyncLock(s.RecursionPolicy);
                        using (al.AcquireLock(s.LockType))
                        {
                            Assert.AreEqual(LockState.Write, al.State);

                            await Task.Delay(25);

                            Assert.AreEqual(LockState.Write, al.State);
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
                            Assert.AreEqual(LockState.Write, al.State);

                            await Task.Delay(25);

                            Assert.AreEqual(LockState.Write, al.State);
                        }
                    }
                    else
                    {
                        var al = new AsyncLock(s.RecursionPolicy);
                        using (al.AcquireLock(s.LockType))
                        {
                            Assert.AreEqual(LockState.Write, al.State);

                            await Task.Delay(25);

                            Assert.AreEqual(LockState.Write, al.State);
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
                        // ignored, async recursive throw as per acquire_lock test
                    }
                    else
                    {
                        var al = new AsyncLock(s.RecursionPolicy);
                        using (al.AcquireLock(s.LockType))
                        {
                            Assert.AreEqual(LockState.Write, al.State);

                            var l2 = Task.Factory.StartNewThread(() => al.AcquireLock(s.LockType, new SyncOptions(50))).Result;

                            Assert.IsFalse(l2.IsLockHeld, s.ToString());
                            Assert.AreEqual(LockState.Write, al.State, s.ToString());
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
                            Assert.AreEqual(LockState.Write, al.State);

                            var l2 = await Task.Factory.StartNewThread(async () => await al.AcquireLockAsync(s.LockType, new AsyncOptions(50))).Result;

                            Assert.IsFalse(l2.IsLockHeld, s.ToString());
                            Assert.AreEqual(LockState.Write, al.State, s.ToString());
                        }
                    }
                    else
                    {
                        var al = new AsyncLock(s.RecursionPolicy);
                        using (al.AcquireLock(s.LockType))
                        {
                            Assert.AreEqual(LockState.Write, al.State);
                            
                            Trace.WriteLine(Environment.CurrentManagedThreadId);


                            var l2 = await Task.Factory.StartNewThread(() => al.AcquireLock(s.LockType, new SyncOptions(50)));

                            Assert.IsFalse(l2.IsLockHeld, s.ToString());
                            Assert.AreEqual(LockState.Write, al.State, s.ToString());
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
                        // ignored, async recursive throw as per acquire_lock test
                    }
                    else
                    {
                        var al = new AsyncLock(s.RecursionPolicy);
                        using (al.AcquireLock(s.LockType))
                        {
                            Assert.AreEqual(LockState.Write, al.State);

                            var l2 = al.AcquireLock(s.LockType, new SyncOptions(50)); ;

                            // recursive lock granted
                            Assert.IsTrue(l2.IsLockHeld, s.ToString());
                            Assert.AreEqual(LockState.Write, al.State, s.ToString());
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
                            Assert.AreEqual(LockState.Write, al.State);

                            var l2 = await al.AcquireLockAsync(s.LockType, new AsyncOptions(50));

                            // should time out no not acquire lock
                            Assert.IsFalse(l2.IsLockHeld, s.ToString());
                            Assert.AreEqual(LockState.Write, al.State, s.ToString());
                        }
                    }
                    else
                    {
                        var al = new AsyncLock(s.RecursionPolicy);
                        using (al.AcquireLock(s.LockType))
                        {
                            Assert.AreEqual(LockState.Write, al.State);

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
