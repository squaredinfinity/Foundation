using SquaredInfinity.ComponentModel;
using SquaredInfinity.Disposables;
using SquaredInfinity.Threading.Locks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SquaredInfinity
{
    public partial class SupportsBulkUpdate : NotifyPropertyChangedObject, ISupportsBulkUpdate
    {
        readonly IAsyncLock _lock;
        public IAsyncLock Lock => _lock;

        /// <summary>
        /// Counts the number of bulk updates in progress.
        /// When a lock is specified, normally only one update can happen at the time.
        /// When no lock is specified, several updates can happen (but they risk overriding each other changes).
        /// </summary>
        int BulkUpdateCount = 0;

        #region Constructors

        public SupportsBulkUpdate()
            : this(LockFactory.Current.CreateAsyncLock())
        { }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="updateLock">Lock to be acquired when update is in progress. When specified, only one bulk update at the time can happen. NULL if no lock to acquire, this allows muliple concurrent bulk updates (but risks them overriding each other)</param>
        public SupportsBulkUpdate(IAsyncLock updateLock)
        {
            _lock = updateLock;
        }

        #endregion

        public bool IsBulkUpdateInProgress()
        {
            return BulkUpdateCount != 0;
        }

        void EndBulkUpdate()
        {
            var _count = Interlocked.Decrement(ref BulkUpdateCount);
            
            if (_count == 0)
            {
                IncrementVersion();
                OnEndBulkUpdate();
            }
        }

        /// <summary>
        /// Occurs when bulk update has started.
        /// </summary>
        protected void OnBeginBulkUpdate()
        { }

        /// <summary>
        /// Occurs when update has ended.
        /// </summary>
        protected virtual void OnEndBulkUpdate()
        { }
    }
}
