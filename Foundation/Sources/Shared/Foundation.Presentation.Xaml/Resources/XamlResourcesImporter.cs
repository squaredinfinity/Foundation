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
            //compositionService.SatisfyImportsOnce(this);
            var cc = (compositionService as System.ComponentModel.Composition.Hosting.CompositionContainer);

            var all_providers = cc.GetExports<IXamlResourcesProvider, IXamlResourcesProviderMetadata>();

            var z = all_providers.ToArray();

            var providers = z.OrderBy(x => x.Metadata.ImportOrder);

            using (var enumerator = providers.GetEnumerator())
            {
                while(enumerator.MoveNext())
                {
                    try
                    {
                        InternalTrace.Information($"Importing xaml resources from {enumerator.Current.Value.GetType().FullName}...");
                        enumerator.Current.Value.LoadAndMergeResources();
                    }
                    catch(Exception ex)
                    {
                        InternalTrace.Error(ex.ToString());
                    }
                }
            }
        }
    }
}
