using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SquaredInfinity.Foundation.Collections
{
	[TestClass]
    public class CollectionEx__GetSnapshot
    {
        [TestMethod]
        public void VersionChanged__CanCallGetSnapshot()
        {
            var c = new CollectionEx<int>();

            c.VersionChanged += (s, e) =>
                {
                    var _c = (CollectionEx<int>)s;

                    _c.GetSnapshot();
                };

            c.Add(1);
        }
    }
}
