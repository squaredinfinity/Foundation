using Microsoft.Build.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace SquaredInfinity.Build.Tasks.AssemblyInfo
{
    public class GetAssemblyFileVersion : GetAssemblyInfoPartUsingSingleLineRegex
    {
        [Output]
        public string FileVersion { get; set; }

        public GetAssemblyFileVersion()
        {
            PartRegex = @"AssemblyFileVersion\(""(?<version>.*)""";
        }

        protected override bool TryProcessSingleLine(string line, Match match)
        {
            var version_group = match.Groups["version"];

            if (!version_group.Success)
                return false;

            FileVersion = version_group.Value;

            LogInformation($"Found Assembly File Version: {FileVersion} in {AssemblyInfoPath}.");

            return true;
        }
    }
}
