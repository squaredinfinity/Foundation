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
using System.Threading;
using SquaredInfinity.Foundation.Collections;
using SquaredInfinity.Foundation.Settings;
using SquaredInfinity.Foundation.Serialization.FlexiXml;
using System.Windows;

namespace Nuget.DeployAllProjects
{
    public class MainViewModel : ViewModel
    {
        string _nugetExePath;
        public string NugetExePath
        {
            get { return _nugetExePath; }
            set
            {
                if(TrySetThisPropertyValue(ref _nugetExePath, value))
                {
                    SettingsService.SetSetting<string>("Deployment.NugetExePath", SettingScope.UserMachine, value);
                }
            }
        }


        string _remoteDeploymentServers;
        public string RemoteDeploymentServers
        {
            get { return _remoteDeploymentServers; }
            set
            {
                if(TrySetThisPropertyValue(ref _remoteDeploymentServers, value))
                {
                    SettingsService.SetSetting<string>("Deployment.RemoteDeploymentServers", SettingScope.UserMachine, value);
                }
            }
        }


        string _localDeploymentDirectoryFullPath;
        public string LocalDeploymentDirectoryFullPath
        {
            get { return _localDeploymentDirectoryFullPath; }
            set
            {
                if(TrySetThisPropertyValue(ref _localDeploymentDirectoryFullPath, value))
                {
                    SettingsService.SetSetting<string>("Deployment.LocalDeploymentDirectory", SettingScope.UserMachine, value);
                }
            }
        }

        bool _deployLocally;
        public bool DeployLocally
        {
            get { return _deployLocally; }
            set
            {
                if(TrySetThisPropertyValue(ref _deployLocally, value))
                {
                    SettingsService.SetSetting<bool>("Deployment.DeployLocally", SettingScope.UserMachine, value);
                }
            }
        }

        bool _deployRemotely;
        public bool DeployRemotely
        {
            get { return _deployRemotely; }
            set
            {
                if(TrySetThisPropertyValue(ref _deployRemotely, value))
                {
                    SettingsService.SetSetting<bool>("Deployment.DeployRemotely", SettingScope.UserMachine, value);
                }
            }
        }

        List<ProjectInfo> _allProjects = new List<ProjectInfo>();
        public List<ProjectInfo> AllProjects
        {
            get { return _allProjects; }
        }

        XamlObservableCollectionEx<ProjectInfo> _selectedProjects = new XamlObservableCollectionEx<ProjectInfo>();
        public XamlObservableCollectionEx<ProjectInfo> SelectedProjects
        {
            get { return _selectedProjects; }
        }

        string _versionNumber;
        public string VersionNumber
        {
            get { return _versionNumber; }
            set { TrySetThisPropertyValue(ref _versionNumber, value); }
        }

        ISettingsService SettingsService = new FileSystemSettingsService(new FlexiXmlSerializer(), new DirectoryInfo(Environment.CurrentDirectory));

