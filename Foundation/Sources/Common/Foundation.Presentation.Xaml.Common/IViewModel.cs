using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace SquaredInfinity.Foundation.Presentation
{
    public interface IViewModel : INotifyPropertyChanged
    {
        object View { get; set; }
    }
}
