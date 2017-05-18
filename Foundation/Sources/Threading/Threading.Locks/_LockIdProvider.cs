using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SquaredInfinity.Threading.Locks
{
    static class _LockIdProvider
    {
        static long NextId = 0;

        public static long GetNextId()
        {
            // NOTE:    It is safe to keep incrementing the id, even for very long running applications

            // Interlocked.Increment handles an overflow condition by wrapping: 
            // if location = Int32.MaxValue, location + 1 = Int32.MinValue. 
            // No exception is thrown.
            return Interlocked.Increment(ref NextId);
        }
    }
}
