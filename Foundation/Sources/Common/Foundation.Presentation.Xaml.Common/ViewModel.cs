using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using SquaredInfinity.Foundation.Presentation.Commands;

namespace SquaredInfinity.Foundation.Presentation
{
    /// <summary>
    /// Base class for all viewmodels
    /// </summary>
    public abstract partial class ViewModel : NotifyPropertyChangedObject, IViewModel, IHostAwareViewModel
    {
        object view;
        public object View
        {
            get { return view; }
            set
            {
                view = value;

                if (view is FrameworkElement)
                    (view as FrameworkElement).DataContext = this;
            }
        }

        ViewModelState state = ViewModelStates.Initialising;
        /// <summary>
        /// Gets or sets the state of a ViewModel
        /// </summary>
        /// <value>The state.</value>
        public ViewModelState State
        {
            get { return state; }
            set
            {
                state = value;
                RaisePropertyChanged(() => State);

            }
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

        public ViewModel()
        {
            CancelInteractionCommand = new DelegateCommand(() =>
            {
                CompleteInteraction(UserInteractionOutcome.Cancelled);
            });

            CompleteInteractionCommand = new DelegateCommand(() =>
            {
                CompleteInteraction(UserInteractionOutcome.OtherSuccess);
            });
        }
    }
}
