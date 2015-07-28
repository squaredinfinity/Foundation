using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SquaredInfinity.Foundation.Presentation.ViewModels
{
    public class AlertViewModel : ViewModel
    {
        string alertMessage = string.Empty;
        public string AlertMessage
        {
            get { return alertMessage; }
            set
            {
                alertMessage = value;
                RaiseThisPropertyChanged();
            }
        }
    }
}
