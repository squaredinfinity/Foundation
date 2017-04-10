using Microsoft.Build.Framework;
using System.Linq;
using System;
using System.Xml.Linq;
using System.IO;
using System.Collections.Generic;
using System.Text;

namespace SquaredInfinity.Build.Tasks.NuGet
{
    public interface IFileSystemEntryPath
    {
        string CurrentDirectory { get; }
        string RawPath { get; }
        string FullPath { get; }

        void UpdateCurrentDirectory(string currentDirectory);
    }

    public enum PathKind
    {
        Relative,
        Absolute
    }

    public class FileSystemEntryPath : IFileSystemEntryPath
    {
        public string CurrentDirectory { get; private set; }
        public string RawPath { get; private set; }
        public string FullPath { get; private set; }


        readonly PathKind RawPathKind = PathKind.Absolute;

        public FileSystemEntryPath(string relativeOrAbsolutePath)
            : this(relativeOrAbsolutePath, Environment.CurrentDirectory)
        {}

        public FileSystemEntryPath(string relativeOrAbsolutePath, string currentDirectory)
        {
            CurrentDirectory = currentDirectory;
            RawPath = relativeOrAbsolutePath;

            if (Path.IsPathRooted(relativeOrAbsolutePath))
                RawPathKind = PathKind.Absolute;
            else
                RawPathKind = PathKind.Relative;

            Refresh();
        }

        void Refresh()
        {
            if (RawPathKind == PathKind.Relative)
                FullPath = Path.GetFullPath(Path.Combine(CurrentDirectory, RawPath));
            else
                FullPath = RawPath;
        }

        public void UpdateCurrentDirectory(string currentDirectory)
        {
            CurrentDirectory = currentDirectory;
            Refresh();
        }
    }

    public class NuSpecUpdateDependenciesFromPackagesConfig : CustomBuildTask
    {
        public string NuSpecPath { get; set; }
        public string PackageConfigPaths { get; set; }
        public string TargetFramework { get; set; }

        protected override bool DoExecute()
        {
            if (!File.Exists(NuSpecPath))
            {
                LogError($"Specified nuspec file does not exist. {NuSpecPath}");
                return false;
            }

            var package_config_paths = PackageConfigPaths.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries);

            // get all referenced packages from all package.configs

            var all_package_elements = new List<XElement>();

            foreach(var pc in package_config_paths)
            {
                var p = new FileSystemEntryPath(pc);

                if(!File.Exists(p.FullPath))
                {
                    LogWaring($"Specified package file does not exist. {p.FullPath}");
                    continue;
                }
                
                var xml = XDocument.Load(p.FullPath);

                all_package_elements.AddRange(xml.Root.Elements("package"));
            }

            var has_error = false;

            // if different versions of same package referenced, error
            var by_id = 
                all_package_elements
                .Select(x => new { Id = x.Attribute("id").Value, Version = x.Attribute("version").Value})
                .GroupBy(x => x.Id);

            foreach(var packages in by_id)
            {
                if(packages.Count() > 1)
                {
                    var version = (string) null;

                    foreach(var x in packages)
                    {
                        if (version == null)
                            version = x.Version;
                        else if(version != x.Version)
                        {
                            has_error = true;
                            LogError($"Multiple versions of same package referenced. {x.Id}");
                        }
                    }
                }
            }

            if(has_error)
            {
                LogError("Execution terminated due to previous errros.");
                return false;
            }

            var nuspec_path = new FileSystemEntryPath(NuSpecPath);

            var nuspec_xml = XDocument.Load(nuspec_path.FullPath);
            var nuspec_namespace = nuspec_xml.Root.GetDefaultNamespace();

            // ensure metadata and dependencies elements
            var metadata_element = nuspec_xml.Root.Element(nuspec_namespace.GetName("metadata"));

            if(metadata_element == null)
            {
                metadata_element = new XElement(nuspec_namespace.GetName("metadata"));
                nuspec_xml.Root.Add(metadata_element);
            }

            var dependencies_element = metadata_element.Element(nuspec_namespace.GetName("dependencies"));

            if(dependencies_element == null)
            {
                dependencies_element = new XElement(nuspec_namespace.GetName("dependencies"));
                metadata_element.Add(dependencies_element);
            }

            // ensure target framework group
            var group_element =
                (from el in dependencies_element.Elements(nuspec_namespace.GetName("group"))
                 where el.Attribute("targetFramework")?.Value == TargetFramework
                 select el)
                .SingleOrDefault();

            if(group_element == null)
            {
                group_element = new XElement(nuspec_namespace.GetName("group"));
                group_element.SetAttributeValue("targetFramework", TargetFramework);
                dependencies_element.Add(group_element);
            }

            //# add dependencies

            // remove all children
            group_element.RemoveNodes();

            foreach(var p in by_id)
            {
                var el = new XElement(nuspec_namespace.GetName("dependency"));
                el.Add(new XAttribute("id", p.Key));
                el.Add(new XAttribute("version", p.First().Version));
                group_element.Add(el);
            }

            nuspec_xml.Save(nuspec_path.FullPath);

            return true;
        }
    }
}
