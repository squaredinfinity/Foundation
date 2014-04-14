﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace SquaredInfinity.Foundation.Presentation
{
    public abstract class UIService : IUIService
    {
        public UIService()
        {
        }

        /// <summary>
        /// Displays a dialog window to the user using default DialogScope and DialogMode.
        /// </summary>
        /// <param name="viewModel"></param>
        public virtual void ShowDialog(IHostAwareViewModel viewModel)
        {
            ShowDialog(viewModel, DialogScope.Default, DialogMode.Default);
        }
        
        /// <summary>
        /// Displays a dialog window to the user.
        /// </summary>
        /// <param name="viewModel"></param>
        /// <param name="dialogScope"></param>
        public abstract void ShowDialog(
            IHostAwareViewModel viewModel, 
            DialogScope dialogScope, 
            DialogMode dialogMode, 
            bool showActivated = true,
            Func<Window> getViewModelHostView = null);

        public abstract IHostAwareViewModel ShowConfirmationDialog(string message, string dialogTitle);

        /// <summary>
        /// Shows an alert window using default DialogScope and DialogMode.
        /// </summary>
        /// <param name="viewModel"></param>
        public virtual void ShowAlert(IHostAwareViewModel viewModel)
        {
            ShowAlert(viewModel, DialogScope.Default, DialogMode.Default);
        }

        public abstract void ShowAlert(IHostAwareViewModel viewModel, DialogScope dialogScope, DialogMode dialogMode);

        public IHostAwareViewModel ShowAlert(string message)
        {
            var hostAwareViewModel = GetDefaultAlertViewModel(message);

            ShowAlert(hostAwareViewModel);

            return hostAwareViewModel;
        }

        public IHostAwareViewModel ShowAlert(string message, string title)
        {
            var hostAwareViewModel = GetDefaultAlertViewModel(message);

            hostAwareViewModel.Title = title;

            ShowAlert(hostAwareViewModel);

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

        public abstract bool IsDesignTime
        {
            get;
        }

        public abstract bool IsUIThread
        {
            get;
        }

        public abstract void RunAsync(Action action);

        public abstract void Run(Action action);
    }
}