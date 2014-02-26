using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SquaredInfinity.Foundation.Serialization.FlexiXmlSerialization.Services
{
    public partial class FlexiXmlSerializationService
    {
        public class ResolveElementReferenceEventArgs : CommandHandlerEventArgs<object>
        {
            public object DeserializationRootObject { get; private set; }
            public string ReferenceType { get; private set; }
            public string ReferencedObjectName { get; private set; }

            public ResolveElementReferenceEventArgs(
                object deserializationRootObject,
                string referenceType,
                string referencedObjectName)
            {
                this.DeserializationRootObject = deserializationRootObject;
                this.ReferenceType = referenceType;
                this.ReferencedObjectName = referencedObjectName;
            }
        }
    }
}
