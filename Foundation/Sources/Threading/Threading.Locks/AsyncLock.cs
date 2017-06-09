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

        readonly SemaphoreSlim InternalSemaphore = new SemaphoreSlim(1, 1);
        readonly SyncLock InternalLock = new SyncLock();

        readonly LockRecursionPolicy _recursionPolicy = LockRecursionPolicy.NoRecursion;
        public LockRecursionPolicy RecursionPolicy => _recursionPolicy;

        public LockState State
        {
            get
            {
                if (InternalSemaphore.CurrentCount == 0)
                    return LockState.Write;
                else
                    return LockState.NoLock;
            }
        }

        public long LockId { get; } = _LockIdProvider.GetNextId();
        public string Name { get; private set; }

        #region Oeners

        volatile _LockOwnership _writeOwner;
        public LockOwnership WriteOwner => _writeOwner?.CorrelationTokenToAcquisitionCount.Select(x => new LockOwnership(x.Key, x.Value)).FirstOrDefault();
        public IReadOnlyList<LockOwnership> ReadOwners
        {
            get
            {
                using (InternalLock.AcquireWriteLock())
                {
                    return _writeOwner?.CorrelationTokenToAcquisitionCount.Select(x => new LockOwnership(x.Key, x.Value)).ToArray();
                }
            }
        }
        public IReadOnlyList<LockOwnership> AllOwners
        {
            get
            {
                using (InternalLock.AcquireWriteLock())
                {
                    return _writeOwner?.CorrelationTokenToAcquisitionCount.Select(x => new LockOwnership(x.Key, x.Value)).ToArray();
                }
            }
        }

        #endregion

        static readonly string Default__Name = "";
        static readonly LockRecursionPolicy Default__RecursionPolicy = LockRecursionPolicy.NoRecursion;
        static readonly bool Default__SupportsComposition = true;

        #region Constructors

        public AsyncLock()
            : this(Default__Name, Default__RecursionPolicy, Default__SupportsComposition) { }

        public AsyncLock(string name)
            : this(name, Default__RecursionPolicy, Default__SupportsComposition) { }

        public AsyncLock(LockRecursionPolicy recursionPolicy)
            : this(Default__Name, recursionPolicy, Default__SupportsComposition) { }

        public AsyncLock(bool supportsComposition)
            : this(Default__Name, Default__RecursionPolicy, supportsComposition) { }

        public AsyncLock(
            string name,
            LockRecursionPolicy recursionPolicy,
            bool supportsComposition)
        {
            Name = name;

            _recursionPolicy = recursionPolicy;

            if (supportsComposition)
                CompositeLock = new _CompositeAsyncLock();
            else
                CompositeLock = null;
        }

        #endregion

        void _AddRecursiveWriter__NOLOCK(ICorrelationToken correlationToken)
        {
            if (_writeOwner == null) throw new InvalidOperationException($"Attempt to add recursive writer but no writer held.");
            if (!_writeOwner.ContainsAcquisition(correlationToken)) throw new InvalidOperationException($"Attempt to add recursive writer but different writer held.");

            _writeOwner.AddAcquisition(correlationToken);
        }

        void _AddWriter__NOLOCK(ICorrelationToken correlationToken, IDisposable disposeWhenReleased)
        {
            using (InternalLock.AcquireWriteLock())
            {
                if (_writeOwner != null) throw new InvalidOperationException($"Lock already has a writer.");

                _writeOwner = new _LockOwnership(disposeWhenReleased);
                _writeOwner.AddAcquisition(correlationToken);
            }
        }

        void ReleaseLock(ICorrelationToken correlationToken)
        {
            using (InternalLock.AcquireWriteLock())
            {
                if (_writeOwner == null) throw new InvalidOperationException($"Attempt to release lock which isn't held.");
                if (!_writeOwner.ContainsAcquisition(correlationToken)) throw new InvalidOperationException($"Attempt to release write lock which isn't held.");

                _writeOwner.RemoveAcquisition(correlationToken);

                if (_writeOwner.IsOwned)
                    return;

                _writeOwner.Dispose();
                _writeOwner = null;

                InternalSemaphore.Release();
            }
        }

        #region ICompositeAsyncLock

        public int ChildrenCount => CompositeLock.ChildrenCount;

        public void AddChild(IAsyncLock childLock)
        {
            if (CompositeLock == null)
                throw new NotSupportedException("This lock does not support .AddChild()");

            CompositeLock.AddChild(childLock);
        }

        public void AddChild(ILock childLock)
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

        public void RemoveChild(ILock childLock)
        {
            if (CompositeLock == null)
                throw new NotSupportedException("This lock does not support .RemoveChild()");

            CompositeLock.RemoveChild(childLock);
        }

        #endregion

        /// <summary>
        /// True if current lock acquisition attempt is recursive, false otherwise.
        /// Throws if attempt is recursive but recursion isn't allowed.
        /// </summary>
        /// <returns></returns>
        bool _IsLockAcquisitionAttemptRecursive__NOLOCK(ICorrelationToken correlationToken)
        {
            if (RecursionPolicy == LockRecursionPolicy.SupportsRecursion)
            {
                if (_writeOwner == null)
                    return false;
                return _writeOwner.ContainsAcquisition(correlationToken);
            }
            else
            {
                if (_writeOwner?.ContainsAcquisition(correlationToken) == true)
                {
                    throw new LockRecursionException("Recursive locks are not supported by a lock with No Recursion policy.");
                }
            }

            return false;
        }

        public string DebuggerDisplay => $"AsyncLock {LockId}, name: {Name.ToStringWithNullOrEmpty()}, owner: {WriteOwner}";
    }
}
