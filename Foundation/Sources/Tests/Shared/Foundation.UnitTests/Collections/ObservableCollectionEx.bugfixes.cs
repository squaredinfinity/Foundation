using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SquaredInfinity.Foundation.Collections
{
	[TestClass]
    public class ObservableCollectionExTests__BugFixes
    {
        [TestMethod]
        public void BUG__0001__CollectionIsReadOnlyAfterInstanceCreated_NothingCanBeAdded()
        {
            var col = new ObservableCollectionEx<object>();
            col.Add(1);

            Assert.IsTrue(true);
        }

        [TestMethod]
        public void BUG__0002__BulkUpdateFailsWhenCalledFromMultipleThreads()
        {
            var col = new ObservableCollectionEx<int>();

            var t1 =
                Task.Factory.StartNew(() =>
                {
                    using (col.BeginBulkUpdate())
                    {
                        col.Add(1);
                        Thread.Sleep(10);
                    }
                });

            using (col.BeginBulkUpdate())
            {
                col.Clear();
            }

            // code above would normally fail by now.
        }
    }
}
