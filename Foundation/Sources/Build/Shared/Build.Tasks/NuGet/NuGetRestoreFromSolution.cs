using Microsoft.Build.Framework;
using SquaredInfinity.Foundation.Extensions;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace SquaredInfinity.Build.Tasks.NuGet
{
    public class NuGetRestoreFromSolution : NuGetTask
    {
        [Required]
        public string SolutionPath { get; set; }

        public string Source { get; set; }

        public string MSBuildVersion { get; set; }

        public NuGetRestoreFromSolution()
        {
            TimeOut = (int)TimeSpan.FromSeconds(90).TotalMilliseconds;
        }

        protected override bool DoExecute()
        {
            if (!base.DoExecute())
                return false;

            if (!File.Exists(SolutionPath))
            {
                LogError($"Provided path {SolutionPath} is invalid. File does not exist.");
                return false;
            }

            string arguments = "restore";

            arguments += $" \"{SolutionPath}\"";

            if (!Source.IsNullOrEmpty())
                arguments += $" -source \"{Source}\"";

            if (!MSBuildVersion.IsNullOrEmpty())
                arguments += $" -MSBuildVersion {MSBuildVersion}";

            var psi = new ProcessStartInfo(NuGetExePath);
            psi.Arguments = arguments;

            LogInformation($"Executing:  \"{NuGetExePath}\" {psi.Arguments}");

            var std_output = string.Empty;
            var std_err = string.Empty;
            var exit_code = -1;

            if (psi.StartAndWaitForExit(TimeSpan.FromMilliseconds(TimeOut), out exit_code, out std_output, out std_err) && std_err.IsNullOrEmpty())
            {
                LogInformation($"NuGet Restore complete.");
                LogVerbose("NuGet Output: " + std_output);
                return true;
            }
            else
            {
                LogError($"NuGet Restore failed with exit code {exit_code}, {std_err}");
                return false;
            }
        }
    }
}
