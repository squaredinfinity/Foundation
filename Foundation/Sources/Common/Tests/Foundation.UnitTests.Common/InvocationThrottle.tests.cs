using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SquaredInfinity.Foundation
{
    [TestClass]
    public class InvocationThrottleTests
    {
        [TestMethod]
        public void ActionIsInvokedOnDifferentThread()
        {
            var it = new InvocationThrottle(TimeSpan.MinValue);

            var are = new AutoResetEvent(initialState: false);

            var tid = Thread.CurrentThread.ManagedThreadId;

            it.Invoke(() => 
                {
                    Assert.AreNotEqual(tid, Thread.CurrentThread.ManagedThreadId);

                    are.Set();
                });
            
            are.WaitOne();

            Assert.IsTrue(true);
        }


        [TestMethod]
        public void MultipleRequestsForInvocationDelayActualInvocationUntilMaxTimespanElapsed()
        {
            var min = TimeSpan.FromMilliseconds(100);
            var max = TimeSpan.FromMilliseconds(500);

            var it = new InvocationThrottle(min, max);
            var tid = Thread.CurrentThread.ManagedThreadId;

            var are = new AutoResetEvent(initialState: false);

            var t = new Task(() =>
            {
                bool keep_going = true;

                while(keep_going)
                {
                    it.Invoke(() =>
                    {
                        Assert.AreNotEqual(tid, Thread.CurrentThread.ManagedThreadId);
                        are.Set();
                        keep_going = false;
                    });
                }
            });

            var sw = Stopwatch.StartNew();

            t.Start();

            are.WaitOne();

            sw.Stop();

            Assert.IsTrue(sw.Elapsed.TotalMilliseconds >= max.TotalMilliseconds);
        }

        [TestMethod]
        public void WaitsMinTimespanBeforeExecuting()
        {
            var min = TimeSpan.FromMilliseconds(100);

            var it = new InvocationThrottle(min);
            var tid = Thread.CurrentThread.ManagedThreadId;

            var are = new AutoResetEvent(initialState: false);

            it.Invoke(() =>
                    {
                        Assert.AreNotEqual(tid, Thread.CurrentThread.ManagedThreadId);
                        are.Set();
                    });

            var sw = Stopwatch.StartNew();
            
            are.WaitOne();

            sw.Stop();

            Assert.IsTrue(sw.Elapsed.TotalMilliseconds >= min.TotalMilliseconds);
        }


        [TestMethod]
        public void CanWaitForInvocationToFinish()
        {
            var min = TimeSpan.FromMilliseconds(100);
            var max = TimeSpan.FromMilliseconds(150);

            var it = new InvocationThrottle(min, max);
            var tid = Thread.CurrentThread.ManagedThreadId;

            var sw = Stopwatch.StartNew();

            it.Invoke(() =>
            {
                Assert.AreNotEqual(tid, Thread.CurrentThread.ManagedThreadId);
            }).Wait();

            sw.Stop();

            Assert.IsTrue(sw.Elapsed.TotalMilliseconds >= min.TotalMilliseconds & sw.Elapsed.TotalMilliseconds < max.TotalMilliseconds);
        }
    }
}
