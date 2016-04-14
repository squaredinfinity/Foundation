using SquaredInfinity.Foundation.Maths.RandomNumbers;
using SquaredInfinity.Foundation.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SquaredInfinity.Foundation.Maths.Statistics.Distributions
{
    public class NormalDistribution : IDistribution
    {
        IRandomNumberProvider RandomNumberProvider { get; set; }

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

        public double GetNext()
        {
            return Normal.Calculate(Mean, Sigma, RandomNumberProvider.NextDouble());
        }
    }
}
