using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SquaredInfinity.Foundation.Extensions;

namespace SquaredInfinity.Foundation.Serialization
{
    public class TypeSerializationStrategy<TSerializationStrategy, T> 
        : TypeSerializationStrategy, ITypeSerializationStrategy<TSerializationStrategy, T>
        where TSerializationStrategy : class, ITypeSerializationStrategy<TSerializationStrategy, T>
    {
        public TypeSerializationStrategy(IFlexiSerializer serializer, Types.Description.ITypeDescriptor typeDescriptor)
            : base(serializer, typeof(T), typeDescriptor)
        { }

        public TypeSerializationStrategy(IFlexiSerializer serializer, Type type, Types.Description.ITypeDescriptor typeDescriptor)
            : base(serializer, type, typeDescriptor)
        { }

        public TSerializationStrategy IgnoreAllMembers()
        {
            ActualContentSerializationStrategies.Clear();

            return this as TSerializationStrategy;
        }

        public TSerializationStrategy IgnoreMember(System.Linq.Expressions.Expression<Func<T, object>> memberExpression)
        {
            var memberName = memberExpression.GetAccessedMemberName();

            var strategy =
                (from s in ActualContentSerializationStrategies
                 where string.Equals(s.MemberName, memberName)
                 select s).Single();

            ActualContentSerializationStrategies.Remove(strategy);

            return this as TSerializationStrategy;
        }

        public TSerializationStrategy ResolveReferenceWith<TypeToResolve>(Action<ReferenceResolutionContext<T, TypeToResolve>> resolveReference)
        {
            throw new NotImplementedException();
            return this as TSerializationStrategy;
        }

        public TSerializationStrategy SerializeMember(System.Linq.Expressions.Expression<Func<T, object>> memberExpression)
        {
            return SerializeMember(memberExpression, shouldSerializeMember: null);
        }


        public TSerializationStrategy SerializeMember(System.Linq.Expressions.Expression<Func<T, object>> memberExpression, Func<T, bool> shouldSerializeMember)
        {
            var memberName = memberExpression.GetAccessedMemberName();

            bool shouldAddToActual = false;

            var strategy =
                    (from s in ActualContentSerializationStrategies
                     where string.Equals(s.MemberName, memberName)
                     select s).FirstOrDefault();

            if (strategy == null)
            {
                strategy =
                    (from s in OriginalContentSerializationStrategies
                     where string.Equals(s.MemberName, memberName)
                     select s).FirstOrDefault();

                shouldAddToActual = true;
            }

            if (strategy == null)
            {
                strategy =
                    (from s in OriginalNonSerialiableContentSerializationStrategies
                     where string.Equals(s.MemberName, memberName)
                     select s).FirstOrDefault();

                shouldAddToActual = true;
            }

            if (strategy == null)
            {
                throw new Exception("Unable to create serialization strategy for member {0}.".FormatWith(memberName));
            }


            if (shouldSerializeMember != null)
            {
                strategy.ShouldSerializeMember = new Func<object, bool>((_target) =>
                {
                    var local = shouldSerializeMember;
                    return local((T)_target);
                });
            }

            if (shouldAddToActual)
                ActualContentSerializationStrategies.Add(strategy);

            return this as TSerializationStrategy;
        }

        public TSerializationStrategy SerializeAllRemainingMembers()
        {
            var ignoredMembersStrategies =
                (from ss in OriginalContentSerializationStrategies.Except(ActualContentSerializationStrategies)
                 select ss);

            ActualContentSerializationStrategies.AddRange(ignoredMembersStrategies);

            return this as TSerializationStrategy;
        }


        public TSerializationStrategy CopySerializationSetupFromBaseClass()
        {
            var baseType = Type.BaseType;

            if (baseType == null)
                return this as TSerializationStrategy;

            var base_strategy = Serializer.GetOrCreateTypeSerializationStrategy(baseType);

            ActualContentSerializationStrategies.AddRange(base_strategy.GetContentSerializationStrategies());

            return this as TSerializationStrategy;
        }
    }
}
