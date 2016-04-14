using Microsoft.Build.Framework;
using SquaredInfinity.Foundation.Extensions;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace SquaredInfinity.Build.Tasks.AssemblyInfo
{
    public class ConstructSemanticVersion : CustomBuildTask
    {
        [Required]
        public string Version { get; set; }
        public string PreReleaseName { get; set; }
        public bool UseDotAfterPreReleaseName { get; set; }
        public string PatchNumberFormat { get; set; }
        [Output]
        public string SemanticVersion { get; set; }

        public ConstructSemanticVersion()
        {
            UseDotAfterPreReleaseName = false;
            PatchNumberFormat = "000";
        }

        protected override bool DoExecute()
        {
            LogVerbose($"Constructing semantic version from {Version} and {PreReleaseName}");

            var ver = new Version(Version);

            if (PreReleaseName.IsNullOrEmpty())
            {
                SemanticVersion = $"{ver.Major}.{ver.Minor}.{ver.Build}";
            }
            else
            {
                if (ver.Revision == 0)
                {
                    SemanticVersion = $"{ver.Major}.{ver.Minor}.{ver.Build}-{PreReleaseName}";
                }
                else
                {
                    SemanticVersion = $"{ver.Major}.{ver.Minor}.{ver.Build}-{PreReleaseName}";

                    if (UseDotAfterPreReleaseName)
                        SemanticVersion += ".";

                    SemanticVersion += $"{ver.Revision.ToString(PatchNumberFormat)}";
                }
            }

            LogInformation($"Constructed Semantic Version: {SemanticVersion}");

            return true;
        }
    }
}
