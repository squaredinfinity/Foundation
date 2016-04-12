using Microsoft.Build.Framework;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using SquaredInfinity.Foundation.Extensions;

namespace SquaredInfinity.Build.Tasks.NuGet
{
    public class NuGetPush : CustomBuildTask
    {
        public string Source { get; set; }
        public string ApiKey { get; set; }

        public string NuGetExePath { get; set; }

        [Required]
        public string PackagePath { get; set; }

        public NuGetPush()
        {
            // Empty Source means nuget.org, or DefaultPushSource config value (per NuGet Doc)
            Source = string.Empty;
        }

        protected override bool DoExecute()
        {
            if (string.IsNullOrEmpty(NuGetExePath))
            {
                // try find exe by checking current and each parent directory until the root is found.
                LogVerbose("NuGetExe Path not specified. Searching..");

                var dir = new DirectoryInfo(Environment.CurrentDirectory);

                while (dir != null)
                {
                    var nuget_dir_path = Path.Combine(dir.FullName, ".nuget");
                    var nuget_file = new FileInfo(Path.Combine(nuget_dir_path, "nuget.exe"));

                    LogVerbose($"Checking {nuget_file.FullName}");

                    if (nuget_file.Exists)
                    {
                        NuGetExePath = nuget_file.FullName;
                        break;
                    }

                    dir = dir.Parent;
                }
            }

            if (string.IsNullOrEmpty(NuGetExePath))
            {
                LogError($"Unable to find nuget.exe. Make sure to set NuGetExePath or add .nuget/nuget.exe in parent directory.");
                return false;
            }

            if (!File.Exists(NuGetExePath))
            {
                LogError($"Provided path to nuget.exe: {NuGetExePath} is invalid. File does not exist.");
                return false;
            }

            if (!File.Exists(PackagePath))
            {
                LogError($"Provider path to nuget package {PackagePath} is invalid. File does not exist.");
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

            if(psi.StartAndWaitForExit(TimeSpan.FromSeconds(30), out exit_code, out std_output, out std_err) && std_err.IsNullOrEmpty())
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
