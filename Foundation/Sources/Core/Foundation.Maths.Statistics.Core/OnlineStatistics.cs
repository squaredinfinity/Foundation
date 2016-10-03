﻿using System;
using System.Collections;
using System.Collections.Generic;
using SquaredInfinity.Foundation.Extensions;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SquaredInfinity.Foundation.Maths.Statistics
{
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
            params AdditionalStatistic[] additionalStatistics)
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

            if (additionalStatistics != null)
            {
                foreach (var astat in additionalStatistics)
                {
                    all_stats =
                        all_stats.Zip<OnlineStatisticsInfo, object, OnlineStatisticsInfo>(astat.Calculate(samples), (stats_info, x) =>
                    {
                        stats_info.AdditionalStatisticsResults.AddOrUpdate(astat.Key, x);

                        return stats_info;
                    });
                }
            }

            return all_stats;
        }
    }
}
