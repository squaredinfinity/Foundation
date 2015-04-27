using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace SquaredInfinity.Foundation.Serialization
{
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


        //ITypeSerializationStrategy<T> SerializeMemberAsAttribute<TMember>(
        //    Expression<Func<T, object>> memberExpression, 
        //    Func<T, bool> shouldSerializeMember,
        //    Func<T, TMember, string> serialize,
        //    Func<XAttribute, TMember> deserialize);

        /// <summary>
        /// Enables serialization of a specified member (even if its serialization has been disabled before).
        /// </summary>
        /// <param name="memberExpression"></param>
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
}
