using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SquaredInfinity.Threading.Locks
{
    public enum LockState
    {
        NoLock = 0,
        Read = 1,
        Write = 2
    }
}
