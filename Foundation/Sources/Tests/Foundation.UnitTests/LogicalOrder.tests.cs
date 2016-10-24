using Microsoft.VisualStudio.TestTools.UnitTesting;
using SquaredInfinity.Foundation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Foundation.UnitTests
{
    [TestClass]
    public class LogicalOrderTests
    {
        [TestMethod]
        public void Min__Max__Undefined()
        {
            // min < max

            Assert.IsTrue(LogicalOrder.Last < LogicalOrder.First);

            // max > min

            Assert.IsTrue(LogicalOrder.First > LogicalOrder.Last);

            // undefined < min

            Assert.IsTrue(LogicalOrder.Undefined < LogicalOrder.Last);

            // undefined < max

            Assert.IsTrue(LogicalOrder.Undefined < LogicalOrder.First);

            // undefined == undefined

            Assert.IsTrue(new LogicalOrder(LogicalOrder.Undefined.Value) == LogicalOrder.Undefined);
        }

        [TestMethod]
        public void CanDecreasePriority()
        {
            var x = new LogicalOrder(100);
            x = x.PushBackBy(50);

            var y = new LogicalOrder(100);

            Assert.IsTrue(x < y);
        }

        [TestMethod]
        public void CanIncreasePriority()
        {
            var x = new LogicalOrder(100);
            x = x.BringForwardBy(50);

            var y = new LogicalOrder(100);

            Assert.IsTrue(x > y);
        }
    }
}
