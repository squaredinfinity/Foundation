using SquaredInfinity.Foundation.Presentation.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SquaredInfinity.Foundation.Extensions;

namespace Foundation.Presentation.Xaml.UITests.MVVM.ViewModelEvents
{
    public class TestViewModel : ViewModel
    {
        string _eventToHandle = "MVVM.*";
        public string EventNameToHandle
        {
            get { return _eventToHandle; }
            set
            {
                _eventToHandle = value;
                RaiseThisPropertyChanged();
            }
        }

        bool _handleBubbleEvents = true;
        public bool HandleBubbleEvents
        {
            get { return _handleBubbleEvents; }
            set
            {
                _handleBubbleEvents = value;
                RaiseThisPropertyChanged();
            }
        }

        bool _handleTunnelEvents = true;
        public bool HandleTunnelEvents
        {
            get { return _handleTunnelEvents; }
            set
            {
                _handleTunnelEvents = value;
                RaiseThisPropertyChanged();
            }
        }

        bool _handleBroadcastEvents = true;
        public bool HandleBroadcastEvents
        {
            get { return _handleBroadcastEvents; }
            set
            {
                _handleBroadcastEvents = value;
                RaiseThisPropertyChanged();
            }
        }

        string _messages;
        public string Messages
        {
            get { return _messages; }
            set
            {
                _messages = value;
                RaiseThisPropertyChanged();
            }
        }

        string _eventToRaise = "MVVM.*";
        public string EventToRaise
        {
            get { return _eventToRaise; }
            set
            {
                _eventToRaise = value;
                RaiseThisPropertyChanged();
            }
        }

        SquaredInfinity.Foundation.Presentation.Views.View.EventRoutingStrategy _eventRoutingStrategyToRaise = SquaredInfinity.Foundation.Presentation.Views.View.EventRoutingStrategy.BubbleTunnelBroadcastToChildren;
        public SquaredInfinity.Foundation.Presentation.Views.View.EventRoutingStrategy EventRoutingStrategyToRaise
        {
            get { return _eventRoutingStrategyToRaise; }
            set
            {
                _eventRoutingStrategyToRaise = value;
                RaiseThisPropertyChanged();
            }
        }

        public void RaiseEvent()
        {
            base.RaiseEvent(EventToRaise, EventRoutingStrategyToRaise);
        }

        protected override void OnPreviewViewModelEvent(SquaredInfinity.Foundation.Presentation.Views.View.ViewModelEvent ev)
        {
            base.OnPreviewViewModelEvent(ev);

            if (!HandleBubbleEvents)
                return;

            Messages = "{0}: Bubble Event {1} Handled".FormatWith(Messages.GetLines().Length, ev.Name);
        }

        protected override void OnViewModelEvent(SquaredInfinity.Foundation.Presentation.Views.View.ViewModelEvent ev)
        {
            base.OnViewModelEvent(ev);

            if(ev.RoutingStrategy == SquaredInfinity.Foundation.Presentation.Views.View.EventRoutingStrategy.Tunnel)
            {
                if (!HandleTunnelEvents)
                    return;

                Messages = "{0}: Tunnel Event {1} Handled".FormatWith(Messages.GetLines().Length, ev.Name);
            }

            if (ev.RoutingStrategy == SquaredInfinity.Foundation.Presentation.Views.View.EventRoutingStrategy.BroadcastToChildren)
            {
                if (!HandleTunnelEvents)
                    return;

                Messages = "{0}: Broadcast To Children Event {1} Handled".FormatWith(Messages.GetLines().Length, ev.Name);
            }
        }
    }
}
