using SquaredInfinity.Diagnostics.ContextDataCollectors;
using System;
using System.Collections.Generic;
using System.Deployment.Application;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using SquaredInfinity.Extensions;
using System.Reflection;
using System.Globalization;
using System.ComponentModel;
using System.Diagnostics;
using SquaredInfinity.Diagnostics;

namespace SquaredInfinity.ContextDataCollectors
{
    /// <summary>
    /// Collects envoronment data.
    /// Once initialised, objects of this type should not be modified (i.e. they should be replaced by new instances).
    /// </summary>
    public partial class EnvironmentDataCollector : ContextDataCollector
    {
        bool? _applicationIsNetworkDeployed;

        /// <summary>
        /// Calling ApplicationDeployment.IsNetworkDeployed is slow when app is not network deployed.
        /// Since the deployment status cannot change once application is run, we can cache first result here and always deliver
        /// the same value without performing full checks.
        /// </summary>
        bool ApplicationIsNetworkDeployed
        {
            get
            {
                //! If called from multiple threads, ApplicationDeployment.IsNetworkDeployed may be checked several times
                //  (once for each thread if entered before _applicationIsNetworkDeployed is assigned)
                //  This is by design 
                //      possible performance impact is tiny and short-lived 
                //      compared to long-lived performance impact of introducing locking here   
                if (_applicationIsNetworkDeployed == null)
                    _applicationIsNetworkDeployed = ApplicationDeployment.IsNetworkDeployed;

                return _applicationIsNetworkDeployed.Value;
            }
        }

