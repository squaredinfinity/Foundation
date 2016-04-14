using Microsoft.Build.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace SquaredInfinity.Build.Tasks.AssemblyInfo
{
    public class GetAssemblyConfiguration : GetAssemblyInfoPartUsingSingleLineRegex
    {
        [Output]
        public string AssemblyConfiguration { get; set; }

        public GetAssemblyConfiguration()
        {
            PartRegex = @"AssemblyConfiguration\(""(?<config>.*)""";
        }

        protected override bool TryProcessSingleLine(string line, Match match)
        {
            var config_group = match.Groups["config"];

            if (!config_group.Success)
                return false;

            AssemblyConfiguration = config_group.Value;

            LogInformation($"Found Assembly Configuration: {AssemblyConfiguration} in {AssemblyInfoPath}.");

            return true;
        }
    }
}
