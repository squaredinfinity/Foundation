using System;
using System.Collections.Generic;
using SquaredInfinity.Extensions;
using System.Reflection;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Xaml;
using System.IO;
using System.ComponentModel.Composition;
using System.Diagnostics;

namespace SquaredInfinity.Windows.Resources
{
    public static class ResourcesManager
    {
        static XamlResourcesImporter XamlResourcesImporter = new XamlResourcesImporter();


        public static IEnumerable<ResourceDictionary> ImportAllResources(ICompositionService compositionService)
        {
            if (compositionService == null)
                throw new ArgumentNullException(nameof(compositionService));

            return XamlResourcesImporter.ImportAllResources(compositionService, merge: false);
        }

        public static IEnumerable<ResourceDictionary> ImportAndMergeAllResources(ICompositionService compositionService)
        {
            if (compositionService == null)
                throw new ArgumentNullException(nameof(compositionService));

            return XamlResourcesImporter.ImportAllResources(compositionService, merge:true);
        }

        #region Load Compiled Resource Dictionary

        public static ResourceDictionary LoadCompiledResourceDictionaryFromThisAssembly(string resourceDictionaryRelativeUri)
        {
            string assembly_name = string.Empty;

            try
            {
                assembly_name = Assembly.GetCallingAssembly().FullName.Substring(@"[^\s,]*");

                var uri = new Uri($"{assembly_name};component/{resourceDictionaryRelativeUri}", UriKind.Relative);

                var resourceDictionary = LoadCompiledResourceDictionary(uri);

                return resourceDictionary;
            }
            catch(Exception ex)
            {
                ex.TryAddContextData("Assembly Name", () => assembly_name);
                ex.TryAddContextData("Uri", () => resourceDictionaryRelativeUri);

                throw ex;
            }
        }

        /// <summary>
        /// Loads a compiled (BAML) resource dictionary.
        /// </summary>
        /// <param name="resourceDictionaryUri">The resource dictionary URI  in format: 'applicationName;/component/pathToResource'.</param>
        /// <returns></returns>
        public static ResourceDictionary LoadCompiledResourceDictionary(Uri resourceDictionaryUri)
        {
            if(resourceDictionaryUri.IsAbsoluteUri)
            {
                // cannot use absolute uris here, make it relative

                var relativeUriString = resourceDictionaryUri.OriginalString.Replace(@"pack://application:,,,/", "");

                resourceDictionaryUri = new Uri(relativeUriString, UriKind.Relative);
            }

            ResourceDictionary result = Application.LoadComponent(resourceDictionaryUri) as ResourceDictionary;

            return result;
        }

        #endregion

        #region Load And Merge Complied Resource Dictionary

        /// <summary>
        /// Loads a compiled (BAML) resource dictionary and merges it with current Application dictionaries
        /// </summary>
        /// <param name="resourceDictionaryUri"></param>
        public static void LoadAndMergeCompiledResourceDictionary(Uri resourceDictionaryUri)
        {
            Application.Current.Resources.MergedDictionaries.Add(LoadCompiledResourceDictionary(resourceDictionaryUri));
        }

        public static void LoadAndMergeCompiledResourceDictionaryFromAssembly(string assemblyName, string resourceDictionaryRelativeUri)
        {
            try
            {
                var uri = new Uri($"{assemblyName};component/{resourceDictionaryRelativeUri}", UriKind.Relative);

                var resourceDictionary = LoadCompiledResourceDictionary(uri);

                MergeResourceDictionary(resourceDictionary);
            }
            catch (Exception ex)
            {
                ex.TryAddContextData("assemblyName", () => assemblyName);
                ex.TryAddContextData("resourceDictionaryRelativeUri", () => resourceDictionaryRelativeUri);
                throw ex;
            }
        }

        public static void LoadAndMergeCompiledResourceDictionaryFromThisAssembly(string resourceDictionaryRelativeUri)
        {
            var assembly_name = "";

            try
            {
                assembly_name = Assembly.GetCallingAssembly().FullName.Substring(@"[^\s,]*");

                LoadAndMergeCompiledResourceDictionaryFromAssembly(assembly_name, resourceDictionaryRelativeUri);
            }
            catch (Exception ex)
            {
                throw new Exception($"Unable to load resource '{resourceDictionaryRelativeUri}' from assembly '{assembly_name}'", ex);
            }
        }

        #endregion

        public static void MergeResourceDictionary(ResourceDictionary dictionaryToMerge)
        {
            MergeResourceDictionary(Application.Current.Resources, dictionaryToMerge);
        }

        public static void MergeResourceDictionary(ResourceDictionary target, ResourceDictionary dictionaryToMerge)
        {
            target.MergedDictionaries.Add(dictionaryToMerge);
        }

        public static void MergeAllResourceDictionaries(IEnumerable<ResourceDictionary> dictionaries)
        {
            MergeAllResourceDictionaries(Application.Current.Resources, dictionaries);
        }

