using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SquaredInfinity.Foundation.Maths.Statistics
{
    [TestClass]
    public class OnlineStatisticsUnitTests
    {
        [TestMethod]
        public void MyTestMethod()
        {
            OnlineStatistics.Calculate(TestSamples.Sample_1_to_10.ToObservable())
                .Subscribe(x =>
                {
                    Trace.WriteLine($"c:{x.Count}, min:{x.Range.Min}, max:{x.Range.Max}, r:{x.Range.Range}, m: {x.Variance.Mean}, v: {x.Variance}");
                });
        }
    }
}
