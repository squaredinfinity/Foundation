using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SquaredInfinity.Threading.Locks
{  
    public interface ILock : IWriteLock, IReadLock
    {
        long LockId { get; }
        string Name { get; }

        /// <summary>
        /// Specifies recursion policy of this lock.
        /// </summary>
        LockRecursionPolicy RecursionPolicy { get; }
    }
}
