using SquaredInfinity.Foundation.Presentation.ViewModels;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;

namespace SquaredInfinity.Foundation.Presentation.Xaml.UITests.MVVM
{
    public class UsingViewCommandViewModel : ViewModel
    {

        public void HandleDefaultActionHappening(object parameter)
        {
            MessageBox.Show($"Done: {parameter ?? "null"}");
        }
    }
}
