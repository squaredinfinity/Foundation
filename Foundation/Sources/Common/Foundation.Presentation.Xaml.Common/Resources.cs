using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Xaml;
using System.IO;
using SquaredInfinity.Foundation;

using SquaredInfinity.Foundation.Extensions;

namespace SquaredInfinity.Foundation.Presentation
{
    public static class Resources
    {
        /// <summary>
        /// Loads a compiled (BAML) resource dictionary.
        /// </summary>
        /// <param name="resourceDictionaryUri">The resource dictionary URI  in format: 'applicationName;/component/pathToResource'.</param>
        /// <returns></returns>
        public static ResourceDictionary LoadCompiledResourceDictionary(string resourceDictionaryUri)
        {
            ResourceDictionary result = Application.LoadComponent(new Uri(resourceDictionaryUri, UriKind.Relative)) as ResourceDictionary;

            return result;
        }

        /// <summary>
        /// Loads a compiled (BAML) resource dictionary and merges it with current Application dictionaries
        /// </summary>
        /// <param name="resourceDictionaryUri"></param>
        public static void LoadAnMergeCompiledResourceDictionary(string resourceDictionaryUri)
        {
            Application.Current.Resources.MergedDictionaries.Add(LoadCompiledResourceDictionary(resourceDictionaryUri));
        }

        public static void LoadAndMergeCompiledResourceDictionaryFromAssembly(string assemblyName, string resourceDictionaryRelativeUri)
        {
            try
            {
                var resourceDictionary = LoadCompiledResourceDictionary("{0};component/{1}".FormatWith(assemblyName, resourceDictionaryRelativeUri));

                MergeResourceDictionary(resourceDictionary);
            }
            catch (Exception ex)
            {
                //ex.TryAddContextData("assemblyName", () => assemblyName);
                //ex.TryAddContextData("resourceDictionaryRelativeUri", () => resourceDictionaryRelativeUri);
                throw ex;
            }
        }

        public static void MergeResourceDictionary(ResourceDictionary dict)
        {
            Application.Current.Resources.MergedDictionaries.Add(dict);
            return;
        }

        public static void LoadAndMergeCompiledResourceDictionaryFromThisAssembly(string resourceDictionaryRelativeUri)
        {
            string assemblyName = Assembly.GetCallingAssembly().FullName.Substring(@"[^\s,]*");

            LoadAndMergeCompiledResourceDictionaryFromAssembly(assemblyName, resourceDictionaryRelativeUri);
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
                var resourceDictionary = LoadResourceDictionary("{0};component/{1}".FormatWith(assemblyName, resourceDictionaryRelativeUri));

                MergeResourceDictionary(resourceDictionary);
            }
            catch (Exception ex)
            {
                //ex.TryAddContextData("assemblyName", () => assemblyName);
                //ex.TryAddContextData("resourceDictionaryRelativeUri", () => resourceDictionaryRelativeUri);
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

        /// <summary>
        /// registeres pack uri scheme - use this when main application is a console app or unit test runner and not real wpf app
        /// </summary>
        public static void RegisterPackUriScheme()
        {
            //! creating instance of Application class will register pack uri scheme
            var ignored = new System.Windows.Application();
        }

        public static ImageSource LoadImageFromEntryAssembly(string resourceRelativeUri)
        {
            var uri = new Uri("pack://application:,,,/{0}".FormatWith(resourceRelativeUri), UriKind.Absolute);

            return BitmapFrame.Create(uri);
        }

        public static ImageSource LoadImageFromAssembly(string assemblyName, string resourceRelativeUri)
        {
            var uri = new Uri("pack://application:,,,/{0};component/{1}".FormatWith(assemblyName, resourceRelativeUri), UriKind.Absolute);

            return BitmapFrame.Create(uri);
        }

        public static T LoadResourceFromAssembly<T>(string assemblyName, string resourceDictionaryRelativeUri)
            where T : class
        {
            return LoadResource<T>("{0};component/{1}".FormatWith(assemblyName, resourceDictionaryRelativeUri));
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
            return LoadResource<T>("{0};component/{1}".FormatWith(assemblyName, resourceDictionaryRelativeUri), key);
        }

        public static T LoadResourceFromThisAssembly<T>(string resourceRelativeUri)
            where T : class
        {
            string assemblyName = Assembly.GetCallingAssembly().FullName.Substring(@"[^\s,]*");
            return LoadResource<T>("{0};component/{1}".FormatWith(assemblyName, resourceRelativeUri));
        }

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
            var uri = "pack://application:,,,/{0};component/{1}".FormatWith(assemblyName, resourceRelativePath);

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
            var uri = "pack://application:,,,/{0};component/{1}".FormatWith(applicationName, resourceRelativePath);

            return new Uri(uri, UriKind.Absolute);
        }
    }
}
