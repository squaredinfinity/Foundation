using SquaredInfinity.Foundation.Types.Description;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SquaredInfinity.Foundation.Types.Mapping.MemberMatching
{
    public interface IMemberMatch
    {
        ITypeMemberDescription From { get; }
        ITypeMemberDescription To { get; }
    }
}
