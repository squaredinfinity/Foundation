using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Documents;

namespace SquaredInfinity.Foundation.Presentation.Behaviors
{
    public partial class OpenInDefaultBrowser
    {
        #region IsEnabled

        public static readonly DependencyProperty IsEnabledProperty = 
            DependencyProperty.RegisterAttached(
            "IsEnabled",
            typeof(bool),
            typeof(OpenInDefaultBrowser), 
            new PropertyMetadata(false, OnIsEnabledChanged));

        public static void SetIsEnabled(Hyperlink element, bool value)
        {
            element.SetValue(IsEnabledProperty, value);
        }

        public static bool GetIsEnabled(Hyperlink element)
        {
            return (bool)element.GetValue(IsEnabledProperty);
        }

        #endregion

        static void OnIsEnabledChanged(DependencyObject owner, DependencyPropertyChangedEventArgs e)
        {
            var hyperlink = owner as Hyperlink;

            if ((bool)e.NewValue == true)
            {
                hyperlink.RequestNavigate += hyperlink_RequestNavigate;
            }
            else
            {
                hyperlink.RequestNavigate -= hyperlink_RequestNavigate;
            }
        }

        static void hyperlink_RequestNavigate(object sender, System.Windows.Navigation.RequestNavigateEventArgs e)
        {
            if (e.Uri.Scheme == "file")
            {
                Process.Start(new ProcessStartInfo(e.Uri.LocalPath));
            }
            else
            {
                Process.Start(new ProcessStartInfo(e.Uri.AbsoluteUri));
            }

            e.Handled = true;
        }
    }
}
