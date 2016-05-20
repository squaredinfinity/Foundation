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
using System.Diagnostics;

namespace SquaredInfinity.Foundation.Presentation
{
    public static class SharedResourceDictionaryRepository
    {
        static readonly ConcurrentDictionary<string, SharedResourceDictionary> NameToDictionaryMappings =
            new ConcurrentDictionary<string, SharedResourceDictionary>();

        public static void AddOrUpdateSharedDictionary(SharedResourceDictionary dict)
        {
            NameToDictionaryMappings.AddOrUpdate(dict.UniqueName, dict, (_key, _old) => dict);
        }

        public static bool TryGetSharedDictionary(string uniqueName, out SharedResourceDictionary dict)
        {
            return NameToDictionaryMappings.TryGetValue(uniqueName, out dict);
        }
    }

    public partial class ResourceDictionaryReference : ResourceDictionary
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

        void MergeReferencedDictionary()
        {
            if (DictionaryName.IsNullOrEmpty())
                return;

            InternalTrace.Information($"Looking for Referenced ResourceDictionary '{DictionaryName}'");

            var dict = (SharedResourceDictionary)null;

            if(SharedResourceDictionaryRepository.TryGetSharedDictionary(DictionaryName, out dict))
            {
                InternalTrace.Verbose($"'{DictionaryName}' Shared Dictionary found in the repository.");
                MergedDictionaries.Add(dict);
                SharedResourceDictionaryRepository.AddOrUpdateSharedDictionary(dict);
            }
            else 
            if(TryFindSharedDictionaryByName(Application.Current.Resources, DictionaryName, out dict))
            {
                InternalTrace.Verbose($"'{DictionaryName}' Shared Dictionary found by manual lookup.");
                MergedDictionaries.Add(dict);
                SharedResourceDictionaryRepository.AddOrUpdateSharedDictionary(dict);
            }
            else
            {
                InternalTrace.Warning($"Unable to find Shared Dictionary '{DictionaryName}'");
            }
        }

        static bool TryFindSharedDictionaryByName(ResourceDictionary searchOrigin, string dictionaryName, out SharedResourceDictionary dict)
        {
            if (searchOrigin is SharedResourceDictionary)
            {
                var sd = (SharedResourceDictionary)searchOrigin;

                if (string.Equals(sd.UniqueName, dictionaryName, StringComparison.InvariantCultureIgnoreCase))
                {
                    dict = sd;
                    return true;
                }
            }

            foreach (var d in searchOrigin.MergedDictionaries)
            {
                if(TryFindSharedDictionaryByName(d, dictionaryName, out dict))
                    return true;
            }

            dict = null;
            return false;
        }
    }

    public partial class SharedResourceDictionary : ResourceDictionary
    {
        string _uniqueName;
        public string UniqueName
        {
            get { return _uniqueName; }

            set 
            {
                _uniqueName = value;

                UpdateRepository();
            }
        }

        Uri _source;
        public new Uri Source
        {
            get { return _source; }
            set
            {
                _source = value;

                var dict = ResourcesManager.LoadCompiledResourceDictionary(_source);

                MergedDictionaries.Add(dict);

                UpdateRepository();
            }
        }

        void UpdateRepository()
        {
            if (UniqueName.IsNullOrEmpty())
                return;

            if (Source == null)
                return;

            SharedResourceDictionaryRepository.AddOrUpdateSharedDictionary(this);
        }
    }
}
