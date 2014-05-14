using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SquaredInfinity.Foundation.Presentation.Windows
{
    public partial class ViewHostWindow : IViewModelHost
    {
        public IHostAwareViewModel HostedViewModel { get; set; }
    }
}
