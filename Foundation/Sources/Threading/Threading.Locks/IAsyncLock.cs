using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SquaredInfinity.Threading.Locks
{
    public interface IAsyncLock : ILock, IAsyncWriteLock, IAsyncReadLock, ICompositeAsyncLock
    {
        Task<ILockAcquisition> AcquireLockAsync(LockType lockType);
    }
}
