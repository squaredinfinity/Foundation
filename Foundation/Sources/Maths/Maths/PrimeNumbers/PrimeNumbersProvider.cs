using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SquaredInfinity.Foundation.Maths.PrimeNumbers
{
    public abstract class PrimeNumbersProvider : Implementation, IPrimeNumbersProvider
    {
        public static ImplementationCollection<IPrimeNumbersProvider> Providers { get; }
            = new ImplementationCollection<IPrimeNumbersProvider>();


        /// <summary>
        /// Returns all prime numbers up to n.
        /// </summary>
        /// <param name="n"></param>
        /// <returns></returns>
        public abstract IEnumerable<uint> GetPrimeNumbers(uint n);
    }
}
