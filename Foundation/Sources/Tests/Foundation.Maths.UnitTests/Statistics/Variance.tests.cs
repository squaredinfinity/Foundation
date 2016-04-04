using Microsoft.VisualStudio.TestTools.UnitTesting;
using SquaredInfinity.Foundation.Maths.Statistics;
using SquaredInfinity.Foundation.Maths;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Foundation.Maths.Statistics
{
    [TestClass]
    public partial class VarianceUnitTests
    {
        public static readonly double Sample_1_to_10_Biased_Variance = 8.25;
        public static readonly double Sample_1_to_10_Unbiased_Variance = 9.1666666666666661;

        /// <summary>
        /// General test to check if variance is calculated
        /// </summary>
        [TestMethod]
        public void Calculate__CalculatesUnbiasedVariance()
        {
            var r = Variance.Calculate(TestSamples.Sample_1_to_10);

            Assert.AreEqual(Sample_1_to_10_Unbiased_Variance, r);
        }

        [TestMethod]
        public void Calculate__BiasedVarianceMode__CalculatesBiasedVariance()
        {
            var r = Variance.Calculate(TestSamples.Sample_1_to_10, VarianceMode.Biased);

            Assert.AreEqual(Sample_1_to_10_Biased_Variance, r);
        }

        [TestMethod]
        public void Calculate__UnbiasedVarianceMode__CalculatesUnbiasedVariance()
        {
            var r = Variance.Calculate(TestSamples.Sample_1_to_10, VarianceMode.Unbiased);

            Assert.AreEqual(Sample_1_to_10_Unbiased_Variance, r);
        }









        [TestMethod]
        public void Calculate__Observable__CalcualtesUnbiasedVariance()
        {
            var samples = TestSamples.Sample_1_to_10.ToObservable();

            var r = Variance.Calculate(samples).LastAsync().Wait();

            Assert.AreEqual(Sample_1_to_10_Unbiased_Variance, r);
            
            // for step-by-step debugging
            //Variance.Calculate(samples, VarianceMode.Unbiased).Subscribe(x => { Debug.WriteLine(x); });
        }

        [TestMethod]
        public void Calculate__Observable__BiasedVarianceMode__CalcualtesBiasedVariance()
        {
            var samples = TestSamples.Sample_1_to_10.ToObservable();

            var r = Variance.Calculate(samples, VarianceMode.Biased).LastAsync().Wait();

            Assert.AreEqual(Sample_1_to_10_Biased_Variance, r);
        }

        [TestMethod]
        public void Calculate__Observable__UnbiasedVarianceMode__CalcualtesBiasedVariance()
        {
            var samples = TestSamples.Sample_1_to_10.ToObservable();

            var r = Variance.Calculate(samples, VarianceMode.Unbiased).LastAsync().Wait();

            Assert.AreEqual(Sample_1_to_10_Unbiased_Variance, r);
        }
    }
}
