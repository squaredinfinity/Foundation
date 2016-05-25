using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SquaredInfinity.Foundation.Maths
{
    [TestClass]
    public class Interval__Exapnd
    {
        [TestMethod]
        public void EmptyInterval__EmptyInterval()
        {
            var a = new Interval(-10, 10);
            var b = new Interval(-10, 10);

            a.Expand(Interval.Empty);

            Assert.IsTrue(a == b);


            a = Interval.Empty;
            b = Interval.Empty;

            a.Expand(b);

            Assert.IsTrue(a == b);
        }

        [TestMethod]
        public void BiggerInterval__ExpandsInterval()
        {
            var a = Interval.Empty;
            var b = new Interval(-10, 10);

            a.Expand(b);

            Assert.IsTrue(a == b);
        }
    }
}
