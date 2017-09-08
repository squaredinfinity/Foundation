using System;
using System.Windows;
using SquaredInfinity.Extensions;

namespace SquaredInfinity.Windows.Resources
{
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
