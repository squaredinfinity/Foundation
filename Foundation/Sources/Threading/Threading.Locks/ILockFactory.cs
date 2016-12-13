using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SquaredInfinity.Foundation.Threading
{
    public interface ILockFactory
    {
        bool CollectDiagnostics { get; set; }
        ILock CreateLock();
        ILock CreateLock(string name);
    }
}
