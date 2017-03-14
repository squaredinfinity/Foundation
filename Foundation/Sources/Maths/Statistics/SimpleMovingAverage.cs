using SquaredInfinity.Maths.Statistics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reactive.Linq;

namespace SquaredInfinity.Maths.Statistics
{
    public class SimpleMovingAverage
    {
        public uint N { get; set; }

        public SimpleMovingAverage()
            : this(10)
        { }

        public SimpleMovingAverage(uint n)
        {
            this.N = n;
        }

        public static IObservable<double> Calculate(uint n, IObservable<double> samples)
        {
            return new SimpleMovingAverage(n).Calculate(samples);
        }

        public virtual IObservable<double> Calculate(IObservable<double> samples)
        {
            var q = new Queue<double>();
            var sma = double.NaN;

            return samples.Select(x =>
            {
                q.Enqueue(x);

                if (double.IsNaN(sma))
                {
                    sma = x;
                    return x;
                }

                if (q.Count <= N)
                {
                    sma = q.Sum() / q.Count;
                }
                else
                {
                    sma = sma + (x / N) - (q.Dequeue() / N);
                }

                return sma;
            });
        }
    }
}
