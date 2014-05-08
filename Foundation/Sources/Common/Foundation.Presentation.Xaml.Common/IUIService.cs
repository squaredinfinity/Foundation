using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace SquaredInfinity.Foundation.Presentation
{
    public interface IUIService
    {
        void ShowToolWindow(IHostAwareViewModel viewModel, Func<Window> getWindow = null);

        void ShowDialog(IHostAwareViewModel viewModel);

        void ShowDialog(
            IHostAwareViewModel viewModel, 
            DialogScope dialogScope, 
            DialogMode dialogMode, 
            bool showActivated = true,
            Func<Window> getViewModelHostView = null);

        IHostAwareViewModel ShowConfirmationDialog(string message, string dialogTitle);

        void ShowAlert(IHostAwareViewModel viewModel);
        void ShowAlert(IHostAwareViewModel viewModel, DialogScope dialogScope, DialogMode dialogMode);
        IHostAwareViewModel ShowAlert(string message);
        IHostAwareViewModel ShowAlert(string message, string dialogTitle);

        IHostAwareViewModel GetDefaultConfirmationDialogViewModel(string message, string dialogTitle);

        IHostAwareViewModel GetDefaultAlertViewModel();
        IHostAwareViewModel GetDefaultAlertViewModel(string alertMessage);
        IHostAwareViewModel GetDefaultAlertViewModel(string alertMessage, string alertDialogTitle);
        
        /// <summary>
        /// Gets a value indicating whether this application is in design time.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this lication is in design mode, <c>false</c> otherwise, <c>false</c>.
        /// </value>
        bool IsDesignTime { get; }

        /// <summary>
        /// Runs true if current thread is a UI thread.
        /// </summary>
        /// <param name="action">The action.</param>
        bool IsUIThread { get; }

        SynchronizationContext SynchronizationContext { get; }
    }
}
