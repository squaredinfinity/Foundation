using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SquaredInfinity.Threading.Locks
{
    public enum ReaderWriterLockState
    {
        Write = -1,
        NoLock = 0,
        Read = 1
    }
}
