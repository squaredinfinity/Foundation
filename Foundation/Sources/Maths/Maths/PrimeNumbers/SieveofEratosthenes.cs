using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SquaredInfinity.Foundation.Maths.PrimeNumbers
{
    /// <summary>
    /// https://en.wikipedia.org/wiki/Sieve_of_Eratosthenes
    /// http://primes.utm.edu/glossary/xpage/SieveOfEratosthenes.html
    /// </summary>
    public class SieveofEratosthenes : PrimeNumbersProvider
    {
        public SieveofEratosthenes()
        {
            this.Name = "Sieve of Eratosthenes";
        }

        public override IEnumerable<uint> GetPrimeNumbers(uint n)
        {
            // create list of boolean values for consecutive integers from 2 through n: (2,3,4,...,n)
            // true indicates that integer at index is a prime number, false indicates that it isn't
            // initially all numbers are considered prime, until proven otherwise

            // to avoid having to initialize the array, we'll consider false to indicate prime number
            // true - number is rejected
            // false - number is not rejected, i.e. number is prime

            var candidates = new bool[n + 1];

            // it is enough to check prime numbers <= sqrt(n)
            // not all numbers have to be checked

            var sqrt_n = (uint)Math.Floor(Math.Sqrt(n));

            for (uint i = 2; i <= sqrt_n; i++)
            {
                if (candidates[i] == false)
                {
                    yield return i;

                    for (var m = i * 2; m <= n; m += i)
                    {
                        candidates[m] = true;
                    }
                }
            }

            for (uint i = sqrt_n + 1; i <= n; i++)
            {
                if (candidates[i] == false)
                    yield return i;
            }
        }
    }
}
