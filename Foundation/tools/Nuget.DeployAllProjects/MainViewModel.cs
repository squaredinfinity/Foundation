using SquaredInfinity.Foundation.Presentation.ViewModels;
using SquaredInfinity.Foundation.Extensions;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Xml.XPath;

namespace Nuget.DeployAllProjects
{
    public class MainViewModel : ViewModel
    {
        string _versionNumber;
        public string VersionNumber
        {
            get { return _versionNumber; }
            set { TrySetThisPropertyValue(ref _versionNumber, value); }
        }

        public MainViewModel()
        {
            // get version number of assemblies in solution
            // assumes default solution structure
            var asm_info = File.ReadAllText(@"../../../../AssemblyInfo.shared.cs");

            var version_number_match = Regex.Match(asm_info, @"\[assembly: AssemblyVersion\(""(?<version>.*)""");

            var version_number = new Version(version_number_match.Groups["version"].Value);

            VersionNumber = version_number.ToString() + "-beta";
        }

        public void UpdateAllProjects()
        {
            // find all Package.symbols.nuspec files in solution under deployment project
            foreach(var file in Directory.EnumerateFiles("../../../../Deployment", "Package.symbols.nuspec", SearchOption.AllDirectories))
            {
                var xml = XDocument.Parse(File.ReadAllText(file));

                // update version
                var el_version = xml.XPathSelectElement("package/metadata/version");
                el_version.Value = VersionNumber;

                // update Squared Infinity dependencies

                var dependencies = xml.XPathSelectElements("package/metadata/dependencies/group/dependency").ToArray();

                if(dependencies.Length > 0)
                {
                    foreach(var dep in dependencies)
                    {
                        if (!dep.Attribute("id").Value.StartsWith("SquaredInfinity."))
                            continue;

                        dep.Attribute("version").Value = VersionNumber;
                    }
                }

                xml.Save(file);
            }

            // build solution in release version

            var solution_file_path = new FileInfo("../../../../Foundation.sln").FullName;

            ExecuteApplicationUsingCommandLine(
                application: @"C:\Program Files (x86)\Microsoft Visual Studio 12.0\Common7\IDE\devenv.exe",
                arguments: "\"{0}\" /rebuild Release".FormatWith(solution_file_path),
                showUi: true,
                continueAfterExecution:true,
                waitForExit: true);
        }

        public void PublishAll()
        {
            // Nuget Packages must be published in a right order so that package dependencies can be resolved

            // 1. Foundation

            PublishProject("NuGet.Foundation");

            // 2. Foundation.Diagnostics.Infrastructure

            PublishProject("NuGet.Foundation.Diagnostics.Infrastructure");

            // 3. Everything else

            PublishProject("NuGet.Foundation.Data");
            PublishProject("NuGet.Foundation.Diagnostics");
            PublishProject("NuGet.Foundation.Presentation.Xaml");
            PublishProject("NuGet.Foundation.Presentation.Xaml.Styles.Modern");
            PublishProject("NuGet.Foundation.Unsafe");
            PublishProject("NuGet.Foundation.Win32Api");
        }

        void PublishProject(string projectName)
        {
            var solution_file_path = new FileInfo("../../../../Foundation.sln").FullName;

            ExecuteApplicationUsingCommandLine(
               application: @"C:\Program Files (x86)\Microsoft Visual Studio 12.0\Common7\IDE\devenv.exe",
               arguments: "\"{0}\" /rebuild Release /project {1} Release".FormatWith(solution_file_path, projectName),
               showUi: true,
               continueAfterExecution:true,
               waitForExit: true);
        }

        public void ExecuteApplicationUsingCommandLine(
            string application, 
            string arguments, 
            string                                           workingDirectory = "",
            bool showUi = true, 
            bool continueAfterExecution = true,
            bool runAsAdmin = false, 
            bool waitForExit = true)
        {
            ProcessStartInfo psi = null;

            if (showUi)
            {
                var arg_txt = "\"\"{0}\" {1}\" & exit".FormatWith(application, arguments);

                if (continueAfterExecution)
                    arg_txt = "/K " + arg_txt;

                psi = new ProcessStartInfo("cmd", arg_txt);
            }
            else
            {
                psi = new ProcessStartInfo(application, arguments);
            }

            if (!showUi)
            {
                psi.CreateNoWindow = true;
                psi.ErrorDialog = false;
                psi.RedirectStandardInput = true;
                psi.RedirectStandardOutput = true;
                psi.UseShellExecute = false;
            }

            psi.WorkingDirectory = workingDirectory;

            if (runAsAdmin)
            {
                psi.Verb = "runas";
            }

            var process = Process.Start(psi);

            if(waitForExit)
                process.WaitForExit();

            if (!showUi)
            {
                //output = process.StandardOutput.ReadToEnd();

                process.Close();

                //return output;
            }
            else
            {
                //return string.Empty;
            }
        }
    }
}
