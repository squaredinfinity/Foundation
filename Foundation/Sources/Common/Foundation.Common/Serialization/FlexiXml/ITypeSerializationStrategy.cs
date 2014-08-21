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
    public interface ITypeSerializationStrategy
    {
        Version Version { get; }

        Type Type { get; }

        ITypeDescription TypeDescription { get; }

        XElement Serialize(object instance, ITypeSerializationContext cx);
        string ConstructElementNameForType(Type type);
    }

    public interface ITypeSerializationStrategy<T> : ITypeSerializationStrategy
    {
        ITypeSerializationStrategy<T> IgnoreAllMembers();

        ITypeSerializationStrategy<T> IgnoreMember(Expression<Func<T>> memberExpression);

        ITypeSerializationStrategy<T> ResolveReferenceWith<TypeToResolve>(Action<ReferenceResolutionContext<T, TypeToResolve>> resolveReference);

        ITypeSerializationStrategy<T> SerializeMember(Expression<Func<T>> memberExpression);
    }

    public class ReferenceResolutionContext<TRoot, TypeToResolve>
    {
        public XDocument RootDocument { get; private set; }
        public TRoot Root { get; private set; }

        public string ReferenceName { get; private set; }
        public TypeToResolve Result { get; set; }
        public bool IsSuccesful { get; set; }


        public ReferenceResolutionContext(XDocument rootDocument, TRoot root, string referenceName)
        {
            this.RootDocument = rootDocument;
            this.Root = root;
            this.ReferenceName = referenceName;
        }
    }
}
