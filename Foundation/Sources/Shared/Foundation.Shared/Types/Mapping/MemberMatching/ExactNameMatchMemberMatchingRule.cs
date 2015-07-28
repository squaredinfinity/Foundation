using SquaredInfinity.Foundation.Types.Description;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SquaredInfinity.Foundation.Types.Mapping.MemberMatching
{
    public class ExactNameMatchMemberMatchingRule : IMemberMatchingRule
    {
        public bool IsCaseSensitive { get; set; }

        public bool TryMapMembers(ITypeMemberDescription source, IList<ITypeMemberDescription> targetCandidates, out ITypeMemberDescription target)
        {
            target = null;

            var strComp = StringComparison.InvariantCultureIgnoreCase;

            if (IsCaseSensitive)
                strComp = StringComparison.InvariantCulture;

            for (int i = 0; i < targetCandidates.Count; i++)
            {
                var t = targetCandidates[i];

                if(string.Equals(source.Name, t.Name, strComp))
                {
                    target = t;
                    return true;
                }
            }

            return false;
        }
    }
}
