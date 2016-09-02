using SquaredInfinity.Foundation.Presentation.ViewModels;
using System;
using System.Collections.Generic;
using System.Text;

namespace SquaredInfinity.Foundation.Presentation.Xaml.UITests.MVVM
{
    public class VMEvents_AnotherViewModel : ViewModel
    {
        string _message = "";
        public string Message
        {
            get { return _message; }
            set { TrySetThisPropertyValue(ref _message, value); }
        }

        public VMEvents_AnotherViewModel()
        {
            SubscribeToEvent("VMEvents.TestEvent", (x) =>
            {
                Message =
                $"received {x.Event.Payload.ToString()} at {DateTime.Now.ToShortTimeString()}"
                + Environment.NewLine
                + Message;
            });
        }
    }
}
