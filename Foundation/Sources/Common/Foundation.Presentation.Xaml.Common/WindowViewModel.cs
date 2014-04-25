using SquaredInfinity.Foundation.Presentation.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace SquaredInfinity.Foundation.Presentation
{
    public class WindowViewModel : ViewModel, IViewModelHost, IViewModel
    {
        private IHostAwareViewModel _hostedViewModel;
        private ICommand _cancelCommand;

        public IHostAwareViewModel HostedViewModel
        {
            get
            {
                return this._hostedViewModel;
            }
            set
            {
                _hostedViewModel = value;
                _hostedViewModel.ViewModelHost = (IViewModelHost) this;
                RaiseThisPropertyChanged();
            }
        }

        public ICommand CancelCommand
        {
            get
            {
                return _cancelCommand;
            }
            set
            {
                _cancelCommand = value;
                RaiseThisPropertyChanged();
            }
        }

        public WindowViewModel()
        {
            this.CancelCommand = (ICommand)new DelegateCommand((Action)(() => this.CompleteInteraction(UserInteractionOutcome.Cancelled)));
        }
    }
}
