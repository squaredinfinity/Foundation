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
        public void MyTestMethod()
        {
            var c = 25;

            List<ILock> locks = new List<ILock>(capacity: c);

            Lock.Factory.CollectDiagnostics = true;

            for(int i = 0; i < c; i++)
            {
                locks.Add(Lock.Factory.CreateLock());
            }


            var r = new Random();

            Parallel.For(0, 100000, _ =>
            {
                var l = locks[r.Next(0, locks.Count)];

                switch (r.Next() % 3)
                {
                    case 0:
                        using (l.AcquireReadLock())
                        {
                            Task.Delay(r.Next(0, 100));
                        }
                        break;
                    case 1:
                        using (l.AcquireUpgradeableReadLock())
                        {
                            Task.Delay(r.Next(0, 100));
                        }
                        break;
                    case 2:
                        using (l.AcquireWriteLock())
                        {
                            Task.Delay(r.Next(0, 100));
                        }
                        break;
                }
            });
            

            Assert.IsFalse(true);
        }

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
