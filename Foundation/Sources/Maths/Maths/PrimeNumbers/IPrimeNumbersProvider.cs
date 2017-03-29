using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SquaredInfinity.Maths.PrimeNumbers
{
#if DEBUG

    public interface IPrimeNumbersProvider
    {
        IEnumerable<uint> GetPrimeNumbers(uint n);
    }

#endif
}
