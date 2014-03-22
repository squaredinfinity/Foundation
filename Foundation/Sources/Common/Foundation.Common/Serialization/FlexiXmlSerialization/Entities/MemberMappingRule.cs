using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SquaredInfinity.Foundation.Serialization.FlexiXmlSerialization.Entities
{
    public abstract class MemberMappingRule
    {
        public string MemberName { get; set; }
    }

    public class MemberToAttributeMappingRule : MemberMappingRule
    {
        public string AttributeName { get; set; }
        public MemberToAttributeConverter GetAttribute { get; set; }
        public AttributeToMemberConverter GetMemberValue { get; set; }
    }

    public class MemberToAttributeMappingRule<TMember> : MemberToAttributeMappingRule
    {
        public string AttributeName { get; set; }
        public MemberToAttributeConverter<TMember> GetAttribute { get; set; }
        public AttributeToMemberConverter<TMember> GetMemberValue { get; set; }
    }

    public class MemberToElementMappingRule : MemberMappingRule
    {
        public MemberToElementConverter GetElement { get; set; }
        public ElementToMemberConverter GetMemberValue { get; set; }
    }
}
