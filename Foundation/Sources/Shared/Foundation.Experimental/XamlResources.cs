using SquaredInfinity.Presentation.Resources;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;

namespace SquaredInfinity
{
    [XamlResourcesProviderMetadata(ImportOrder = XamlResources.ImportOrder)]
    public class XamlResources : IXamlResourcesProvider
    {
        // Import Order is higher than Foundation.Presentation Import Order (on which resources from this assembly may depend)
        public const uint ImportOrder = SquaredInfinity.Presentation.XamlResources.ImportOrder + 1;

        public IEnumerable<ResourceDictionary> LoadResources()
        {
            yield return ResourcesManager.LoadCompiledResourceDictionaryFromThisAssembly(@"Themes/Generic.xaml");
        }
    }
}
