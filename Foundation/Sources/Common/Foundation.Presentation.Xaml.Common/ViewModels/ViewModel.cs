using SquaredInfinity.Foundation.Presentation.Commands;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace SquaredInfinity.Foundation.Presentation.ViewModels
{
    public abstract partial class ViewModel<TDataContext> : ViewModel
    {
        TDataContext _dataContext;
        public new TDataContext DataContext
        {
            get { return (TDataContext) base.DataContext; }
            set
            {
                base.DataContext = value;
                RaiseThisPropertyChanged();
            }
        }

        protected virtual void OnAfterDataContextChanged(TDataContext newDataContext)
        {
            
        }

        protected override void OnAfterDataContextChanged(object newDataContext)
        {
            base.OnAfterDataContextChanged(newDataContext);

            // Proper Data Context on view model may be set via binding (e.g. <MyView DataContext={Binding SomeProperty} /> -> Changing Data Context on View in this way will also set DataContext on View Model)
            // In that case initial data context may be of a wrong type
            if (!(newDataContext is TDataContext))
            {
                // Log warning
            }
            else
            {
                this.OnAfterDataContextChanged((TDataContext)newDataContext);
            }
        }

        public ViewModel()
            : base()
        { }

        public ViewModel(IUIService uiService)
            : base(uiService)
        { }
    }

    /// <summary>
    /// Base class for all viewmodels
    /// </summary>
    public abstract partial class ViewModel : NotifyPropertyChangedObject, IViewModel
    {
        object _dataContext;
        public object DataContext
        {
            get { return _dataContext; }
            set
            {
                _dataContext = value;
                OnAfterDataContextChanged(_dataContext);
                RaiseThisPropertyChanged();
            }
        }

        protected IUIService UIService { get; private set; }

        protected virtual void OnAfterDataContextChanged(object newDataContext)
        {

        }

        
        class ViewModelEventSubscription
        {
            public string EventName { get; set; }
            public Action<ViewModelEventArgs> OnEventAction { get; set; }
        }

        class ViewModelEventSubscription<TEvent>
        {

        }

        ConcurrentBag<ViewModelEventSubscription> PreviewViewModelEventSubscriptions = new ConcurrentBag<ViewModelEventSubscription>();
        ConcurrentBag<ViewModelEventSubscription> ViewModelEventSubscriptions = new ConcurrentBag<ViewModelEventSubscription>();

        internal void OnPreviewViewModelEventInternal(ViewModelEventArgs args)
        {
            OnPreviewViewModelEvent(args);

            if (args.IsHandled)
                return;

            // check subscriptions, invoke handlers

            var subscriptions = PreviewViewModelEventSubscriptions.ToArray();

            foreach(var s in subscriptions)
            {
                if (!string.Equals(s.EventName, args.Event.Name, StringComparison.InvariantCultureIgnoreCase))
                    continue;

                s.OnEventAction(args);

                if (args.IsHandled)
                    return;
            }
        }

        protected virtual void OnPreviewViewModelEvent(ViewModelEventArgs args)
        {

        }

        internal void OnViewModelEventInternal(ViewModelEventArgs args)
        {
            OnViewModelEvent(args);

            if (args.IsHandled)
                return;

            // check subscriptions, invoke handlers

            var subscriptions = ViewModelEventSubscriptions.ToArray();

            foreach (var s in subscriptions)
            {
                if (!string.Equals(s.EventName, args.Event.Name, StringComparison.InvariantCultureIgnoreCase))
                    continue;

                s.OnEventAction(args);

                if (args.IsHandled)
                    return;
            }
        }

        protected virtual void OnViewModelEvent(ViewModelEventArgs ev)
        {

        }

        protected void SubscribeToPreviewEvent(string eventName, Action<ViewModelEventArgs> args)
        {
            var s = new ViewModelEventSubscription();
            s.EventName = eventName.ToLower();
            s.OnEventAction = args;

            PreviewViewModelEventSubscriptions.Add(s);
        }

        protected void SubscribeToEvent(string eventName, Action<ViewModelEventArgs> onEvent)
        {
            var s = new ViewModelEventSubscription();
            s.EventName = eventName.ToLower();
            s.OnEventAction = onEvent;

            ViewModelEventSubscriptions.Add(s);
        }

        ViewModelState _state = ViewModelStates.Initialising;
        /// <summary>
        /// Gets or sets the state of a ViewModel
        /// </summary>
        /// <value>The state.</value>
        public ViewModelState State
        {
            get { return _state; }
            set
            {
                _state = value;
                RaisePropertyChanged(() => State);

            }
        }

        public ViewModel()
        { }

        public ViewModel(IUIService uiService)
        {
            this.UIService = uiService;
        }

        internal event EventHandler<AfterViewModelEventRaisedArgs> AfterViewModelEventRaised;

        /// <summary>
        /// Call this method to raise a View Model Event.
        /// </summary>
        /// <param name="eventName"></param>
        /// <param name="routingStrategy"></param>
        protected internal void RaiseEvent(
            string eventName, 
            ViewModelEventRoutingStrategy routingStrategy = ViewModelEventRoutingStrategy.Default)
        {
            if (AfterViewModelEventRaised != null)
            {
                var ev = new ViewModelEvent(eventName);

                var args = new AfterViewModelEventRaisedArgs(ev, routingStrategy);

                AfterViewModelEventRaised(this, args);
            }
        }
    }
}
