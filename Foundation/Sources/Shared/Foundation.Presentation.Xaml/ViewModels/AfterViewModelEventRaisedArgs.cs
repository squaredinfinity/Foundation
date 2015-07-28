using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SquaredInfinity.Foundation.Presentation.ViewModels
{
    /// <summary>
    /// Event Args for AfterViewModelEventRaised event
    /// </summary>
    internal class AfterViewModelEventRaisedArgs : EventArgs
    {
        public ViewModelEventRoutingStrategy RoutingStrategy { get; private set; }

        public IViewModelEvent Event { get; private set; }

        public AfterViewModelEventRaisedArgs(
            IViewModelEvent @event, 
            ViewModelEventRoutingStrategy routingStrategy)
        {
            this.Event = @event;
            this.RoutingStrategy = routingStrategy;
        }
    }
}
