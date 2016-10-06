using Foundation.Maths.Statistics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SquaredInfinity.Foundation.Extensions;
using SquaredInfinity.Foundation.Maths.Statistics;
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
    public class OnlineStatisticsTests
    {
        [TestMethod]
        public void MyTestMethod()
        {
            OnlineStatistics.Calculate(TestSamples.Sample_1_to_10.ToObservable())
                .Subscribe(x =>
                {
                    Trace.WriteLine($"c:{x.Count}, min:{x.Range.Min}, max:{x.Range.Max}, r:{x.Range}, m: {x.Mean}, v: {x.Variance}, σ: {x.StdDev}");
                });


            var sma_1 = new SimpleMovingAverage(1);
            var sma_2 = new SimpleMovingAverage(2);
            var sma_5 = new SimpleMovingAverage(5);
            var sma_10 = new SimpleMovingAverage(10);

            OnlineStatistics.Calculate(
                TestSamples.Sample_1_to_10.ToObservable(),
                defaultStatistics: KnownStatistics.All,
                additionalStatistics: new AdditionalStatistic("sma10", (x) => sma_10.Calculate(x).Select(d => (object)d)))
                .Subscribe(x =>
                {
                    Trace.WriteLine($"c:{x.Count}, min:{x.Range.Min}, max:{x.Range.Max}, r:{x.Range}, m: {x.Mean}, v: {x.Variance}, σ: {x.StdDev}");
                    Trace.WriteLine(x.GetValue<double>("sma1"));
                    Trace.WriteLine(x.GetValue<double>("sma2"));
                    Trace.WriteLine(x.GetValue<double>("sma5"));
                    Trace.WriteLine(x.GetValue<double>("sma10"));
                });
        }
    }

   
}
