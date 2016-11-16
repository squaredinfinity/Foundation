using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SquaredInfinity.Foundation.Presentation
{
    public interface IViewModelEvent
    {
        string Name { get; }

        object Payload { get; }
    }
}