        public override bool TryGetData(IDataRequest request, IDataCollectionContext cx, out object result)
        {
            try
            {
                result = null;

                if (request.Data == "Application.Version")
                {
                    if (ApplicationIsNetworkDeployed)
                    {
                        result = ApplicationDeployment.CurrentDeployment.CurrentVersion;
                        return true;
                    }

                    var entryAssembly = Assembly.GetEntryAssembly();

                    if (entryAssembly != null)
                    {
                        result = entryAssembly.GetName().Version;
                        return true;
                    }
                }

                //# Current Thread
                if (request.Data == "Thread.Id")
                {
                    result = cx.CurrentThread.ManagedThreadId;
                    return true;
                }

                if (request.Data == "Thread.Name")
                {
                    result = cx.CurrentThread.Name;
                    return true;
                }

                if (request.Data == "Thread.ThreadState")
                {
                    result = cx.CurrentThread.ThreadState;
                    return true;
                }

                if (request.Data == "Thread.IsBackground")
                {
                    result = cx.CurrentThread.IsBackground;
                    return true;
                }

                if (request.Data == "Thread.UICulture")
                {
                    result = CultureInfo.CurrentUICulture.EnglishName;
                    return true;
                }

                if (request.Data == "Thread.Culture")
                {
                    result = CultureInfo.CurrentCulture.EnglishName;
                    return true;
                }

                //# Time

                if (request.Data == "DateTime.Utc")
                {
                    result = DateTime.UtcNow;
                    return true;
                }

                if (request.Data == "DateTime.Local")
                {
                    result = DateTime.Now;
                    return true;
                }

                //# Environment
                if (request.Data == "Environment.CommandLineArgs")
                {
                    result = string.Join(
                            " ",
                            Environment.GetCommandLineArgs()
                            .Select(s =>
                            {
                                if (s.Contains(" "))
                                    return string.Format("\"{0}\"", s);
                                else return s;
                            }));

                    return true;
                }

                if (request.Data == "Environment.Version")
                {
                    result = Environment.Version;
                    return true;
                }

                if (request.Data == "Environment.HasShutdownStarted")
                {
                    result = Environment.HasShutdownStarted;
                    return true;
                }

                if (request.Data == "Environment.OSVersion")
                {
                    result = Environment.OSVersion;
                    return true;
                }

                if (request.Data == "Environment.OSVersion.Platform")
                {
                    result = Environment.OSVersion.Platform;
                    return true;
                }

                if (request.Data == "Environment.OSVersion.ServicePack")
                {
                    result = Environment.OSVersion.ServicePack;
                    return true;
                }

                if (request.Data == "Environment.OSVersion.Version")
                {
                    result = Environment.OSVersion.Version;
                    return true;
                }

                if (request.Data == "Environment.CurrentDirectory")
                {
                    result = Environment.CurrentDirectory;
                    return true;
                }

                if (request.Data == "Environment.SystemDirectory")
                {
                    result = Environment.SystemDirectory;
                    return true;
                }

                if (request.Data == "Environment.Is64BitOperatingSystem")
                {
                    result = Environment.Is64BitOperatingSystem;
                    return true;
                }

                if (request.Data == "Environment.Is64BitProcess")
                {
                    result = Environment.Is64BitProcess;
                    return true;
                }

                if (request.Data == "Environment.MachineName")
                {
                    result = Environment.MachineName;
                    return true;
                }

                if (request.Data == "Environment.ProcessorCount")
                {
                    result = Environment.ProcessorCount;
                    return true;
                }

                if (request.Data == "Environment.SystemPageSizeMB")
                {
                    result = Environment.SystemPageSize.ToMegaBytes();
                    return true;
                }

                if (request.Data == "Environment.SystemPageSizeMiB")
                {
                    result = Environment.SystemPageSize.ToMebiBytes();
                    return true;
                }

                if (request.Data == "Environment.UserDomainName")
                {
                    result = Environment.UserDomainName;
                    return true;
                }

                if (request.Data == "Environment.UserName")
                {
                    result = Environment.UserName;
                    return true;
                }

                if (request.Data == "Environment.UserInteractive")
                {
                    result = Environment.UserInteractive;
                    return true;
                }

                //# Current Process

                if (request.Data == "Process.Id")
                {
                    result = cx.CurrentProcess.Id;
                    return true;
                }

                if (request.Data == "Process.PagedMemorySize (MB)")
                {
                    result = cx.CurrentProcess.PagedMemorySize64.ToMegaBytes();
                    return true;
                }

                if (request.Data == "Process.MaxWorkingSet (MB)")
                {
                    result = cx.CurrentProcess.MaxWorkingSet.ToInt64().ToMegaBytes();
                    return true;
                }

                if (request.Data == "Process.NonpagedSystemMemorySize (MB)")
                {
                    result = cx.CurrentProcess.NonpagedSystemMemorySize64.ToMegaBytes();
                    return true;
                }

                if (request.Data == "Process.PagedSystemMemorySize (MB)")
                {
                    result = cx.CurrentProcess.PagedSystemMemorySize64.ToMegaBytes();
                    return true;
                }

                if (request.Data == "Process.PrivateMemorySize (MB)")
                {
                    result = cx.CurrentProcess.PrivateMemorySize64.ToMegaBytes();
                    return true;
                }

                if (request.Data == "Process.VirtualMemorySize (MB)")
                {
                    result = cx.CurrentProcess.VirtualMemorySize64.ToMegaBytes();
                    return true;
                }

                if (request.Data == "Process.WorkingSet (MB)")
                {
                    result = cx.CurrentProcess.WorkingSet64.ToMegaBytes();
                    return true;
                }

                if (request.Data == "Process.PagedMemorySize (MiB)")
                {
                    result = cx.CurrentProcess.PagedMemorySize64.ToMebiBytes();
                    
                    return true;
                }

                if (request.Data == "Process.PagedMemorySize (MB)")
                {
                    result = cx.CurrentProcess.PagedMemorySize64.ToMegaBytes();
                    return true;
                }

                if (request.Data == "Process.MaxWorkingSet (MiB)")
                {
                    result = cx.CurrentProcess.MaxWorkingSet.ToInt64().ToMebiBytes();
                    return true;
                }

                
                if (request.Data == "Process.NonpagedSystemMemorySize (MiB)")
                {
                    result = cx.CurrentProcess.NonpagedSystemMemorySize64.ToMebiBytes();
                    return true;
                }

                if (request.Data == "Process.NonpagedSystemMemorySize (MB)")
                {
                    result = cx.CurrentProcess.NonpagedSystemMemorySize64.ToMegaBytes();
                    return true;
                }

                if (request.Data == "Process.PagedSystemMemorySize (MiB)")
                {
                    result = cx.CurrentProcess.PagedSystemMemorySize64.ToMebiBytes();
                    return true;
                }

                if (request.Data == "Process.PagedSystemMemorySize (MB)")
                {
                    result = cx.CurrentProcess.PagedSystemMemorySize64.ToMegaBytes();
                    return true;
                }

                if (request.Data == "Process.PrivateMemorySize (MiB)")
                {
                    result = cx.CurrentProcess.PrivateMemorySize64.ToMebiBytes();
                    return true;
                }

                if (request.Data == "Process.PrivateMemorySize (MB)")
                {
                    result = cx.CurrentProcess.PrivateMemorySize64.ToMegaBytes();
                    return true;
                }

                if (request.Data == "Process.VirtualMemorySize (MiB)")
                {
                    result = cx.CurrentProcess.VirtualMemorySize64.ToMebiBytes();
                    return true;
                }

                if (request.Data == "Process.VirtualMemorySize (MB)")
                {
                    result = cx.CurrentProcess.VirtualMemorySize64.ToMegaBytes();
                    return true;
                }

                if (request.Data == "Process.WorkingSet (MiB)")
                {
                    result = cx.CurrentProcess.WorkingSet64.ToMebiBytes();
                    return true;
                }

                if (request.Data == "MemoryStatus.MemoryLoad")
                {
                    if (!this.MemoryStatus.IsOSSupported)
                        return false;

                    this.MemoryStatus.Refresh(TimeSpan.FromSeconds(1));

                    result = MemoryStatus.MemoryLoad;
                    return true;
                }

                if (request.Data == "MemoryStatus.AvailablePageFile (MB)")
                {
                    if (!this.MemoryStatus.IsOSSupported)
                        return false;

                    this.MemoryStatus.Refresh(TimeSpan.FromSeconds(1));

                    result = MemoryStatus.AvailablePageFile.ToMegaBytes();
                    return true;
                }

                if (request.Data == "MemoryStatus.AvailablePhysical (MB)")
                {
                    if (!this.MemoryStatus.IsOSSupported)
                        return false;

                    this.MemoryStatus.Refresh(TimeSpan.FromSeconds(1));

                    result = MemoryStatus.AvailablePhysical.ToMegaBytes();
                    return true;
                }

                if (request.Data == "MemoryStatus.AvailableVirtual (MB)")
                {
                    if (!this.MemoryStatus.IsOSSupported)
                        return false;

                    this.MemoryStatus.Refresh(TimeSpan.FromSeconds(1));

                    result = MemoryStatus.AvailableVirtual.ToMegaBytes();
                    return true;
                }

                if (request.Data == "MemoryStatus.TotalPageFile (MB)")
                {
                    if (!this.MemoryStatus.IsOSSupported)
                        return false;

                    this.MemoryStatus.Refresh(TimeSpan.FromSeconds(1));

                    result = MemoryStatus.TotalPageFile.ToMegaBytes();
                    return true;
                }

                if (request.Data == "MemoryStatus.TotalPhysical (MB)")
                {
                    if (!this.MemoryStatus.IsOSSupported)
                        return false;

                    this.MemoryStatus.Refresh(TimeSpan.FromSeconds(1));

                    result = MemoryStatus.TotalPhysical.ToMegaBytes();
                    return true;
                }

                if (request.Data == "MemoryStatus.TotalVirtual (MB)")
                {
                    if (!this.MemoryStatus.IsOSSupported)
                        return false;

                    this.MemoryStatus.Refresh(TimeSpan.FromSeconds(1));

                    result = MemoryStatus.TotalVirtual.ToMegaBytes(); ;
                    return true;
                }

                if (request.Data == "Process.UpTime")
                {
                    result = ((TimeSpan)(DateTime.Now - cx.CurrentProcess.StartTime)).ToString();
                    return true;
                }

                if (request.Data == "Deployment.IsNetworkDeployed")
                {
                    result = ApplicationIsNetworkDeployed;
                    return true;
                }

                if (request.Data == "Deployment.ActivationUri")
                {
                    if (!ApplicationIsNetworkDeployed)
                        return false;

                    result = ApplicationDeployment.CurrentDeployment.ActivationUri;
                    return true;
                }

                if (request.Data == "Deployment.CurrentVersion")
                {
                    if (!ApplicationIsNetworkDeployed)
                        return false;

                    result = ApplicationDeployment.CurrentDeployment.CurrentVersion;
                    return true;
                }

                if (request.Data == "Deployment.DataDirectory")
                {
                    if (!ApplicationIsNetworkDeployed)
                        return false;

                    result = ApplicationDeployment.CurrentDeployment.DataDirectory;
                    return true;
                }

                if (request.Data == "Deployment.IsFirstRun")
                {
                    if (!ApplicationIsNetworkDeployed)
                        return false;

                    result = ApplicationDeployment.CurrentDeployment.IsFirstRun;
                    return true;
                }

                if (request.Data == "Deployment.TimeOfLastUpdateCheck")
                {
                    if (!ApplicationIsNetworkDeployed)
                        return false;

                    result = ApplicationDeployment.CurrentDeployment.TimeOfLastUpdateCheck;
                    return true;
                }

                if (request.Data == "Deployment.UpdatedApplicationFullName")
                {
                    if (!ApplicationIsNetworkDeployed)
                        return false;

                    result = ApplicationDeployment.CurrentDeployment.UpdatedApplicationFullName;
                    return true;
                }

                if (request.Data == "Deployment.UpdatedVersion")
                {
                    if (!ApplicationIsNetworkDeployed)
                        return false;

                    result = ApplicationDeployment.CurrentDeployment.UpdatedVersion;
                    return true;
                }

                if (request.Data == "Deployment.UpdateLocation")
                {
                    if (!ApplicationIsNetworkDeployed)
                        return false;

                    result = ApplicationDeployment.CurrentDeployment.UpdateLocation;
                    return true;
                }

                if (request.Data == "System.Processes (Top 10 by Memory)")
                {
                    result =
                        (from p in Process.GetProcesses()
                         orderby p.WorkingSet64 descending
                         select new { Name = p.ProcessName, WorkingSetMB = p.WorkingSet64.ToMegaBytes() })
                         .Take(10)
                         .ToList();

                    return true;
                }


                // TODO:
                //result.TryAddProperty("Process.IsElevated", () => shell32.IsUserAnAdmin());
                //result.TryAddProperty("Process.IntegrityLevel", () => advapi32.GetProcessIntegrityLevel());

            }
            catch (Exception ex)
            {
                
                //DiagnosticLogger.Global.in
                // TODO: log warning
            }

            result = null;
            return false;
        }

