using SquaredInfinity.Foundation.Presentation.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Foundation.Presentation.Xaml.UITests.MVVM.ReferenceImplementation
{
    public class DetailsView : View<DetailsViewModel>
    {
        public DetailsView()
        {

        }

        protected override void OnAfterDataContextChanged()
        {
            if (object.Equals(DataContext, "Odd"))
            {
                ViewModel.DataContext = new List<int> { 1, 3, 5, 7, 9 };
            }
            else
            {
                ViewModel.DataContext = new List<int> { 2, 4, 6 , 8, 10 };
            }
        }
    }
}
