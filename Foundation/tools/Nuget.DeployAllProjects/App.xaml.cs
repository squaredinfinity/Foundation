using SquaredInfinity.Foundation.Presentation.Resources;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition.Hosting;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace Nuget.DeployAllProjects
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            //# Initialize MEF
            var applicationCatalog = new ApplicationCatalog();
            var compositionContainer = new CompositionContainer(applicationCatalog);
            compositionContainer.Compose(new CompositionBatch());

            //# Import Xaml Resources
            ResourcesManager.ImportAndMergeAllResources(compositionContainer);
        }
    }
}
