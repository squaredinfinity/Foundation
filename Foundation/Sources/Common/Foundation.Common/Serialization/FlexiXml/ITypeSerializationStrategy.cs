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

        XElement Serialize(
            object instance, 
            ISerializationContext cx,
            out bool hasAlreadyBeenSerialized);

        XElement Serialize(
            object instance, 
            ISerializationContext serializationContext, 
            string rootElementName,
            out bool hasAlreadyBeenSerialized);
        
        object Deserialize(
            XElement xml, 
            Type targetType,
            ISerializationContext cx);

        void Deserialize(
            XElement xml,
            object targetInstance,
            ISerializationContext cx);

        string ConstructElementNameForType(Type type);

        IReadOnlyList<IMemberSerializationStrategy> GetContentSerializationStrategies();
    }

    public interface ITypeSerializationStrategy<T> : ITypeSerializationStrategy
    {
        /// <summary>
        /// All members will be ignored during serialization (unless explicitly enabled later).
        /// </summary>
        /// <returns></returns>
        ITypeSerializationStrategy<T> IgnoreAllMembers();


        /// <summary>
        /// Specified member will not be serialized (unless explicitly enabled later)
        /// </summary>
        /// <param name="memberExpression"></param>
        /// <returns></returns>
        ITypeSerializationStrategy<T> IgnoreMember(Expression<Func<T, object>> memberExpression);


        ITypeSerializationStrategy<T> ResolveReferenceWith<TypeToResolve>(Action<ReferenceResolutionContext<T, TypeToResolve>> resolveReference);

        /// <summary>
        /// Enables serialization of a specified member (even if its serialization has been disabled before).
        /// </summary>
        /// <param name="memberExpression"></param>
        /// <param name="shouldSerializeMember"></param>
        /// <returns></returns>
        ITypeSerializationStrategy<T> SerializeMember(Expression<Func<T, object>> memberExpression, Func<T, bool> shouldSerializeMember);


        ITypeSerializationStrategy<T> SerializeMember(
            Expression<Func<T, object>> memberExpression, 
            Func<T, bool> shouldSerializeMember,
            Func<T, string> serialize,
            Func<string, T> deserialize);

        /// <summary>
        /// Enables serialization of a specified member (even if its serialization has been disabled before).
        /// </summary>
        /// <param name="memberExpression"></param>
        /// <param name="shouldSerializeMember"></param>
        /// <returns></returns>
        ITypeSerializationStrategy<T> SerializeMember(Expression<Func<T, object>> memberExpression);

        /// <summary>
        /// Enables serialization of all members which have not been explicitly ignored before.
        /// </summary>
        /// <returns></returns>
        ITypeSerializationStrategy<T> SerializeAllRemainingMembers();

        /// <summary>
        /// Copies member serialization setup from base class
        /// </summary>
        /// <returns></returns>
        ITypeSerializationStrategy<T> CopySerializationSetupFromBaseClass();
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
