using SquaredInfinity.Presentation.DataTemplateSelectors;
using SquaredInfinity.Presentation.Resources;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace SquaredInfinity.Presentation
{
    [XamlResourcesProviderMetadata()]
    public class XamlResources : IXamlResourcesProvider
    {
        public const uint ImportOrder = int.MaxValue - 1;

        public IEnumerable<ResourceDictionary> LoadResources()
        {
            // All resources are loaded from ResroucesDictionary All.xaml located in this assembly
            var all = ResourcesManager.LoadCompiledResourceDictionaryFromThisAssembly(@"Xaml\All.xaml");
            yield return all;
            
            var data_template_selectors = new SharedResourceDictionary { UniqueName = "Foundation.DataTemplateSelectors" };
            data_template_selectors.Add("DataTemplateSelectors.TypeNameToResourceKey", new TypeNameToResourceKeyMappingDataTemplateSelector());
            data_template_selectors.Add("DataTemplateSelectors.ContextAware", new ContextAwareDataTemplateSelector());
            data_template_selectors.Add("DataTemplateSelectors.ContextAware.Tooltip", new ContextAwareDataTemplateSelector { IsTooltip = true });

            yield return data_template_selectors;
        }

        public static void LoadAndMergeAll()
        {
            XamlResources xaml = new XamlResources();
            ResourcesManager.MergeAllResourceDictionaries(xaml.LoadResources());
        }
    }
}
