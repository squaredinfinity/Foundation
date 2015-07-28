using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SquaredInfinity.Foundation.Extensions
{
    [TestClass]
    public class Double__Clamp
    {
        [TestMethod]
        public void Clamp__Handles_NaN_By_Default()
        {
            Assert.AreEqual(1.0, double.NaN.Clamp(1.0, 2.0));            
        }

        [TestMethod]
        public void Clamp__NaN_Handling_Can_Be_Disabled()
        {
            Assert.IsTrue(double.IsNaN(double.NaN.Clamp(1.0, 2.0, NaN_value: double.NaN)));
        }

        [TestMethod]
        public void Clamp__Handles_Positive_Infinity_By_Default()
        {
            Assert.AreEqual(2.0, double.PositiveInfinity.Clamp(1.0, 2.0));
        }

        [TestMethod]
        public void Clamp__Handles_Negative_Infinity_By_Default()
        {
            Assert.AreEqual(1.0, double.NegativeInfinity.Clamp(1.0, 2.0));
        }
    }
}
