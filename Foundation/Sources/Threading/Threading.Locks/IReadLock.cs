using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SquaredInfinity.Threading.Locks
{
    public class LockOwnership
    {
        public ICorrelationToken CorrelationToken { get; private set; }
        public int AcquisitionCount { get; private set; }

        public LockOwnership(ICorrelationToken ct, int acquisitionCount)
        {
            CorrelationToken = ct;
            AcquisitionCount = acquisitionCount;
        }
    }

    public interface IReadLock
    {
        IReadOnlyList<LockOwnership> ReadOwners { get; }

        ILockAcquisition AcquireReadLock();
        ILockAcquisition AcquireReadLock(CancellationToken ct);
        ILockAcquisition AcquireReadLock(TimeSpan timeout);
        ILockAcquisition AcquireReadLock(TimeSpan timeout, CancellationToken ct);
        ILockAcquisition AcquireReadLock(int millisecondsTimeout);
        ILockAcquisition AcquireReadLock(int millisecondsTimeout, CancellationToken ct);
        ILockAcquisition AcquireReadLock(SyncOptions options);
    }
}
