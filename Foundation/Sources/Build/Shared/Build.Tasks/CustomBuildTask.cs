using System;
using System.Collections.Generic;
using System.Text;

namespace SquaredInfinity.Build.Tasks
{
    public abstract class CustomBuildTask : ITask
    {
        public IBuildEngine BuildEngine { get; set; }

        public ITaskHost HostObject { get; set; }

        public bool Execute()
        {
            try
            {
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
            LogError(ex.ToString());
        }

        protected void LogError(string message)
        {
            var build_event =
                    new BuildMessageEventArgs(
                        "ERROR: " + message,
                        string.Empty,
                        this.GetType().Name,
                        MessageImportance.High);

            BuildEngine.LogMessageEvent(build_event);
        }

        protected void LogVerbose(string message)
        {
            var build_event =
                    new BuildMessageEventArgs(
                        message,
                        string.Empty,
                        this.GetType().Name,
                        MessageImportance.Low);

            BuildEngine.LogMessageEvent(build_event);
        }

        protected void LogInformation(string message)
        {
            var build_event =
                    new BuildMessageEventArgs(
                        "INFO: " + message,
                        string.Empty,
                        this.GetType().Name,
                        MessageImportance.Normal);

            BuildEngine.LogMessageEvent(build_event);
        }

        protected void LogWaring(string message)
        {
            var build_event =
                    new BuildMessageEventArgs(
                        "WARNING: " + message,
                        string.Empty,
                        this.GetType().Name,
                        MessageImportance.High);

            BuildEngine.LogMessageEvent(build_event);
        }

        protected abstract bool DoExecute();
    }
}
