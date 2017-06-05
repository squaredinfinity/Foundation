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

        public LockState State
        {
            get
            {
                if (InternalWriteLock.CurrentCount == 0)
                    return LockState.Write;
                else
                    return LockState.NoLock;
            }
        }

        public long LockId { get; } = _LockIdProvider.GetNextId();
        public string Name { get; private set; }

        #region Oeners

        volatile LockOwnership _writeOwner;
        public LockOwnership WriteOwner => _writeOwner;
        public IReadOnlyList<LockOwnership> ReadOwners => new[] { _writeOwner };
        public IReadOnlyList<LockOwnership> AllOwners => _writeOwner == null ? new LockOwnership[0] : new[] { _writeOwner };

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

        void _AddWriter(ICorrelationToken correlationToken)
        {
            if (_writeOwner != null)
            {
                _writeOwner.AcquisitionCount++;
            }
            else
            {
                _writeOwner = new LockOwnership(correlationToken);
            }
        }

        void ReleaseLock()
        {
            if (_writeOwner == null)
                throw new InvalidOperationException($"Attempt to release lock which isn't held.");

            _writeOwner.AcquisitionCount--;

            if (_writeOwner.AcquisitionCount == 0)
            {
                _writeOwner = null;
                InternalWriteLock.Release();
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
        /// True if lock acquisition is recursive, false otherwise
        /// </summary>
        /// <returns></returns>
        bool IsLockAcquisitionRecursive(ICorrelationToken correlationToken)
        {
            if (_writeOwner == null)
                return false;

            if (RecursionPolicy == LockRecursionPolicy.SupportsRecursion)
            {
                if(_writeOwner.CorrelationToken == correlationToken)
                {
                    // lock support re-entrancy and is already owned by this thread
                    // just return
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else if (_writeOwner.CorrelationToken == correlationToken)
            {
                // cannot acquire non-recursive lock from thread which already owns it.
                throw new LockRecursionException();
            }

            return false;
        }

        public string DebuggerDisplay => $"AsyncLock {LockId}, name: {Name.ToStringWithNullOrEmpty()}, owner: {WriteOwner}";
    }
}
