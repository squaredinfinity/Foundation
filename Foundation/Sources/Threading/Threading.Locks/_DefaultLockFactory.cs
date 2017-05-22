using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SquaredInfinity.Threading.Locks
{
    class _DefaultLockFactory : ILockFactory
    {
        public IAsyncLock CreateAsyncLock() => CreateAsyncLock("");

        public IAsyncLock CreateAsyncLock(string name, LockRecursionPolicy recursionPolicy = LockRecursionPolicy.SupportsRecursion) 
            => new AsyncLock(name, recursionPolicy);
    }
}
