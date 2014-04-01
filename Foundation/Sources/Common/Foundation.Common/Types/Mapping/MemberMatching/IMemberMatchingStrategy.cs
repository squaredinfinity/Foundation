using SquaredInfinity.Foundation.Types.Description;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SquaredInfinity.Foundation.Types.MemberMatching
{
    public interface IMemberMatchingStrategy
    {
        bool TryMapMembers(
            ITypeMemberDescription source,
            IList<ITypeMemberDescription> targetCandidates, // todo: make it readonly list in 4.5
            out ITypeMemberDescription target);
    }
}
