using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SquaredInfinity.Foundation.Maths
{
    [TestClass]
    public class Interval__General
    {
        [TestMethod]
        public void constructor__creates_closed_interval_by_default()
        {
            var i = new Interval(1, 100);

            Assert.IsTrue(i.IsClosed);
        }
    }
}
