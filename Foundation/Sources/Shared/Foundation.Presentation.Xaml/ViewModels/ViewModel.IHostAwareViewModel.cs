using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Input;

namespace SquaredInfinity.Foundation.Presentation.ViewModels
{
    public abstract partial class ViewModel : IHostAwareViewModel
    {
        public event Action<IHostAwareViewModel> InteractionCompleting;
        public event Action<IHostAwareViewModel> InteractionCompleted;

        IViewModelHost viewModelHost;
        public IViewModelHost ViewModelHost
        {
            get { return viewModelHost; }
            set
            {
                viewModelHost = value;
                RaisePropertyChanged(() => ViewModelHost);
            }
        }

        UserInteractionOutcome interactionOutcome = UserInteractionOutcome.Unset;
        public UserInteractionOutcome InteractionOutcome
        {
            get { return interactionOutcome; }
            set
            {
                interactionOutcome = value;
                RaisePropertyChanged(() => InteractionOutcome);
            }
        }

        public virtual void CompleteInteraction()
        {
            CompleteInteraction(UserInteractionOutcome.OtherSuccess);
        }

        public virtual void CancelInteraction()
        {
            CompleteInteraction(UserInteractionOutcome.Cancelled);
        }

        public virtual void CompleteInteraction(UserInteractionOutcome interactionOutcome)
        {
            // do nothing if interaction outcome has already been set
            if (this.InteractionOutcome != UserInteractionOutcome.Unset)
                return;

            this.InteractionOutcome = interactionOutcome;

            if (InteractionCompleting != null)
                InteractionCompleting(this);

            if (InteractionCompleted != null)
                InteractionCompleted(this);
        }

        public virtual bool CanCompleteInteraction()
        {
            return true;
        }

        string title;
        public string Title
        {
            get { return title; }
            set
            {
                title = value;
                RaisePropertyChanged(() => Title);
            }
        }
    }
}
