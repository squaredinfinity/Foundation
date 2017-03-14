using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SquaredInfinity;
using System.Threading;
using SquaredInfinity.Extensions;
using SquaredInfinity.Diagnostics.TextTemplates;

namespace SquaredInfinity.Diagnostics.Sinks.File.ArchiveStrategies
{
    public class DefaultArchiveStrategy
    {
        static ILogger Logger = DiagnosticLogger.Global.CreateLoggerForType<DefaultArchiveStrategy>();

        /// <summary>
        /// Pattern which specifies full location of the Archive
        /// e.g. {AppDomain.CurrentDomain.BaseDirectory}\Logs\Archive
        /// </summary>
        public string LocationPattern { get; set; }

        /// <summary>
        /// Maximum capacity of the Archive.
        /// The Archive will remove older files once Capacity is reached.
        /// </summary>
        public int Capacity { get; set; }

        /// <summary>
        /// When true, last log file will be archived every time the application starts.
        /// When false, last log file continue being written to.
        /// </summary>
        public bool TriggerOnStartup { get; set; }

        /// <summary>
        /// When set, last log file will be archived when its size reaches the limit (in bytes).
        /// </summary>
        public long? TriggerOnFileSize { get; set; }

        /// <summary>
        /// When set, last log file will be archived when it is older than the limit.
        /// </summary>
        public TimeSpan? TriggerOnFileAge { get; set; }

        /// <summary>
        /// When set, last log file will be archived every day on a specified time (local).
        /// </summary>
        public TimeSpan? TriggerOnTime { get; set; }

        FileSink Host { get; set; }

        DirectoryInfo ArchiveLocation { get; set; }

        public DefaultArchiveStrategy()
        {
            Capacity = 3;
        }

        void RefreshArchiveLocation()
        {
            var templateService = new TemplateProcessingService();

            var archivePath = templateService.ProcessTemplate(LocationPattern);
            ArchiveLocation = new DirectoryInfo(archivePath);

            if (!ArchiveLocation.Exists)
                ArchiveLocation.Create();
        }

        public void Attach(FileSink sink)
        {
            Host = sink;

            RefreshArchiveLocation();

            Host.BeforeInitialized += Host_BeforeInitialized;
            Host.BeforeWrite += Host_BeforeWrite;
        }

        void Host_BeforeInitialized(object sender, EventArgs e)
        {
            ArchiveIfNeeded((FileSink)sender, isStartup: true);
        }

        void Host_BeforeWrite(object sender, FileSink.BeforeWriteEventArgs e)
        {
            ArchiveIfNeeded((FileSink)sender, isStartup: false, messageSize: e.MessageSize);
        }

        void ArchiveIfNeeded(FileSink sink, bool isStartup, long? messageSize = null)
        {
            if(CheckArchiveNeeded(sink, isStartup, messageSize))
            {
                TryArchive(sink, isStartup, messageSize);
            }
        }

        bool CheckArchiveNeeded(FileSink sink, bool isStartup, long? messageSize = null)
        {
            sink.LogFile.Refresh();

            if (!sink.LogFile.Exists)
                return false;

            if (isStartup && TriggerOnStartup && sink.LogFile.Length > 0)
            {
                return true;
            }

            if (TriggerOnFileSize.HasValue && messageSize.HasValue)
            {
                if (sink.LogFile.Length + messageSize.Value >= TriggerOnFileSize.Value)
                {
                    return true;
                }
            }

            if (TriggerOnFileAge.HasValue)
            {
                if ((sink.LogFile.CreationTimeUtc - DateTime.UtcNow) >= TriggerOnFileAge.Value)
                {
                    return true;
                }
            }

            // NOTE:    use current machine time, not UTC 
            //          assumption is that local time is to be used always
            //          this may cause different behavior on days when local time changes
            //          Example:
            //          British Summer Time (BST) ends at 01:00 GMT when clock is moved back 1 hour.
            //          If TriggerOnTime is set to 01:00 then log can be archive twice within 1 hour period
            //          instead of expected once within 24 hour period
            if (TriggerOnTime.HasValue 
                && DateTime.Now.TimeOfDay >= TriggerOnTime.Value
                && sink.LogFile.CreationTime.TimeOfDay < TriggerOnTime.Value)
            {
                return true;
            }

            return false;
        }

        void TryArchive(FileSink sink, bool isStartup, long? messageSize = null)
        {
            try
            {
                using (var lockToken = sink.FileLockingStrategy.AcquireLock(sink.LogFile.FullName))
                {
                    // double check if archive is still needed now that we have lock
                    if (!CheckArchiveNeeded(sink, isStartup, messageSize))
                        return;

                    EnsureFreeSlotInArchive();

                    // todo: support different archive naming strategies (e.g. numeric prefix indicating slot number in the archive)

                    var newFileName =
                        $"[{DateTime.UtcNow.ToString("yyyy-MM-dd HH-mm-ss").ToValidFileName()}]  {sink.LogFile.Name}";

                    var newFileFullPath = Path.Combine(ArchiveLocation.FullName, newFileName);

                    lockToken.CloseFileIfNeeded();

                    System.IO.File.Move(sink.LogFile.FullName, newFileFullPath);
                }
            }
            catch(Exception ex)
            {
                // todo: internal log warning
            }
        }

        /// <summary>
        /// Removes old entries if Archive is full beyond its capacity
        /// </summary>
        void EnsureFreeSlotInArchive()
        {
            // ensure archive directory
            ArchiveLocation.Refresh();

            if (!ArchiveLocation.Exists)
                ArchiveLocation.Create();

            var filesInArchive =
                (from f in ArchiveLocation.GetFiles()
                 orderby f.CreationTimeUtc descending
                 select f).ToArray();

            var numberOfFiles = filesInArchive.Length;

            // this will try to delete enough files to make space for a new entry in the Archive
            // it may fail if files are in use, in which case Archive may be left over its capacity for a while
            while(numberOfFiles-- >= Capacity)
            {
                try
                {
                    filesInArchive[numberOfFiles].Delete();
                }
                catch(Exception ex)
                {
                    Logger.Warning(
                        ex, 
                        () =>
                            $"Unable to delete {filesInArchive[numberOfFiles].FullName} from the archive");
                }
            }            
        }

    }
}
