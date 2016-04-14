using System;
using SquaredInfinity.Foundation.Extensions;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace SquaredInfinity.Build.Tasks.NuGet
{
    public abstract class NuGetTask : CustomBuildTask
    {
        protected string NuGetExePath { get; set; }

        protected bool TryFindNuGetExePath(out string nugetexePath)
        {
            var dir = new DirectoryInfo(Environment.CurrentDirectory);

            while (dir != null)
            {
                var nuget_dir_path = Path.Combine(dir.FullName, ".nuget");
                var nuget_file = new FileInfo(Path.Combine(nuget_dir_path, "nuget.exe"));

                LogVerbose($"Checking {nuget_file.FullName}");

                if (nuget_file.Exists)
                {
                    nugetexePath = nuget_file.FullName;
                    return true;
                }

                dir = dir.Parent;
            }

            nugetexePath = null;
            return false;
        }

        bool EnsureNuGetExePath()
        {
            if (string.IsNullOrEmpty(NuGetExePath))
            {
                // try find exe by checking current and each parent directory until the root is found.
                LogVerbose("NuGetExe Path not specified. Searching..");

                var nugetexe_path = string.Empty;

                if (TryFindNuGetExePath(out nugetexe_path))
                    NuGetExePath = nugetexe_path;
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

            return true;
        }

        protected override bool DoExecute()
        {
            if (!EnsureNuGetExePath())
                return false;

            return true;
        }
    }
}
