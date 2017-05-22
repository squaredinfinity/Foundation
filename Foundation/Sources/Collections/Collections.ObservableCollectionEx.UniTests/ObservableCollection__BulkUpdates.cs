using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SquaredInfinity.Collections;
using System.Threading.Tasks;

namespace Collections.ObservableCollectionEx.UniTests
{
    [TestClass]
    public class ObservableCollection__BulkUpdates
    {
        [TestMethod]
        public void can_add_item_inside_bulk_update()
        {
            var oc = new ObservableCollectionEx<int>();

            using (var blk = oc.BeginBulkUpdate())
            {
                Assert.IsTrue(blk.HasStarted);

                oc.Add(1);
            }

            Assert.AreEqual(1, oc.Count);
            Assert.AreEqual(1, oc[0]);
        }

        [TestMethod]
        public async Task can_add_item_inside_async_bulk_update()
        {
            var oc = new ObservableCollectionEx<int>();

            using (var blk = await oc.BeginBulkUpdateAsync())
            {
                Assert.IsTrue(blk.HasStarted);

                oc.Add(1);
            }

            Assert.AreEqual(1, oc.Count);
            Assert.AreEqual(1, oc[0]);
        }

        [TestMethod]
        public async Task nested_bulk_updates_are_allowed()
        {
            var oc = new ObservableCollectionEx<int>();

            using (var blk1 = await oc.BeginBulkUpdateAsync())
            {
                Assert.IsTrue(blk1.HasStarted);

                using (var blk2 = await oc.BeginBulkUpdateAsync())
                {
                    Assert.IsTrue(blk2.HasStarted);
                    oc.Add(1);
                }

                Assert.IsTrue(blk1.HasStarted);
            }

            Assert.AreEqual(1, oc.Count);
            Assert.AreEqual(1, oc[0]);
        }
    }
}
