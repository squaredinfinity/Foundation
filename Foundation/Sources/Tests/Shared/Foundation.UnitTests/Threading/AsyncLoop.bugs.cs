using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SquaredInfinity.Foundation.Threading
{
    [TestClass]
    public class AsyncLoop__Bugs
    {
        #region BUG 001

        [TestMethod]
        public void Bug001__MemoryLeakDueToDirectUseOfAction()
        {
            var are = new AutoResetEvent(initialState: false);

            var x = new BUG001__LoopOwner(are);

            Assert.IsNotNull(x);

            are.WaitOne();

            x = null;
            GC.Collect(GC.MaxGeneration, GCCollectionMode.Forced);
            GC.WaitForPendingFinalizers();

            if (are.WaitOne(TimeSpan.FromMilliseconds(250)))
            {
                Assert.Fail("are was signaled, but loop should have finished.");
            }
        }

        class BUG001__LoopOwner
        {
            AsyncLoop Loop;

            public BUG001__LoopOwner(AutoResetEvent are)
            {
                //  NOTE:   For test purposes lambdas passed to AsyncLoop should not reference any variable which is a class instance or passed via parameter)
                //          otherwise labda will create closure and test will be invalid.

                var _are = are;

                Loop = new AsyncLoop(() => 
                {
                    are.Set();
                });

                Loop.Start(TimeSpan.FromMilliseconds(10), new CancellationToken());
            }

            ~BUG001__LoopOwner()
            {

            }
        }

        #endregion
    }
}
