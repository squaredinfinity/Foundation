using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SquaredInfinity.Threading
{
    public enum LockModes
    {
        Read,
        Write,
        UpgradeableRead
    }
}
