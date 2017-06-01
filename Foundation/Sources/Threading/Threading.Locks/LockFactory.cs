using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SquaredInfinity.Threading.Locks
{
    public static class LockFactory
    {
        public static ILockFactory Current { get; set; } = new _DefaultLockFactory();
        public static void SetCurrent(ILockFactory newLockFactory)
        {
            Current = newLockFactory;
        }
    }
}
