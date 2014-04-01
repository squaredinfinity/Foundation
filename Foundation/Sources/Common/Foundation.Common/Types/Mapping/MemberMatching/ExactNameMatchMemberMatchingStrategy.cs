using SquaredInfinity.Foundation.Types.Description;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SquaredInfinity.Foundation.Types.MemberMatching
{
    public class ExactNameMatchMemberMatchingStrategy : IMemberMatchingStrategy
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

                if(string.Equals(source.RawName, t.RawName, strComp))
                {
                    target = t;
                    return true;
                }
            }

            return false;
        }
    }
}
