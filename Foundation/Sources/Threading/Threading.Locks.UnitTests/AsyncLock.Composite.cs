using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SquaredInfinity.Threading;
using System.Threading.Tasks;
using System.Threading;
using SquaredInfinity.Threading.Locks;

namespace Threading.Locks.UnitTests
{
    [TestClass]
    public partial class AsyncLock__Composite
    {     
        [TestMethod]
        public async Task acquires_childLocks()
        {
            var master = new AsyncLock();
            var c1 = new AsyncLock();
            var c2 = new AsyncLock();
            var c3 = new AsyncLock();
            var c4 = new AsyncLock();

            master.AddChild(c1);
            master.AddChild(c2);
            master.AddChild(c3);
            master.AddChild(c4);

            var a = await master.AcquireWriteLockAsync();

            Assert.IsTrue(a.IsLockHeld);

            Assert.AreEqual(System.Environment.CurrentManagedThreadId, master.WriteOwnerThreadId);
            Assert.AreEqual(System.Environment.CurrentManagedThreadId, c1.WriteOwnerThreadId);
            Assert.AreEqual(System.Environment.CurrentManagedThreadId, c2.WriteOwnerThreadId);
            Assert.AreEqual(System.Environment.CurrentManagedThreadId, c3.WriteOwnerThreadId);
            Assert.AreEqual(System.Environment.CurrentManagedThreadId, c4.WriteOwnerThreadId);

            a.Dispose();

            Assert.AreEqual(-1, master.WriteOwnerThreadId);
            Assert.AreEqual(-1, c1.WriteOwnerThreadId);
            Assert.AreEqual(-1, c2.WriteOwnerThreadId);
            Assert.AreEqual(-1, c3.WriteOwnerThreadId);
            Assert.AreEqual(-1, c4.WriteOwnerThreadId);
        }
    }
}
