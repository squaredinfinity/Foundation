using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text;

namespace SquaredInfinity.IntraMessaging
{
    public interface IIntraMessage
    {
        string UniqueName { get; }
        object Payload { get; }

        DateTime TimeStamp { get; }
        bool IsSynchronous { get; }

        IntraMessagePropertyCollection Properties { get; }
    }
}
