using SquaredInfinity.Types.Description;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SquaredInfinity.Types.Mapping.MemberMatching
{
    public class MemberMatch : IMemberMatch
    {
        public ITypeMemberDescription From { get; set; }
        public ITypeMemberDescription To { get; set; }
    }
}
