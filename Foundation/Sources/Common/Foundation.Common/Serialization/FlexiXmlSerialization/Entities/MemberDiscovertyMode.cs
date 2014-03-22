using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SquaredInfinity.Foundation.Serialization.FlexiXmlSerialization.Entities
{
    [Flags]
    public enum MemberDiscoveryMode
    {
        Public = 0,
        NonPublic = 2,

        Default = Public
    }
}
