using SquaredInfinity.Foundation.Presentation.ViewModels;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;

namespace SquaredInfinity.Foundation.Presentation.Xaml.UITests.Behaviors
{
    public class ClickViewModel : ViewModel
    {
        public void ShowSingleClickMessage()
        {
            MessageBox.Show("Single Click handled.");
        }

        public void ShowDoubleClickMessage()
        {
            MessageBox.Show("Double Click handled.");
        }
    }
}
