using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using System.Threading;

namespace SquaredInfinity.Build.Tasks
{
    public abstract class CustomBuildTask : ITask
    {
        protected TaskLoggingHelper TaskLoggingHelper { get; private set; }
        
        public IBuildEngine BuildEngine { get; set; }

        public ITaskHost HostObject { get; set; }

        public int TimeOut { get; set; }

        public bool AttachDebugger { get; set; }

        public CustomBuildTask()
        {
            TimeOut = (int) TimeSpan.FromSeconds(10).TotalMilliseconds;
        }

        public bool Execute()
        {
            try
            {
                if (AttachDebugger && !Debugger.IsAttached)
                    Debugger.Launch();

                TaskLoggingHelper = new TaskLoggingHelper(this);

                return DoExecute();
            }
            catch (Exception ex)
            {
                LogError(ex);
            }

            return false;
        }

        protected void LogError(Exception ex)
        {
            TaskLoggingHelper.LogErrorFromException(ex, showStackTrace: true);
        }

        protected void LogError(string message)
        {
            TaskLoggingHelper.LogError(message);
        }

        protected void LogVerbose(string message)
        {
            TaskLoggingHelper.LogMessage(MessageImportance.Low, message);
        }

        protected void LogInformation(string message)
        {
            TaskLoggingHelper.LogMessage(MessageImportance.Normal, message);
        }

        protected void LogWaring(string message)
        {
            TaskLoggingHelper.LogWarning(message);
        }

        protected abstract bool DoExecute();
    }
}