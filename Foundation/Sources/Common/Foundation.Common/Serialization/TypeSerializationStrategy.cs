using SquaredInfinity.Foundation.Types.Description;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SquaredInfinity.Foundation.Serialization
{
    public class TypeSerializationStrategy : ITypeSerializationStrategy
    {
        public Version Version { get; private set; }

        public Type Type { get; private set; }

        public IFlexiSerializer Serializer { get; private set; }

        public Types.Description.ITypeDescription TypeDescription { get; private set; }

        public Types.Description.ITypeDescriptor TypeDescriptor { get; private set; }

        readonly List<IMemberSerializationStrategy> _originalContentSerializationStrategies =
            new List<IMemberSerializationStrategy>();

        protected List<IMemberSerializationStrategy> OriginalContentSerializationStrategies
        {
            get { return _originalContentSerializationStrategies; }
        }

        readonly List<IMemberSerializationStrategy> _originalNonSerialiableContentationStrategies =
            new List<IMemberSerializationStrategy>();

        protected List<IMemberSerializationStrategy> OriginalNonSerialiableContentSerializationStrategies
        {
            get { return _originalNonSerialiableContentationStrategies; }
        }

        readonly List<IMemberSerializationStrategy> _actualContentSerializationStrategies =
            new List<IMemberSerializationStrategy>();

        protected List<IMemberSerializationStrategy> ActualContentSerializationStrategies
        {
            get { return _actualContentSerializationStrategies; }
        }

        public TypeSerializationStrategy(IFlexiSerializer serializer, Type type, Types.Description.ITypeDescriptor typeDescriptor)
        {
            this.Serializer = serializer;
            this.Type = type;
            this.TypeDescriptor = typeDescriptor;

            this.TypeDescription = TypeDescriptor.DescribeType(type);

            Initialize();
        }

        public IReadOnlyList<IMemberSerializationStrategy> GetContentSerializationStrategies()
        {
            return ActualContentSerializationStrategies;
        }

        protected void Initialize()
        {
            //# get all source members that can be serialized
            var serializableMembers =
                (from m in TypeDescription.Members
                 where CanSerializeMember(m)
                 select m);

            //# create default strategies for each serializable member

            foreach (var member in serializableMembers)
            {
                var strategy = CreateSerializationStrategyForMember(member);

                OriginalContentSerializationStrategies.Add(strategy);
            }

            ActualContentSerializationStrategies.AddRange(OriginalContentSerializationStrategies);


            //# get all source members that cannot be serialized
            var nonserializableMembers =
               (from m in TypeDescription.Members
                where !CanSerializeMember(m)
                select m);

            //# create default strategies for each serializable member

            foreach (var member in nonserializableMembers)
            {
                var strategy = CreateSerializationStrategyForMember(member);

                OriginalNonSerialiableContentSerializationStrategies.Add(strategy);
            }
        }

        protected virtual bool CanSerializeMember(ITypeMemberDescription member)
        {
            // public members only
            if (member.Visibility != MemberVisibility.Public)
                return false;

            // explicit interface implementations are not supported
            if (member.IsExplicitInterfaceImplementation)
                return false;

            // only members which have getters can be serialized
            if (!member.CanGetValue)
                return false;

            // if member is read-only, serialize it only if it's enumerable and not IReadOnlyList or IReadOnlyCollection
            // and !string
            if (!member.CanSetValue)
            {
                if (!typeof(IEnumerable).IsAssignableFrom(member.MemberType.Type))
                    return false;

                if (member.MemberType.Type == typeof(string))
                    return false;

                if (member.MemberType.Type.IsGenericType)
                {
                    var genericTypeDefinition = member.MemberType.Type.GetGenericTypeDefinition();

                    if (genericTypeDefinition == typeof(IReadOnlyCollection<>))
                        return false;

                    if (genericTypeDefinition == typeof(IReadOnlyList<>))
                        return false;

                    if (genericTypeDefinition == typeof(IReadOnlyDictionary<,>))
                        return false;
                }
            }

            // read-only members which are not enumerable should not be serialized
            // because they could not be deserialized after
            // for read-only enumerable members there's always a chance th
            //if (!member.CanSetValue && !typeof(IEnumerable).IsAssignableFrom(member.MemberType.Type))
            //    return false;

            return true;
        }

        protected virtual IMemberSerializationStrategy CreateSerializationStrategyForMember(ITypeMemberDescription member)
        {
            var strategy =
                new MemberSerializationStrategy(member);

            return strategy;
        }

        protected virtual bool TryConvertToStringIfTypeSupports(Type objType, object obj, out string result)
        {
            var typeConverter = System.ComponentModel.TypeDescriptor.GetConverter(objType);

            if (typeConverter == null
                // what is serialized will need to be deserialized
                // check if conversion can work both ways
                || !typeConverter.CanConvertFrom(typeof(string))
                || !typeConverter.CanConvertTo(typeof(string)))
            {
                result = null;
                return false;
            }

            result = (string)typeConverter.ConvertTo(obj, typeof(string));

            return true;
        }

        protected virtual bool TryConvertFromStringIfTypeSupports(string stringRepresentation, Type resultType, out object result)
        {
            var typeConverter = System.ComponentModel.TypeDescriptor.GetConverter(resultType);

            if (typeConverter == null || !typeConverter.CanConvertFrom(typeof(string)))
            {
                result = null;
                return false;
            }

            result = typeConverter.ConvertFrom(stringRepresentation);

            return true;
        }
    }
}
