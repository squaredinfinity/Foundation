using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace SquaredInfinity.Foundation.Presentation
{
    public interface IViewModel
    {
        object DataContext { get; set; }

        ViewModelState State { get; set; }
    }
}
