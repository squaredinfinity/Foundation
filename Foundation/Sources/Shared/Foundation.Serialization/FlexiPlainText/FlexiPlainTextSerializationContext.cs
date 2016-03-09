using SquaredInfinity.Foundation.Types.Description;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SquaredInfinity.Foundation.Serialization.FlexiPlainText
{
    public class FlexiPlainTextSerializationContext : SerializationContext, IFlexiPlainTextSerializationContext
    {
        public FlexiPlainTextSerializationContext(
            IFlexiPlainTextSerializer serializer,
            ITypeDescriptor typeDescriptor,
            TypeResolver typeResolver,
            Func<Type, CreateInstanceContext, object> createInstanceWith = null)
            : base(serializer, typeDescriptor, typeResolver, createInstanceWith)
        { }

        public string Serialize(object obj)
        {
            return null;
        }
    }
}
