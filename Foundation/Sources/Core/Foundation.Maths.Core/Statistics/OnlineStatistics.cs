using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SquaredInfinity.Foundation.Maths.Statistics
{
    public struct OnlineStatisticsInfo
    {
        Int64 _count;
        public Int64 Count {  get { return _count; } set { _count = value; } }

        public double Mean;
        public RangeInfo Range;
        public VarianceInfo Variance;
        public StdDevInfo StdDev;
    }

    public static class OnlineStatistics
    {
        public static IObservable<OnlineStatisticsInfo> Calculate(IObservable<double> samples)
        {
            var stats = new OnlineStatisticsInfo();

            Int64 n = 0;
            
            var count = samples.Select(x => ++n);
            var range = Range.Calculate(samples);
            var std_dev = StdDev.Calculate(samples);

            return samples.Zip(count, range, std_dev, (s, c, r, std) =>
            {
                stats.Count = c;
                stats.Range = r;
                stats.Mean = std.Variance.Mean;
                stats.Variance = std.Variance;
                stats.StdDev = std;

                return stats;
            });
        }
    }
}
