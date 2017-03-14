using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SquaredInfinity.Threading
{
    public static class LockFactory
    {
        public static ILockFactory Current { get; set; } = new DefaultLockFactory();
    }
}
