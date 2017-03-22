using Microsoft.Build.Framework;
using System.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Xml.Linq;
using System.IO;
using System.Text;
using SquaredInfinity.Extensions;

namespace SquaredInfinity.Build.Tasks.NuGet
{
    public class NuGetPack : NuGetTask
    {
        string _nuSpecPath;
        [Required]
        public string NuSpecPath
        {
            get { return _nuSpecPath; }
            set
            {
                _nuSpecPath = new FileSystemEntryPath(value).FullPath;
            }
        }

        [Required]
        public string Version { get; set; }
        public string OutputDirectory { get; set; }
        public string NuSpecProperties { get; set; }
        [Output]
        public string PackageFullPath { get; set; }

        public NuGetPack()
        { }

        protected override bool DoExecute()
        {
            if (!base.DoExecute())
                return false;

            if (!File.Exists(NuSpecPath))
            {
                LogError($"Provided path {NuSpecPath} is invalid. File does not exist.");
                return false;
            }

            string arguments = "pack";

            arguments += $" {NuSpecPath}";

            arguments += $" -version {Version}";

            if(!OutputDirectory.IsNullOrEmpty())
                arguments += $" -outputdirectory \"{OutputDirectory}\"";

            if (!NuSpecProperties.IsNullOrEmpty())
                arguments += $" -properties \"{NuSpecProperties}\"";

            var psi = new ProcessStartInfo(NuGetExePath);
            psi.Arguments = arguments;

            LogInformation($"Executing:  \"{NuGetExePath}\" {psi.Arguments}");

            var std_output = string.Empty;
            var std_err = string.Empty;
            var exit_code = -1;

            if(psi.StartAndWaitForExit(TimeSpan.FromMilliseconds(TimeOut), out exit_code, out std_output, out std_err) && std_err.IsNullOrEmpty())
            {
                LogInformation($"NuGet Pack complete.");
                LogVerbose("NuGet Output: " + std_output);

                
                var output_dir = new FileSystemEntryPath(OutputDirectory).FullPath;
                
                var id =
                    (from el in XDocument.Load(NuSpecPath).Descendants().OfType<XElement>()
                     where el.Name.LocalName == "id"
                     select el)
                     .Single().Value;

                var output_file = $"{id}.{Version}.nupkg";

                PackageFullPath = Path.Combine(output_dir, output_file);
                return true;
            }
            else
            { 
                LogError($"NuGet Pack failed with exit code {exit_code}, {std_err}");
                return false;
            }
        }
    }
}
