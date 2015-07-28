using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SquaredInfinity.Foundation.Presentation.ViewModels
{
    public class ViewModelEvent : IViewModelEvent
    {
        public string Name { get; private set; }

        public object Payload { get; set; }

        public ViewModelEvent()
        {
            this.Name = GetType().FullName;
        }

        public ViewModelEvent(string eventName)
        {
            this.Name = eventName;
        }
    }
}
