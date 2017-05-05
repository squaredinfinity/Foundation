using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Markup;
using SquaredInfinity.Extensions;
using System.Xaml;
using SquaredInfinity.Presentation.Resources;
using System.Diagnostics;

namespace SquaredInfinity.Presentation
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