        public static void MergeAllResourceDictionaries(ResourceDictionary target, IEnumerable<ResourceDictionary> dictionaries)
        {
            target.MergedDictionaries.AddRange(dictionaries);
        }

        /// <summary>
        /// Loads the resource dictionary from resource.
        /// Resource must be stored as a text (i.e. NOT be compiled to BAML)
        /// </summary>
        /// <param name="resourceDictionaryUri">The resource dictionary URI  in format: 'applicationName;/component/pathToResource'.</param>
        /// <returns></returns>
        public static ResourceDictionary LoadResourceDictionary(string resourceDictionaryUri)
        {
            ResourceDictionary result = null;
            var resourceInfo = Application.GetResourceStream(new Uri(resourceDictionaryUri, UriKind.Relative));
            using (StreamReader reader = new StreamReader(resourceInfo.Stream))
            {
                string xaml = reader.ReadToEnd();
                if (!string.IsNullOrEmpty(xaml))
                {
                    result = System.Windows.Markup.XamlReader.Parse(xaml) as ResourceDictionary;
                    //! Can't use XamlServices here, it supports subset of what WPF exposes
                    //x result = XamlServices.Parse(xaml) as ResourceDictionary;
                }
            }
            return result;
        }

        #region Load And Merge Resource Dictionary

        /// <summary>
        /// Loads the and merge resource dictionary.
        /// </summary>
        /// <param name="resourceDictionaryUri">The resource dictionary URI in format: 'applicationName;/component/pathToResource'.</param>
        public static void LoadAndMergeResourceDictionary(string resourceDictionaryUri)
        {
            Application.Current.Resources.MergedDictionaries.Add(LoadResourceDictionary(resourceDictionaryUri));
        }

        public static void LoadAndMergeResourceDictionaryFromAssembly(string assemblyName, string resourceDictionaryRelativeUri)
        {
            try
            {
                var resourceDictionary = LoadResourceDictionary($"{assemblyName};component/{resourceDictionaryRelativeUri}");

                MergeResourceDictionary(resourceDictionary);
            }
            catch (Exception ex)
            {
                ex.TryAddContextData("assemblyName", () => assemblyName);
                ex.TryAddContextData("resourceDictionaryRelativeUri", () => resourceDictionaryRelativeUri);
                throw ex;
            }
        }

        /// <summary>
        /// Loads the and merge resource dictionary located in this assembly.
        /// </summary>
        /// <param name="resourceDictionaryRelativeUri">The resource dictionary relative URI in format: 'Temes/Generic.xaml'.</param>
        public static void LoadAndMergeResourceDictionaryFromThisAssembly(string resourceDictionaryRelativeUri)
        {
            string assemblyName = Assembly.GetCallingAssembly().FullName.Substring(@"[^\s,]*");

            LoadAndMergeResourceDictionaryFromAssembly(assemblyName, resourceDictionaryRelativeUri);
        }

        #endregion

        public static bool CheckResourceExists(Uri resourceUri)
        {
            try
            {
                var resourceInfo = Application.GetResourceStream(resourceUri);

                return resourceInfo != null;
            }
            catch
            {
                return false;
            }
        }

        #region Load Embedded Resrouce

        /// <summary>
        /// Retrives a resource with specified relative name from this assembly.
        /// NOTE: this only works when AssemblyName is the same as the default namespace 
        /// (e.g. Assembly Name is MyAssembly and default namespace is also MyAssembly)
        /// If that's not the case then use overload with full name
        /// </summary>
        /// <param name="resourceName">name relative to assembly (e.g. Reosurces.MyResource.xml)</param>
        /// <returns></returns>
        public static Stream LoadEmbeddedResourceFromThisAssembly(string relativeResourceName)
        {
            var asm = Assembly.GetCallingAssembly();

            var asm_name = asm.GetName().Name;

            var resource_full_name = $"{asm_name}.{relativeResourceName}";

            return asm.GetManifestResourceStream(resource_full_name);
        }

        /// <summary>
        /// Retrives a resource with specified full name from this assembly.
        /// </summary>
        /// <param name="resourceNamespace">namespace of the resource (e.g. MyNamespace.Reosurces)</param>
        /// <param name="resourceName">name of the resource (e.g. MyResource.xml)</param>
        /// <returns></returns>
        public static Stream LoadEmbeddedResourceFromThisAssembly(string resourceNamespace, string resourceName)
        {
            var asm = Assembly.GetCallingAssembly();
            
            var resource_full_name = $"{resourceNamespace}.{resourceName}";

            return asm.GetManifestResourceStream(resource_full_name);
        }

        #endregion Load Embedded Resource

        #region Load Resource

        /// <summary>
        /// Loads the resource.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="resourceUri">The resource URI.</param>
        /// <returns></returns>
        public static T LoadResource<T>(string resourceUri)
            where T : class
        {
            var resourceInfo = Application.GetResourceStream(new Uri(resourceUri, UriKind.Relative));
            using (StreamReader reader = new StreamReader(resourceInfo.Stream))
            {
                string xaml = reader.ReadToEnd();
                if (!string.IsNullOrEmpty(xaml))
                {
#if SILVERLIGHT
                    return XamlReader.Load(xaml) as T;
#else
                    return XamlServices.Parse(xaml) as T;
#endif
                }
            }
            return null;
        }

