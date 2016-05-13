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
using SquaredInfinity.Foundation.Presentation.Resources;

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

                var dict = ResourcesManager.LoadCompiledResourceDictionary(_source);

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
}
