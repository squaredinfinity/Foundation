using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SquaredInfinity
{
    public class CommandHandlerEventArgs : EventArgs
    {
        public bool IsHandled { get; private set; }

        public void Handle()
        {
            IsHandled = true;
        }
    }
}
