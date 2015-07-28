using SquaredInfinity.Foundation.Presentation.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace SquaredInfinity.Foundation.Presentation.Views
{
    public class ViewModelEventRoutedEventArgs : RoutedEventArgs
    {
        public ViewModelEventArgs ViewModelEventArgs { get; private set; }

        public ViewModelEventRoutedEventArgs(ViewModelEventArgs args, RoutedEvent routedEvent, object source)
            : base(routedEvent, source)
        {
            this.ViewModelEventArgs = args;
        }
    }
}
