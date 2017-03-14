using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SquaredInfinity.IntraMessaging
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
