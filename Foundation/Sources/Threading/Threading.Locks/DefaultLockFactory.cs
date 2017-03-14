using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SquaredInfinity.Threading
{
    public class DefaultLockFactory : ILockFactory
    {        
        public ILock CreateLock()
        {
            return CreateLock("");
        }

        public ILock CreateLock(string name)
        {
            return new ReaderWriterLockSlimEx(name);
        }
    }
}
