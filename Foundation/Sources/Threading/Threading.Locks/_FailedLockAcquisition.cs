using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SquaredInfinity.Threading.Locks
{
    class _FailedLockAcquisition : ILockAcquisition
    {
        public static ILockAcquisition Instance = new _FailedLockAcquisition();

        public bool IsLockHeld => false;

        public void Dispose() { }
    }
}
