using SquaredInfinity.Foundation.Presentation.ViewModels;
using SquaredInfinity.Foundation.Presentation.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using SquaredInfinity.Foundation.Extensions;
using SquaredInfinity.Foundation.Presentation.Behaviors;
using SquaredInfinity.Foundation.Maths.Graphs.Trees;

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
            RefreshViewModel(oldDataContext: null, newDataContext: null);
            DataContextChanged += (s, e) => RefreshViewModel(e.OldValue, e.NewValue);

            this.LayoutUpdated += DefaultDialogWindow_LayoutUpdated;

            PreviewViewModelEvent += ViewHostWindow_PreviewViewModelEvent;

            Initialized += ViewHostWindow_Initialized;
        }

        void ViewHostWindow_Initialized(object sender, EventArgs e)
        {
            if (ViewModel != null)
            {
                var isHostedInDialogWindow = false;

                var window = Window.GetWindow(this);

                if (window != null)
                {
                    isHostedInDialogWindow = window.IsDialog();
                }

                ViewModel.Initialize(isHostedInDialogWindow);
            }
        }

        void RefreshViewModel(object oldDataContext, object newDataContext)
        {
            var newVM = ResolveViewModel(this.GetType(), newDataContext);

            if(newVM != null)
                newVM.DataContext = newDataContext;

            OnBeforeNewViewModelAddedInternal(newDataContext, newVM);

            if (ViewModel != null && !object.ReferenceEquals(ViewModel, newVM))
            {
                ViewModel.Dispose();
            }

            ViewModel = newVM;
        }

        IDisposable AfterViewModelRaisedSubscription;

        void OnBeforeNewViewModelAddedInternal(object newDataContext, IHostAwareViewModel newViewModel)
        {
            if (newViewModel != null)
            {
                AfterViewModelRaisedSubscription =
                (newViewModel as ViewModel) // todo: update IViewModel interface
                    .CreateWeakEventHandler()
                    .ForEvent<AfterViewModelEventRaisedArgs>(
                    (vm, h) => vm.AfterViewModelEventRaised += h,
                    (vm, h) => vm.AfterViewModelEventRaised -= h)
                    .Subscribe((vm, args) => OnAfterViewModelEventRaised(vm as ViewModel, args.Event, args.RoutingStrategy));
            }

            OnBeforeNewViewModelAdded(newDataContext, newViewModel);
        }

        void OnAfterViewModelEventRaised(
            IViewModel viewModel,
            IViewModelEvent ev,
            ViewModelEventRoutingStrategy routingStrategy)
        {
            RaiseViewModelEvent(viewModel, ev, routingStrategy);
        }

        void RaiseViewModelEvent(IViewModel vm, IViewModelEvent ev, ViewModelEventRoutingStrategy routingStrategy)
        {
            if (routingStrategy.HasFlag(ViewModelEventRoutingStrategy.Bubble))
            {
                var preview_args = new ViewModelEventArgs(vm, ev, ViewModelEventRoutingStrategy.Bubble);

                // Raise Bubbling WPF Routed Event
                var wpf_preview_args = new ViewModelEventRoutedEventArgs(preview_args, View.PreviewViewModelEventProperty, this);

                RaiseEvent(wpf_preview_args);

                if (wpf_preview_args.Handled || preview_args.IsHandled)
                    return;
            }

            if (routingStrategy.HasFlag(ViewModelEventRoutingStrategy.Tunnel))
            {
                var args = new ViewModelEventArgs(vm, ev, ViewModelEventRoutingStrategy.Tunnel);

                // Raise Tunneling WPF Routed Event
                var wpf_args = new ViewModelEventRoutedEventArgs(args, View.ViewModelEventProperty, this);

                RaiseEvent(wpf_args);

                if (wpf_args.Handled || args.IsHandled)
                    return;
            }

            if (routingStrategy.HasFlag(ViewModelEventRoutingStrategy.BroadcastToChildren))
            {
                // search visual tree for children deriving from View

                var children =
                    this.VisualTreeTraversal(
                    includeChildItemsControls: true,
                    traversalMode: TreeTraversalMode.BreadthFirst)
                    .OfType<View>();

                var args = new ViewModelEventArgs(vm, ev, ViewModelEventRoutingStrategy.BroadcastToChildren);

                foreach (var v in children)
                {
                    v.OnViewModelEventInternal(args);

                    if (args.IsHandled)
                        return;
                }
            }
        }

        protected virtual void OnBeforeNewViewModelAdded(object newDataContext, IHostAwareViewModel newViewModel)
        {

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

            if (ViewModel == null)
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

            if (ViewModel != null)
            {
                // todo: update interface
                (ViewModel as ViewModel).OnViewModelEventInternal(args);
            }
        }

        void DefaultDialogWindow_LayoutUpdated(object sender, EventArgs e)
        {
            if (PART_ContentPresenter == null || PART_ContentPresenter.Content == null)
                return;

            this.LayoutUpdated -= DefaultDialogWindow_LayoutUpdated;

            var fe = PART_ContentPresenter.Content as FrameworkElement;

            if (fe != null)
            {
                var width = DialogHost.GetWidth(fe);
                var height = DialogHost.GetHeight(fe);

                var minWidth = DialogHost.GetMinWidth(fe);
                var minHeight = DialogHost.GetMinHeight(fe);

                SizeToContent = SizeToContent.Manual;

                // those must be set *after* size to content is set to manual
                if (height != null)
                    SetValue(FrameworkElement.HeightProperty, height.Value);

                if(width != null)
                    SetValue(FrameworkElement.WidthProperty, width.Value);

                if(minHeight != null)
                    SetValue(FrameworkElement.MinHeightProperty, minHeight.Value);

                if(minWidth != null)
                    SetValue(FrameworkElement.MinWidthProperty, minWidth.Value);
            }
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            PART_ContentPresenter = FindName("PART_content_presenter") as ContentPresenter;
        }

        protected virtual IHostAwareViewModel ResolveViewModel(Type viewType, object newDatacontext)
        {
            var vmTypeName = viewType.FullName + "Model";

            var vmType = viewType.Assembly.GetType(vmTypeName);

            if (vmType == null)
                return null;

            return Activator.CreateInstance(vmType) as IHostAwareViewModel;
        }
    }
}