        public MainViewModel()
        {

            this.DeployLocally = SettingsService.GetSetting<bool>("Deployment.DeployLocally", SettingScope.UserMachine, () => true);
            this.DeployRemotely = SettingsService.GetSetting<bool>("Deployment.DeployRemotely", SettingScope.UserMachine, () => false);
            this.LocalDeploymentDirectoryFullPath = SettingsService.GetSetting<string>("Deployment.LocalDeploymentDirectory", SettingScope.UserMachine, () => Environment.CurrentDirectory);
            this.NugetExePath = SettingsService.GetSetting<string>("Deployment.NugetExePath", SettingScope.UserMachine, () => @"../../../../.nuget/nuget.exe");
            this.RemoteDeploymentServers = SettingsService.GetSetting<string>("Deployment.RemoteDeploymentServers", SettingScope.UserMachine, () => "nuget.org");


            // get version number of assemblies in solution
            // assumes default solution structure
            //var asm_info = File.ReadAllText(@"../../../../Sources/Shared/Internal/AssemblyInfo.shared.cs");

            //var version_number_match = Regex.Match(asm_info, @"\[assembly: AssemblyVersion\(""(?<version>.*)""");

            //var version_number = new Version(version_number_match.Groups["version"].Value);

            //VersionNumber = version_number.ToString() + "-beta";


            // Nuget Packages must be published in a right order so that package dependencies can be resolved

            // 1. Foundation

            //AllProjects.Add("NuGet.Foundation");

            // 2. Foundation.Diagnostics.Infrastructure

            //AllProjects.Add("NuGet.Foundation.Diagnostics.Infrastructure");

            // 3. Everything else

            //AllProjects.Add("NuGet.Foundation.Cache");
            //AllProjects.Add("NuGet.Foundation.Serialization");
            //AllProjects.Add("NuGet.Foundation.Data");
            //AllProjects.Add("NuGet.Foundation.Diagnostics");
            //AllProjects.Add("NuGet.Foundation.Presentation.Xaml");

            //PublishProject("NuGet.Foundation.Presentation.Xaml.Styles.Modern");
            //AllProjects.Add("NuGet.Foundation.Unsafe");
            //AllProjects.Add("NuGet.Foundation.Win32Api");

            var psi = new ProcessStartInfo(NugetExePath, "list id:SquaredInfinity.Foundation -prerelease");
            psi.RedirectStandardOutput = true;
            psi.UseShellExecute = false;

            var p = Process.Start(psi);
            
            var existing_list = p.StandardOutput.ReadToEnd();
            p.WaitForExit();


            var project_info_regex = @"SquaredInfinity.(?<name>.*) (?<version>.*)";

            foreach (var line in existing_list.GetLines())
            {
                var project_info_match = Regex.Match(line, project_info_regex);

                var pi = new ProjectInfo();

                pi.Name = project_info_match.Groups["name"].Value;
                pi.RemoteVersion = project_info_match.Groups["version"].Value;

                var asminfo_file_path = "../../../../Sources/Shared/{0}/AssemblyInfo.cs".FormatWith(pi.Name);
                pi.AssemblyInfoFile = new FileInfo(asminfo_file_path);

                if (!File.Exists(asminfo_file_path))
                {
                    MessageBox.Show("cannot find " + pi.AssemblyInfoFile.FullName);
                    continue;
                }

                var asm_info = File.ReadAllText(asminfo_file_path);

                var version_match = Regex.Match(asm_info, @"\[assembly: AssemblyVersion\(""(?<version>.*)""");

                pi.LocalVersion = version_match.Groups["version"].Value;

                AllProjects.Add(pi);
            }
        }

