using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SquaredInfinity.Threading.Locks
{
    public interface ILockFactory
    {
        IAsyncLock CreateAsyncLock();
        IAsyncLock CreateAsyncLock(string name);
        IAsyncLock CreateAsyncLock(string name, LockRecursionPolicy recursionPolicy);
    }
}
