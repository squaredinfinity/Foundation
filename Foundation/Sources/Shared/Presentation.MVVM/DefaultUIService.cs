using System;
using System.Collections.Generic;
using System.Text;
using SquaredInfinity.Presentation.Views;
using SquaredInfinity.Presentation.Windows;
using System.Windows.Threading;
using SquaredInfinity.Presentation.ViewModels;
using System.Windows;

namespace SquaredInfinity.Presentation
{
    public class DefaultUIService : UIService
    {
        readonly Func<ViewHostWindow> CreateNewDialogWindow;
        readonly Func<ViewHostWindow> CreateNewToolWindow;

        public DefaultUIService(Dispatcher uiDispatcher, Func<ViewHostWindow> createNewDialogWindow, Func<ViewHostWindow> createNewToolWindow)
            : base(uiDispatcher)
        {
            this.CreateNewDialogWindow = createNewDialogWindow;
            this.CreateNewToolWindow = createNewToolWindow;
        }

        protected ViewHostWindow GetNewDialogWindow()
        {
            var window = CreateNewDialogWindow();

            if (window is Window)
                OnAfterWindowCreated(window as Window);

            return window;
        }

        protected ViewHostWindow GetNewToolWindow()
        {
            var window = CreateNewToolWindow();

            if (window is Window)
                OnAfterWindowCreated(window as Window);

            return window;
        }

        protected virtual void OnAfterWindowCreated(Window window)
        { }

        public override IHostAwareViewModel GetDefaultAlertViewModel(string alertMessage, string alertDialogTitle)
        {
            var viewModel = new AlertViewModel();
            viewModel.AlertMessage = alertMessage;
            viewModel.Title = alertDialogTitle;

            return viewModel;
        }

        public override IHostAwareViewModel GetDefaultConfirmationDialogViewModel(string message, string dialogTitle)
        {
            var viewModel = new AlertViewModel();
            viewModel.AlertMessage = message;
            viewModel.Title = dialogTitle;

            return viewModel;
        }

        public override void ShowAlert(View view, DialogScope dialogScope, DialogMode dialogMode)
        {
            var viewHost = GetNewDialogWindow();

            // prepare host view and viewmodel

            viewHost.HostedView = view;

            viewHost.HostedViewModel = view.ViewModel;

            view.ViewModel.InteractionCompleting += viewModel_modalInteractionCompleting;

            PrepareViewForDisplay(viewHost, view);

            viewHost.ShowDialog();
        }

        public override IHostAwareViewModel ShowConfirmationDialog(string message, string dialogTitle)
        {
            var viewModel = GetDefaultConfirmationDialogViewModel(message, dialogTitle);

            var viewHost = GetNewDialogWindow();

            // prepare host view and viewmodel

            viewHost.HostedViewModel = viewModel;

            viewHost.Closed += (sender, e) =>
            {
                if (viewModel.InteractionOutcome == UserInteractionOutcome.Unset)
                    viewModel.CompleteInteraction(UserInteractionOutcome.Cancelled);
            };

            viewModel.InteractionCompleting += viewModel_modalInteractionCompleting;

            viewHost.Topmost = true;

            if (viewHost != Application.Current.MainWindow) // this may happen when application is closing down
                viewHost.Owner = Application.Current.MainWindow;

            PrepareViewForDisplay(viewHost, null);

            if (!Application.Current.Dispatcher.HasShutdownStarted
                && !Application.Current.Dispatcher.HasShutdownFinished)
            {
                viewHost.ShowDialog();
            }

            return viewModel;
        }

        public override void ShowDialog(View view, DialogScope dialogScope, DialogMode dialogMode, bool showActivated = true, Func<ViewHostWindow> getWindow = null)
        {
            var viewModel = view.ViewModel;

            ViewHostWindow viewHost = null;

            // prepare host view and viewmodel
            if (getWindow == null)
            {
                viewHost = GetNewDialogWindow();
            }
            else
            {
                viewHost = getWindow();

                if (viewHost is Window)
                    OnAfterWindowCreated(viewHost as Window);
            }

            viewHost.Owner = Application.Current.MainWindow;

            viewHost.HostedViewModel = viewModel;

            viewHost.HostedView = view;

            viewHost.Closed += (sender, e) =>
            {
                if (viewModel.InteractionOutcome == UserInteractionOutcome.Unset)
                    viewModel.CompleteInteraction(UserInteractionOutcome.Cancelled);
            };

            if (dialogMode == DialogMode.Modal)
            {
                viewModel.InteractionCompleting += viewModel_modalInteractionCompleting;
            }
            else
            {
                viewModel.InteractionCompleting += viewModel_nonModalInteractionCompleting;
            }

            if (dialogScope == DialogScope.Global)
            {
                if (viewHost != Application.Current.MainWindow) // this may happen when Visual Studio is closing down
                    viewHost.Owner = Application.Current.MainWindow;
            }

            viewHost.ShowActivated = showActivated;

            PrepareViewForDisplay(viewHost, view);


            if (!UIDispatcher.HasShutdownStarted
                && !UIDispatcher.HasShutdownFinished)
            {
                if (dialogMode == DialogMode.Modal)
                {
                    viewHost.ShowDialog();
                }
                else
                {
                    viewHost.Show();
                }
            }
        }

        protected virtual void PrepareViewForDisplay(ViewHostWindow viewHost, View view)
        { }

        public override void ShowToolWindow(View view, Func<ViewHostWindow> getWindow = null)
        {
            ViewHostWindow viewHost = null;

            // prepare host view and viewmodel
            if (getWindow == null)
            {
                viewHost = GetNewToolWindow();
            }
            else
            {
                viewHost = getWindow();

                if (viewHost is Window)
                    OnAfterWindowCreated(viewHost as Window);
            }

            viewHost.Owner = Application.Current.MainWindow;

            viewHost.HostedView = view;

            viewHost.HostedViewModel = view.ViewModel;

            view.ViewModel.InteractionCompleting += viewModel_nonModalInteractionCompleting;

            viewHost.Closed += (sender, e) =>
            {
                if (view.ViewModel.InteractionOutcome == UserInteractionOutcome.Unset)
                    view.ViewModel.CompleteInteraction(UserInteractionOutcome.Cancelled);
            };

            PrepareViewForDisplay(viewHost, view);

            if (!UIDispatcher.HasShutdownStarted
                && !UIDispatcher.HasShutdownFinished)
            {
                viewHost.Show();
            }
        }

        void viewModel_nonModalInteractionCompleting(IHostAwareViewModel hostAwareViewModel)
        {
            // unsubscribe view model from this event (to prevent memory leak)
            hostAwareViewModel.InteractionCompleting -= viewModel_nonModalInteractionCompleting;

            // set dialog result on window
            Window window = hostAwareViewModel.ViewModelHost as Window;

            if (window != null)
            {
                window.Close();
            }
        }

        void viewModel_modalInteractionCompleting(IHostAwareViewModel hostAwareViewModel)
        {
            // unsubscribe view model from this event (to prevent memory leak)
            hostAwareViewModel.InteractionCompleting -= viewModel_modalInteractionCompleting;

            // set dialog result on window
            Window window = hostAwareViewModel.ViewModelHost as Window;

            if (window != null && window.DialogResult == null)
            {
                switch (hostAwareViewModel.InteractionOutcome)
                {
                    case UserInteractionOutcome.Cancelled:
                        window.DialogResult = false;
                        break;
                    case UserInteractionOutcome.OtherSuccess:
                        window.DialogResult = true;
                        break;
                    default:
                        window.DialogResult = false;
                        break;
                }
            }
        }
    }
}
