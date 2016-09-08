using System;
using System.Collections.Generic;
using System.Text;

namespace SquaredInfinity.Foundation.IntraMessaging
{
    public class IntraMessage : IIntraMessage
    {
        public string UniqueName { get; set; }
        public object Payload { get; set; }
        public IntraMessagePropertyCollection Properties { get; private set; } = new IntraMessagePropertyCollection();
    }
}
