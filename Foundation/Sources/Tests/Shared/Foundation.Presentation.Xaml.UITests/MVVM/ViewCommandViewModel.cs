using SquaredInfinity.Foundation.Presentation.ViewModels;
using System;
using System.Collections.Generic;
using System.Text;

namespace SquaredInfinity.Foundation.Presentation.Xaml.UITests.MVVM
{
    public class ViewCommandViewModel : ViewModel
    {
        string _myText = "some text";
        public string MyText
        {
            get { return _myText; }
            set { TrySetThisPropertyValue(ref _myText, value); }
        }
    }
}
