using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SquaredInfinity.Diagnostics.Sinks.File.FileLockingStrategies
{
    public class LockToken : IDisposable
    {
        readonly ReaderWriterLockSlim _lock = new ReaderWriterLockSlim();
        internal ReaderWriterLockSlim Lock
        {
            get { return _lock; }
        }

        internal StreamWriter Writer { get; set; }

        public void Write(string value)
        {
            Writer.Write(value);
            Writer.Flush();  
        }

        public void Dispose()
        {
            Lock.ExitWriteLock();
        }

        internal void CloseFileIfNeeded()
        {
            if (!Lock.IsWriteLockHeld)
                throw new InvalidOperationException("File can be closed only when write lock is requested.");

            Writer.Close();
            Writer = null;
        }
    }
}
