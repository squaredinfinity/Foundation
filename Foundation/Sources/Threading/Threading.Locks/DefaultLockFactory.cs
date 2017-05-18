using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SquaredInfinity.Threading.Locks
{
    public class DefaultLockFactory : ILockFactory
    {
        public IAsyncLock CreateAsyncLock() => CreateAsyncLock("");

        public IAsyncLock CreateAsyncLock(string name) => new AsyncLock(name);

        public ILock CreateLock() => CreateLock("");

        public ILock CreateLock(string name) => new ReaderWriterLockSlimEx(name);
    }
}
