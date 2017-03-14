using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SquaredInfinity.Presentation
{
    public interface IViewModelHost
    {
        IHostAwareViewModel HostedViewModel { get; set; }
    }
}
