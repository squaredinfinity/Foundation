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

        public RangeInfo Range;
        public double Variance;
    }

    public static class OnlineStatistics
    {
        public static IObservable<OnlineStatisticsInfo> Calculate(IObservable<double> samples)
        {
            var stats = new OnlineStatisticsInfo();

            Int64 n = 0;

            var count = samples.Select(x => n++);
            var range = Range.Calculate(samples);
            var variance = Variance.Calculate(samples);

            return samples.CombineLatest(count, range, variance, (s, c, r, v) =>
             {
                 stats.Count = c;
                 stats.Range = r;
                 stats.Variance = v;

                 return stats;
             });
        }
    }
}
