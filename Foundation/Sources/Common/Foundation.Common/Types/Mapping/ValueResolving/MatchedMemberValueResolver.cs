using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SquaredInfinity.Foundation.Types.Mapping.ValueResolving
{
    public class MatchedMemberValueResolver : IValueResolver
    {
        MemberMatching.IMemberMatch Match { get; set; }

        public MatchedMemberValueResolver(MemberMatching.IMemberMatch match)
        {
            this.Match = match;
        }

        public object ResolveValue(object source)
        {
            return Match.From.GetValue(source);
        }
    }
}
