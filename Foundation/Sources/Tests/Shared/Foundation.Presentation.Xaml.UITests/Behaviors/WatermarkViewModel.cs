using SquaredInfinity.Foundation.Presentation.ViewModels;
using System;
using System.Collections.Generic;
using System.Text;

namespace SquaredInfinity.Foundation.Presentation.Xaml.UITests.Behaviors
{
    public class WatermarkViewModel : ViewModel
    {
        string _text;
        public string Text
        {
            get { return _text; }
            set { TrySetThisPropertyValue(ref _text, value); }
        }
    }
}
