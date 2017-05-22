using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SquaredInfinity.Threading.Locks
{
    public partial class AsyncReaderWriterLock
    {
        public ILockAcquisition AcquireWriteLock()
        {
            throw new NotImplementedException();
        }

        public ILockAcquisition AcquireWriteLock(SyncOptions options)
        {
            throw new NotImplementedException();
        }
    }
}
