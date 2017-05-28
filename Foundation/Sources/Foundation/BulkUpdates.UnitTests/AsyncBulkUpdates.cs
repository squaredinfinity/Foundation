using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SquaredInfinity;
using System.Threading.Tasks;

namespace BulkUpdates.UnitTests
{
    [TestClass]
    public class AsyncBulkUpdates
    {
        [TestMethod]
        public async Task same_thread__consecutive_calls_to_begin_update_do_not_block()
        {
            var x = new SupportsAsyncBulkUpdate();

            var bu = await x.BeginBulkUpdateAsync();

            Assert.IsTrue(bu.HasStarted);

            var bu2 = await x.BeginBulkUpdateAsync(25);

            Assert.IsTrue(bu2.HasStarted);

            // disposing bu2 should still keep update going
            bu2.Dispose();
            Assert.IsTrue(bu.HasStarted);

            // disposing bu should stop the updates
            bu.Dispose();
            Assert.IsFalse(bu.HasStarted);
        }

        [TestMethod]
        public async Task different_thread__consecutive_calls_to_begin_update_block()
        {
            var x = new SupportsAsyncBulkUpdate();

            var bu = await x.BeginBulkUpdateAsync();

            Assert.IsTrue(bu.HasStarted);

            var bu2 = Task.Factory.StartNew(async () => await x.BeginBulkUpdateAsync(25)).Result.Result;
            Assert.IsFalse(bu2.HasStarted);

            bu.Dispose();
            Assert.IsFalse(bu.HasStarted);
        }
    }
}
