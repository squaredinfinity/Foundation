using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SquaredInfinity.Threading.Locks
{
    public interface IWriteLock
    {
        int WriteOwnerThreadId { get; }
        bool IsWriteLockHeld { get; }

        ILockAcquisition AcquireWriteLock();
        ILockAcquisition AcquireWriteLock(SyncOptions options);
    }
}
