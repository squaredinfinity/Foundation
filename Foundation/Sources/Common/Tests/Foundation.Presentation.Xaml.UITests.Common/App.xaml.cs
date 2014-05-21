using SquaredInfinity.Foundation.Presentation.Xaml;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace Foundation.Presentation.Xaml.UITests.Common
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            All.LoadAndMergeResourcesFromThisAssembly();

            SquaredInfinity.Foundation.Presentation.Resources
                .LoadAndMergeCompiledResourceDictionaryFromThisAssembly(@"AllResources.xaml");
        }
    }
}
