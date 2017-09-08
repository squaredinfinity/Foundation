using System;
using System.Windows;

namespace SquaredInfinity.Windows.Resources
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
            if (string.IsNullOrWhiteSpace(DictionaryName))
                return;

            ResourcesDiagnostics.TraceSource.TraceInformation($"Looking for Referenced ResourceDictionary '{DictionaryName}'");

            var dict = (SharedResourceDictionary)null;

            if (SharedResourceDictionaryRepository.TryGetSharedDictionary(DictionaryName, out dict))
            {
                ResourcesDiagnostics.TraceSource.TraceInformation($"'{DictionaryName}' Shared Dictionary found in the repository.");

                MergedDictionaries.Add(dict);
                SharedResourceDictionaryRepository.AddOrUpdateSharedDictionary(dict);
            }
            else
            if (TryFindSharedDictionaryByName(Application.Current.Resources, DictionaryName, out dict))
            {
                ResourcesDiagnostics.TraceSource.TraceInformation($"'{DictionaryName}' Shared Dictionary found by manual lookup.");

                MergedDictionaries.Add(dict);
                SharedResourceDictionaryRepository.AddOrUpdateSharedDictionary(dict);
            }
            else
            {
                ResourcesDiagnostics.TraceSource.TraceInformation($"Unable to find Shared Dictionary '{DictionaryName}'");
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
