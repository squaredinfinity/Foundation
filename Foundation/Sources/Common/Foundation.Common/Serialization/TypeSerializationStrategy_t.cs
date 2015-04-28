using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SquaredInfinity.Foundation.Extensions;

namespace SquaredInfinity.Foundation.Serialization
{
    public class TypeSerializationStrategy<T> : TypeSerializationStrategy, ITypeSerializationStrategy<T>
    {
        public TypeSerializationStrategy(IFlexiSerializer serializer, Types.Description.ITypeDescriptor typeDescriptor)
            : base(serializer, typeof(T), typeDescriptor)
        { }

        public TypeSerializationStrategy(IFlexiSerializer serializer, Type type, Types.Description.ITypeDescriptor typeDescriptor)
            : base(serializer, type, typeDescriptor)
        { }

        public ITypeSerializationStrategy<T> IgnoreAllMembers()
        {
            ActualContentSerializationStrategies.Clear();

            return this;
        }

        public ITypeSerializationStrategy<T> IgnoreMember(System.Linq.Expressions.Expression<Func<T, object>> memberExpression)
        {
            var memberName = memberExpression.GetAccessedMemberName();

            var strategy =
                (from s in ActualContentSerializationStrategies
                 where string.Equals(s.MemberName, memberName)
                 select s).Single();

            ActualContentSerializationStrategies.Remove(strategy);

            return this;
        }

        public ITypeSerializationStrategy<T> ResolveReferenceWith<TypeToResolve>(Action<ReferenceResolutionContext<T, TypeToResolve>> resolveReference)
        {
            return this;
        }

        public ITypeSerializationStrategy<T> SerializeMember(System.Linq.Expressions.Expression<Func<T, object>> memberExpression)
        {
            return SerializeMember(memberExpression, shouldSerializeMember: null);
        }


        public ITypeSerializationStrategy<T> SerializeMember(System.Linq.Expressions.Expression<Func<T, object>> memberExpression, Func<T, bool> shouldSerializeMember)
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

            return this;
        }

        //public ITypeSerializationStrategy<T> SerializeMemberAsAttribute<TMember>(
        //    System.Linq.Expressions.Expression<Func<T, object>> memberExpression,
        //    Func<T, bool> shouldSerializeMember,
        //    Func<T, TMember, string> serialize,
        //    Func<XAttribute, TMember> deserialize)
        //{
        //    var memberName = memberExpression.GetAccessedMemberName();

        //    bool shouldAddToActual = false;

        //    var strategy =
        //            (from s in ActualContentSerializationStrategies
        //             where string.Equals(s.MemberName, memberName)
        //             select s).FirstOrDefault();

        //    if (strategy == null)
        //    {
        //        strategy =
        //            (from s in OriginalContentSerializationStrategies
        //             where string.Equals(s.MemberName, memberName)
        //             select s).FirstOrDefault();

        //        shouldAddToActual = true;
        //    }

        //    if (strategy == null)
        //    {
        //        var ex = new ArgumentException("Unable to find Serialization Strategy.");
        //        // todo: ex.addcontext(member, type)

        //        throw ex;
        //    }

        //    if (shouldSerializeMember != null)
        //    {
        //        strategy.ShouldSerializeMember = new Func<object, bool>((_target) =>
        //        {
        //            var local = shouldSerializeMember;
        //            return local((T)_target);
        //        });
        //    }



        //    strategy.CustomSerialize = new Func<object, object, string>((owner, member) => serialize((T)owner, (TMember)member));
        //    strategy.CustomDeserialize = new Func<object, object>(attrib => deserialize((XAttribute)attrib));

        //    if (shouldAddToActual)
        //        ActualContentSerializationStrategies.Add(strategy);

        //    return this;
        //}

        public ITypeSerializationStrategy<T> SerializeAllRemainingMembers()
        {
            var ignoredMembersStrategies =
                (from ss in OriginalContentSerializationStrategies.Except(ActualContentSerializationStrategies)
                 select ss);

            ActualContentSerializationStrategies.AddRange(ignoredMembersStrategies);

            return this;
        }


        public ITypeSerializationStrategy<T> CopySerializationSetupFromBaseClass()
        {
            var baseType = Type.BaseType;

            if (baseType == null)
                return this;

            var base_strategy = Serializer.GetOrCreateTypeSerializationStrategy(baseType);

            ActualContentSerializationStrategies.AddRange(base_strategy.GetContentSerializationStrategies());

            return this;
        }
    }
}
