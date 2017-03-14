using System;
using System.Collections.Generic;
using System.Text;

namespace SquaredInfinity.IntraMessaging
{
    public interface IIntraMessageNode
    {
        void Receive(IIntraMessage msg);
    }
}
