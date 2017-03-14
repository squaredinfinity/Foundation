using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SquaredInfinity.Threading
{
    public interface ILockFactory
    {
        ILock CreateLock();
        ILock CreateLock(string name);
    }
}
