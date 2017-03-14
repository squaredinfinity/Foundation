using SquaredInfinity.Types.Description;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SquaredInfinity.Serialization.FlexiPlainText
{
    public class FlexiPlainTextSerializer : FlexiSerializer, IFlexiPlainTextSerializer
    {
        public FlexiPlainTextSerializer(ITypeDescriptor typeDescriptor)
        {
            this.TypeDescriptor = typeDescriptor;
        }

        public string Serialize(object obj)
        {
            if (obj == null)
                throw new ArgumentNullException("obj");

            var objType = obj.GetType();

            var cx = new FlexiPlainTextSerializationContext(this, TypeDescriptor, TypeResolver);

            var result = cx.Serialize(obj);

            return result;
        }

        protected override ITypeSerializationStrategy CreateDefaultTypeSerializationStrategy(Type type, ITypeDescriptor typeDescriptor)
        {
            throw new NotImplementedException();   
        }
    }
}
