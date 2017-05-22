using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SquaredInfinity.Threading;
using System.Threading.Tasks;
using System.Threading;
using SquaredInfinity.Threading.Locks;

namespace Threading.Locks.UnitTests
{
    [TestClass]
    public partial class AsyncLock__Constructor
    {
        [TestMethod]
        public void non_recursive_by_default()
        {
            var al = new AsyncLock();
            Assert.AreEqual(LockRecursionPolicy.NoRecursion, al.RecursionPolicy);
        }
    }
}
