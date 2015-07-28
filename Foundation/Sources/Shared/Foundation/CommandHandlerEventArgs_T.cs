using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SquaredInfinity.Foundation
{
    public class CommandHandlerEventArgs<TResult> : CommandHandlerEventArgs
    {
        public TResult Result { get; private set; }

        public void Handle(TResult result)
        {
            base.Handle();
            Result = result;
        }
    }
}
