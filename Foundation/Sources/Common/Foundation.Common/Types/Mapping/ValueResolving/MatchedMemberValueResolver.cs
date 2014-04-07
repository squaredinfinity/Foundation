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

            this.FromType = Type.GetType(match.From.AssemblyQualifiedMemberTypeName);
            this.ToType = Type.GetType(match.To.AssemblyQualifiedMemberTypeName);
        }

        public object ResolveValue(object source)
        {
            return Match.From.GetValue(source);
        }
    }
}
