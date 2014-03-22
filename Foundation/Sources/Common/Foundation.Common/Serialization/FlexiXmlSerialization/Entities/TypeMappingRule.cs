using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace SquaredInfinity.Foundation.Serialization.FlexiXmlSerialization.Entities
{
    public class TypeMappingRule
    {
        public Type MappedType { get; set; }

        public ParticipationMode MemberParticipation { get; set; }

        public MemberDiscoveryMode MemberDiscovery { get; set; }

        public TypeMappingRule MapMember(
            string memberName,
            MemberToAttributeConverter getAttribute,
            AttributeToMemberConverter getMemberValue)
        {
            var rule = new MemberToAttributeMappingRule();

            rule.MemberName = memberName;
            rule.GetAttribute = getAttribute;
            rule.GetMemberValue = getMemberValue;

            // todo: add to list of some sort

            return this;
        }

        public TypeMappingRule MapMember<TMember>(
            string memberName,
            MemberToAttributeConverter<TMember> getAttribute,
            AttributeToMemberConverter<TMember> getMemberValue)
        {
            var rule = new MemberToAttributeMappingRule<TMember>();

            rule.MemberName = memberName;
            rule.GetAttribute = getAttribute;
            rule.GetMemberValue = getMemberValue;

            // todo: add to list of some sort

            return this;
        }

        public TypeMappingRule MapMember(
            string memberName,
            MemberToElementConverter getElement,
            ElementToMemberConverter getMemberValue)
        {
            var rule = new MemberToElementMappingRule();

            rule.MemberName = memberName;
            rule.GetElement = getElement;
            rule.GetMemberValue = getMemberValue;

            // todo: add to list of some sort

            return this;
        }
    }
}
