using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using SquaredInfinity.Foundation.Extensions;

namespace SquaredInfinity.Foundation.Presentation
{
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

            if (SharedResourceDictionaryRepository.TryGetSharedDictionary(DictionaryName, out dict))
            {
                InternalTrace.Verbose($"'{DictionaryName}' Shared Dictionary found in the repository.");
                MergedDictionaries.Add(dict);
                SharedResourceDictionaryRepository.AddOrUpdateSharedDictionary(dict);
            }
            else
            if (TryFindSharedDictionaryByName(Application.Current.Resources, DictionaryName, out dict))
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
                if (TryFindSharedDictionaryByName(d, dictionaryName, out dict))
                    return true;
            }

            dict = null;
            return false;
        }
    }
}
