using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SquaredInfinity.Maths.RandomNumbers
{
    public class DotNetRandomNumberProvider : IRandomNumberProvider
    {
        readonly Random Rand;

        public DotNetRandomNumberProvider()
        {
            Rand = new Random();
        }

        public DotNetRandomNumberProvider(int seed)
        {
            Rand = new Random(seed);
        }

        public double NextDouble()
        {
            return Rand.NextDouble();
        }
    }
}
