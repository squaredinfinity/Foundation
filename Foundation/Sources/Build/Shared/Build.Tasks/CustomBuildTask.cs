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

        public CustomBuildTask()
        {
            TimeOut = (int) TimeSpan.FromSeconds(10).TotalMilliseconds;
        }

        public bool Execute()
        {
            try
            {
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

namespace SquaredInfinity.Extensions
{
    public static class ProcessStartInfoExtensions
    {
        public static bool StartAndWaitForExit(this ProcessStartInfo psi, TimeSpan timeout, out int exitCode, out string standardOutput, out string standardError)
        {
            var output_builder = new StringBuilder();
            var error_builder = new StringBuilder();

            psi.RedirectStandardError = true;
            psi.RedirectStandardOutput = true;
            psi.UseShellExecute = false;

            var process = new Process();
            process.StartInfo = psi;

            using (var output_are = new AutoResetEvent(false))
            using (var error_are = new AutoResetEvent(false))
            {
                process.OutputDataReceived += (sender, e) =>
                {
                    if (e.Data == null)
                        output_are.Set();
                    else
                        output_builder.AppendLine(e.Data);
                };
                process.ErrorDataReceived += (sender, e) =>
                {
                    if (e.Data == null)
                        error_are.Set();
                    else
                        error_builder.AppendLine(e.Data);
                };

                process.Start();

                process.BeginOutputReadLine();
                process.BeginErrorReadLine();

                if (process.WaitForExit((int)timeout.TotalMilliseconds) &&
                    output_are.WaitOne(timeout) &&
                    error_are.WaitOne(timeout))
                {
                    standardOutput = output_builder.ToString();
                    standardError = error_builder.ToString();
                    exitCode = process.ExitCode;
                    return true;
                }
                else
                {
                    standardOutput = output_builder.ToString();
                    standardError = error_builder.ToString();
                    exitCode = -1;
                    return false;
                }
            }
        }
    }
}
