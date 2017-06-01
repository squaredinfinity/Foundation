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

        public ILockAcquisition AcquireReadLock() => new _DummyLockAcquisition();
        public ILockAcquisition AcquireReadLock(CancellationToken ct) => new _DummyLockAcquisition();
        public ILockAcquisition AcquireReadLock(int millisecondsTimeout) => new _DummyLockAcquisition();
        public ILockAcquisition AcquireReadLock(int millisecondsTimeout, CancellationToken ct) => new _DummyLockAcquisition();
        public ILockAcquisition AcquireReadLock(TimeSpan timeout) => new _DummyLockAcquisition();
        public ILockAcquisition AcquireReadLock(TimeSpan timeout, CancellationToken ct) => new _DummyLockAcquisition();
        public ILockAcquisition AcquireReadLock(SyncOptions options) => new _DummyLockAcquisition();


        public ILockAcquisition AcquireWriteLock() => new _DummyLockAcquisition();
        public ILockAcquisition AcquireWriteLock(CancellationToken ct) => new _DummyLockAcquisition();
        public ILockAcquisition AcquireWriteLock(int millisecondsTimeout) => new _DummyLockAcquisition();
        public ILockAcquisition AcquireWriteLock(int millisecondsTimeout, CancellationToken ct) => new _DummyLockAcquisition();
        public ILockAcquisition AcquireWriteLock(TimeSpan timeout) => new _DummyLockAcquisition();
        public ILockAcquisition AcquireWriteLock(TimeSpan timeout, CancellationToken ct) => new _DummyLockAcquisition();
        public ILockAcquisition AcquireWriteLock(SyncOptions options) => new _DummyLockAcquisition();


        public async Task<ILockAcquisition> AcquireReadLockAsync() => await Task.FromResult(new _DummyLockAcquisition());
        public async Task<ILockAcquisition> AcquireReadLockAsync(CancellationToken ct) => await Task.FromResult(new _DummyLockAcquisition());
        public async Task<ILockAcquisition> AcquireReadLockAsync(int millisecondsTimeout) => await Task.FromResult(new _DummyLockAcquisition());
        public async Task<ILockAcquisition> AcquireReadLockAsync(int millisecondsTimeout, CancellationToken ct) => await Task.FromResult(new _DummyLockAcquisition());
        public async Task<ILockAcquisition> AcquireReadLockAsync(TimeSpan timeout) => await Task.FromResult(new _DummyLockAcquisition());
        public async Task<ILockAcquisition> AcquireReadLockAsync(TimeSpan timeout, CancellationToken ct) => await Task.FromResult(new _DummyLockAcquisition());
        public async Task<ILockAcquisition> AcquireReadLockAsync(AsyncOptions options) => await Task.FromResult(new _DummyLockAcquisition());


        public async Task<ILockAcquisition> AcquireWriteLockAsync() => await Task.FromResult(new _DummyLockAcquisition());
        public async Task<ILockAcquisition> AcquireWriteLockAsync(CancellationToken ct) => await Task.FromResult(new _DummyLockAcquisition());
        public async Task<ILockAcquisition> AcquireWriteLockAsync(int millisecondsTimeout) => await Task.FromResult(new _DummyLockAcquisition());
        public async Task<ILockAcquisition> AcquireWriteLockAsync(int millisecondsTimeout, CancellationToken ct) => await Task.FromResult(new _DummyLockAcquisition());
        public async Task<ILockAcquisition> AcquireWriteLockAsync(TimeSpan timeout) => await Task.FromResult(new _DummyLockAcquisition());
        public async Task<ILockAcquisition> AcquireWriteLockAsync(TimeSpan timeout, CancellationToken ct) => await Task.FromResult(new _DummyLockAcquisition());
        public async Task<ILockAcquisition> AcquireWriteLockAsync(AsyncOptions options) => await Task.FromResult(new _DummyLockAcquisition());

        public void AddChild(IAsyncLock childLock) { }
        public void RemoveChild(IAsyncLock childLock) { }
        public void AddChild(ILock childLock) { }
        public void RemoveChild(ILock childLock) { }
    }
}
