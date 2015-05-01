using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using SquaredInfinity.Foundation.Extensions;
using SquaredInfinity.Foundation.Presentation.ViewModels;

namespace SquaredInfinity.Foundation.Presentation.Views
{
    public partial class View : Control
    {
        #region View Model

        /// <summary>
        /// View Model Placeholder is used in late-viewmodel-binding cases.
        /// For example, one view (A) can contain another view (B) inside.
        /// B view model may be bound to A view model (in xaml control template).
        /// This binding will be ready by the time OnApplyTemplate() is called, 
        /// but changing ViewModel property before that (e.g. in constructor) will effecitvely break this binding.
        /// For that reason, OnApplyTemplate() will check if ViewModel property has binding set, 
        /// if it has, ViewModelPlaceholder will be discarded,
        /// otherwise ViewModel DP will be set to Placeholder value.
        /// 
        /// </summary>
        
        IHostAwareViewModel ViewModelPlaceholder { get; set; }
        
        bool IsTemplateLoaded { get; set; }

        public delegate void ViewModelEventRoutedEventHandler(object sender, ViewModelEventRoutedEventArgs e);

        public static readonly RoutedEvent PreviewViewModelEventProperty = 
            EventManager.RegisterRoutedEvent(
            "PreviewViewModelEvent", 
            RoutingStrategy.Bubble,
            typeof(ViewModelEventRoutedEventHandler), 
            typeof(View));

        public event ViewModelEventRoutedEventHandler PreviewViewModelEvent
        {
            add { AddHandler(PreviewViewModelEventProperty, value); }
            remove { RemoveHandler(PreviewViewModelEventProperty, value); }
        }

        public static readonly RoutedEvent ViewModelEventProperty =
            EventManager.RegisterRoutedEvent(
            "ViewModelEvent",
            RoutingStrategy.Tunnel,
            typeof(ViewModelEventRoutedEventHandler),
            typeof(View));

        public event ViewModelEventRoutedEventHandler ViewModelEvent
        {
            add { AddHandler(ViewModelEventProperty, value); }
            remove { RemoveHandler(ViewModelEventProperty, value); }
        }

