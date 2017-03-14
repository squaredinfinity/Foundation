using System;
using SquaredInfinity.Extensions;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SquaredInfinity.Maths.Statistics.Distributions
{
    public class UniformDistribution : Distribution
    {
        public double a { get; private set; }
        public double b { get; private set; }

        double b_minus_a;

        public UniformDistribution(double a, double b)
        {
            if (a.IsInfinityOrNaN())
                throw new ArgumentException(nameof(a));

            if (b.IsInfinityOrNaN())
                throw new ArgumentException(nameof(a));

            if (a.IsGreaterThanOrClose(b))
                throw new ArgumentException("failed check: a < b");

            this.a = a;
            this.b = b;

            b_minus_a = b - a;
        }

        public override double PDF(double x)
        {
            if (x.IsLessThan(a) || x.IsGreaterThan(b))
                return 0;

            return 1 / b_minus_a;
        }

        public override double CDF(double x)
        {
            if (x.IsLessThan(a))
                return 0.0;

            if (x.IsGreaterThanOrClose(b))
                return 1.0;

            return
                (x - a)
                /
                b_minus_a; // (b-a)
        }
    }
}
