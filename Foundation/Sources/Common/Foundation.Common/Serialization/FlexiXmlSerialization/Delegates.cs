using SquaredInfinity.Foundation.Serialization.FlexiXmlSerialization.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace SquaredInfinity.Foundation.Serialization.FlexiXmlSerialization
{
    public delegate XAttribute MemberToAttributeConverter(SerializationContext context, object memberValue);
    public delegate XAttribute MemberToAttributeConverter<TMember>(SerializationContext context, TMember memberValue);

    public delegate object AttributeToMemberConverter(SerializationContext context, XAttribute attribute);
    public delegate TMember AttributeToMemberConverter<TMember>(SerializationContext context, XAttribute attribute);

    public delegate XElement MemberToElementConverter(SerializationContext context, object memberValue);
    public delegate XElement MemberToElementConverter<TMember>(SerializationContext context, TMember memberValue);

    public delegate object ElementToMemberConverter(SerializationContext context, XElement element);
    public delegate TMember ElementToMemberConverter<TMember>(SerializationContext context, XElement element);
}   
