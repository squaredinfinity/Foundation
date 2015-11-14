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
using SquaredInfinity.Foundation;

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

        bool _processDependantProjects = true;
        public bool ProcessDependantProjects
        {
            get { return _processDependantProjects; }
            set
            {
                if(TrySetThisPropertyValue(ref _processDependantProjects, value))
                {
                    SettingsService.SetSetting<bool>("Deployment.ProcessDependantProjects", SettingScope.UserMachine, value);
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

        XamlObservableCollectionEx<ProjectInfo> _allProjects = new XamlObservableCollectionEx<ProjectInfo>();
        public XamlObservableCollectionEx<ProjectInfo> AllProjects
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

        Dictionary<string, int> ProjectToBuildOrder = new Dictionary<string, int>();

        MultiMap<string, string> ProjectToDependencies = new MultiMap<string, string>();

        public MainViewModel()
        {
            this.DeployRemotely = SettingsService.GetSetting<bool>("Deployment.DeployRemotely", SettingScope.UserMachine, () => false);
            this.LocalDeploymentDirectoryFullPath = SettingsService.GetSetting<string>("Deployment.LocalDeploymentDirectory", SettingScope.UserMachine, () => Environment.CurrentDirectory);
            this.NugetExePath = SettingsService.GetSetting<string>("Deployment.NugetExePath", SettingScope.UserMachine, () => @"../../../../.nuget/nuget.exe");
            this.RemoteDeploymentServers = SettingsService.GetSetting<string>("Deployment.RemoteDeploymentServers", SettingScope.UserMachine, () => "nuget.org");
            
            ProjectToBuildOrder.Add("Foundation", 1);
            ProjectToBuildOrder.Add("Foundation.Diagnostics.Infrastructure", 1);

            ProjectToBuildOrder.Add("Foundation.Serialization", 10);
            ProjectToBuildOrder.Add("Foundation.Unsafe", 10);
            ProjectToBuildOrder.Add("Foundation.Win32Api", 10);
            ProjectToBuildOrder.Add("Foundation.Cache", 10);
            ProjectToBuildOrder.Add("Foundation.Data", 10);
            ProjectToBuildOrder.Add("Foundation.Presentation.Xaml", 10);

            ProjectToBuildOrder.Add("Foundation.Presentation.Xaml.Styles.Modern", 100);

            ProjectToBuildOrder.Add("Foundation.Diagnostics", 20);

            ProjectToDependencies.Add("Foundation.Serialization", "Foundation");
            ProjectToDependencies.Add("Foundation.Serialization", "Foundation.Diagnostics.Infrastructure");

            ProjectToDependencies.Add("Foundation.Unsafe", "Foundation");
            ProjectToDependencies.Add("Foundation.Unsafe", "Foundation.Diagnostics.Infrastructure");

            ProjectToDependencies.Add("Foundation.Win32Api", "Foundation");
            ProjectToDependencies.Add("Foundation.Win32Api", "Foundation.Diagnostics.Infrastructure");

            ProjectToDependencies.Add("Foundation.Cache", "Foundation");
            ProjectToDependencies.Add("Foundation.Cache", "Foundation.Diagnostics.Infrastructure");

            ProjectToDependencies.Add("Foundation.Data", "Foundation");
            ProjectToDependencies.Add("Foundation.Data", "Foundation.Diagnostics.Infrastructure");

            ProjectToDependencies.Add("Foundation.Presentation.Xaml", "Foundation");
            ProjectToDependencies.Add("Foundation.Presentation.Xaml", "Foundation.Diagnostics.Infrastructure");

            ProjectToDependencies.Add("Foundation.Diagnostics", "Foundation");
            ProjectToDependencies.Add("Foundation.Diagnostics", "Foundation.Serialization");
            ProjectToDependencies.Add("Foundation.Diagnostics", "Foundation.Diagnostics.Infrastructure");

            Refresh();
        }

        public void Refresh()
        {
            AllProjects.Clear();

            var psi = new ProcessStartInfo(NugetExePath, "list id:SquaredInfinity.Foundation -prerelease");
            psi.RedirectStandardOutput = true;
            psi.UseShellExecute = false;

            var p = Process.Start(psi);

            var existing_list = p.StandardOutput.ReadToEnd();

            bool nuget_is_online = false;

            nuget_is_online = p.WaitForExit((int)TimeSpan.FromSeconds(10).TotalMilliseconds);

            var all_projects = new List<ProjectInfo>();

            foreach(var kvp in ProjectToBuildOrder)
            {
                var pi = new ProjectInfo();
                pi.Name = kvp.Key;
                pi.BuildOrder = kvp.Value;

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

                all_projects.Add(pi);
            }

            if (nuget_is_online)
            {
                var project_info_regex = @"SquaredInfinity.(?<name>.*) (?<version>.*)";

                foreach (var line in existing_list.GetLines())
                {
                    if (line.IsNullOrEmpty())
                        continue;

                    var project_info_match = Regex.Match(line, project_info_regex);

                    //var pi = new ProjectInfo();

                    var project_name = project_info_match.Groups["name"].Value;

                    var pi =
                        (from proj in all_projects
                         where string.Equals(proj.Name, project_name)
                         select proj).SingleOrDefault();

                    if(pi == null)
                    {
                        MessageBox.Show("{0} does not exist in build order".FormatWith(project_name));
                        continue;
                    }

                    pi.RemoteVersion = project_info_match.Groups["version"].Value;
                }
            }

            AllProjects.AddRange(all_projects.OrderBy(x => x.BuildOrder));
        }

        public void UpdateAssemblyInfoAndRefresh(ProjectInfo project)
        {
            string _ignore = null;

            UpdateAssemblyInfo(project, ProcessDependantProjects, out _ignore);

            Refresh();
        }

        public void UpdateAssemblyInfo(ProjectInfo project)
        {
            string _ignore = null;

            UpdateAssemblyInfo(project, ProcessDependantProjects, out _ignore);
        }

        public void UpdateAssemblyInfo(ProjectInfo project, bool processDependantProjects, out string semantic_version)
        {
            //# get version nubmers

            // .Net compatible version
            var dot_net_version = project.LocalVersion;
            // semantic versioning version
            semantic_version = ToNugetVersion(project.LocalVersion);

            // update project version so it matches configured

            var asminfo_all_lines = File.ReadAllLines(project.AssemblyInfoFile.FullName);

            // replace AssemblyVersion, AssemblyFileVersion and AssemblyInformationalVersion attributes

            for (int i = 0; i < asminfo_all_lines.Length; i++)
            {
                var line = asminfo_all_lines[i];

                if (line.StartsWith("[assembly: AssemblyVersion"))
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
                    asminfo_all_lines[i] = "[assembly: AssemblyInformationalVersion(\"{0}\")]".FormatWith(semantic_version);
                    continue;
                }
            }

            RetryPolicy.FileAccess.Execute(() =>
            {
                File.WriteAllLines(project.AssemblyInfoFile.FullName, asminfo_all_lines);
            });

            if (processDependantProjects)
            {
                // get all projects that depend on this one and update their versions too
                foreach (var kvp in ProjectToDependencies)
                {
                    foreach (var dep in kvp.Value)
                    {
                        if (string.Equals(dep, project.Name))
                        {
                            var dep_proj =
                                (from p in AllProjects
                                 where string.Equals(p.Name, kvp.Key)
                                 select p).Single();

                            // dep proj version should at least match this project version
                            // if it's already higher, then it should be increased (because it's dependency changed)

                            var dep_proj_ver = new Version(dep_proj.LocalVersion);
                            var local_ver = new Version(project.LocalVersion);

                            if (dep_proj_ver < local_ver)
                                dep_proj_ver = local_ver;
                            //else
                            //{
                            // //   dep_proj_ver = new Version(dep_proj_ver.Major, dep_proj_ver.Minor, dep_proj_ver.Build, dep_proj_ver.Revision + 1);
                            //}

                            dep_proj.LocalVersion = dep_proj_ver.ToString();

                            string _ignore;
                            UpdateAssemblyInfo(dep_proj, processDependantProjects: false, semantic_version:out _ignore);
                        }
                    }
                }
            }
        }

        public void DeployProjectAndRefresh(ProjectInfo project)
        {
            DeployProject(project);
            Refresh();
        }

        public void DeployProject(ProjectInfo project)
        {
            string semantic_version = null;

            UpdateAssemblyInfo(project, ProcessDependantProjects, out semantic_version);

            //# build projects for all frameworks

            var solution_file_path = new FileInfo("../../../../Foundation.sln").FullName;

            foreach (var target in new[] { "DotNet45" })
            {
                var project_file_path = new FileInfo("../../../../Sources/{0}/{1}.{0}/{1}.{0}.csproj".FormatWith(target, project.Name)).FullName;



                ExecuteApplicationUsingCommandLine(
                    application: @"C:\Program Files (x86)\Microsoft Visual Studio 14.0\Common7\IDE\devenv.exe",
                    arguments: "\"{0}\" /rebuild Release /project \"{1}\" /projectconfig Release /out build.log".FormatWith(solution_file_path, project_file_path),
                    showUi: true,
                    continueAfterExecution: true,
                    waitForExit: true);
            }

            //# find nuspec in deplyoment project

            var nuspec_file = new FileInfo("../../../../Deployment/Nuget.{0}/Package.symbols.nuspec".FormatWith(project.Name));

            if (!nuspec_file.Exists)
            {
                MessageBox.Show("Cannot find " + nuspec_file.FullName);
                return;
            }

            //# update nuspec with dependencies using remote versions of other projects
            var nuspec_xml = XDocument.Parse(File.ReadAllText(nuspec_file.FullName));

            // update version
            var el_version = nuspec_xml.XPathSelectElement("package/metadata/version");
            el_version.Value = semantic_version;

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

                    dep.Attribute("version").Value = ToNugetVersion(dep_proj.LocalVersion);
                }
            }

            nuspec_xml.Save(nuspec_file.FullName);

            //# build nuget package

            var nugetexe_file = new FileInfo(NugetExePath);
            if (!nugetexe_file.Exists)
            {
                MessageBox.Show("cannot find " + NugetExePath);
                return;
            }

            ExecuteApplicationUsingCommandLine(
                application: "\"{0}\"".FormatWith(nugetexe_file.FullName),
                arguments: "pack \"{0}\" -OutputDirectory {1}".FormatWith(nuspec_file.FullName, LocalDeploymentDirectoryFullPath),
                showUi: true,
                continueAfterExecution: true,
                waitForExit: true);

            if(DeployRemotely)
            {
                var nupkg_file_name = "SquaredInfinity.{0}.{1}.nupkg".FormatWith(project.Name, semantic_version);
                var nupkg_full_path = Path.Combine(LocalDeploymentDirectoryFullPath, nupkg_file_name);

                ExecuteApplicationUsingCommandLine(
                application: "\"{0}\"".FormatWith(nugetexe_file.FullName),
                arguments: "push \"{0}\"".FormatWith(nupkg_full_path),
                showUi: true,
                continueAfterExecution: true,
                waitForExit: true);
            }

            if (ProcessDependantProjects)
            {
                // get all projects that depend on this one and deploy them too
                foreach (var kvp in ProjectToDependencies)
                {
                    foreach (var dep in kvp.Value)
                    {
                        if (string.Equals(dep, project.Name))
                        {
                            var dep_proj =
                                (from p in AllProjects
                                 where string.Equals(p.Name, kvp.Key)
                                 select p).Single();

                            dep_proj.LocalVersion = project.LocalVersion;

                            DeployProject(dep_proj);
                        }
                    }
                }
            }
        }

        string ToNugetVersion(string version)
        {
            var ver = new Version(version);
            if (ver.Revision != 0)
            {
                // NOTE:    a.b.c-dddd.e - .e is not allowed by nuget
                //          a.b.c-dddde used instead
                //
                //          pre-release versions in nuget use lexicographic ascii order
                //          so beta2 > beta10
                //          for that reason beta number will be padded with 0s
                //          i.e. beta002 instead of beta2
                //          making beta002 < beta010

                return "{0}.{1}.{2}-{3}{4}".FormatWith(ver.Major, ver.Minor, ver.Build, "beta", ver.Revision.ToString("000"));
            }
            else
            {
                return version;
            }
        }

        public class ProjectInfo
        {
            public int BuildOrder { get; set; }
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
