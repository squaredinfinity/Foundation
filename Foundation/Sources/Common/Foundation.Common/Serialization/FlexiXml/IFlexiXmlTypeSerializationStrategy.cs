using SquaredInfinity.Foundation.Types.Description;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace SquaredInfinity.Foundation.Serialization.FlexiXml
{
    public interface IFlexiXmlTypeSerializationStrategy : ITypeSerializationStrategy
    {
        XElement Serialize(
            object instance,
            IFlexiXmlSerializationContext cx,
            out bool hasAlreadyBeenSerialized);

        XElement Serialize(
            object instance, 
            IFlexiXmlSerializationContext serializationContext, 
            string rootElementName,
            out bool hasAlreadyBeenSerialized);
        
        object Deserialize(
            XElement xml, 
            Type targetType,
            IFlexiXmlSerializationContext cx);

        void Deserialize(
            XElement xml,
            object targetInstance,
            IFlexiXmlSerializationContext cx);

        string ConstructElementNameForType(Type type);
    }    
}
