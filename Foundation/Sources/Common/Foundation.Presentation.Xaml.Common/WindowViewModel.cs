using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;
using SquaredInfinity.Foundation.Presentation.Commands;

namespace SquaredInfinity.Foundation.Presentation
{
    public class WindowViewModel : ViewModel, IViewModelHost
    {
        IHostAwareViewModel hostedViewModel;
        public IHostAwareViewModel HostedViewModel
        {
            get
            {
                return hostedViewModel;
            }
            set
            {
                hostedViewModel = value;
                hostedViewModel.ViewModelHost = this;
                RaisePropertyChanged(() => HostedViewModel);
            }
        }

        ICommand cancelCommand;
        public ICommand CancelCommand
        {
            get { return cancelCommand; }
            set
            {
                cancelCommand = value;
                RaisePropertyChanged(() => CancelCommand);
            }
        }

        public WindowViewModel()
        {
            this.CancelCommand = new DelegateCommand(() =>
            {
                CompleteInteraction(UserInteractionOutcome.Cancelled);
            });
        }
    }
}
