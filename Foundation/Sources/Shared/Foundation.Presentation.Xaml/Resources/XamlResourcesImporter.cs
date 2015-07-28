using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SquaredInfinity.Foundation.Presentation.Resources
{
    internal class XamlResourcesImporter
    {
        [ImportMany]
        Lazy<IXamlResourcesProvider, IXamlResourcesProviderMetadata>[] XamlResourcesProviders;

        public void ImportAnLoadAllResources(ICompositionService compositionService)
        {
            compositionService.SatisfyImportsOnce(this);

            var providers = XamlResourcesProviders.OrderBy(x => x.Metadata.ImportOrder);

            foreach (var provider in providers)
            {
                provider.Value.LoadAndMergeResources();
            }
        }
    }
}
