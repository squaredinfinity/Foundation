using SquaredInfinity.Foundation.Presentation.ViewModels;
using SquaredInfinity.Foundation.Presentation.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace SquaredInfinity.Foundation.Presentation.Windows
{
    public partial class ViewHostWindow : Window
    {
        ContentPresenter PART_ContentPresenter;

        public IHostAwareViewModel ViewModel
        {
            get { return (IHostAwareViewModel)GetValue(ViewModelProperty); }
            set { SetValue(ViewModelProperty, value); }
        }

        public static readonly DependencyProperty ViewModelProperty =
            DependencyProperty.Register(
            "ViewModel",
            typeof(IHostAwareViewModel),
            typeof(ViewHostWindow),
            new FrameworkPropertyMetadata(null));

        #region Is Dialog Window

        public bool IsDialog
        {
            get { return (bool)GetValue(IsDialogProperty); }
            set { SetValue(IsDialogProperty, value); }
        }

        public static readonly DependencyProperty IsDialogProperty =
            DependencyProperty.Register(
            "IsDialog",
            typeof(bool),
            typeof(ViewHostWindow),
            new FrameworkPropertyMetadata(false, OnIsDialogChanged));

        static void OnIsDialogChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var vhw = d as ViewHostWindow;

            if ((bool)e.NewValue == true)
            {
                vhw.SetValue(Window.MaxHeightProperty, System.Windows.SystemParameters.PrimaryScreenHeight);
                vhw.SetValue(Window.MaxWidthProperty, System.Windows.SystemParameters.PrimaryScreenWidth);
            }
        }

        #endregion

        #region Hosted View

        public FrameworkElement HostedView
        {
            get { return (FrameworkElement)GetValue(HostedViewProperty); }
            set { SetValue(HostedViewProperty, value); }
        }

        public static readonly DependencyProperty HostedViewProperty =
            DependencyProperty.Register(
            "HostedView", 
            typeof(FrameworkElement), 
            typeof(ViewHostWindow), 
            new PropertyMetadata(null));
       
        #endregion

        

        public event View.ViewModelEventRoutedEventHandler PreviewViewModelEvent
        {
            add { AddHandler(View.PreviewViewModelEventProperty, value); }
            remove { RemoveHandler(View.PreviewViewModelEventProperty, value); }
        }

        public ViewHostWindow()
        {
            this.LayoutUpdated += DefaultDialogWindow_LayoutUpdated;

            PreviewViewModelEvent += ViewHostWindow_PreviewViewModelEvent;
        }

        void ViewHostWindow_PreviewViewModelEvent(object sender, Views.ViewModelEventRoutedEventArgs args)
        {
            OnViewModelEventInternal(args);
        }

        void OnViewModelEventInternal(ViewModelEventRoutedEventArgs args)
        {
            OnViewModelEventInternal(args.ViewModelEventArgs);

            if (args.ViewModelEventArgs.IsHandled)
                return;

            if (args.RoutedEvent.RoutingStrategy == RoutingStrategy.Bubble)
            {
                // todo: update interface
                (ViewModel as ViewModel).OnPreviewViewModelEventInternal(args.ViewModelEventArgs);
                return;
            }

            if (args.RoutedEvent.RoutingStrategy == RoutingStrategy.Tunnel)
            {
                // todo: update interface
                (ViewModel as ViewModel).OnViewModelEventInternal(args.ViewModelEventArgs);
                return;
            }
        }

        protected virtual void OnViewModelEvent(ViewModelEventArgs args)
        { }

        protected virtual void OnViewModelEventInternal(ViewModelEventArgs args)
        {
            OnViewModelEvent(args);

            if (args.IsHandled)
                return;

            // todo: update interface
            (ViewModel as ViewModel).OnViewModelEventInternal(args);
        }

        void DefaultDialogWindow_LayoutUpdated(object sender, EventArgs e)
        {
            if (PART_ContentPresenter == null || PART_ContentPresenter.Content == null)
                return;

            var fe = PART_ContentPresenter.Content as FrameworkElement;

            if (fe != null)
            {
                fe.ClearValue(FrameworkElement.WidthProperty);
                fe.ClearValue(FrameworkElement.HeightProperty);
            }

            SizeToContent = SizeToContent.Manual;

            ClearValue(FrameworkElement.HeightProperty);
            ClearValue(FrameworkElement.WidthProperty);
            ClearValue(FrameworkElement.MaxHeightProperty);
            ClearValue(FrameworkElement.MaxWidthProperty);

            this.LayoutUpdated -= DefaultDialogWindow_LayoutUpdated;
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            PART_ContentPresenter = FindName("PART_content_presenter") as ContentPresenter;
        }
    }
}
