using SquaredInfinity.Disposables;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using SquaredInfinity.Extensions;
using System.Diagnostics;

namespace SquaredInfinity.Threading.Locks
{
    /// <summary>
    /// Asynchronous mutex.
    /// Supports one single writer thread.
    /// Exposes reader lock API, but only only ONE reader OR writer is allowed at a time.
    /// </summary>
    [DebuggerDisplay("{DebuggerDisplay}")]
    public partial class AsyncLock : IAsyncLock, ICompositeAsyncLock
    {
        readonly _CompositeAsyncLock CompositeLock;

        readonly internal SemaphoreSlim InternalWriteLock = new SemaphoreSlim(1, 1);

        readonly LockRecursionPolicy _recursionPolicy = LockRecursionPolicy.NoRecursion;
        public LockRecursionPolicy RecursionPolicy => _recursionPolicy;

        public long LockId { get; } = _LockIdProvider.GetNextId();
        public string Name { get; private set; }


        volatile int _writeOwnerThreadId;
        public int WriteOwnerThreadId => _writeOwnerThreadId;
        public bool IsWriteLockHeld => System.Environment.CurrentManagedThreadId == _writeOwnerThreadId;
        // TODO: Read Lock implementation
        public bool IsReadLockHeld => IsWriteLockHeld;
        public IReadOnlyList<int> ReadOwnerThreadIds => new[] { WriteOwnerThreadId };

        #region Constructors

        public AsyncLock(
            string name = "",
            LockRecursionPolicy recursionPolicy = LockRecursionPolicy.NoRecursion,
            bool supportsComposition = true)
        {
            Name = name;

            _recursionPolicy = recursionPolicy;

            if (supportsComposition)
                CompositeLock = new _CompositeAsyncLock();
            else
                CompositeLock = null;
        }

        #endregion
        
        #region ICompositeAsyncLock

        public void AddChild(IAsyncLock childLock)
        {
            if (CompositeLock == null)
                throw new NotSupportedException("This lock does not support .AddChild()");


            CompositeLock.AddChild(childLock);
        }

        public void RemoveChild(IAsyncLock childLock)
        {
            if (CompositeLock == null)
                throw new NotSupportedException("This lock does not support .RemoveChild()");

            CompositeLock.RemoveChild(childLock);
        }

        #endregion

        public static async Task<ILockAcquisition> AcquireWriteLockAsync(params IAsyncLock[] locks)
        {
            var ao = AsyncOptions.Default;

            return await AcquireWriteLockAsync(ao, locks);
        }

        public static async Task<ILockAcquisition> AcquireWriteLockAsync(AsyncOptions options, params IAsyncLock[] locks)
        {
            if (locks == null || locks.Length == 0)
                return _FailedLockAcquisition.Instance;

            var all_acquisitions =
                await 
                Task.WhenAll<ILockAcquisition>(locks.Select(x => x.AcquireWriteLockAsync(options)))
                .ConfigureAwait(options.ContinueOnCapturedContext);

            return new _CompositeLockAcqusition(all_acquisitions);
        }

        public static async Task<ILockAcquisition> AcquireReadLockAsync(params IAsyncLock[] locks)
        {
            var ao = AsyncOptions.Default;

            return await AcquireReadLockAsync(ao, locks);
        }

        public static async Task<ILockAcquisition> AcquireReadLockAsync(AsyncOptions options, params IAsyncLock[] locks)
        {
            if (locks == null || locks.Length == 0)
                return _FailedLockAcquisition.Instance;

            var all_acquisitions =
                await
                Task.WhenAll<ILockAcquisition>(locks.Select(x => x.AcquireReadLockAsync(options)))
                .ConfigureAwait(options.ContinueOnCapturedContext);

            return new _CompositeLockAcqusition(all_acquisitions);
        }

        public string DebuggerDisplay => $"AsyncLock {LockId}, name: {Name.ToStringWithNullOrEmpty()}, owner: {WriteOwnerThreadId}";
    }
}
