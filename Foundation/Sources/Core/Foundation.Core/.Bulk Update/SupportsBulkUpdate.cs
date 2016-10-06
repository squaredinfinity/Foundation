using SquaredInfinity.Foundation.Threading;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SquaredInfinity.Foundation
{
    public class SupportsBulkUpdate : NotifyPropertyChangedObject, ISupportsBulkUpdate
    {
        public ILock Lock { get; private set; } = new ReaderWriterLockSlimEx(LockRecursionPolicy.NoRecursion);

        protected const int STATE__NORMAL = 0;
        protected const int STATE__BULKUPDATE = 1;

        protected int State = STATE__NORMAL;

        public bool IsInBulkUpdate
        {
            get { return State == STATE__BULKUPDATE; }
        }

        public bool IsBulkUpdateInProgress()
        {
            return State == STATE__BULKUPDATE;
        }

        public IBulkUpdate BeginBulkUpdate()
        {
            var write_lock = Lock.AcquireWriteLockIfNotHeld();

            // write lock != null and not normal state => somebody else is doing update
            // make sure that we are in normal state, otherwsie there's a bug somewhere
            if (write_lock != null && Interlocked.CompareExchange(ref State, STATE__BULKUPDATE, STATE__NORMAL) != STATE__NORMAL)
            {
                throw new Exception("UNEXPECTED: Bulk Update Operation has already started");
            }

            if (write_lock == null)
            {
                // bulk update already in progress
                return null;
            }
            else
            {
                // start new bulk update
                return new BulkUpdate(this, write_lock);
            }
        }

        public void EndBulkUpdate(IBulkUpdate bulkUpdate)
        {
            if (Interlocked.CompareExchange(ref State, STATE__NORMAL, STATE__BULKUPDATE) != STATE__BULKUPDATE)
            {
                throw new Exception("UNEXPECTED: Bulk Update Operation has already ended");
            }

            IncrementVersion();

            bulkUpdate.Dispose();

            OnAfterBulkUpdate();
        }

        protected virtual void OnAfterBulkUpdate()
        { }
    }
}
