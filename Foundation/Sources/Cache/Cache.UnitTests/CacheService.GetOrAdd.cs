using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Runtime.Caching;
using SquaredInfinity.Cache;
using System.Threading;
using System.Diagnostics;

namespace Cache.UnitTests
{
    [TestClass]
    public class CacheService__GetOrAdd
    {
        [TestMethod]
        public void does_not_deadlock_when_called_from_different_threads()
        {
            var mc = new MemoryCacheService();

            var are1 = new AutoResetEvent(false);
            var t1 = new Thread(() =>
            {
                var x = mc.GetOrAdd("key", GetValue);
                Trace.WriteLine(x);
                are1.Set();
            });

            var are2 = new AutoResetEvent(false);
            var t2 = new Thread(() =>
            {
                var x = mc.GetOrAdd("key", GetValue);
                Trace.WriteLine(x);
                are2.Set();
            });

            int GetValue()
            {
                Thread.Sleep(500);
                return new Random().Next();
            }

            t1.Start();
            t2.Start();

            if(!are1.WaitOne(500) || !are2.WaitOne(500))
            {
                Assert.Fail("Deadlock occured :(");
            }
        }
    }
}