        readonly MemoryStatus MemoryStatus = new MemoryStatus();


        [DllImport("Kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal extern static bool GlobalMemoryStatusEx([In, Out] MEMORYSTATUSEX lpBuffer);

    }

    public static class EnvironmentData
    {
        public static readonly IDataRequest ApplicationVersion = new DataRequest ("Application.Version", isCached: true);
        public static readonly IDataRequest ThreadId = new DataRequest("Thread.Id");
        public static readonly IDataRequest ThreadName = new DataRequest("Thread.Name");
        public static readonly IDataRequest ThreadState = new DataRequest("Thread.State");
        public static readonly IDataRequest ThreadIsBackground = new DataRequest("Thread.IsBackground");
        public static readonly IDataRequest ThreadUICulture = new DataRequest("Thread.UICulture");
        public static readonly IDataRequest ThreadCulture = new DataRequest("Thread.Culture");
        public static readonly IDataRequest DateTimeUtc = new DataRequest("DateTime.Utc");
        public static readonly IDataRequest DateTimeLocal = new DataRequest("DateTime.Local");
        public static readonly IDataRequest EnvironmentCommandLineArgs = new DataRequest("Environment.CommandLineArgs");
        public static readonly IDataRequest EnvironmentVersion = new DataRequest("Environment.Version");
        public static readonly IDataRequest EnvironmentHasShutdownStarted = new DataRequest("Environment.HasShutdownStarted");
        public static readonly IDataRequest EnvironmentOsVersion = new DataRequest("Environment.OSVersion");
        public static readonly IDataRequest EnvironmentOsVersionPlatform = new DataRequest("Environment.OSVersion.Platform");
        public static readonly IDataRequest EnvironmentOsVersionServicePack = new DataRequest("Environment.OSVersion.ServicePack");
        public static readonly IDataRequest EnvironmentOsVersionVersion = new DataRequest("Environment.OSVersion.Version");
        public static readonly IDataRequest EnvironmentCurrentDirectory = new DataRequest("Environment.CurrentDirectory");
        public static readonly IDataRequest EnvironmentSystemDirectory = new DataRequest("Environment.SystemDirectory");
        public static readonly IDataRequest EnvironmentIs64BitOperatingSystem = new DataRequest("Environment.Is64BitOperatingSystem");
        public static readonly IDataRequest EnvironmentIs64BitProcess = new DataRequest("Environment.Is64BitProcess");
        public static readonly IDataRequest EnvironmentMachineName = new DataRequest("Environment.MachineName");
        public static readonly IDataRequest EnvironmentProcessorCount = new DataRequest("Environment.ProcessorCount");
        public static readonly IDataRequest EnvironmentSystemPageSizeMB = new DataRequest("Environment.SystemPageSizeMB");
        public static readonly IDataRequest EnvironmentUserDomainName = new DataRequest("Environment.UserDomainName");
        public static readonly IDataRequest EnvironmentUserName = new DataRequest("Environment.UserName");
        public static readonly IDataRequest EnvironmentUserInteractive = new DataRequest("Environment.UserInteractive");
        public static readonly IDataRequest ProcessPagedMemorySize = new DataRequest("Process.PagedMemorySize (MB)");
        public static readonly IDataRequest ProcessMaxWorkingSet = new DataRequest("Process.MaxWorkingSet (MB)");
        public static readonly IDataRequest ProcessNonpagedSystemMemorySize = new DataRequest("Process.NonpagedSystemMemorySize (MB)");
        public static readonly IDataRequest ProcessPagedSystemMemorySize = new DataRequest("Process.PagedSystemMemorySize (MB)");
        public static readonly IDataRequest ProcessPrivateMemorySize = new DataRequest("Process.PrivateMemorySize (MB)");
        public static readonly IDataRequest ProcessVirtualMemorySize = new DataRequest("Process.VirtualMemorySize (MB)");
        public static readonly IDataRequest ProcessWorkingSet = new DataRequest("Process.WorkingSet (MB)");
        public static readonly IDataRequest MemoryStatusMemoryLoad = new DataRequest("MemoryStatus.MemoryLoad");
        public static readonly IDataRequest MemoryStatusAvailablePageFile = new DataRequest("MemoryStatus.AvailablePageFile (MB)");
        public static readonly IDataRequest MemoryStatusAvailablePhysical = new DataRequest("MemoryStatus.AvailablePhysical (MB)");
        public static readonly IDataRequest MemoryStatusAvailableVirtual = new DataRequest("MemoryStatus.AvailableVirtual (MB)");
        public static readonly IDataRequest MemoryStatusTotalPageFile = new DataRequest("MemoryStatus.TotalPageFile (MB)");
        public static readonly IDataRequest MemoryStatusTotalPhysical = new DataRequest("MemoryStatus.TotalPhysical (MB)");
        public static readonly IDataRequest MemoryStatusTotalVirtual = new DataRequest("MemoryStatus.TotalVirtual (MB)");
        public static readonly IDataRequest ProcessUpTime = new DataRequest("Process.UpTime");
        public static readonly IDataRequest DeploymentIsNetworkDeplyed = new DataRequest("Deployment.IsNetworkDeployed");
        public static readonly IDataRequest DeploymentActivationUri = new DataRequest("Deployment.ActivationUri");
        public static readonly IDataRequest DeploymentCurrentVersion = new DataRequest("Deployment.CurrentVersion");
        public static readonly IDataRequest DeploymentDataDirectory = new DataRequest("Deployment.DataDirectory");
        public static readonly IDataRequest DeploymentIsFirstRun = new DataRequest("Deployment.IsFirstRun");
        public static readonly IDataRequest DeploymentTimeOfLastUpdateCheck = new DataRequest("Deployment.TimeOfLastUpdateCheck");
        public static readonly IDataRequest DeploymentUpdatedApplicationFullName = new DataRequest("Deployment.UpdatedApplicationFullName");
        public static readonly IDataRequest DeploymentUpdatedVersion = new DataRequest("Deployment.UpdatedVersion");
        public static readonly IDataRequest DeploymentUpdateLocation = new DataRequest("Deployment.UpdateLocation");
        public static readonly IDataRequest SystemTop10ProcessesByMemory = new DataRequest("System.Processes (Top 10 by Memory)");
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
    public class MEMORYSTATUSEX
    {
        public uint dwLength;
        public uint dwMemoryLoad;
        public ulong ullTotalPhys;
        public ulong ullAvailPhys;
        public ulong ullTotalPageFile;
        public ulong ullAvailPageFile;
        public ulong ullTotalVirtual;
        public ulong ullAvailVirtual;
        public ulong ullAvailExtendedVirtual;

        public MEMORYSTATUSEX()
        {
            this.dwLength = (uint)Marshal.SizeOf(typeof(MEMORYSTATUSEX));
        }
    }


    /// <summary>
    /// Represents memory status.
    /// see http://msdn.microsoft.com/en-us/library/windows/desktop/aa366770(v=vs.85).aspx for details
    /// </summary>
    class MemoryStatus
    {
        MEMORYSTATUSEX MemoryStatusEx;

        public bool ContainsValidData
        {
            get { return MemoryStatusEx != null; }
        }

        /// <summary>
        /// A number between 0 and 100 that specifies the approximate percentage of physical memory that is in use 
        /// (0 indicates no memory use and 100 indicates full memory use).
        /// </summary>
        public UInt32 MemoryLoad
        {
            get
            {
                return MemoryStatusEx.dwMemoryLoad;
            }
        }

        /// <summary>
        /// The amount of physical memory currently available, in bytes. 
        /// This is the amount of physical memory that can be immediately reused without having to write its contents to disk first. 
        /// It is the sum of the size of the standby, free, and zero lists.
        /// </summary>
        public ulong AvailablePhysical
        {
            get
            {
                return MemoryStatusEx.ullAvailPhys;
            }
        }

        /// <summary>
        /// The amount of unreserved and uncommitted memory currently in the user-mode portion of the virtual address space 
        /// of the calling process, in bytes.
        /// </summary>
        public ulong AvailableVirtual
        {
            get
            {
                return MemoryStatusEx.ullAvailVirtual;
            }
        }

        /// <summary>
        /// The maximum amount of memory the current process can commit, in bytes.
        /// This value is equal to or smaller than the system-wide available commit value.
        /// </summary>
        public ulong AvailablePageFile
        {
            get
            {
                return MemoryStatusEx.ullAvailPageFile;
            }
        }

        /// <summary>
        /// Private on purpose.
        /// At the moment of writing ullAvailExtendedVirutal is reserved and does not contain useful information.
        /// See http://msdn.microsoft.com/en-us/library/windows/desktop/aa366770(v=vs.85).aspx for details
        /// </summary>
        private ulong AvailableExtendedVirtual
        {
            get
            {
                return MemoryStatusEx.ullAvailExtendedVirtual;
            }
        }

        /// <summary>
        /// The amount of actual physical memory, in bytes.
        /// </summary>
        public ulong TotalPhysical
        {
            get
            {
                return MemoryStatusEx.ullTotalPhys;
            }
        }

        /// <summary>
        /// The size of the user-mode portion of the virtual address space of the calling process, in bytes. 
        /// This value depends on the type of process, the type of processor, 
        /// and the configuration of the operating system. For example, 
        /// this value is approximately 2 GB for most 32-bit processes on an x86 processor and approximately 3 GB for 32-bit processes 
        /// that are large address aware running on a system with 4-gigabyte tuning enabled.
        /// </summary>
        public ulong TotalVirtual
        {
            get
            {
                return MemoryStatusEx.ullTotalVirtual;
            }
        }

        /// <summary>
        /// The current committed memory limit for the system or the current process, whichever is smaller, in bytes. 
        /// </summary>
        public ulong TotalPageFile
        {
            get
            {
                return MemoryStatusEx.ullTotalPageFile;
            }
        }

        public bool IsOSSupported
        {
            get
            {
                return Environment.OSVersion.Version.Major > 5;
            }
        }

        DateTime LastRefreshTime = DateTime.MinValue;

        public void Refresh(TimeSpan? allowedDataAge = null)
        {
            if (allowedDataAge != null
                && (DateTime.Now - LastRefreshTime) < allowedDataAge.Value)
            {
                // no need to update, data is still valid
                return;
            }

            if (!IsOSSupported)
            {
                // TODO: log that this is not supported at the moment
            }
            else
            {
                MemoryStatusEx = new MEMORYSTATUSEX();

                if (!EnvironmentDataCollector.GlobalMemoryStatusEx(MemoryStatusEx))
                {
                    MemoryStatusEx = null;
                    throw new Win32Exception(Marshal.GetLastWin32Error());
                }
            }

            LastRefreshTime = DateTime.Now;
        }
    }

    // TODO:
    //http://msdn.microsoft.com/en-us/library/windows/desktop/ms684824(v=vs.85).aspx
}
