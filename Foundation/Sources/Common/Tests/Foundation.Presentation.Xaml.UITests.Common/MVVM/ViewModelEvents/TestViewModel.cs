using SquaredInfinity.Foundation.Presentation.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SquaredInfinity.Foundation.Extensions;
using System.Threading;

namespace Foundation.Presentation.Xaml.UITests.MVVM.ViewModelEvents
{
    public class TestViewModel : ViewModel
    {
        static int _lastMessageNumber = 0;
        static int GetNextMessageNumber()
        {
            return Interlocked.Increment(ref _lastMessageNumber);
        }

        string _eventToHandle = "MVVM.*";
        public string EventToHandle
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

        string _messages = string.Empty;
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

        ViewModelEventRoutingStrategy _eventRoutingStrategyToRaise = ViewModelEventRoutingStrategy.BubbleTunnelBroadcastToChildren;
        public ViewModelEventRoutingStrategy EventRoutingStrategyToRaise
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

        protected override void OnPreviewViewModelEvent(ViewModelEventArgs args)
        {
            base.OnPreviewViewModelEvent(args);

            if (!HandleBubbleEvents)    
                return;

            Messages = 
                "{0}: RECEIVED Bubble Event {1}"
                .FormatWith(GetNextMessageNumber(), args.Event.Name)
                + Environment.NewLine
                + Messages;
        }

        protected override void OnViewModelEvent(ViewModelEventArgs args)
        {
            base.OnViewModelEvent(args);

            if(args.RoutingStrategy == ViewModelEventRoutingStrategy.Tunnel)
            {
                if (!HandleTunnelEvents)
                    return;

                Messages = 
                    "{0}: RECEIVED Tunnel Event {1}"
                    .FormatWith(GetNextMessageNumber(), args.Event.Name)
                    + Environment.NewLine
                    + Messages;
            }

            if (args.RoutingStrategy == ViewModelEventRoutingStrategy.BroadcastToChildren)
            {
                if (!HandleTunnelEvents)
                    return;

                Messages = 
                    "{0}: RECEIVED Broadcast To Children Event {1}"
                    .FormatWith(GetNextMessageNumber(), args.Event.Name)
                    + Environment.NewLine
                    + Messages;
            }
        }
    }
}
