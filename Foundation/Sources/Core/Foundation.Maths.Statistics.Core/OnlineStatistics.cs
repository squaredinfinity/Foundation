using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SquaredInfinity.Foundation.Maths.Statistics
{
    public class OnlineStatisticsInfo
    {
        public UInt64 Count { get; internal set; }
        public double Mean { get; internal set; }
        public RangeInfo Range { get; internal set; }
        public double Min {  get { return Range.Min; } }
        public double Max { get { return Range.Max; } }
        public VarianceInfo Variance { get; internal set; }
        public StdDevInfo StdDev { get; internal set; }

        readonly internal Dictionary<object, object> AdditionalStatisticsResults = new Dictionary<object, object>();
        public T GetValue<T>(object key)
        {
            return (T) AdditionalStatisticsResults[key];
        }
    }

    public class AdditionalStatistic<TKey, TValue>
    {
        public TKey Key { get; private set; }
        public Func<IObservable<double>, IObservable<TValue>> Calculate { get; private set; }

        public AdditionalStatistic(TKey key, Func<IObservable<double>, IObservable<TValue>> calculate)
        {
            this.Key = key;
            this.Calculate = calculate;
        }
    }

    public enum KnownStatistics
    {
        None        = 0x0,
        Count       = 0x1,
        Min         = 0x2,
        Max         = 0x4,
        Range       = 0x8,
        Mean        = 0x10,
        Variance    = 0x20,
        StdDev      = 0x40,

        All = Count | Min | Max | Range | Mean | Variance | StdDev
    }

    public static class OnlineStatistics
    {
        public static IObservable<OnlineStatisticsInfo> Calculate(IObservable<double> samples)
        {
            return Calculate(samples, KnownStatistics.All, null);
        }

        public static IObservable<OnlineStatisticsInfo> Calculate(IObservable<double> samples, KnownStatistics defaultStatistics)
        {
            return Calculate(samples, defaultStatistics, null);
        }

        public static IObservable<OnlineStatisticsInfo> Calculate(
            IObservable<double> samples,
            KnownStatistics defaultStatistics,
            IDictionary<object, Func<IObservable<double>, IObservable<object>>> additionalStatistics)
        {
            var stats = new OnlineStatisticsInfo();
            var all_stats = samples.Select(x => stats);

            #region Count

            if (defaultStatistics.HasFlag(KnownStatistics.Count))
            {
                UInt64 n = 0;

                var count = samples.Select(x => ++n);

                all_stats =
                   all_stats.Zip<OnlineStatisticsInfo, UInt64, OnlineStatisticsInfo>(count, (stats_info, x) =>
                   {
                       stats_info.Count = x;
                       return stats_info;
                   });
            }

            #endregion

            #region Range | Min | Max

            if (defaultStatistics.HasFlag(KnownStatistics.Min) 
                || defaultStatistics.HasFlag(KnownStatistics.Max) 
                || defaultStatistics.HasFlag(KnownStatistics.Range))
            {
                var range = Range.Calculate(samples);

                all_stats =
                   all_stats.Zip<OnlineStatisticsInfo, RangeInfo, OnlineStatisticsInfo>(range, (stats_info, x) =>
                   {
                       stats.Range = x;
                       return stats_info;
                   });
            }

            #endregion

            #region Std Dev | Variance

            // std dev includes variance, no point calculating it twice

            if (defaultStatistics.HasFlag(KnownStatistics.StdDev))
            {
                var std_dev = StdDev.Calculate(samples);

                all_stats =
                   all_stats.Zip<OnlineStatisticsInfo, StdDevInfo, OnlineStatisticsInfo>(std_dev, (stats_info, x) =>
                   {
                       stats.StdDev = x;
                       stats.Variance = x.Variance;
                       return stats_info;
                   });
            }
            else if(defaultStatistics.HasFlag(KnownStatistics.Variance))
            {
                var variance = Variance.Calculate(samples);

                all_stats =
                   all_stats.Zip<OnlineStatisticsInfo, VarianceInfo, OnlineStatisticsInfo>(variance, (stats_info, x) =>
                   {
                       stats.Variance = x;
                       return stats_info;
                   });
            }

            #endregion

            foreach (var kvp in additionalStatistics)
            {
                all_stats =
                    all_stats.Zip<OnlineStatisticsInfo, object, OnlineStatisticsInfo>(kvp.Value(samples), (stats_info, x) =>
                {
                    stats_info.AdditionalStatisticsResults.Add(kvp.Key, x);

                    return stats_info;
                });
            }

            return all_stats;
        }
    }
}
