using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SquaredInfinity.Foundation.Presentation
{
    public class ViewModelEventArgs
    {
        public IViewModel Sender { get; private set; }

        public IViewModelEvent Event { get; private set; }

        public ViewModelEventRoutingStrategy RoutingStrategy { get; private set; }
        
        public bool IsHandled { get; set; }

        public ViewModelEventArgs(
            IViewModel sender, 
            IViewModelEvent @event, 
            ViewModelEventRoutingStrategy routingStrategy)
        {
            this.Sender = sender;
            this.Event = @event;
            this.RoutingStrategy = routingStrategy;
        }
    }
}
