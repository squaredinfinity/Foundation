using Microsoft.Build.Framework;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using SquaredInfinity.Foundation.Extensions;

namespace SquaredInfinity.Build.Tasks.NuGet
{
    public class NuGetPush : NuGetTask
    {
        public string Source { get; set; }
        public string ApiKey { get; set; }

        [Required]
        public string PackagePath { get; set; }

        public NuGetPush()
        {
            // Empty Source means nuget.org, or DefaultPushSource config value (per NuGet Doc)
            Source = string.Empty;
        }

        protected override bool DoExecute()
        {
            if (!base.DoExecute())
                return false;

            if (!File.Exists(PackagePath))
            {
                LogError($"Provided path {PackagePath} is invalid. File does not exist.");
                return false;
            }

            string arguments = "push";

            if (!string.IsNullOrWhiteSpace(Source))
                arguments += $" -source {Source}";

            arguments += $" \"{PackagePath}\"";

            if (!string.IsNullOrWhiteSpace(ApiKey))
                arguments += $" {ApiKey}";

            var psi = new ProcessStartInfo(NuGetExePath);
            psi.Arguments = arguments;

            LogInformation($"Executing:  \"{NuGetExePath}\" {psi.Arguments}");

            var std_output = string.Empty;
            var std_err = string.Empty;
            var exit_code = -1;

            if(psi.StartAndWaitForExit(TimeSpan.FromMilliseconds(TimeOut), out exit_code, out std_output, out std_err) && std_err.IsNullOrEmpty())
            {
                LogInformation($"NuGet Push complete.");
                LogVerbose("NuGet Output: " + std_output);
                return true;
            }
            else
            { 
                LogError($"NuGet Push failed with exit code {exit_code}, {std_err}");
                return false;
            }
        }
    }
}
