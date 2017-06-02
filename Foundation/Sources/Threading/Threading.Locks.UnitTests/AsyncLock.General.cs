using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SquaredInfinity.Threading;
using System.Threading.Tasks;
using System.Threading;
using SquaredInfinity.Threading.Locks;
using System.Collections.Generic;

namespace Threading.Locks.UnitTests
{
    class _lock_test_setup
    {
        public LockRecursionPolicy RecursionPolicy { get; private set; }
        public LockType LockType { get; private set; }
        public bool IsAsync { get; private set; }

        public static IReadOnlyList<_lock_test_setup> AllTestSetups { get; private set; }

        static _lock_test_setup()
        {
            var all_setups = new List<_lock_test_setup>();

            foreach (var rp in new[] { LockRecursionPolicy.NoRecursion, LockRecursionPolicy.SupportsRecursion })
                foreach (var lt in new[] { LockType.Read, LockType.Write })
                    foreach(var ia in new[] { true, false })
                        all_setups.Add(new _lock_test_setup { LockType = lt, RecursionPolicy = rp, IsAsync = ia });

            AllTestSetups = all_setups;
        }

        public override string ToString()
        {
            return $"{LockType}, {RecursionPolicy}, async:{IsAsync}";
        }
    }

    [TestClass]
    public partial class AsyncLock__General
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
                            var al = new AsyncLock(s.RecursionPolicy);
                            var l = await al.AcquireLockAsync(s.LockType);

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
                        var al = new AsyncLock(s.RecursionPolicy);
                        var l = al.AcquireLock(s.LockType);

                        Assert.AreEqual(LockState.Write, al.State);
                    }
                }
                else
                {
                    if (s.IsAsync)
                    {
                        var al = new AsyncLock(s.RecursionPolicy);
                        var l = await al.AcquireLockAsync(s.LockType);

                        Assert.AreEqual(LockState.Write, al.State);
                    }
                    else
                    {
                        var al = new AsyncLock(s.RecursionPolicy);
                        var l = al.AcquireLock(s.LockType);

                        Assert.AreEqual(LockState.Write, al.State);
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
                        // ignored, async recursive throw
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
                        // ignored, async recursive throw
                    }
                    else
                    {
                        var al = new AsyncLock(s.RecursionPolicy);
                        var l = al.AcquireLock(s.LockType);
                        Assert.AreEqual(LockState.Write, al.State);

                        var l2 = Task.Factory.StartNew(() => al.AcquireWriteLock(new SyncOptions(50))).Result;
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

                        var l2 = Task.Factory.StartNew(() => al.AcquireWriteLock(new SyncOptions(50))).Result;
                        Assert.IsFalse(l2.IsLockHeld);
                    }
                    else
                    {
                        var al = new AsyncLock(s.RecursionPolicy);
                        var l = al.AcquireLock(s.LockType);
                        Assert.AreEqual(LockState.Write, al.State);

                        var l2 = Task.Factory.StartNew(() => al.AcquireWriteLock(new SyncOptions(50))).Result;
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
                        // ignored, async recursive throw
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

        //[TestMethod]
        //public async Task acquire_multiple_locks_asynchronously()
        //{
        //    var l1 = new AsyncLock();
        //    var l2 = new AsyncLock();
        //    var l3 = new AsyncLock();
        //    var l4 = new AsyncLock();

        //    var all_acquisitions =
        //        await AsyncLock.AcquireWriteLockAsync(l1, l2, l3, l4);

        //    Assert.IsTrue(all_acquisitions.IsLockHeld);

        //    Assert.AreEqual(System.Environment.CurrentManagedThreadId, l1.WriteOwnerThreadId);
        //    Assert.AreEqual(System.Environment.CurrentManagedThreadId, l2.WriteOwnerThreadId);
        //    Assert.AreEqual(System.Environment.CurrentManagedThreadId, l3.WriteOwnerThreadId);
        //    Assert.AreEqual(System.Environment.CurrentManagedThreadId, l4.WriteOwnerThreadId);

        //    all_acquisitions.Dispose();

        //    Assert.IsFalse(all_acquisitions.IsLockHeld);

        //    Assert.AreEqual(-1, l1.WriteOwnerThreadId);
        //    Assert.AreEqual(-1, l2.WriteOwnerThreadId);
        //    Assert.AreEqual(-1, l3.WriteOwnerThreadId);
        //    Assert.AreEqual(-1, l4.WriteOwnerThreadId);
        //}
    }
}
