using SquaredInfinity.Foundation.Presentation.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Foundation.Presentation.Xaml.UITests.Common.Controls
{
    public class ZommableListViewTestsView : View
    {
        protected override SquaredInfinity.Foundation.Presentation.IHostAwareViewModel ResolveViewModel(Type viewType, object newDatacontext)
        {
            return new ZoomableListViewTestsViewModel();
        }
    }
}
