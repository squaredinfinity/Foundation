using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SquaredInfinity.Foundation.Collections
{
	[TestClass]
    public class CollectionEx__BulkUpdates
    {
        [TestMethod]
        public void BulkUpdate__IndividualOperationsDoNotTriggerVersionChanged__EndingBulkUpdateTriggersVersionChanged()
        {
            var c = new CollectionEx<int>();

            var eventCalled = false;
            var lastVersion = c.Version;

            c.VersionChanged += (s, e) =>
                {
                    eventCalled = true;
                };

            using (var bulkUpdate = c.BeginBulkUpdate())
            {
                c.Add(13);
                c.AddRange(new int[] { 1, 2, 3, 4, 5 });
                c.Move(1, 0);
                c.Remove(1);
                c.RemoveAt(0);
                c.RemoveRange(0, 2);
                c.Replace(5, 6);
                c.Reset(new int[] { 11, 12, 13 });

                c[0] = 5;

                Assert.IsFalse(eventCalled);
                Assert.AreEqual(lastVersion, c.Version);
            }

            // event and version should be updated now as bulkUpdate finished

            Assert.IsTrue(eventCalled);
            Assert.AreNotEqual(lastVersion, c.Version);
        }
    }
}
