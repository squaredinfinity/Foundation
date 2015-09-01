using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SquaredInfinity.Foundation.Collections
{
	[TestClass]
    public partial class CollectionExTests__BugFixes
    {
        [TestMethod]
        public void BUG__001__CountAmbiguity()
        {
            // when casted to ICollectionEx<>, using Count fails build due to
            // Error	Ambiguity between 'ICollection<>.Count' and 'IReadOnlyCollection<>.Count'

            var col = new CollectionEx<int>();

            var icol = col as ICollectionEx<int>;

            Assert.AreEqual(icol.Count, 0);
        }
    }
}
