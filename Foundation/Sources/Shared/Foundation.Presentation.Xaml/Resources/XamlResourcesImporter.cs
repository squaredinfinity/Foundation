using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace SquaredInfinity.Foundation.Presentation.Resources
{
    internal class XamlResourcesImporter
    {
        public IEnumerable<ResourceDictionary> ImportAllResources(ICompositionService compositionService, bool merge)
        {
            var result = new List<ResourceDictionary>();

            var cc = (compositionService as System.ComponentModel.Composition.Hosting.CompositionContainer);

            var providers_with_metadata = cc.GetExports<IXamlResourcesProvider, IXamlResourcesProviderMetadata>();

            var ordered_providers = providers_with_metadata.OrderBy(x => x.Metadata.ImportOrder).ToList();

            var providers_no_metadata = cc.GetExports<IXamlResourcesProvider>();

            var all_providers =
                ordered_providers.Concat(providers_no_metadata).Distinct();

            using (var enumerator = all_providers.GetEnumerator())
            {
                while(enumerator.MoveNext())
                {
                    try
                    {
                        InternalTrace.Information($"Importing xaml resources from {enumerator.Current.Value.GetType().FullName}...");

                        var resources = enumerator.Current.Value.LoadResources().ToArray();

                        result.AddRange(resources);

                        if(merge)
                            foreach (var dict in resources)
                                ResourcesManager.MergeResourceDictionary(dict);
                    }
                    catch(Exception ex)
                    {
                        InternalTrace.Error(ex.ToString());
                    }
                }
            }

            return result;
        }
    }
}
