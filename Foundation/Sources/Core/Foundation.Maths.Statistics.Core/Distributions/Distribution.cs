using SquaredInfinity.Foundation.Maths.RandomNumbers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SquaredInfinity.Foundation.Maths.Statistics.Distributions
{
    public abstract class Distribution : IDistribution
    {
        public IRandomNumberProvider RandomNumberProvider { get; private set; } = new DotNetRandomNumberProvider();

        public double GetNext()
        {
            throw new NotImplementedException();
            //return PDF(RandomNumberProvider.NextDouble());
        }

        public abstract double PDF(double x);
    }
}
