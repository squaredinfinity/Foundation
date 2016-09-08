using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text;

namespace SquaredInfinity.Foundation.IntraMessaging
{
    public interface IIntraMessage
    {
        string UniqueName { get; }
        object Payload { get; }

        IntraMessagePropertyCollection Properties { get; }
    }
}
