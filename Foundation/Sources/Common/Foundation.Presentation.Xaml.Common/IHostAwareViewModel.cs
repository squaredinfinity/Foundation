using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;

namespace SquaredInfinity.Foundation.Presentation
{
    public interface IHostAwareViewModel : IViewModel
    {
        event Action<IHostAwareViewModel> InteractionCompleting;
        event Action<IHostAwareViewModel> InteractionCompleted;

        string Title { get; set; }

        UserInteractionOutcome InteractionOutcome { get; }
        IViewModelHost ViewModelHost { get; set; }

        ICommand CancelInteractionCommand { get; }
        ICommand CompleteInteractionCommand { get; }

        void CompleteInteraction(UserInteractionOutcome interactionOutcome);
    }
}
