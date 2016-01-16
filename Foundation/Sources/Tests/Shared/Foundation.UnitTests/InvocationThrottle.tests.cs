using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using SquaredInfinity.Foundation.Extensions;

namespace SquaredInfinity.Foundation
{
    [TestClass]
    public class InvocationThrottleTests
    {
        [TestMethod]
        public void InvokesOnlyOneAtTheTimeAndLastOneWins()
        {
            var it = new InvocationThrottle(min: TimeSpan.FromMilliseconds(1), max: TimeSpan.FromMilliseconds(2));

            int im_doing_stuff = 0;
            bool hasException = false;

            int invocation_count = 0;

            ConcurrentBag<AutoResetEvent> ares = new ConcurrentBag<AutoResetEvent>();

            for (int i = 0; i < 10; i++)
            {
                var local_i = i;

                it.Invoke(() => 
                    {
                        var are = new AutoResetEvent(initialState: false);
                        ares.Add(are);

                        try
                        {
                            Interlocked.Increment(ref invocation_count);
                            LongAction(ref im_doing_stuff);

                            if((local_i < 9 && invocation_count != 1) || (local_i == 9 && invocation_count != 2))
                            {
                                throw new Exception();
                            }
                        }
                        catch(Exception ex)
                        {
                            hasException = true;
                        }

                        are.Set();
                    });

                Thread.Sleep(1);
            }

            Thread.Sleep(200);

            OnMtaThread(() => AutoResetEvent.WaitAll(ares.ToArray()));

            if (hasException)
                Assert.Fail();

        }

        public static void OnMtaThread(Action action)
        {
            var thread = new Thread(new ThreadStart(action));
            thread.SetApartmentState(ApartmentState.MTA);
            thread.Start();
            thread.Join();
        }

        void LongAction(ref int execition_marker)
        {
            if (Interlocked.CompareExchange(ref execition_marker, 1, 0) == 1)
                throw new Exception();

            Thread.Sleep(100);

            if (Interlocked.CompareExchange(ref execition_marker, 0, 1) == 0)
                throw new Exception();
        }

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

            Trace.WriteLine(sw.Elapsed.TotalMilliseconds + " , " + max.TotalMilliseconds);

            Assert.IsTrue(sw.Elapsed.TotalMilliseconds >= max.TotalMilliseconds);
        }

        [TestMethod]
        public void WaitsMinTimespanBeforeExecuting()
        {
            var min = TimeSpan.FromMilliseconds(50);

            var it = new InvocationThrottle(min);
            var tid = Thread.CurrentThread.ManagedThreadId;

            for (int i = 1; i <= 2; i++)
            {
                var are = new AutoResetEvent(initialState: false);

                var sw = Stopwatch.StartNew();

                it.Invoke(() =>
                        {
                            sw.Stop();
                            Assert.AreNotEqual(tid, Thread.CurrentThread.ManagedThreadId);
                            are.Set();
                        });

                are.WaitOne();

                // wait so that next request is not throttled, but initial delay should still apply
                Thread.Sleep(100);

                Assert.IsTrue(sw.Elapsed.TotalMilliseconds >= min.TotalMilliseconds, "failed iteration {0}, elapse: {1}".FormatWith(i.ToString(), sw.Elapsed.TotalMilliseconds));
            }
        }
    }
}
