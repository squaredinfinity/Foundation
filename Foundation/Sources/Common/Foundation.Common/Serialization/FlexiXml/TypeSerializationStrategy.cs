using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SquaredInfinity.Foundation.Serialization.FlexiXml
{
    public class TypeSerializationStrategy : ITypeSerializationStrategy
    {
        public Version Version { get; set; }

        public Type Type { get; set; }

        public Types.Description.ITypeDescription TypeDescription { get; set; }

        public Types.Description.ITypeDescriptor TypeDescriptor { get; set; }

        public TypeSerializationStrategy(Type type, Types.Description.ITypeDescriptor typeDescriptor)
        {
            this.Type = type;
            this.TypeDescriptor = TypeDescriptor;

            this.TypeDescription = TypeDescriptor.DescribeType(type);
        }
    }

    public class TypeSerializationStrategy<T> : TypeSerializationStrategy, ITypeSerializationStrategy<T>
    {
        public TypeSerializationStrategy(Types.Description.ITypeDescriptor typeDescriptor)
            : base(typeof(T), typeDescriptor)
        {

        }

        public ITypeSerializationStrategy<T> IgnoreAllMembers()
        {
            throw new NotImplementedException();
        }


        public ITypeSerializationStrategy<T> IgnoreMember(System.Linq.Expressions.Expression<Func<object>> memberExpression)
        {
            throw new NotImplementedException();
        }
        
        public ITypeSerializationStrategy<T> ResolveReferenceWith<TypeToResolve>(Action<ReferenceResolutionContext<T, TypeToResolve>> resolveReference)
        {
            throw new NotImplementedException();
        }
    }
}
