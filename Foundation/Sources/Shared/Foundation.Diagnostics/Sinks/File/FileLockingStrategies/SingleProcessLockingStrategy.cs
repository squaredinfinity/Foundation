using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SquaredInfinity.Diagnostics.Sinks.File.FileLockingStrategies
{
    /// <summary>
    /// Access to the file will be guarded within a boundaries of a single process.
    /// Access to the file from other processes may cause dirty writes / reads.
    /// </summary>
    public class SingleProcessLockingStrategy : IFileLockingStrategy
    {
        public static IFileLockingStrategy Instance;

        static SingleProcessLockingStrategy()
        {
            Instance = new SingleProcessLockingStrategy();
        }

        SingleProcessLockingStrategy() {}

        int _bufferSize = 4096;
        public int BufferSize 
        {
            get { return _bufferSize; }
            set { _bufferSize = value; }
        }

        FileOptions _fileOptions = FileOptions.None;
        public FileOptions FileOptions
        {
            get { return _fileOptions; }
            set { _fileOptions = value; }
        }

        FileShare _fileShare = FileShare.ReadWrite;
        public FileShare FileShare
        {
            get { return _fileShare; }
            set { _fileShare = value; }
        }

        ConcurrentDictionary<string, LockToken> FilePathToLockTokenMappings = new ConcurrentDictionary<string, LockToken>();
        
        public LockToken AcquireLock(string fullFilePath)
        {
            var lockToken = FilePathToLockTokenMappings.GetOrAdd(fullFilePath.ToLower(), (key) =>
                {
                    var token = new LockToken();

                    var stream = new FileStream(fullFilePath, FileMode.Append, FileAccess.Write, FileShare, BufferSize, FileOptions);

                    token.Writer = new StreamWriter(stream);

                    return token;
                });

            lockToken.Lock.EnterWriteLock();

            if(lockToken.Writer == null)
            {
                var stream = new FileStream(fullFilePath, FileMode.Append, FileAccess.Write, FileShare, BufferSize, FileOptions);

                lockToken.Writer = new StreamWriter(stream);
            }

            return lockToken;
        }

        public void Dispose()
        {
        }
    }
}
