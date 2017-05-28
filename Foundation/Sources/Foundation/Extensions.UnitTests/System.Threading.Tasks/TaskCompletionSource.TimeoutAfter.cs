using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SquaredInfinity.Extensions;
using System.Threading.Tasks;
using System.Threading;
using System.Diagnostics;

namespace Extensions.UnitTests
{
    [TestClass]
    public class TaskCompletionSource__TimeoutAfter
    {
        [TestMethod]
        public void on_timeout_throws_timeoutexception()
        {
            var tcs = new TaskCompletionSource<int>();

            tcs.TimeoutAfter(25);

            try
            {
                // this should throw timeout exception
                var x = tcs.Task.Wait(50);
            }
            catch(AggregateException ex)
            {
                Assert.IsTrue(ex.InnerException is TimeoutException);
                return;
            }

            Assert.Fail();
        }

        [TestMethod]
        public void can_customize_on_timeout_behavior()
        {
            var tcs = new TaskCompletionSource<int>();

            // set custom task result on itmeout
            // this will not throw timeout exception
            tcs.TimeoutAfter(25, _ => _.SetResult(13));

            Assert.IsTrue(tcs.Task.Wait(50));
            Assert.AreEqual(13, tcs.Task.Result);
        }
    }
}
