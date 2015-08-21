using SquaredInfinity.Foundation.Presentation.Resources;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition.Hosting;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace Foundation.Presentation.Xaml.UITests
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
            ResourcesManager.ImportAndLoadAllResources(compositionContainer);

            //# Alternative way to load default SquaredInfinity.Presentation.Foundation resources (without MEF)
            // var resources = new SquaredInfinity.Foundation.Presentation.XamlResources();
            // resources.LoadAndMergeResources();

            // TODO: LOAD DEFAULT STYLES
            //var modern_style_resources = new SquaredInfinity.Foundation.Presentation.Styles.Modern.DefaultXamlResources();
            //modern_style_resources.LoadAndMergeResources();
            //modern_style_resources.ApplyAllStyles();
        }
    }
}
