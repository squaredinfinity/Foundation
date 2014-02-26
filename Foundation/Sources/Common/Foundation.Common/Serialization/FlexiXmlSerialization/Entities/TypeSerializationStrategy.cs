using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SquaredInfinity.Foundation.Serialization.FlexiXmlSerialization.Entities
{
    public class TypeSerializationStrategy
    {
        readonly List<MemberSerializationStrategy> MemberSerializationStrategies 
            = new List<MemberSerializationStrategy>();

        public string TypeFullName { get; set; }

        public string XmlElementName { get; set; }
    }
}
