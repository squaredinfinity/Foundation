using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SquaredInfinity.Foundation.Maths.Statistics
{
    public static class StdDev
    {
        public static StdDevInfo Calculate(IReadOnlyList<double> samples)
        {
            return Calculate(samples, VarianceMode.Unbiased);
        }

        public static StdDevInfo Calculate(IReadOnlyList<double> samples, VarianceMode varianceMode)
        {
            return Calculate(samples, varianceMode, VarianceAlgorithm.Online);
        }

        public static StdDevInfo Calculate(IReadOnlyList<double> samples, VarianceMode varianceMode, VarianceAlgorithm varianceAlgorithm)
        {
            var std_dev = new StdDevInfo();
            std_dev.Variance = Variance.Calculate(samples, varianceMode, varianceAlgorithm);

            return std_dev;
        }

        public static IObservable<StdDevInfo> Calculate(IObservable<double> samples)
        {
            return Observable.Create<StdDevInfo>(o =>
            {
                var std_dev = new StdDevInfo();

                Variance.Calculate(samples).Subscribe(variance =>
                {
                    std_dev.Variance = variance;
                    o.OnNext(std_dev);
                });

                return Disposable.Empty;
            });
        }
    }
}
