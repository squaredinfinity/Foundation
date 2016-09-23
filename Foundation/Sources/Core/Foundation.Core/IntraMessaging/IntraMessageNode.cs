using System;
using System.Collections.Generic;
using System.Text;

namespace SquaredInfinity.Foundation.IntraMessaging
{
    public abstract class IntraMessageNode : IIntraMessageNode
    {
        public void Receive(IIntraMessage msg)
        {
            OnMessageReceived(msg);
        }

        protected abstract void OnMessageReceived(IIntraMessage msg);
    }
}
