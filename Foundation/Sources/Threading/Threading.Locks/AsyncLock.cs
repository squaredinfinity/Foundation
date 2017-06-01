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

        void ReleaseLock()
        {
            _writeOwnerThreadId = -1;
            InternalWriteLock.Release();
        }
        
        #region ICompositeAsyncLock

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
        bool IsLockAcquisitionRecursive()
        {
            if (RecursionPolicy == LockRecursionPolicy.SupportsRecursion)
            {
                if (_writeOwnerThreadId == System.Environment.CurrentManagedThreadId)
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
            else if (System.Environment.CurrentManagedThreadId == WriteOwnerThreadId)
            {
                // cannot acquire non-recursive lock from thread which already owns it.
                throw new LockRecursionException();
            }

            return false;
        }

        public string DebuggerDisplay => $"AsyncLock {LockId}, name: {Name.ToStringWithNullOrEmpty()}, owner: {WriteOwnerThreadId}";
    }
}
