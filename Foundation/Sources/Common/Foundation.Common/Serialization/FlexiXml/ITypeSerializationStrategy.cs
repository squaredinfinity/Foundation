using SquaredInfinity.Foundation.Types.Description;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace SquaredInfinity.Foundation.Serialization.FlexiXml
{
    public interface ITypeSerializationStrategy
    {
        Version Version { get; }

        Type Type { get; }

        ITypeDescription TypeDescription { get; }
    }

    public interface ITypeSerializationStrategy<T> : ITypeSerializationStrategy
    {
        ITypeSerializationStrategy<T> IgnoreAllMembers();

        ITypeSerializationStrategy<T> IgnoreMember(Expression<Func<object>> memberExpression);
    }
}
