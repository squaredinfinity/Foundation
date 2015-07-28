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

        bool IsHostedInDialogWindow { get; }

        void CompleteInteraction(UserInteractionOutcome interactionOutcome);

        void CompleteInteraction();

        void CancelInteraction();

        bool CanCompleteInteraction();
    }
}
