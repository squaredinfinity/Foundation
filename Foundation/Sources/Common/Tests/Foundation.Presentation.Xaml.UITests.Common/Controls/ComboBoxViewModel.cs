using SquaredInfinity.Foundation.Extensions;
using SquaredInfinity.Foundation.Presentation.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Foundation.Presentation.Xaml.UITests.Controls
{
    public class ComboBoxViewModel : ViewModel
    {
        List<DayOfWeek> _numbers = Enum.GetValues(typeof(DayOfWeek)).Cast <DayOfWeek>().ToList();
        public List<DayOfWeek> Numbers
        {
            get { return _numbers; }
        }

        public DayOfWeek _selectedNumber = DayOfWeek.Thursday;

        public DayOfWeek SelectedNumber
        {
            get { return _selectedNumber; }
            set { TrySetThisPropertyValue(ref _selectedNumber, value); }
        }
    }
}
