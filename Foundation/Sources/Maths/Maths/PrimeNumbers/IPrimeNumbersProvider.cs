using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SquaredInfinity.Foundation.Maths.PrimeNumbers
{
    public interface IPrimeNumbersProvider
    {
        IEnumerable<uint> GetPrimeNumbers(uint n);
    }
}
