using SquaredInfinity.Foundation.ILGeneration;
using System;
using System.Collections.Generic;
using System.Linq;
using SquaredInfinity.Foundation.Extensions;
using System.Text;
using System.Reflection.Emit;
using System.Reflection;
using SquaredInfinity.Foundation.Types.Description.Reflection;
using SquaredInfinity.Foundation.Types.Description.IL;

namespace SquaredInfinity.Foundation.Types.Mapping.ValueResolving
{
    public class MatchedMemberValueResolver : IValueResolver
    {
        public Type FromType { get; set; }

        public Type ToType { get; set; }

        MemberMatching.IMemberMatch Match { get; set; }

        public bool AreFromAndToTypesSame { get; set; }
        public bool AreFromAndToImmutable { get; set; }
        public bool AreFromAndToValueType { get; set; }
        public bool IsMappingNeeded { get; set; }

        public MatchedMemberValueResolver(MemberMatching.IMemberMatch match)
        {
            this.Match = match;

            this.FromType = match.From.MemberType.Type;
            this.ToType = match.To.MemberType.Type;

            AreFromAndToTypesSame = FromType == ToType;
            AreFromAndToImmutable = match.From.MemberType.AreAllMembersImmutable && match.To.MemberType.AreAllMembersImmutable;
            AreFromAndToValueType = FromType.IsValueType && ToType.IsValueType;

            IsMappingNeeded =
                ! (AreFromAndToTypesSame && AreFromAndToValueType && AreFromAndToImmutable);
        }

        public object ResolveValue(object source)
        {
            return Match.From.GetValue(source);
        }
    }
}
