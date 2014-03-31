using SquaredInfinity.Foundation.Types.Description;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SquaredInfinity.Foundation.Types.Mapping
{
    public class MemberMapping : IMemberMapping
    {
        public ITypeMemberDescription From { get; set; }
        public ITypeMemberDescription To { get; set; }
    }
}
