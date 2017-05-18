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
        public async Task consecutive_call_to_begin_update_does_not_start_bulk_update()
        {
            var x = new SupportsAsyncBulkUpdate();

            var bu = await x.BeginBulkUpdateAsync();

            Assert.IsTrue(bu.HasStarted);

            var bu2 = await x.BeginBulkUpdateAsync(25);

            Assert.IsFalse(bu2.HasStarted);

            bu.Dispose();

            Assert.IsFalse(bu.HasStarted);
        }
    }
}
