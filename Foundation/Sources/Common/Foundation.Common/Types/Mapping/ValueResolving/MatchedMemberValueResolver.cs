using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SquaredInfinity.Foundation.Types.Mapping.ValueResolving
{
    public class MatchedMemberValueResolver : IValueResolver
    {
        public Type FromType { get; set; }

        public Type ToType { get; set; }

        MemberMatching.IMemberMatch Match { get; set; }

        public MatchedMemberValueResolver(MemberMatching.IMemberMatch match)
        {
            this.Match = match;

            this.FromType = match.From.MemberType.Type;
            this.ToType = match.To.MemberType.Type;
        }

        public object ResolveValue(object source)
        {
            return Match.From.GetValue(source);
        }
    }
}
