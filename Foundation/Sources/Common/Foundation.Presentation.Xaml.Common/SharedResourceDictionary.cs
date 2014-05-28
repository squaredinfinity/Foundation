using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Markup;
using SquaredInfinity.Foundation.Extensions;
using System.Xaml;

namespace SquaredInfinity.Foundation.Presentation
{
    public partial class SharedResourceDictionary
    {
        static readonly ConcurrentDictionary<string, ResourceDictionary> NameToDictionaryMappings =
            new ConcurrentDictionary<string, ResourceDictionary>();

        public static void AddOrUpdateSharedDictionary(string name, ResourceDictionary dict)
        {
            NameToDictionaryMappings.AddOrUpdate(name, dict, (_key, _old) => dict);
        }

        public static void AddOrUpdateSharedDictionary(SharedResourceDictionary dict)
        {
            NameToDictionaryMappings.AddOrUpdate(dict.DictionaryName, dict, (_key, _old) => dict);
        }

        static SharedResourceDictionary() { }
    }

    public partial class SharedResourceDictionary : ResourceDictionary
    {
        string _dictionaryName;
        public string DictionaryName
        {
            get { return _dictionaryName; }

            set
            {
                _dictionaryName = value;
                
                MergeReferencedDictionary();
            }
        }

        Uri _source;
        public new Uri Source
        {
            get { return _source; }

            set
            {
                if (DictionaryName.IsNullOrEmpty())
                    return;

                _source = value;

                var dict = Resources.LoadCompiledResourceDictionary(_source);

                AddOrUpdateSharedDictionary(DictionaryName, dict);

                MergeReferencedDictionary();
            }
        }

        void MergeReferencedDictionary()
        {
            var dict = (ResourceDictionary)null;

            if (NameToDictionaryMappings.TryGetValue(DictionaryName, out dict))
            {
                MergedDictionaries.Add(dict);
            }
        }
    }

    public class ResourceFromThisAssembly : MarkupExtension
    {
        public string RelativePath { get; set; }

        public ResourceFromThisAssembly()
        { }

        public ResourceFromThisAssembly(string relativePath)
        {
            this.RelativePath = relativePath;
        }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            var uriContext = serviceProvider.GetService(typeof(IUriContext)) as IUriContext;

            var ix_packseparator = uriContext.BaseUri.OriginalString.IndexOf(";");

            var pack = uriContext.BaseUri.OriginalString.Substring(0, ix_packseparator + 1) + "component";

            if (RelativePath.StartsWith(@"/"))
                pack += RelativePath;
            else
            {
                pack += @"/";
                pack += RelativePath;
            }

            return new Uri(pack);
        }
    }

    public class ImageFromThisAssembly : MarkupExtension
    {
        public string RelativePath { get; set; }

        public ImageFromThisAssembly()
        { }

        public ImageFromThisAssembly(string relativePath)
        {
            this.RelativePath = relativePath;
        }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            var uriContext = serviceProvider.GetService(typeof(IUriContext)) as IUriContext;

            var ix_packseparator = uriContext.BaseUri.OriginalString.IndexOf(";");

            var pack = uriContext.BaseUri.OriginalString.Substring(0, ix_packseparator + 1) + "component";

            if (RelativePath.StartsWith(@"/"))
                pack += RelativePath;
            else
            {
                pack += @"/";
                pack += RelativePath;
            }

            

            return Resources.LoadImage(new Uri(pack, UriKind.Absolute));
        }
    }
}
