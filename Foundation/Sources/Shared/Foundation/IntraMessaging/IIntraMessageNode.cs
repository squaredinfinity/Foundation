using System;
using System.Collections.Generic;
using System.Text;

namespace SquaredInfinity.Foundation.IntraMessaging
{
    public interface IIntraMessageNode
    {
        void Receive(IIntraMessage msg);
    }
}
