using SquaredInfinity.Foundation.Types.Description;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SquaredInfinity.Foundation.Serialization
{
    //public class EnumerableTypeSerializationStrategy<T, I> : EnumerableTypeSerializationStrategy, IEnumerableTypeSerializationStrategy<T, I>
    //       where T : IEnumerable<I>
    //{
    //    public Predicate<I> ElementFilterPredicate { get; private set; }

    //    public EnumerableTypeSerializationStrategy(IFlexiSerializer serializer, ITypeDescriptor typeDescriptor)
    //    {
    //        throw new NotImplementedException();
    //    }

    //    public IEnumerableTypeSerializationStrategy<T, I> ElementFilter(Predicate<I> filterElement)
    //    {
    //        this.ElementFilterPredicate = filterElement;

    //        return this;
    //    }

    //    protected override bool ShouldSerializeItem(object item)
    //    {
    //        return ElementFilterPredicate((I)item);
    //    }

    //    public ITypeSerializationStrategy<T> IgnoreAllMembers()
    //    {
    //        throw new NotImplementedException();
    //    }

    //    public ITypeSerializationStrategy<T> IgnoreMember(System.Linq.Expressions.Expression<Func<T, object>> memberExpression)
    //    {
    //        throw new NotImplementedException();
    //    }

    //    public ITypeSerializationStrategy<T> ResolveReferenceWith<TypeToResolve>(Action<ReferenceResolutionContext<T, TypeToResolve>> resolveReference)
    //    {
    //        throw new NotImplementedException();
    //    }

    //    public ITypeSerializationStrategy<T> SerializeMember(System.Linq.Expressions.Expression<Func<T, object>> memberExpression, Func<T, bool> shouldSerializeMember)
    //    {
    //        throw new NotImplementedException();
    //    }

    //    public ITypeSerializationStrategy<T> SerializeMember(System.Linq.Expressions.Expression<Func<T, object>> memberExpression)
    //    {
    //        throw new NotImplementedException();
    //    }

    //    public ITypeSerializationStrategy<T> SerializeAllRemainingMembers()
    //    {
    //        throw new NotImplementedException();
    //    }

    //    public ITypeSerializationStrategy<T> CopySerializationSetupFromBaseClass()
    //    {
    //        throw new NotImplementedException();
    //    }
    //}
}
