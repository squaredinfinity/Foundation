using Microsoft.Build.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace SquaredInfinity.Build.Tasks.AssemblyInfo
{
    public class GetAssemblyFileVersion : CustomBuildTask
    {
        public string FileVersionRegex { get; set; }

        [Required]
        public string AssemblyInfoPath { get; set; }

        [Output]
        public string FileVersion { get; set; }

        public GetAssemblyFileVersion()
        {
            FileVersionRegex = @"AssemblyFileVersion\(""(?<version>.*)""";
        }

        protected override bool DoExecute()
        {
            if (!File.Exists(AssemblyInfoPath))
            {
                LogError($"File {AssemblyInfoPath} could not be found.");

                return false;
            }

            using (var sr = new StreamReader(AssemblyInfoPath))
            {
                var line = string.Empty;

                var regex = new Regex(FileVersionRegex);

                while ((line = sr.ReadLine()) != null)
                {
                    var match = regex.Match(line);

                    if (!match.Success)
                        continue;

                    var version_group = match.Groups["version"];

                    if (!version_group.Success)
                        continue;

                    FileVersion = version_group.Value;

                    LogInformation($"Found File Version: {FileVersion} in {AssemblyInfoPath}.");
                }
            }

            return true;
        }
    }
}
