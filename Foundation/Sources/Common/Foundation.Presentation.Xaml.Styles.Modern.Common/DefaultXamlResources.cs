﻿using SquaredInfinity.Foundation.Presentation;
using SquaredInfinity.Foundation.Presentation.Resources;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace  SquaredInfinity.Foundation.Presentation.Styles.Modern
{
    [XamlResourcesProviderMetadata(ImportOrder = XamlResources.ImportOrder)]
    public partial class DefaultXamlResources : IXamlResourcesProvider
    {
        public const uint ImportOrder = SquaredInfinity.Foundation.Presentation.XamlResources.ImportOrder + 100;

        public IEnumerable<ResourceDictionary> LoadResources()
        {
            yield return ResourcesManager.LoadCompiledResourceDictionaryFromThisAssembly(@"All.xaml");
        }

        public void ApplyAllStyles()
        {
            ApplyAllStyles(Application.Current.Resources);
        }

        public void ApplyAllStyles(ResourceDictionary resourceDictionary)
        {
            resourceDictionary[typeof(Button)] = Application.Current.Resources["Styles.Button"];
            resourceDictionary[typeof(CheckBox)] = Application.Current.Resources["Styles.CheckBox"];
            resourceDictionary[typeof(ComboBox)] = Application.Current.Resources["Styles.ComboBox"];
            resourceDictionary[typeof(Expander)] = Application.Current.Resources["Styles.Expander"];
            resourceDictionary[typeof(Menu)] = Application.Current.Resources["Styles.Menu"];
            resourceDictionary[typeof(MenuItem)] = Application.Current.Resources["Styles.MenuItem"];
            resourceDictionary[typeof(ListView)] = Application.Current.Resources["Styles.ListView"];
            resourceDictionary[typeof(ListViewItem)] = Application.Current.Resources["Styles.ListViewItem"];
            resourceDictionary[typeof(RadioButton)] = Application.Current.Resources["Styles.RadioButton"];
            resourceDictionary[typeof(ScrollBar)] = Application.Current.Resources["Styles.ScrollBar"];
            resourceDictionary[typeof(TabControl)] = Application.Current.Resources["Styles.TabControl"];
            resourceDictionary[typeof(TabItem)] = Application.Current.Resources["Styles.TabItem"];
            resourceDictionary[typeof(TextBox)] = Application.Current.Resources["Styles.TextBox"];
            resourceDictionary[typeof(TextBlock)] = Application.Current.Resources["Styles.TextBlock"];
            resourceDictionary[typeof(Label)] = Application.Current.Resources["Styles.Label"];
        }
    }
}
