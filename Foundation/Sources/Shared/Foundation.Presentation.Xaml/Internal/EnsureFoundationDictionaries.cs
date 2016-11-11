using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;

namespace SquaredInfinity.Foundation.Presentation.Internal
{
    public class EnsureFoundationDictionaries : ResourceDictionary
    {
        public EnsureFoundationDictionaries()
        {
            XamlResources.LoadAndMergeAll();
        }
    }
}
