Foundation Presentation Xaml
=

[TOC]

#overview


#Resources

##Loading Foundation Presentation Xaml Resources
Foundation Presenation Xaml exposes different Xaml Resources (converters, data template selectors) which can be automatically loaded into your application by calling 
*ResourcesManager.ImportAndLoadResources()* (if your application uses MEF), or going for XamlResources class directly:

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
            // var resources = new SquaredInfinity.Presentation.XamlResources();
            // resources.LoadAndMergeResources();
        }
    }

##Resources In Modular Application
[See UITestsApp.xaml.cs]
In a composite application with several dependencies Xaml Resources must often be loaded in a specific order.
This may be achieved by implementing IXamlResourcesProvider in each referenced assembly and using MEF (via ResourcesManger) to import and merge all Xaml Resources in correct order.

Assembly A (Foundation.Presentation.Xaml) can export IXamlResourcesProvider class which loads and merges all required Xaml Resources in LoadAndMergeResources method:

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

Assembly B (Foundation.Presentation.Xaml.UITests) exposes Xaml Resources which may depend on resources from assembly A.
Assembly B resources will be imported after Assembly A resources thanks to higher ImportOrder:

    [ExportAttribute(typeof(IXamlResourcesProvider))]
    [XamlResourcesProviderMetadata(ImportOrder = XamlResources.ImportOrder)]
    public class XamlResources : IXamlResourcesProvider
    {
        // Import Order is higher than Foundation.Presentation Import Order (on which resources from this assembly may depend)
        public const int ImportOrder = SquaredInfinity.Presentation.XamlResources.ImportOrder + 100;

        public void LoadAndMergeResources()
        {
            ResourcesManager.LoadAndMergeCompiledResourceDictionaryFromThisAssembly("XamlResources.xaml");
        }
    }

All resources are loaded and merged to main Windows Application in App.xaml.cs OnStartup() method using ResourcesManager class.
This class will use MEF to import all IXamlResourcesProviders and load and merge them in a correct order.

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
        }
    }

#Converters


#MVVM

