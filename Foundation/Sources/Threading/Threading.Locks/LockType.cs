using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SquaredInfinity.Threading
{
    public enum LockType
    {
        Read = 1,
        Write = 2,
        UpgradeableRead = 3
    }
}
