using SquaredInfinity.Foundation.Presentation.ViewModels;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Foundation.Presentation.Xaml.UITests.UIExamples.ToolTip
{
    public class ExampleTooltipViewModel : ViewModel
    {
        int _id;
        public int Id
        {
            get { return _id; }
            set { TrySetThisPropertyValue(ref _id, value); }
        }

        public ExampleTooltipViewModel()
        {
            
        }

        protected override void OnAfterDataContextChanged(object newDataContext)
        {
            Id = new Random().Next();
        }
    }
}
