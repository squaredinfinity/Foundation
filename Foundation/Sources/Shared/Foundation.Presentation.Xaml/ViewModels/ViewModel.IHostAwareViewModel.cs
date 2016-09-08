using SquaredInfinity.Foundation.IntraMessaging;
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

        IViewModelHost _viewModelHost;
        public IViewModelHost ViewModelHost
        {
            get { return _viewModelHost; }
            set { TrySetThisPropertyValue(ref _viewModelHost, value); }
        }

        UserInteractionOutcome _interactionOutcome = UserInteractionOutcome.Unset;
        public UserInteractionOutcome InteractionOutcome
        {
            get { return _interactionOutcome; }
            set { TrySetThisPropertyValue(ref _interactionOutcome, value); }
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

        string _title;
        public string Title
        {
            get { return _title; }
            set { TrySetThisPropertyValue(ref _title, value); }
        }

        #region IntraMessage

        public void Receive(IIntraMessage msg)
        {
            OnIntraMessageReceived(msg);
        }

        protected virtual void OnIntraMessageReceived(IIntraMessage msg)
        {
            var routing_strategy = ViewModelEventRoutingStrategy.Bubble;

            var event_name = msg.UniqueName;

            if(msg.Properties.ContainsKey("event.name"))
            {
                event_name = (string) msg.Properties["event.name"];
            }

            if(msg.Properties.ContainsKey(typeof(ViewModelEventRoutingStrategy)))
            {
                routing_strategy = (ViewModelEventRoutingStrategy)msg.Properties[typeof(ViewModelEventRoutingStrategy)];
            }

            RaiseEvent(msg.UniqueName, msg.Payload, routing_strategy);
        }

        #endregion
    }
}
