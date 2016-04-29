using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using SquaredInfinity.Foundation.Presentation.Resources;

namespace SquaredInfinity.Foundation.Maths.Parsing
{
	[TestClass]
    public class SamplePerLineParserTets
    {
        [TestMethod]
        public void ValidSamples()
        {
            var sample_stream = ResourcesManager.LoadEmbeddedResourceFromThisAssembly(@"SquaredInfinity.Foundation.Maths.Parsing", "SamplePerLine__ValidInput.txt");

            var parser = new SamplePerLineParser();
            var samples = parser.Parse(sample_stream);

            var samples_array = samples.ToArray();

            var expected_array = new double[] { 20, 25, 31, 27, 24, 34, 21, 19, 18, 26 };

            CollectionAssert.AreEqual(samples_array, expected_array);

            var v1 = Statistics.Variance.Calculate(samples_array, Statistics.VarianceMode.Unbiased, Statistics.VarianceAlgorithm.Online);
            var v2 = Statistics.Variance.Calculate(samples_array, Statistics.VarianceMode.Unbiased, Statistics.VarianceAlgorithm.SumsAndSumSquares);

            Trace.WriteLine("2");
        }
    }
}
