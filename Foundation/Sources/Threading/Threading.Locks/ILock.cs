using SquaredInfinity.Disposables;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SquaredInfinity.Threading.Locks
{  
    public class LockOwnership : DisposableObject
    {
        public ICorrelationToken CorrelationToken { get; private set; }

        internal int AcquisitionCount { get; set; }

        public LockOwnership(ICorrelationToken correlationToken, IDisposable disposeWhenLockReleased)
        {
            CorrelationToken = correlationToken ?? throw new ArgumentNullException(nameof(correlationToken));
            AcquisitionCount = 1;
        }

        protected override void DisposeManagedResources()
        {
            base.DisposeManagedResources();


        }

        #region Equality + Hash Code

        public override int GetHashCode()
        {
            return CorrelationToken.GetHashCode();
        }

        public override bool Equals(object other)
        {
            return Equals(other as LockOwnership);
        }

        public bool Equals(LockOwnership other)
        {
            if (other == null)
                return false;

            if (object.ReferenceEquals(this, other))
                return true;

            return object.Equals(CorrelationToken, other.CorrelationToken);
        }

        #endregion

        public override string ToString()
        {
            return $"{CorrelationToken.ToString()}, acquisition count: {AcquisitionCount}";
        }
    }

    public interface ILock : IWriteLock, IReadLock, ICompositeLock
    {
        long LockId { get; }
        string Name { get; }

        IReadOnlyList<LockOwnership> AllOwners { get; }

        /// <summary>
        /// Specifies recursion policy of this lock.
        /// </summary>
        LockRecursionPolicy RecursionPolicy { get; }

        LockState State { get; }

        ILockAcquisition AcquireLock(LockType lockType);
    }
}
