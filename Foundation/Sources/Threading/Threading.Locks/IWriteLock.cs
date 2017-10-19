﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SquaredInfinity.Threading.Locks
{
    public interface IWriteLock
    {
        LockOwnership WriteOwner { get; }

        ILockAcquisition AcquireWriteLock();
        ILockAcquisition AcquireWriteLock(CancellationToken ct);
        ILockAcquisition AcquireWriteLock(TimeSpan timeout);
        ILockAcquisition AcquireWriteLock(TimeSpan timeout, CancellationToken ct);
        ILockAcquisition AcquireWriteLock(int millisecondsTimeout);
        ILockAcquisition AcquireWriteLock(int millisecondsTimeout, CancellationToken ct);
        ILockAcquisition AcquireWriteLock(SyncOptions options);
    }
}