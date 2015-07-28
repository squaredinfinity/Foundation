using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
    }
}