        /// <summary>
        /// Loads the resource from ResourceDictionary specified by uri.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="resourceDictionaryUri">The resource dictionary URI in format: 'applicationName;/component/pathToResource'.</param>
        /// <param name="key">The key.</param>
        /// <returns></returns>
        public static T LoadResource<T>(string resourceDictionaryUri, string key)
            where T : class
        {
            T result = null;

            ResourceDictionary dictionary = LoadResourceDictionary(resourceDictionaryUri);

            result = dictionary[key] as T;
            // Remove key from dictionary - items such as Image won't be usable otherwise, 
            // as Silverlight will throw 'Element is already the child of another element' exception.
            dictionary.Remove(key);
            return result;
        }

        #endregion

        /// <summary>
        /// registeres pack uri scheme - use this when main application is a console app or unit test runner and not real wpf app
        /// </summary>
        public static void RegisterPackUriScheme()
        {
            //! creating instance of Application class will register pack uri scheme
            var ignored = new System.Windows.Application();
        }

        #region Load Image

        public static ImageSource LoadImage(Uri resourceUri)
        {
            var isc = new ImageSourceConverter();
            
            return new BitmapImage(resourceUri);
        }

        public static ImageSource LoadImageFromEntryAssembly(string resourceRelativeUri)
        {
            var uri = new Uri($"pack://application:,,,/{resourceRelativeUri}", UriKind.Absolute);

            return new BitmapImage(uri);
        }

        public static ImageSource LoadImageFromAssembly(string assemblyName, string resourceRelativeUri)
        {
            var uri = new Uri($"pack://application:,,,/{assemblyName};component/{resourceRelativeUri}", UriKind.Absolute);

            return new BitmapImage(uri);
        }

        #endregion

        #region Load Resource

        public static T LoadResourceFromAssembly<T>(string assemblyName, string resourceDictionaryRelativeUri)
            where T : class
        {
            return LoadResource<T>($"{assemblyName};component/{resourceDictionaryRelativeUri}");
        }

        /// <summary>
        /// Loads the resource from ResourceDictionary specified by uri.
        /// The ResourceDictionary must have Build Action set to 'Resource' (do not copy).
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="resourceDictionaryRelativeUri">The resource dictionary relative URI in format: 'Resource/MyDictionary.xaml'.</param>
        /// <param name="key">The key.</param>
        /// <returns></returns>
        public static T LoadResourceFromThisAssembly<T>(string resourceDictionaryRelativeUri, string key)
            where T : class
        {
            string assemblyName = Assembly.GetCallingAssembly().FullName.Substring(@"[^\s,]*");
            return LoadResource<T>($"{assemblyName};component/{resourceDictionaryRelativeUri}", key);
        }

        public static T LoadResourceFromThisAssembly<T>(string resourceRelativeUri)
            where T : class
        {
            string assemblyName = Assembly.GetCallingAssembly().FullName.Substring(@"[^\s,]*");
            return LoadResource<T>($"{assemblyName};component/{resourceRelativeUri}");
        }

        #endregion

        #region Get Absolute Uri

        /// <summary>
        /// Gets the absolute resource url for resource in this application.
        /// </summary>
        /// <param name="resourceRelativePath"></param>
        /// <returns></returns>
        /// <example>
        /// Resources.GetAbsoluteAppResourceUri(@"UI/Resource/1.png");
        /// returns: pack://application:,,,/applicationName;component/UI/Resource/1.png
        /// </example>
        public static Uri GetAbsoluteAppResourceUri(string resourceRelativePath)
        {
            return GetAbsoluteResourceUri(resourceRelativePath, Assembly.GetEntryAssembly());
        }

        public static Uri GetAbsoluteResourceUriFromThisAssembly(string resourceRelativePath)
        {
            return GetAbsoluteResourceUri(resourceRelativePath, Assembly.GetCallingAssembly());
        }

        public static Uri GetAbsoluteResourceUri(string resourceRelativePath, Assembly assembly)
        {
            string assemblyName = assembly.FullName.Substring(@"[^\s,]*");
            var uri = $"pack://application:,,,/{assemblyName};component/{resourceRelativePath}";

            return new Uri(uri, UriKind.Absolute);
        }

        /// <summary>
        /// Gets the absolute resource uri
        /// </summary>
        /// <param name="resourceRelativePath"></param>
        /// <returns></returns>
        /// <example>
        /// Resources.GetAbsoluteAppResourceUri("applicationName", @"UI/Resource/1.png");
        /// returns: pack://application:,,,/applicationName;component/UI/Resource/1.png
        /// </example>
        public static Uri GetAbsoluteResourceUri(string applicationName, string resourceRelativePath)
        {
            var uri = $"pack://application:,,,/{applicationName};component/{resourceRelativePath}";

            return new Uri(uri, UriKind.Absolute);
        }

        #endregion
    }
}
