using Microsoft.VisualStudio.TestTools.UnitTesting;
using SquaredInfinity.Foundation.Extensions;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SquaredInfinity.Foundation.Maths.Framework_Extensions.System
{
    [TestClass]
    public class Double__RoundToSignificantFigures
    {
        [TestMethod]
        public void CanProcessLargeNumbers()
        {
            var x = 1234567890123456.0;
            Assert.IsTrue(1234567890123460.0.IsCloseTo(x.RoundToSignificantFigures(10)));

            x = 12345678901234567.0;
            Assert.IsTrue(12345678901234600.0.IsCloseTo(x.RoundToSignificantFigures(10)));
        }

        [TestMethod]
        public void DoesntWorkWithAccuracyGreaterThanDoubleAccuracy()
        {
            //  NOTE:    
            //      double guarantees precision of 15 digits (17 internall)
            //      this number has 18 digits and cannot be rounded properly    

            try
            {
                // this should work
                var x = 12345678901234567.0;
                var y = x.RoundToSignificantFigures(15);
            }
            catch(Exception)
            { throw; }

            try
            {
                // this shouldn't
                var x = 123456789012345678.0;
                var y = x.RoundToSignificantFigures(15);
            }
            catch (OverflowException)
            { /* this was expected */ }
        }

        [TestMethod]
        public void CanProcessFractions()
        {
            var x = 0.1234567890123456;
            // 15 sig figs
            Assert.IsTrue(0.123456789012346.IsCloseTo(x.RoundToSignificantFigures(15)));
            // 14 sig figs
            Assert.IsTrue(0.12345678901235.IsCloseTo(x.RoundToSignificantFigures(14)));
        }

        [TestMethod]
        public void CanProcessSmallFractions()
        {
            var x = 0.00001234567890123456;

            // 15 sig figs
            Assert.IsTrue(0.0000123456789012346.IsCloseTo(x.RoundToSignificantFigures(15)));
            // 14 sig figs
            Assert.IsTrue(0.000012345678901235.IsCloseTo(x.RoundToSignificantFigures(14)));
        }

        [TestMethod]
        public void DoesNotChangeTheNumberIfNumberOfFiguresToReturnIsGreaterThanNumberOfDigitsInTheNumber()
        {
            var x = 2.001856;
            Assert.IsTrue(x.IsCloseTo(x.RoundToSignificantFigures(15)));
        }
    }
}
