using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SquaredInfinity.Foundation.Threading
{
    [TestClass]
    public class ReaderWriterLockSlimEx__ChildLocks
    {
        TimeSpan TEST_DefaultTimeout = TimeSpan.FromMilliseconds(50);

        [TestMethod]
        public void using_child_locks__avoids_deadlock_situations()
        {
            // Lock_1 (more coarse than Lock_2)
            // Lock_2 (child of Lock_1, more granual)

            // Thread_1 accesses Lock_1 then Lock_2
            // Thread_2 accesses Lock_2 only

            var l1 = new ReaderWriterLockSlimEx(LockRecursionPolicy.NoRecursion);
            var l2 = new ReaderWriterLockSlimEx(LockRecursionPolicy.NoRecursion);
            l1.AddChild(l2);

            bool oh_no = false;

            var t1 = Task.Run(() =>
            {
                using (l1.AcquireWriteLockIfNotHeld())
                {
                    Trace.WriteLine("t1 acquired l1");
                    Thread.Sleep(1000);

                    using (l2.AcquireReadLockIfNotHeld())
                    {
                        if (oh_no)
                            Assert.Fail(":(");
                    }
                }
            });


            var t2 = Task.Run(() =>
            {
                Thread.Sleep(100);

                using (l2.AcquireWriteLock())
                {
                    oh_no = true;
                    Trace.WriteLine("t2 acquired l2");
                }
            });

            Task.WaitAll(new[] { t1, t2 });
        }

       
    }
}
