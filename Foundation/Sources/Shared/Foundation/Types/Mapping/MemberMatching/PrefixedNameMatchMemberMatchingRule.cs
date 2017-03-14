using SquaredInfinity.Types.Description;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SquaredInfinity.Types.Mapping.MemberMatching
{
    public class PrefixedNameMatchMemberMatchingRule : IMemberMatchingRule
    {
        public bool IsCaseSensitive { get; set; }
        public string SourcePrefix { get; set; }
        public string TargetPrefix { get; set; }

        public bool TryMapMembers(ITypeMemberDescription source, IList<ITypeMemberDescription> targetCandidates, out ITypeMemberDescription target)
        {
            target = null;
            return false;
        }
    }
}