        public void DeployProject(ProjectInfo project)
        {
            //# get version nubmers

            // .Net compatible version
            var dot_net_version = project.LocalVersion;
            // semantic versioning version
            var sem_ver_version = project.LocalVersion;

            var version = new Version(project.LocalVersion);
            if(version.Revision != 0)
            {
                sem_ver_version = "{0}.{1}.{2}-{3}.{4}".FormatWith(version.Major, version.Minor, version.Build, "beta", version.Revision);
            }
            
            // update project version so it matches configured

            var asminfo_all_lines = File.ReadAllLines(project.AssemblyInfoFile.FullName);

            // replace AssemblyVersion, AssemblyFileVersion and AssemblyInformationalVersion attributes

            for (int i = 0; i < asminfo_all_lines.Length; i++)
            {
                var line = asminfo_all_lines[i];

                if(line.StartsWith("[assembly: AssemblyVersion"))
                {
                    asminfo_all_lines[i] = "[assembly: AssemblyVersion(\"{0}\")]".FormatWith(dot_net_version);
                    continue;
                }

                if (line.StartsWith("[assembly: AssemblyFileVersion"))
                {
                    asminfo_all_lines[i] = "[assembly: AssemblyFileVersion(\"{0}\")]".FormatWith(dot_net_version);
                    continue;
                }

                if (line.StartsWith("[assembly: AssemblyInformationalVersion"))
                {
                    asminfo_all_lines[i] = "[assembly: AssemblyInformationalVersion(\"{0}\")]".FormatWith(sem_ver_version);
                    continue;
                }
            }

            File.WriteAllLines(project.AssemblyInfoFile.FullName, asminfo_all_lines);

            //# build projects for all frameworks

            var solution_file_path = new FileInfo("../../../../Foundation.sln").FullName;

            foreach (var target in new [] {  "DotNet45" })
            {
                var project_file_path = new FileInfo("../../../../Sources/{0}/{1}.{0}/{1}.{0}.csproj".FormatWith(target, project.Name)).FullName;



                ExecuteApplicationUsingCommandLine(
                    application: @"C:\Program Files (x86)\Microsoft Visual Studio 14.0\Common7\IDE\devenv.exe",
                    arguments: "\"{0}\" /rebuild Debug /project \"{1}\" /projectconfig Debug /out build.log".FormatWith(solution_file_path, project_file_path),
                    showUi: true,
                    continueAfterExecution: true,
                    waitForExit: true);
            }

            //# find nuspec in deplyoment project

            var nuspec_file = new FileInfo("../../../../Deployment/Nuget.{0}/Package.symbols.nuspec".FormatWith(project.Name));

            if(!nuspec_file.Exists)
            {
                MessageBox.Show("Cannot find " + nuspec_file.FullName);
                return;
            }

            //# update nuspec with dependencies using remote versions of other projects
            var nuspec_xml = XDocument.Parse(File.ReadAllText(nuspec_file.FullName));

            // update version
            var el_version = nuspec_xml.XPathSelectElement("package/metadata/version");
            el_version.Value = sem_ver_version;

            // update Squared Infinity dependencies
            var dependencies = nuspec_xml.XPathSelectElements("package/metadata/dependencies/group/dependency").ToArray();

            if (dependencies.Length > 0)
            {
                foreach (var dep in dependencies)
                {
                    if (!dep.Attribute("id").Value.StartsWith("SquaredInfinity."))
                        continue;

                    var dep_name = dep.Attribute("id").Value.Substring("SquaredInfinity.".Length);

                    var dep_proj =
                        (from p in AllProjects
                         where string.Equals(p.Name, dep_name, StringComparison.InvariantCultureIgnoreCase)
                         select p).Single();

                    dep.Attribute("version").Value = dep_proj.RemoteVersion;
                }
            }

            nuspec_xml.Save(nuspec_file.FullName);

            //# build nuget package

            var nugetexe_file = new FileInfo(NugetExePath);
            if(!nugetexe_file.Exists)
            {
                MessageBox.Show("cannot find " + NugetExePath);
                return;
            }

            ExecuteApplicationUsingCommandLine(
                    application: "\"{0}\"".FormatWith(nugetexe_file.FullName),
                    arguments: "pack \"{0}\"".FormatWith(solution_file_path),
                    showUi: true,
                    continueAfterExecution: true,
                    waitForExit: true);

            // deploy

            if (DeployLocally)
            {
                // copy nuget to output folder
            }
        }

        public class ProjectInfo
        {
            public string Name { get; set; }
            public string LocalVersion { get; set; }
            public string RemoteVersion { get; set; }
            public FileInfo AssemblyInfoFile { get; internal set; }
        }

        public static void CopyDirectoryRecursively(DirectoryInfo source, DirectoryInfo target) 
        {
            if (source.Name == "obj" || source.Name == "bin")
                return;

            if (!target.Exists)
                target.Create();

            foreach (DirectoryInfo dir in source.GetDirectories())
            {
                CopyDirectoryRecursively(dir, target.CreateSubdirectory(dir.Name));
            }

            foreach (FileInfo file in source.GetFiles())
            {
                if (file.Extension == "dll")
                    continue;

                file.CopyTo(Path.Combine(target.FullName, file.Name), overwrite:true);
            }
        }

