using SquaredInfinity.Foundation.Diagnostics.Sinks.File.ArchiveStrategies;
using SquaredInfinity.Foundation.Diagnostics.Sinks.File.FileLockingStrategies;
using SquaredInfinity.Foundation.Diagnostics.TextTemplates;
using SquaredInfinity.Foundation.Extensions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SquaredInfinity.Foundation.Diagnostics.Sinks.File
{
    public class FileSink : Sink
    {
        static ILogger Logger = DiagnosticLogger.Global.CreateLoggerForType<FileSink>();

        /// <summary>
        /// A pattern which specifies file location (directory).
        /// </summary>
        public string FileLocationPattern { get; set; }

        /// <summary>
        /// A pattern which specifies file name.
        /// </summary>
        public string FileNamePattern { get; set; }

        /// <summary>
        /// Represents locking strategy to be used when writting to a file.
        /// </summary>
        public IFileLockingStrategy FileLockingStrategy { get; set; }

        public DefaultArchiveStrategy Archive { get; set; }
        
        public FileInfo LogFile { get; private set; }

        public event EventHandler<EventArgs> BeforeInitialized;

        /// <summary>
        /// Raised before Write to a file occurs.
        /// This can be used by Archive strategies to perform action on file before write happens.
        /// </summary>
        public event EventHandler<BeforeWriteEventArgs> BeforeWrite;
        
        public class BeforeWriteEventArgs : EventArgs
        {
            public long MessageSize { get; private set; }

            public BeforeWriteEventArgs(long messageSize)
            {
                this.MessageSize = messageSize;
            }
        }

        public override void Initialize()
        {
            if (Archive != null)
                Archive.Attach(this);
            
            var templateService = new TemplateProcessingService();

            var fileDirectoryPath = templateService.ProcessTemplate(FileLocationPattern);
            var fileName = templateService.ProcessTemplate(FileNamePattern);

            LogFile = new FileInfo(Path.Combine(fileDirectoryPath, fileName));

            Directory.CreateDirectory(fileDirectoryPath);

            if(FileLockingStrategy == null)
            {
                Logger.Warning(() => "File Locking Strategy not specified, Single Process Locking will be used for " + SinkLocation.ToString());
                FileLockingStrategy = SingleProcessLockingStrategy.Instance;
            }

            if (BeforeInitialized != null)
                BeforeInitialized(this, EventArgs.Empty);
        }

        public override void Write(IDiagnosticEvent de)
        {
            var text = Formatter.Format(de);

            if (BeforeWrite != null)
            {
                // todo: this may need to support different encoding types)
                var args = new BeforeWriteEventArgs(text.Length);

                BeforeWrite(this, args);
            }

            using (var lockToken = FileLockingStrategy.AcquireLock(LogFile.FullName))
            {
                lockToken.Write(text);
            }
        }

        public override ISinkLocation SinkLocation
        {
            get { return new SinkLocation("file", LogFile.FullName); }
        }
    }
}
