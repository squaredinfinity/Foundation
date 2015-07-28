using SquaredInfinity.Foundation.Diagnostics.Sinks.File.FileLockingStrategies;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SquaredInfinity.Foundation.Diagnostics.Sinks.File
{
    public interface IFileLockingStrategy: IDisposable
    {
        LockToken AcquireLock(string fullFilePath);
    }
}
