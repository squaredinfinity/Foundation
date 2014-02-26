using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace SquaredInfinity.Foundation.Serialization.FlexiXmlSerialization.Services
{
    public partial class FlexiXmlSerializationService
    {
        public class ConvertStringToPropertyValueEventArgs : CommandHandlerEventArgs<object>
        {
            public object DeserializationRootObject { get; private set; }
            public Type DeclaringType { get; private set; }
            public PropertyInfo PropertyInfo { get; private set; }
            public string ValueAsString { get; private set; }

            public ConvertStringToPropertyValueEventArgs(
                object deserializationRootObject,
                Type declaringType,
                PropertyInfo propertyInfo,
                string valueAsString)
            {
                this.DeserializationRootObject = deserializationRootObject;
                this.DeclaringType = declaringType;
                this.PropertyInfo = propertyInfo;
                this.ValueAsString = valueAsString;
            }
        }
    }
}
