using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SquaredInfinity.Foundation.Presentation
{
    public interface IViewModelHost : IViewModel
    {
        IHostAwareViewModel HostedViewModel { get; set; }
    }
}
