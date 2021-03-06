﻿using SquaredInfinity.Foundation.IntraMessaging;
using SquaredInfinity.Foundation.Presentation.ViewModels;
using System;
using System.Collections.Generic;
using System.Text;

namespace SquaredInfinity.Foundation.Presentation.Xaml.UITests.MVVM
{
    public class VMEvents_ProducerViewModel : ViewModel
    {
        string _message = ":)";
        public string Message
        {
            get { return _message; }
            set { TrySetThisPropertyValue(ref _message, value); }
        }

        public void SendMessage()
        {
            RaiseEvent("VMEvents.TestEvent", Message);

            // or alternatively using INtraMessage api:
            //(this as IIntraMessageNode).Receive(new IntraMessage { UniqueName = "VMEvents.TestEvent", Payload = Message });
        }
    }
}
