using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SquaredInfinity.Foundation.Extensions
{
    [TestClass]
    public class Double__Round
    {
        [TestMethod]
        public void RoundUp()
        {
            Assert.AreEqual(0, 0.0.RoundUp());
            Assert.AreEqual(0, 0.0.RoundUp(precision: -1));
            Assert.AreEqual(0, 0.0.RoundUp(precision: 1));


            Assert.AreEqual(9, 9.0.RoundUp());
            Assert.AreEqual(10, 9.1.RoundUp());
            Assert.AreEqual(10, 9.5.RoundUp());
            Assert.AreEqual(10, 9.9.RoundUp());

            Assert.AreEqual(10, 2.0.RoundUp(precision: 1));
            Assert.AreEqual(100, 3.0.RoundUp(precision: 2));
            Assert.AreEqual(1000, 4.0.RoundUp(precision: 3));


            Assert.AreEqual(11, 11.0.RoundUp());
            Assert.AreEqual(20, 11.0.RoundUp(precision:1));
            Assert.AreEqual(100, 11.0.RoundUp(precision: 2));


            Assert.AreEqual(0, (-0.01).RoundUp());
            Assert.AreEqual(0, (-0.1).RoundUp());
            Assert.AreEqual(0, (-0.5).RoundUp());
            Assert.AreEqual(0, (-0.9).RoundUp());


            Assert.AreEqual(0, (-0.01).RoundUp(precision: 1));
            Assert.AreEqual(0, (-0.01).RoundUp(precision: 2));


            Assert.AreEqual(0, (-0.01).RoundUp(precision: -1));

            Assert.AreEqual(-0.1, (-0.11).RoundUp(precision: -1));
            Assert.AreEqual(-0.1, (-0.15).RoundUp(precision: -1));
            Assert.AreEqual(-0.1, (-0.19).RoundUp(precision: -1));

            Assert.AreEqual(-0.11, (-0.111).RoundUp(precision: -2));
            Assert.AreEqual(-0.11, (-0.115).RoundUp(precision: -2));
            Assert.AreEqual(-0.11, (-0.119).RoundUp(precision: -2));
        }

        [TestMethod]
        public void RoundDown()
        {
            Assert.AreEqual(0, 0.0.RoundDown());
            Assert.AreEqual(0, 0.0.RoundDown(precision: -1));
            Assert.AreEqual(0, 0.0.RoundDown(precision: 1));


            Assert.AreEqual(9, 9.0.RoundDown());
            Assert.AreEqual(9, 9.1.RoundDown());
            Assert.AreEqual(9, 9.5.RoundDown());
            Assert.AreEqual(9, 9.9.RoundDown());

            Assert.AreEqual(1230, 1234.0.RoundDown(precision: 1));
            Assert.AreEqual(1200, 1234.0.RoundDown(precision: 2));
            Assert.AreEqual(1000, 1234.0.RoundDown(precision: 3));


            Assert.AreEqual(123, 123.0.RoundDown());
            Assert.AreEqual(120, 123.0.RoundDown(precision: 1));
            Assert.AreEqual(100, 123.0.RoundDown(precision: 2));


            Assert.AreEqual(0, (0.01).RoundDown());
            Assert.AreEqual(0, (0.1).RoundDown());
            Assert.AreEqual(0, (0.5).RoundDown());
            Assert.AreEqual(0, (0.9).RoundDown());

            Assert.AreEqual(-1, (-0.01).RoundDown());
            Assert.AreEqual(-1, (-0.1).RoundDown());
            Assert.AreEqual(-1, (-0.5).RoundDown());
            Assert.AreEqual(-1, (-0.9).RoundDown());


            Assert.AreEqual(0, (0.01).RoundDown(precision: -1));
            Assert.AreEqual(0.01, (0.01).RoundDown(precision: -2));


            Assert.AreEqual(0.1, (0.11).RoundDown(precision: -1));
            Assert.AreEqual(0.1, (0.15).RoundDown(precision: -1));
            Assert.AreEqual(0.1, (0.19).RoundDown(precision: -1));

            Assert.AreEqual(0.11, (0.111).RoundDown(precision: -2));
            Assert.AreEqual(0.11, (0.115).RoundDown(precision: -2));
            Assert.AreEqual(0.11, (0.119).RoundDown(precision: -2));
        }
    }
}
