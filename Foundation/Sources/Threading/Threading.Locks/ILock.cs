using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SquaredInfinity.Threading.Locks
{
    public interface IUpgradeableReaderWriterLock
        : ILock, IReadLock, IUpgradeableReadLock, ICompositeLock
    { }   


    public interface ILock
    {
        bool IsWriteLockHeld { get; }

        IWriteLockAcquisition AcquireWriteLock();
        IWriteLockAcquisition AcquireWriteLockIfNotHeld();
        bool TryAcquireWriteLock(TimeSpan timeout, out IWriteLockAcquisition writeLockAcquisition);
    }

    public interface IReadLock : ILock
    {
        bool IsReadLockHeld { get; }

        IReadLockAcquisition AcquireReadLock();
        IReadLockAcquisition AcquireReadLockIfNotHeld();
        bool TryAcquireReadLock(TimeSpan timeout, out IReadLockAcquisition readLockAcquisition);
    }

    public interface IUpgradeableReadLock : ILock
    {
        bool IsUpgradeableReadLockHeld { get; }
        IUpgradeableReadLockAcquisition AcquireUpgradeableReadLock();
        bool TryAcquireUpgradeableReadLock(TimeSpan timeout, out IUpgradeableReadLockAcquisition upgradeableReadLockAcquisition);
    }

    public interface ICompositeLock
    {
        void AddChild(ILock childLock);
        void RemoveChild(ILock childLock);
    }
}
