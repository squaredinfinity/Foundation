using SquaredInfinity.ComponentModel;
using SquaredInfinity.Disposables;
using SquaredInfinity.Threading;
using SquaredInfinity.Threading.Locks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SquaredInfinity
{
    public class SupportsBulkUpdate : NotifyPropertyChangedObject, ISupportsBulkUpdate
    {
        readonly ILock _lock;
        public ILock Lock => _lock;

        /// <summary>
        /// Counts the number of bulk updates in progress.
        /// When a lock is specified, normally only one update can happen at the time.
        /// When no lock is specified, several updates can happen (but they risk overriding each other changes).
        /// </summary>
        int BulkUpdateCount = 0;

        #region Constructors

        public SupportsBulkUpdate()
            : this(LockFactory.Current.CreateLock())
        { }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="updateLock">Lock to be acquired when update is in progress. When specified, only one bulk update at the time can happen. NULL if no lock to acquire, this allows muliple concurrent bulk updates (but risks them overriding each other)</param>
        public SupportsBulkUpdate(ILock updateLock)
        {
            _lock = updateLock;
        }

        #endregion

        public bool IsBulkUpdateInProgress()
        {
            return BulkUpdateCount != 0;
        }

        public IBulkUpdate BeginBulkUpdate()
        {
            OnBeforeBeginBulkUpdate();

            var write_lock = Lock?.AcquireWriteLockIfNotHeld();

            Interlocked.Increment(ref BulkUpdateCount);

            OnAfterBeginBulkUpdate();

            return new _BulkUpdate(write_lock, DisposableObject.Create(() =>
            {
                EndBulkUpdate();
            }));
        }
        
        public void EndBulkUpdate()
        {
            OnBeforeEndBulkUpdate();

            var _count = Interlocked.Decrement(ref BulkUpdateCount);
            
            if (_count == 0)
            {
                IncrementVersion();
                OnAfterEndBulkUpdate();
            }
        }

        /// <summary>
        /// Occurs when bulk update start has been requested but not yet started.
        /// </summary>
        protected void OnBeforeBeginBulkUpdate()
        { }

        /// <summary>
        /// Occurs when bulk update has started.
        /// </summary>
        protected void OnAfterBeginBulkUpdate()
        { }

        /// <summary>
        /// Occurs when update end has been requested but update not ended yet.
        /// </summary>
        protected virtual void OnBeforeEndBulkUpdate()
        { }

        /// <summary>
        /// Occurs when update has ended.
        /// </summary>
        protected virtual void OnAfterEndBulkUpdate()
        { }
    }
}
