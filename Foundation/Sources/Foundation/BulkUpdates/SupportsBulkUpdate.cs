using SquaredInfinity.ComponentModel;
using SquaredInfinity.Threading;
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
        public ILock Lock { get; protected set; }

        int BulkUpdateCount = 0;

        public SupportsBulkUpdate()
            : this(LockFactory.Current.CreateLock())
        { }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="updateLock">Lock to be acquired when update is in progress. When specified, only one bulk update at the time can happen. NULL if no lock to acquire, this allows muliple concurrent bulk updates (but risks them overriding each other)</param>
        public SupportsBulkUpdate(ILock updateLock)
        {
            Lock = updateLock;
        }

        public bool IsBulkUpdateInProgress()
        {
            return BulkUpdateCount != 0;
        }

        public IBulkUpdate BeginBulkUpdate()
        {
            OnBeforeBeginBulkUpdate();

            var write_lock = Lock?.AcquireWriteLockIfNotHeld();

            Interlocked.Increment(ref BulkUpdateCount);

            return new BulkUpdate(this, write_lock);
        }

        protected void OnBeforeBeginBulkUpdate()
        { }

        public void EndBulkUpdate(IBulkUpdate bulkUpdate)
        {
            var _count = Interlocked.Decrement(ref BulkUpdateCount);

            bulkUpdate.Dispose();

            if (_count == 0)
            {
                IncrementVersion();
                OnAfterBulkUpdate();
            }
        }

        protected virtual void OnAfterBulkUpdate()
        { }
    }
}
