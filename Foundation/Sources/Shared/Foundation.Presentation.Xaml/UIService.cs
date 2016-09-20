using SquaredInfinity.Foundation.Presentation.Views;
using SquaredInfinity.Foundation.Presentation.Windows;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace SquaredInfinity.Foundation.Presentation
{
    public abstract class UIService : IUIService
    {
        internal static Dispatcher GetMainThreadDispatcher()
        {
            if (Application.Current != null && Application.Current.Dispatcher != null)
                return Application.Current.Dispatcher;

            return Dispatcher.CurrentDispatcher;
        }

        public Dispatcher UIDispatcher { get; private set; }

        public UIService()
            : this(GetMainThreadDispatcher())
        {}

        public UIService(Dispatcher uiDispatcher)
        {
            this.UIDispatcher = uiDispatcher;
        }

        public void ChangeDispatcher(Dispatcher newDispatcher)
        {
            this.UIDispatcher = newDispatcher;
        }

        /// <summary>
        /// Displays a tool window to the user.
        /// </summary>
        /// <param name="viewModel"></param>
        public abstract void ShowToolWindow(View view, Func<ViewHostWindow> getWindow = null);

        /// <summary>
        /// Displays a dialog window to the user using default DialogScope and DialogMode.
        /// </summary>
        /// <param name="viewModel"></param>
        public virtual void ShowDialog(View view)
        {
            ShowDialog(view, DialogScope.Default, DialogMode.Default);
        }
        
        /// <summary>
        /// Displays a dialog window to the user.
        /// </summary>
        /// <param name="viewModel"></param>
        /// <param name="dialogScope"></param>
        public abstract void ShowDialog(
            View view, 
            DialogScope dialogScope, 
            DialogMode dialogMode, 
            bool showActivated = true,
            Func<ViewHostWindow> getWindow = null);

        public abstract IHostAwareViewModel ShowConfirmationDialog(string message, string dialogTitle);

        /// <summary>
        /// Shows an alert window using default DialogScope and DialogMode.
        /// </summary>
        /// <param name="viewModel"></param>
        public virtual void ShowAlert(View view)
        {
            ShowAlert(view, DialogScope.Default, DialogMode.Default);
        }

        public abstract void ShowAlert(View view, DialogScope dialogScope, DialogMode dialogMode);

        public IHostAwareViewModel ShowAlert(string message)
        {
            var hostAwareViewModel = GetDefaultAlertViewModel(message);

            

            //ShowAlert(hostAwareViewModel);

            return hostAwareViewModel;
        }

        public IHostAwareViewModel ShowAlert(string message, string title)
        {
            var hostAwareViewModel = GetDefaultAlertViewModel(message);

            hostAwareViewModel.Title = title;

            //ShowAlert(hostAwareViewModel);

            return hostAwareViewModel;
        }

        /// <summary>
        /// Gets the default view model for Alerts.
        /// Caller can replace the default View with a custom view.
        /// </summary>
        /// <returns></returns>
        public virtual IHostAwareViewModel GetDefaultAlertViewModel()
        {
            return GetDefaultAlertViewModel(string.Empty);
        }

        /// <summary>
        /// Gets the default view model for Alerts.
        /// Caller can replace the default View with a custom view.
        /// </summary>
        /// <returns></returns>
        public virtual IHostAwareViewModel GetDefaultAlertViewModel(string alertMessage)
        {
            return GetDefaultAlertViewModel(alertMessage, string.Empty);
        }

        /// <summary>
        /// Gets the default view model for Alerts.
        /// Caller can replace the default View with a custom view.
        /// </summary>
        /// <returns></returns>
        public abstract IHostAwareViewModel GetDefaultAlertViewModel(string alertMessage, string alertDialogTitle);

        public abstract IHostAwareViewModel GetDefaultConfirmationDialogViewModel(string message, string dialogTitle);

        public bool IsDesignTime
        {
            get { return (bool)DesignerProperties.IsInDesignModeProperty.GetMetadata(typeof(DependencyObject)).DefaultValue; }
        }

        public bool IsUIThread
        {
            get { return CheckIsUIThread(); }
        }

        protected virtual bool CheckIsUIThread()
        {
            return UIDispatcher.CheckAccess();
        }

        public void Run(Action action)
        {
            if(IsUIThread)
            {
                action();
                return;
            }

            UIDispatcher.Invoke(action);
        }

        public void RunAsync(Action action)
        {
            UIDispatcher.BeginInvoke(action);
        }
    }
}
