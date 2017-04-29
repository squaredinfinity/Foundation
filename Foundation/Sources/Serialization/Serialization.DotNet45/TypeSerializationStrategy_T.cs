using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SquaredInfinity.Extensions;

namespace SquaredInfinity.Serialization
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
            foreach (var si in ActualMembersSerializationStrategies.Values)
                si.SerializationOption = MemberSerializationOption.ImplicitIgnore;

            return this as TSerializationStrategy;
        }

        public TSerializationStrategy IgnoreMember(System.Linq.Expressions.Expression<Func<T, object>> memberExpression)
        {
            var memberName = memberExpression.GetAccessedMemberName();

            base.IgnoreMember(memberName);

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
            
            var si = (MemberSerializationStrategyInfo)null;

            if(ActualMembersSerializationStrategies.TryGetValue(memberName, out si))
            {
                if (shouldSerializeMember != null)
                {
                    si.Strategy.ShouldSerializeMember = new Func<object, bool>((_target) =>
                    {
                        var local = shouldSerializeMember;
                        return local((T)_target);
                    });
                }

                si.SerializationOption = MemberSerializationOption.ExplicitSerialize;
            }
            else
            {
                throw new NotImplementedException();
            }

            return this as TSerializationStrategy;
        }

        public TSerializationStrategy SerializeAllRemainingMembers()
        {
            foreach(var si in ActualMembersSerializationStrategies.Values)
            {
                if (si.SerializationOption == MemberSerializationOption.ImplicitIgnore)
                    si.SerializationOption = MemberSerializationOption.ImplicitSerialize;
            }

            return this as TSerializationStrategy;
        }


        public TSerializationStrategy CopySerializationSetupFromBaseClass()
        {
            var baseType = Type.BaseType;

            if (baseType == null)
                return this as TSerializationStrategy;

            var base_strategy = Serializer.GetOrCreateTypeSerializationStrategy(baseType);

            CopySerializationSetupFrom(base_strategy);
            
            return this as TSerializationStrategy;
        }
    }
}
