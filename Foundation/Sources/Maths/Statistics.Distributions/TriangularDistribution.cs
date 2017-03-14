using System;
using SquaredInfinity.Extensions;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace SquaredInfinity.Maths.Statistics.Distributions
{
    /// <summary>
    /// https://en.wikipedia.org/wiki/Triangular_distribution#Use_of_the_distribution
    /// </summary>
    [DebuggerDisplay("{DebuggerDisplay}")]
    public class TriangularDistribution : Distribution
    {
        public double a { get; private set; }
        public double b { get; private set; }
        public double c { get; private set; }

        double b_minus_a;
        double b_minus_c;
        double c_minus_a;

        double b_minus_a_TIMES_c_minus_a;
        double b_minus_a_TIMES_b_minus_c;

        public TriangularDistribution(double a, double b, double c)
        {
            if (a.IsInfinityOrNaN())
                throw new ArgumentException("'a' must be in range (-infinity, infinity)", nameof(a));

            if (!b.IsGreaterThan(a))
                throw new ArgumentException("'b' must be greater than 'a', that is a < b ", nameof(b));

            if (c.IsLessThan(a) || c.IsGreaterThan(b))
                throw new ArgumentException("failed check: a ≤ c ≤ b", nameof(c));

            this.a = a;
            this.b = b;
            this.c = c;

            b_minus_a = b - a;
            b_minus_c = b - c;
            c_minus_a = c - a;

            b_minus_a_TIMES_c_minus_a = b_minus_a * c_minus_a;
            b_minus_a_TIMES_b_minus_c = b_minus_a * b_minus_c;
        }

        public override double PDF(double x)
        {
            // x < a
            if (x.IsLessThan(a))
                return 0;

            // b < x
            if (b.IsLessThan(x))
                return 0;

            // x = c
            if (x.IsCloseTo(c))
                return (2 / (b_minus_a));

            // a ≤ x < c
            if (x.IsLessThan(c))
                return 
                    (2 * (x - a)) 
                    /
                    b_minus_a_TIMES_c_minus_a; // (b-a)(c-a)

            // c < x ≤ b, only remaining option
            return
                (2 * (b - x))
                /
                b_minus_a_TIMES_b_minus_c; // (b-a)(b-c)
        }

        public override double CDF(double x)
        {
            // x ≤ a
            if (x.IsLessThanOrClose(a))
                return 0;

            // b ≤ x
            if (b.IsLessThanOrClose(x))
                return 1;

            // a < x ≤ c
            if (x.IsLessThanOrClose(c))
                return
                    (x - a) * (x - a) // (x-a)^2
                    /
                    b_minus_a_TIMES_c_minus_a; // (b-a)(b-c)

            // c < b < b, only remaining option
            return
                1 -
                ((b - x) * (b - x)) // (b-x)^2
                /
                b_minus_a_TIMES_b_minus_c; // (b-a)(b-c)
        }

        public double Mode
        {
            get { return c; }
        }

        public string DebuggerDisplay
        {
            get { return $"a: {a}, b:{b}, c:{c}"; }
        }
    }
}
