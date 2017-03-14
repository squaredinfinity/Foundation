using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace SquaredInfinity.Presentation.Windows
{
    public partial class ViewHostWindow : IViewModelHost
    {
        #region Hosted View Model
        
        public IHostAwareViewModel HostedViewModel
        {
            get { return (IHostAwareViewModel)GetValue(HostedViewModelProperty); }
            set { SetValue(HostedViewModelProperty, value); }
        }

        public static readonly DependencyProperty HostedViewModelProperty =
            DependencyProperty.Register(
            "HostedViewModel", 
            typeof(IHostAwareViewModel), 
            typeof(ViewHostWindow),
            new PropertyMetadata(null, OnHostedViewModelChanged));

        static void OnHostedViewModelChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var oldHostedViewModel = e.OldValue as IHostAwareViewModel;
            if (oldHostedViewModel != null)
                oldHostedViewModel.ViewModelHost = null;

            var newHostedViewModel = e.NewValue as IHostAwareViewModel;
            if(newHostedViewModel != null)
            {
                var vhw = d as ViewHostWindow;
                newHostedViewModel.ViewModelHost = vhw;
            }
        }

        #endregion
    }
}
