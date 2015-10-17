using System;
using System.Collections.Generic;
using System.IO;
using SquaredInfinity.Foundation.Extensions;
using System.Text;
using System.Xml.Linq;
using System.Security;
using System.Linq;

namespace SquaredInfinity.Foundation.Settings
{
    public class FileSystemSettingsService : SettingsService
    {
        protected DirectoryInfo Location { get; private set; }
        protected string PartSeparator { get; private set; }
        protected string Extension { get; private set; }

        const string DefaultPartSeparator = ";";
        const string DefaultExtension = ".config";

        public FileSystemSettingsService(ISerializer defaultSerializer, DirectoryInfo location)
            : this("", "", defaultSerializer, location, DefaultPartSeparator, DefaultExtension)
        { }

        public FileSystemSettingsService(ISerializer defaultSerializer, DirectoryInfo location, string partSeparator, string extension)
            : this("", "", defaultSerializer, location, partSeparator, extension)
        { }

        public FileSystemSettingsService(
            string defaultApplication,
            string defaultContainer,
            ISerializer defaultSerializer,
            DirectoryInfo location)
            : this(defaultApplication, defaultContainer, defaultSerializer, location, DefaultPartSeparator, DefaultExtension)
        { }

        public FileSystemSettingsService(
            string defaultApplication,
            string defaultContainer,
            ISerializer defaultSerializer,
            DirectoryInfo location,
            string partSeparator,
            string extension)
            : base(defaultApplication, defaultContainer, defaultSerializer)
        {
            if (location == null)
                throw new ArgumentNullException("location");

            this.PartSeparator = partSeparator;
            this.Extension = extension;

            this.Location = location;

            if (!Directory.Exists(location.FullName))
                location.Create();
        }

        protected virtual FileInfo GetFile(
            DirectoryInfo location,
            string application,
            string container,
            string key,
            int scope,
            string machineName,
            string userName)
        {
            if (string.IsNullOrWhiteSpace(key))
                throw new ArgumentException("key must not be null or empty", "key");

            List<string> file_parts = new List<string>();

            file_parts.Add(application.ToString(valueWhenNull: ""));
            file_parts.Add(container.ToString(valueWhenNull: ""));

            if (scope == SettingScope.User || scope == SettingScope.UserMachine)
                file_parts.Add(userName.ToString(valueWhenNull: ""));
            else
                file_parts.Add("");

            if (scope == SettingScope.Machine || scope == SettingScope.UserMachine)
                file_parts.Add(machineName.ToString(valueWhenNull: ""));
            else
                file_parts.Add("");

            var full_file_name = string.Join(PartSeparator, file_parts) + Extension;

            var full_path = Path.Combine(location.FullName, full_file_name);

            return new FileInfo(full_path);
        }

        public override void DoSetSetting<T>(string application, string container, string key, int scope, string machineName, string userName, Encoding valueEncoding, byte[] value)
        {
            var file = GetFile(Location, application, container, key, scope, machineName, userName);

            //# perpare as much data as possible before accessing the file
            //  to avoid long locks
            var xDoc = (XDocument)null;
            xDoc = new XDocument();
            xDoc.Add(new XElement("Settings"));

            var new_key_element = new XElement("Setting");
            new_key_element.AddAttribute("key", key);
            new_key_element.AddAttribute("encoding", valueEncoding.CodePage.ToString());

            using (var ms = new MemoryStream(value))
            {
                using (var sr = new StreamReader(ms, valueEncoding))
                {
                    var value_as_string = sr.ReadToEnd();

                    // if value is a valid xml text, then place it verbatim
                    // otherwise wrap it in CData so it can be stored in original form
                    if (SecurityElement.IsValidText(value_as_string))
                    {
                        new_key_element.Value = value_as_string;
                    }
                    else
                    {
                        new_key_element.Add(new XCData(value_as_string));
                    }
                }
            }

            RetryPolicy.FileAccess.Execute(() =>
            {
                using (var fs = file.Open(FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.None))
                {
                    // open document
                    if (fs.Length != 0)
                    {
                        // file is non-empty, load its contents
                        xDoc = XDocument.Load(fs);
                    }

                    // remove old value if it exists
                    var existing_element =
                    (from e in xDoc.Root.Elements("Setting")
                     where string.Equals(e.GetAttributeValue("key"), key)
                     select e).FirstOrDefault();

                    if (existing_element != null)
                    {
                        existing_element.Remove();
                    }

                    xDoc.Root.Add(new_key_element);

                    // save back to file overwriting everything
                    fs.SetLength(0);
                    xDoc.Save(fs);
                }
            }, maxRetryAttempts: 25, minDelayInMiliseconds: 10, maxDelayInMiliseconds: 250);
        }

        public override bool DoTryGetSetting(string application, string container, string key, int scope, string machineName, string userName, out Encoding valueEncoding, out byte[] value)
        {
            value = null;
            valueEncoding = null;

            var file = GetFile(Location, application, container, key, scope, machineName, userName);

            if (!File.Exists(file.FullName))
                return false;

            var xDoc =
            RetryPolicy.FileAccess.Execute<XDocument>(() =>
            {
                using (var fs = file.Open(FileMode.Open, FileAccess.Read, FileShare.Read))
                {
                    if (fs.Length == 0)
                    {
                        return null;
                    }
                    else
                    {
                        return XDocument.Load(fs);
                    }
                }
            }, maxRetryAttempts: 25, minDelayInMiliseconds: 10, maxDelayInMiliseconds: 250);

            // find element
            var existing_element =
            (from e in xDoc.Root.Elements("Setting")
             where string.Equals(e.GetAttributeValue("key"), key)
             select e).FirstOrDefault();

            if (existing_element == null)
            {
                return false;
            }
            else
            {
                var code_page = int.Parse(existing_element.GetAttributeValue("encoding"));
                valueEncoding = Encoding.GetEncoding(code_page);

                var cdata = existing_element.FirstNode as XCData;

                if (cdata == null)
                {
                    value = valueEncoding.GetBytes(existing_element.Value);
                }
                else
                {
                    value = valueEncoding.GetBytes(cdata.Value);
                }

                return true;
            }
        }
    }
}
