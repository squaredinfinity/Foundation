using System;
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
    public partial class ICompositeLock__LockChildren
    {
        IReadOnlyList<IAsyncLock> GetLocks(LockRecursionPolicy rp) => new IAsyncLock[] { new AsyncLock(rp), new AsyncReaderWriterLock(rp) };

        [TestMethod]
        public async Task can_acquire_and_release_child_lock()
        {
            foreach (var s in _lock_test_setup.AllTestSetups)
                foreach (var l in GetLocks(s.RecursionPolicy))
                {
                    Trace.WriteLine($"TESTING: {s.ToString()} {l.GetType().Name}");

                    if (s.RecursionPolicy == LockRecursionPolicy.SupportsRecursion)
                    {
                        if (s.IsAsync)
                        {
                            // ignored, async recursive throw as per acquire_lock test
                        }
                        else
                        {
                            var child = new AsyncReaderWriterLock(s.RecursionPolicy);
                            l.AddChild(child);

                            using (l.AcquireLock(s.LockType))
                                Assert.AreEqual((int)s.LockType, (int)child.State);

                            Assert.AreEqual(LockState.NoLock, child.State);
                        }
                    }
                    else
                    {
                        if (s.IsAsync)
                        {
                            var child = new AsyncReaderWriterLock(s.RecursionPolicy);
                            l.AddChild(child);

                            using (await l.AcquireLockAsync(s.LockType))
                                Assert.AreEqual((int)s.LockType, (int)child.State);

                            Assert.AreEqual((int)LockState.NoLock, (int)child.State);
                        }
                        else
                        {
                            var child = new AsyncReaderWriterLock(s.RecursionPolicy);
                            l.AddChild(child);

                            using (l.AcquireLock(s.LockType))
                                Assert.AreEqual((int)s.LockType, (int)child.State);

                            Assert.AreEqual((int)LockState.NoLock, (int)child.State);
                        }
                    }
                }
        }
    }
}
