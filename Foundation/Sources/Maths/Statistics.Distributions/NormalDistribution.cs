using SquaredInfinity.Maths.RandomNumbers;
using SquaredInfinity.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SquaredInfinity.Maths.Statistics.Distributions
{
    public class NormalDistribution : Distribution
    {
        public double Mean { get; private set; }
        public double Sigma { get; private set; }

        public NormalDistribution(double mean, double sigma)
        {
            if (mean.IsInfinityOrNaN())
                throw new ArgumentException($"{nameof(mean)} must be a valid number");

            if (sigma.IsInfinityOrNaN())
                throw new ArgumentException($"{nameof(sigma)} must be a valid number");

            if (sigma <= 0)
                throw new ArgumentException($"{nameof(sigma)} must be a positive number");
        }

        public override double PDF(double x)
        {
            return PDF(Mean, Sigma, x);
        }

        public static double PDF(double mean, double sigma, double x)
        {
            var x1 = 1 / sigma / Math.Sqrt(2 * Math.PI);
            var x2 = (x - mean) * (x - mean) / (2 * sigma * sigma);

            return x1 * Math.Exp(-x2);
        }

        public override double CDF(double x)
        {
            throw new NotImplementedException();
        }
    }
}
