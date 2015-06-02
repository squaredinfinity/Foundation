using SquaredInfinity.Foundation.Presentation.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Foundation.Presentation.Xaml.UITests.Fonts
{
    public class FontsiView : View
    {
        public void Lol()
        {
            Window w = new Window();
            w.Content = new FontsiView();
            w.Show();
        }
    }
}
