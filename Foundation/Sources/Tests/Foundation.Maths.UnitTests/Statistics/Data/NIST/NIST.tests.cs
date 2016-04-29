using Microsoft.VisualStudio.TestTools.UnitTesting;
using SquaredInfinity.Foundation.Extensions;
using SquaredInfinity.Foundation.Maths.Parsing;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SquaredInfinity.Foundation.Maths.Statistics.Data.NIST
{
    [TestClass]
    public class NIST
    {
        [TestMethod]
        public void UnivariateSummary()
        {
            var parser = new Parsing.NISTDataFileParser();

            var tests = new[]
            {
                "PiDigits",
                "Lottery",
                "Lew",
                "Mavro",
                "Michelso",
                "NumAcc1",
                "NumAcc2",
                "NumAcc3",
                "NumAcc4"
            };

            var variance_algorithms = new[]
            {
                VarianceAlgorithm.Online,
                VarianceAlgorithm.SumsAndSumSquares,
                //VarianceAlgorithm.SumsAndSumSquaresWithShift
            };

            foreach (var t in tests)
            {
                var test = parser.GetUnivariateTest(t);

                foreach (var variance_algo in variance_algorithms)
                {
                    var v = StdDev.Calculate(test.Data, VarianceMode.Unbiased, variance_algo);

                    var msg = $"test: {t}, variance: {variance_algo}";

                    Assert.IsTrue(test.ExpectedSampleMean.IsCloseTo(v.Variance.Mean.RoundToSignificantFigures(15)), msg);
                    Assert.IsTrue(test.ExpectedSampleStdDev.IsCloseTo(v.StdDev.RoundToSignificantFigures(15)), msg);
                }
            }
        }
    }
}
