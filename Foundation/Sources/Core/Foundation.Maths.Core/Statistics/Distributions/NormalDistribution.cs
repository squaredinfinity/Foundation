using SquaredInfinity.Foundation.Maths.RandomNumbers;
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

        double Mean { get; set; }
        double Sigma { get; set; }

        public double GetNext()
        {
            return Normal.Calculate(Mean, Sigma, RandomNumberProvider.NextDouble());
        }
    }
}
