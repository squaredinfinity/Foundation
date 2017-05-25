using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SquaredInfinity.Threading.Locks
{
    public interface IReadLock
    {
        bool IsReadLockHeld { get; }

        IReadOnlyList<int> ReadOwnerThreadIds { get; }

        ILockAcquisition AcquireReadLock();
        ILockAcquisition AcquireReadLock(CancellationToken ct);
        ILockAcquisition AcquireReadLock(TimeSpan timeout);
        ILockAcquisition AcquireReadLock(TimeSpan timeout, CancellationToken ct);
        ILockAcquisition AcquireReadLock(int millisecondsTimeout);
        ILockAcquisition AcquireReadLock(int millisecondsTimeout, CancellationToken ct);
        ILockAcquisition AcquireReadLock(SyncOptions options);
    }
}
