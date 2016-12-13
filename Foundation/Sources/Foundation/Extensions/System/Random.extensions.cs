using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SquaredInfinity.Foundation.Extensions
{
    public static class RandomExtensions
    {
        /// <summary>
        /// Returns an Int32 with a random value across the entire range of
        /// possible values.
        /// </summary>
        /// <remarks>
        /// default Random.Next() does not include int32.Max in results.
        /// this implementation will include int32.Max
        /// </remarks>
        public static int NextInt32(this Random r)
        {
            unchecked
            {
                int firstBits = r.Next(0, 1 << 4) << 28;
                int lastBits = r.Next(0, 1 << 28);

                return firstBits | lastBits;
            }
        }

        public static decimal NextDecimalInteger(this Random rng)
        {
            return new decimal(rng.NextInt32(),
                               rng.NextInt32(),
                               rng.NextInt32(),
                               rng.Next(0, 2) == 1,
                               0);
        }
    }
}
