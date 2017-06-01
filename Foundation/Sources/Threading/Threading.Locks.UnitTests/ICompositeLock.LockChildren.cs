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
        IReadOnlyList<IAsyncLock> GetLocks() => new IAsyncLock[] { new AsyncLock(), new AsyncReaderWriterLock() };

        [TestMethod]
        public void BUG001__can_acquire_and_release_child_lock()
        {
            foreach(var l in GetLocks())
            {
                Trace.WriteLine($"TESTING: {l.GetType().FullName}");

                var child = new AsyncLock();
                l.AddChild(child);

                using (var a = l.AcquireWriteLock())
                {
                    Assert.IsTrue(child.IsWriteLockHeld);
                }

                Assert.IsFalse(child.IsWriteLockHeld);
            }
        }
    }
}
