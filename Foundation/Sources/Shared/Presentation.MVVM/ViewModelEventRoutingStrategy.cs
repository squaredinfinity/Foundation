using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SquaredInfinity.Presentation
{
    public enum ViewModelEventRoutingStrategy
    {
        Tunnel = 0,
        Bubble = 2,
        BroadcastToChildren = 4,

        BubbleTunnelBroadcastToChildren = Tunnel | Bubble | BroadcastToChildren,
        Default = BubbleTunnelBroadcastToChildren
    }
}
