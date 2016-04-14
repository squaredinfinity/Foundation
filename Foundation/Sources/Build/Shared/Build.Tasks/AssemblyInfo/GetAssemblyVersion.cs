using Microsoft.Build.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace SquaredInfinity.Build.Tasks.AssemblyInfo
{
    public class GetAssemblyVersion : GetAssemblyInfoPartUsingSingleLineRegex
    {
        [Output]
        public string AssemblyVersion { get; set; }

        public GetAssemblyVersion()
        {
            PartRegex = @"AssemblyVersion\(""(?<version>.*)""";
        }

        protected override bool TryProcessSingleLine(string line, Match match)
        {
            var version_group = match.Groups["version"];

            if (!version_group.Success)
                return false;

            AssemblyVersion = version_group.Value;

            LogInformation($"Found Assembly Version: {AssemblyVersion} in {AssemblyInfoPath}.");

            return true;
        }
    }
}
