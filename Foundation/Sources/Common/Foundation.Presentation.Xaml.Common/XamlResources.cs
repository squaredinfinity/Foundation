using SquaredInfinity.Foundation.Presentation.Resources;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace SquaredInfinity.Foundation.Presentation
{
    [ExportAttribute(typeof(IXamlResourcesProvider))]
    [XamlResourcesProviderMetadata(ImportOrder = XamlResources.ImportOrder)]
    public class XamlResources : IXamlResourcesProvider
    {
        /// <summary>
        /// Order is set to int.MinValue so that this resources are imported first
        /// </summary>
        public const int ImportOrder = int.MinValue;

        public void LoadAndMergeResources()
        {
            // All resources are loaded from ResroucesDictionary All.xaml located in this assembly
            ResourcesManager.LoadAndMergeCompiledResourceDictionaryFromThisAssembly(@"Xaml\All.xaml");
        }
    }
}
