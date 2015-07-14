using SquaredInfinity.Foundation.Presentation.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Foundation.Presentation.Xaml.UITests.Behaviors
{
    public class ClickViewModel : ViewModel
    {
        int _clickCount = 0;
        public int ClickCount
        {
            get { return _clickCount; }
            set { TrySetThisPropertyValue(ref _clickCount, value); }
        }

        public void DoClick()
        {
            ClickCount++;
        }
    }
}
