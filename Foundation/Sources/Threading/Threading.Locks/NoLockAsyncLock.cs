using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SquaredInfinity.Threading.Locks
{
    /// <summary>
    /// This lock never actually locks, but acts as if it does.
    /// </summary>
    public class NoLockAsyncLock : IAsyncLock
    {
        public LockRecursionPolicy RecursionPolicy => LockRecursionPolicy.SupportsRecursion;

        readonly long _lockId = _LockIdProvider.GetNextId();
        public long LockId => _lockId;

        public string Name => "";

        public bool IsReadLockHeld => true;
        public bool IsWriteLockHeld => true;

        public int WriteOwnerThreadId => -1;
        public IReadOnlyList<int> ReadOwnerThreadIds => new[] { -1 };

        public ILockAcquisition AcquireReadLock()
        {
            return new _DummyLockAcquisition();
        }

        public ILockAcquisition AcquireReadLock(SyncOptions options)
        {
            return new _DummyLockAcquisition();
        }

        public async Task<ILockAcquisition> AcquireReadLockAsync()
        {
            return await Task.FromResult(new _DummyLockAcquisition());
        }

        public async Task<ILockAcquisition> AcquireReadLockAsync(AsyncOptions options)
        {
            return await Task.FromResult(new _DummyLockAcquisition()).ConfigureAwait(options.ContinueOnCapturedContext);
        }

        public ILockAcquisition AcquireWriteLock()
        {
            return new _DummyLockAcquisition();
        }

        public ILockAcquisition AcquireWriteLock(SyncOptions options)
        {
            return new _DummyLockAcquisition();
        }

        public async Task<ILockAcquisition> AcquireWriteLockAsync()
        {
            return await Task.FromResult(new _DummyLockAcquisition());
        }

        public async Task<ILockAcquisition> AcquireWriteLockAsync(AsyncOptions options)
        {
            return await Task.FromResult(new _DummyLockAcquisition()).ConfigureAwait(options.ContinueOnCapturedContext);
        }
    }
}