        void RaiseViewModelEvent(IViewModel vm, IViewModelEvent ev, ViewModelEventRoutingStrategy routingStrategy)
        {
            if (routingStrategy.IsFlagSet(ViewModelEventRoutingStrategy.Bubble))
            {
                var preview_args = new ViewModelEventArgs(vm, ev, ViewModelEventRoutingStrategy.Bubble);

                // Raise Bubbling WPF Routed Event
                var wpf_preview_args = new ViewModelEventRoutedEventArgs(preview_args, PreviewViewModelEventProperty, this);

                RaiseEvent(wpf_preview_args);

                if (wpf_preview_args.Handled || preview_args.IsHandled)
                    return;
            }

            if (routingStrategy.IsFlagSet(ViewModelEventRoutingStrategy.Tunnel))
            {
                var args = new ViewModelEventArgs(vm, ev, ViewModelEventRoutingStrategy.Tunnel);

                // Raise Tunneling WPF Routed Event
                var wpf_args = new ViewModelEventRoutedEventArgs(args, ViewModelEventProperty, this);

                RaiseEvent(wpf_args);

                if (wpf_args.Handled || args.IsHandled)
                    return;
            }

            if(routingStrategy.IsFlagSet(ViewModelEventRoutingStrategy.BroadcastToChildren))
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

        internal protected virtual void OnViewModelEventInternal(ViewModelEventArgs args)
        {
            OnViewModelEvent(args);

            if (args.IsHandled)
                return;

            // todo: update interface
            (ViewModel as ViewModel).OnViewModelEventInternal(args);
        }

        public IHostAwareViewModel ViewModel
        {
            get 
            {
                if (!IsTemplateLoaded)
                    return ViewModelPlaceholder;

                return (IHostAwareViewModel)GetValue(ViewModelProperty); 
            }
            set 
            {
                if(!IsTemplateLoaded)
                {
                    ViewModelPlaceholder = value;
                    return;
                }

                SetValue(ViewModelProperty, value); 
            }
        }

        public static readonly DependencyProperty ViewModelProperty =
            DependencyProperty.Register(
            "ViewModel",
            typeof(IHostAwareViewModel),
            typeof(View),
            new FrameworkPropertyMetadata(null));

        #endregion

        public View() 
        {
            RefreshViewModel(oldDataContext: null, newDataContext: null);
            DataContextChanged += (s, e) => RefreshViewModel(e.OldValue, e.NewValue);
            DataContextChanged += (s, e) => OnAfterDataContextChanged();

            PreviewViewModelEvent += View_ViewModelMessage;
            ViewModelEvent += View_ViewModelMessage;

            this.Initialized += View_Initialized;
            this.Loaded += View_Loaded;
        }

        void View_Loaded(object sender, RoutedEventArgs e)
        {
            InitializeViewModelIfNeeded();
        }

        protected virtual void OnAfterDataContextChanged() { }

        void View_Initialized(object sender, EventArgs e)
        {
            InitializeViewModelIfNeeded();
        }

        void InitializeViewModelIfNeeded()
        {
            if (ViewModel != null)
            {
                ViewModel.DataContext = DataContext;

                ViewModel.Initialize(GetIsHostedInDialogWindow());
            }
        }

        public bool GetIsHostedInDialogWindow()
        {
            var isHostedInDialogWindow = false;

            var window = Window.GetWindow(this);

            if (window != null)
            {   
                isHostedInDialogWindow = window.IsDialog();
            }

            return isHostedInDialogWindow;
        }

        void View_ViewModelMessage(object sender, ViewModelEventRoutedEventArgs args)
        {
            OnViewModelEventInternal(args);
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            IsTemplateLoaded = true;

            // check if view model is bound to something
            // if it is, use the binding value (discard placeholder
            // otherwise use placeholder
            if (IsViewModelBound())
            {
                ViewModelPlaceholder = null;
            }
            else
            {
                ViewModel = ViewModelPlaceholder;
            }
        }

        void RefreshViewModel(object oldDataContext, object newDataContext)
        {
            // if View Model Property has binding, then there's nothing to do here
            // only continue with automatic VM Discovery when VM property is not bound
            if (IsTemplateLoaded && IsViewModelBound())
            {
                return;
            }

            OnBeforeOldViewModelRemoved(oldDataContext, ViewModel);

            var newVM = ResolveViewModel(this.GetType(), newDataContext);

            OnBeforeNewViewModelAddedInternal(newDataContext, newVM);

            if(ViewModel != null && !object.ReferenceEquals(ViewModel, newVM))
            {
                ViewModel.Dispose();
            }

            ViewModel = newVM;

            if (newVM != null)
            {
                newVM.DataContext = newDataContext;

                if(IsLoaded)
                {
                    newVM.Initialize(GetIsHostedInDialogWindow());
                }
            }
        }

        protected virtual bool IsViewModelBound()
        {
            var vmBinding = BindingOperations.GetBinding(this, ViewModelProperty);

            if (vmBinding == null)
                return false;

            return true;
        }

        protected virtual void OnBeforeOldViewModelRemoved(object oldDataContext, IHostAwareViewModel oldViewModel)
        { }

        IDisposable SUBSCRIPTION_AfterViewModelRaised;
        IDisposable SUBSCRIPTION_FindDataContextInVisualTree;

        void OnBeforeNewViewModelAddedInternal(object newDataContext, IHostAwareViewModel newViewModel)
        {
            if(newViewModel != null)
            {
                if (SUBSCRIPTION_AfterViewModelRaised != null)
                    SUBSCRIPTION_AfterViewModelRaised.Dispose();

                SUBSCRIPTION_AfterViewModelRaised =
                (newViewModel as ViewModel) // todo: update IViewModel interface
                    .CreateWeakEventHandler()
                    .ForEvent<AfterViewModelEventRaisedArgs>(
                    (vm, h) => vm.AfterViewModelEventRaised += h,
                    (vm, h) => vm.AfterViewModelEventRaised -= h)
                    .Subscribe((vm, args) => OnAfterViewModelEventRaised(vm as ViewModel, args.Event, args.RoutingStrategy));


                if (SUBSCRIPTION_FindDataContextInVisualTree != null)
                    SUBSCRIPTION_FindDataContextInVisualTree.Dispose();

                SUBSCRIPTION_FindDataContextInVisualTree =
                    (newViewModel as ViewModel)
                    .CreateWeakEventHandler()
                    .ForEvent<SquaredInfinity.Foundation.Presentation.ViewModels.ViewModel.FindDataContextInVisualTreeEventArgs>(
                    (vm, h) => vm.TryFindDataContext += h,
                    (vm, h) => vm.TryFindDataContext -= h)
                    .Subscribe((_s, _e) =>
                        {
                            var vp = this.GetVisualOrLogicalParent();

                            while(vp != null)
                            {
                                var fe = vp as FrameworkElement;

                                if(fe != null)
                                {
                                    var dx = fe.DataContext;

                                    if(dx != null)
                                    {
                                        var result = (object) null;

                                        if(dx.TryConvert(_e.DataContextType, out result))
                                        {
                                            _e.DataContext = result;
                                            _e.Handle();
                                            return;
                                        }
                                    }

                                    // if fe is a View, try to convert view model to desired type

                                    var v = fe as View;

                                    if(v != null)
                                    {
                                        var vm = v.ViewModel;

                                        if(vm != null)
                                        {
                                            var result = (object)null;

                                            if(vm.TryConvert(_e.DataContextType, out result))
                                            {
                                                _e.DataContext = result;
                                                _e.Handle();
                                                return;
                                            }
                                        }
                                    }
                                }

                                vp = vp.GetVisualOrLogicalParent();
                            }
                        });
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

        protected virtual void OnBeforeNewViewModelAdded(object newDataContext, IHostAwareViewModel newViewModel)
        { 

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
