using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SquaredInfinity.Threading
{
    public interface ILock
    {
        bool IsReadLockHeld { get; }
        bool IsUpgradeableReadLockHeld { get; }
        bool IsWriteLockHeld { get; }


        IReadLockAcquisition AcquireReadLock();
        IReadLockAcquisition AcquireReadLockIfNotHeld();
        bool TryAcquireReadLock(TimeSpan timeout, out IReadLockAcquisition readLockAcquisition);
        IUpgradeableReadLockAcquisition AcquireUpgradeableReadLock();
        bool TryAcquireUpgradeableReadLock(TimeSpan timeout, out IUpgradeableReadLockAcquisition upgradeableReadLockAcquisition);
        IWriteLockAcquisition AcquireWriteLock();
        IWriteLockAcquisition AcquireWriteLockIfNotHeld();
        bool TryAcquireWriteLock(TimeSpan timeout, out IWriteLockAcquisition writeLockAcquisition);

        void AddChild(ILock childLock);
        void RemoveChild(ILock childLock);
    }
}
