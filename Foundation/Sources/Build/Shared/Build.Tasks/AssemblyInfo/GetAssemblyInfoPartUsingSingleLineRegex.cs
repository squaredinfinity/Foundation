using Microsoft.Build.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace SquaredInfinity.Build.Tasks.AssemblyInfo
{
    public abstract class GetAssemblyInfoPartUsingSingleLineRegex : CustomBuildTask
    {
        public string PartRegex { get; set; }
        [Required]
        public string AssemblyInfoPath { get; set; }

        protected override bool DoExecute()
        {
            if (!File.Exists(AssemblyInfoPath))
            {
                LogError($"File {AssemblyInfoPath} could not be found.");

                return false;
            }

            LogVerbose($"Searching for part using regex {PartRegex} in {AssemblyInfoPath}...");

            using (var sr = new StreamReader(AssemblyInfoPath))
            {
                var line = string.Empty;

                var regex = new Regex(PartRegex);

                bool success = false;

                while ((line = sr.ReadLine()) != null)
                {
                    var match = regex.Match(line);

                    if (!match.Success)
                        continue;

                    if (TryProcessSingleLine(line, match))
                    {
                        success = true;
                        LogVerbose($"Found Part: {match.Value} using regex {PartRegex} in {AssemblyInfoPath}.");
                        break;
                    }
                }

                return success;
            }
        }

        protected abstract bool TryProcessSingleLine(string line, Match match);
    }
}
