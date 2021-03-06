﻿using SquaredInfinity.Foundation.Presentation.Resources;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Nuget.DeployAllProjects
{
    [XamlResourcesProviderMetadata(ImportOrder = XamlResources.ImportOrder)]
    public class XamlResources : IXamlResourcesProvider
    {
        // Import Order is higher than Foundation.Presentation Import Order (on which resources from this assembly may depend)
        public const uint ImportOrder = SquaredInfinity.Foundation.Presentation.XamlResources.ImportOrder + 100;

        public IEnumerable<ResourceDictionary> LoadResources()
        {
            yield return ResourcesManager.LoadCompiledResourceDictionaryFromThisAssembly(@"XamlResources.xaml");
        }
    }
}
