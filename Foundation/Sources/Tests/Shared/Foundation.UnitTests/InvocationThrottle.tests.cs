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
        public void Invokes_Only_One_Action_At_A_Time()
        {
            var it = new InvocationThrottle(min: TimeSpan.FromMilliseconds(1), max: TimeSpan.FromMilliseconds(2));

            int im_doing_stuff = 0;
            bool hasException = false;

            int invocation_count = 0;

            ConcurrentBag<AutoResetEvent> ares = new ConcurrentBag<AutoResetEvent>();

            // invoke action in a look
            for (int i = 0; i < 10; i++)
            {
                var local_i = i;

                // some actions will be cancelled bevause min time has not passed
                // those should never even start running

                // there always should be at most one action running at a tim
                it.InvokeAsync(() => 
                    {
                        var are = new AutoResetEvent(initialState: false);
                        ares.Add(are);

                        try
                        {
                            Interlocked.Increment(ref invocation_count);
                            LongAction(ref im_doing_stuff);

                            // given min of 1s, this should be invoked only once
                            if(local_i < 9 && invocation_count != 1)
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

        void LongAction(ref int execution_marker)
        {
            if(Interlocked.Increment(ref execution_marker) != 1)
                throw new Exception();

            Thread.Sleep(100);

            if (Interlocked.Decrement(ref execution_marker) != 0)
                throw new Exception();
        }

        public static void OnMtaThread(Action action)
        {
            var thread = new Thread(new ThreadStart(action));
            thread.SetApartmentState(ApartmentState.MTA);
            thread.Start();
            thread.Join();
        }

        [TestMethod]
        public void Action_Invoked_On_Different_Thread()
        {
            var it = new InvocationThrottle();

            var are = new AutoResetEvent(initialState: false);

            var tid = Thread.CurrentThread.ManagedThreadId;

            it.InvokeAsync(() => 
                {
                    Assert.AreNotEqual(tid, Thread.CurrentThread.ManagedThreadId);
                    are.Set();
                });
            
            are.WaitOne();

            Assert.IsTrue(true);
        }

        [TestMethod]
        public void Multiple_Requests_For_Invocation_Delay_Actual_Invocation_Until_Max_Timespan_Elapsed()
        {
            var min = TimeSpan.FromMilliseconds(50);
            var max = TimeSpan.FromMilliseconds(250);

            var it = new InvocationThrottle(min, max);
            var tid = Thread.CurrentThread.ManagedThreadId;
            
            var start_time = DateTime.UtcNow;
            var invocation_time = DateTime.UtcNow;

            var should_break = false;

            while (!should_break && (DateTime.UtcNow - start_time).TotalMilliseconds < 1000)
            {
                it.InvokeAsync((ct) =>
                {
                    if (ct.IsCancellationRequested)
                        return;

                    invocation_time = DateTime.UtcNow;
                    should_break = true;
                });
            }

            Assert.IsTrue((invocation_time - start_time).TotalMilliseconds >= 250);
            Assert.IsTrue((invocation_time - start_time).TotalMilliseconds < 500);
        }

        [TestMethod]
        public void Does_Not_Invoke_Actions_Before_Min_Passed()
        {
            var it = new InvocationThrottle(TimeSpan.FromMilliseconds(50), TimeSpan.FromMilliseconds(100));

            var failed = false;

            var sw = Stopwatch.StartNew();
            var are = new AutoResetEvent(false);

            for(int i = 0; i < 100; i++)
            {
                it.InvokeAsync((ct) =>
                {
                    sw.Stop();

                    var _i = i;
                    if (_i != 100)
                        failed = true;

                    are.Set();
                });
            }

            are.WaitOne();

            if (failed)
                Assert.Fail();

            Assert.IsTrue(sw.Elapsed.TotalMilliseconds > 50);

        }

        [TestMethod]
        public void Waits_Min_Before_Executing()
        {
            var min = TimeSpan.FromMilliseconds(50);

            var it = new InvocationThrottle(min);
            var tid = Thread.CurrentThread.ManagedThreadId;

            for (int i = 1; i <= 2; i++)
            {
                var are = new AutoResetEvent(initialState: false);

                var sw = Stopwatch.StartNew();

                it.InvokeAsync(() =>
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

        [TestMethod]
        public void MyTestMethod()
        {
            var min = TimeSpan.FromMilliseconds(50);

            var it = new InvocationThrottle(min);
            var tid = Thread.CurrentThread.ManagedThreadId;

            int invocation_count = 0;

            for (int i = 1; i <= 2; i++)
            {
                var sw = Stopwatch.StartNew();

                if(i == 2)
                {

                }

                it.InvokeAsync(() =>
                {
                    Interlocked.Increment(ref invocation_count);

                    sw.Stop();
                    Assert.AreNotEqual(tid, Thread.CurrentThread.ManagedThreadId);
                    
                    if(invocation_count == 1)
                        throw new Exception();
                });

                Thread.Sleep(75);
            }

            Thread.Sleep(1000);

            Assert.AreEqual(2, invocation_count);
        }
    }
}
