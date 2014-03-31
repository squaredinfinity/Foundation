using SquaredInfinity.Foundation.Types.Description;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SquaredInfinity.Foundation.Types.Mapping
{
    public interface IMemberMappingStrategy
    {
        bool TryMapMembers(
            ITypeMemberDescription source,
            IList<ITypeMemberDescription> targetCandidates,
            out ITypeMemberDescription target);
    }
}
