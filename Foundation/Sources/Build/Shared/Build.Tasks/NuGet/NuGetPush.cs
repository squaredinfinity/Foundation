using Microsoft.Build.Framework;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;

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

            psi.RedirectStandardOutput = true;
            psi.RedirectStandardError = true;
            psi.UseShellExecute = false;

            LogInformation($"Executing:  \"{NuGetExePath}\" {psi.Arguments}");

            var process = Process.Start(psi);

            if (!process.WaitForExit((int)TimeSpan.FromSeconds(30).TotalMilliseconds) || process.ExitCode < 0)
            {
                LogError($"NuGet Push failed: {process.StandardError.ReadToEnd()}");
                return false;
            }
            else
            {
                LogInformation($"NuGet Push complete.");
                LogVerbose("NuGet Output: " + process.StandardOutput.ReadToEnd());
                return true;
            }
        }
    }
}
