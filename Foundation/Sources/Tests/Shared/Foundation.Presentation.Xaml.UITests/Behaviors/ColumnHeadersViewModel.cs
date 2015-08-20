using SquaredInfinity.Foundation.Presentation.ViewModels;
using System;
using System.Collections.Generic;
using System.Text;

namespace SquaredInfinity.Foundation.Presentation.Xaml.UITests.Behaviors
{
    public class ColumnHeadersViewModel : ViewModel
    {
        public List<string> TheList { get; set; }

        public ColumnHeadersViewModel()
        {
            TheList = new List<string>
            {
                "one",
                "two",
                "three"
            };
        }
    }
}
