using Microsoft.VisualStudio.TestTools.UnitTesting;
using SquaredInfinity.Foundation.Extensions;
using SquaredInfinity.Foundation.Maths.Statistics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SquaredInfinity.Foundation.Maths.Statistics
{
    [TestClass]
    public class RangeTests
    {
        [TestMethod]
        public void Calculate__EmptyList__ReturnsNaN()
        {
            var r = Range.Calculate(new double [] { });

            Assert.IsTrue(double.IsNaN(r.Range));
        }

        [TestMethod]
        public void Calculate__OneElement_Infinity__ReturnsNaN()
        {
            var r = Range.Calculate(new double[] { double.PositiveInfinity });
            Assert.IsTrue(double.IsNaN(r.Range));

            r = Range.Calculate(new double[] { double.NegativeInfinity });
            Assert.IsTrue(double.IsNaN(r.Range));
        }

        [TestMethod]
        public void Calculate__OneElement_NaN__ReturnsNaN()
        {
            var r = Range.Calculate(new double[] { double.NaN });
            Assert.IsTrue(double.IsNaN(r.Range));
        }

        [TestMethod]
        public void Calculate__NegativeAndPositiveInfinity__ReturnsNaN()
        {
            var r = Range.Calculate(new double[] { double.NegativeInfinity, double.PositiveInfinity });
            Assert.IsTrue(double.IsNaN(r.Range));
        }


        [TestMethod]
        public void Calculate__ReturnsRange()
        {
            var r = Range.Calculate(TestSamples.Sample_1_to_10);
            Assert.AreEqual(9, r.Range);
        }










        // TODO: not sure what the behavior should be yet
        //[TestMethod]
        //public void Calculate__Observable__EmptyList__ReturnsNaN()
        //{
        //    var r = new List<double> { }.ToObservable().LastAsync().Wait();

        //    Assert.IsTrue(double.IsNaN(r));
        //}

        [TestMethod]
        public void Calculate__Observable__OneElement_Infinity__ReturnsNaN()
        {
            var r = Range.Calculate(new List<double> { double.PositiveInfinity }.ToObservable()).LastAsync().Wait();
            Assert.IsTrue(double.IsNaN(r.Range));

            r = Range.Calculate(new List<double> { double.NegativeInfinity }.ToObservable()).LastAsync().Wait();
            Assert.IsTrue(double.IsNaN(r.Range));
        }

        [TestMethod]
        public void Calculate__Observable__OneElement_NaN__ReturnsNaN()
        {
            var r = Range.Calculate(new List<double> { double.NaN }.ToObservable()).LastAsync().Wait();
            Assert.IsTrue(double.IsNaN(r.Range));
        }

        [TestMethod]
        public void Calculate__Observable__NegativeAndPositiveInfinity__ReturnsNaN()
        {
            var r = Range.Calculate(new List<double> { double.NegativeInfinity, double.PositiveInfinity }.ToObservable()).LastAsync().Wait();
            Assert.IsTrue(double.IsNaN(r.Range));
        }


        [TestMethod]
        public void Calculate__Observable__ReturnsRange()
        {
            var r = Range.Calculate(TestSamples.Sample_1_to_10.ToObservable()).LastAsync().Wait();
            Assert.AreEqual(9, r.Range);
        }
    }
}