        public void UpdateAllProjects()
        {
            // find all Package.symbols.nuspec files in solution under deployment project
            foreach (var file in Directory.EnumerateFiles("../../../../Deployment", "Package.symbols.nuspec", SearchOption.AllDirectories))
            {
                var xml = XDocument.Parse(File.ReadAllText(file));

                // update version
                var el_version = xml.XPathSelectElement("package/metadata/version");
                el_version.Value = VersionNumber;

                // update Squared Infinity dependencies

                var dependencies = xml.XPathSelectElements("package/metadata/dependencies/group/dependency").ToArray();

                if (dependencies.Length > 0)
                {
                    foreach (var dep in dependencies)
                    {
                        if (!dep.Attribute("id").Value.StartsWith("SquaredInfinity."))
                            continue;

                        dep.Attribute("version").Value = VersionNumber;
                    }
                }

                xml.Save(file);

                // ensure sources structure
                var packageRoot = Path.GetDirectoryName(file);

                var srcRoot = Path.Combine(packageRoot, "src");

                for (int i = 0; i < 10; i++)
                {
                    try
                    {
                        if (Directory.Exists(srcRoot))
                        {
                            Directory.Delete(srcRoot, recursive: true);
                            Thread.Sleep(50);
                        }

                        Directory.CreateDirectory(srcRoot);
                        Directory.CreateDirectory(Path.Combine(srcRoot, "Shared"));
                        Directory.CreateDirectory(Path.Combine(srcRoot, "Common"));
                        Directory.CreateDirectory(Path.Combine(srcRoot, "DotNet45"));

                        break;
                    }
                    catch (Exception ex)
                    {
                        Trace.WriteLine(ex.ToString());
                    }
                }

                // copy source code files specific to this project
                var projectName = xml.XPathSelectElement("package/metadata/id").Value.Substring("SquaredInfinity.".Length);


                var solution_root = new DirectoryInfo("../../../../");
                var sources_root = new DirectoryInfo("../../../../Sources");

                // sources/common
                var commonTargetDir = new DirectoryInfo(Path.Combine(sources_root.FullName, "Common"));
            }

            // build solution in release version

            var solution_file_path = new FileInfo("../../../../Foundation.sln").FullName;

            ExecuteApplicationUsingCommandLine(
                application: @"C:\Program Files (x86)\Microsoft Visual Studio 14.0\Common7\IDE\devenv.exe",
                arguments: "\"{0}\" /rebuild Release".FormatWith(solution_file_path),
                showUi: true,
                continueAfterExecution: true,
                waitForExit: true);
        }


        public void PublishSelected()
        {
            foreach(var project in SelectedProjects)
            {
             //   PublishProject(project);
            }
        }


        public void PublishAll()
        {
            // Nuget Packages must be published in a right order so that package dependencies can be resolved

            // 1. Foundation

            PublishProject("NuGet.Foundation");

            // 2. Foundation.Diagnostics.Infrastructure

            PublishProject("NuGet.Foundation.Diagnostics.Infrastructure");

            // 3. Everything else

            PublishProject("NuGet.Foundation.Cache");
            PublishProject("NuGet.Foundation.Serialization");
            PublishProject("NuGet.Foundation.Data");
            PublishProject("NuGet.Foundation.Diagnostics");
            PublishProject("NuGet.Foundation.Presentation.Xaml");
            // this will be published manually
            //PublishProject("NuGet.Foundation.Presentation.Xaml.Styles.Modern");
            PublishProject("NuGet.Foundation.Unsafe");
            PublishProject("NuGet.Foundation.Win32Api");
        }
        void PublishProject(string projectName)
        {
            var solution_file_path = new FileInfo("../../../../Foundation.sln").FullName;

            ExecuteApplicationUsingCommandLine(
               application: @"C:\Program Files (x86)\Microsoft Visual Studio 14.0\Common7\IDE\devenv.exe",
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
                var output = process.StandardOutput.ReadToEnd();

                process.Close();

                Trace.WriteLine(output);

                //return output;
            }
            else
            {
                //return string.Empty;
            }
        }
    }
}
