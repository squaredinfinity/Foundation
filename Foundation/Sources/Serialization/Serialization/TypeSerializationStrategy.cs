using SquaredInfinity.Types.Description;
using SquaredInfinity.Extensions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SquaredInfinity.Serialization
{
    public enum MemberSerializationOption
    {
        ImplicitIgnore,
        ExplicitIgnore,
        ImplicitSerialize,
        ExplicitSerialize
    }

    public class TypeSerializationStrategy : ITypeSerializationStrategy
    {
        protected class MembersSerializationStrategies : Dictionary<string, MemberSerializationStrategyInfo>
        { 
            public MembersSerializationStrategies()
                : base(StringComparer.OrdinalIgnoreCase)
            { }
        }

        [DebuggerDisplay("{DebuggerDisplay}")]
        protected class MemberSerializationStrategyInfo
        {
            public IMemberSerializationStrategy Strategy { get; set; }
            public bool IsMemberSerializable { get; set; }

            public MemberSerializationOption SerializationOption { get; set; }

            public string DebuggerDisplay
            {
                get
                {
                    return $"{Strategy.MemberName.ToStringWithNullOrEmpty()}: {SerializationOption}";
                }
            }

            public MemberSerializationStrategyInfo(IMemberSerializationStrategy strategy, bool isSerializable)
            {
                this.Strategy = strategy;
                this.IsMemberSerializable = isSerializable;

                // be default enable serialization if member is serializable
                if (isSerializable)
                    SerializationOption = MemberSerializationOption.ImplicitSerialize;
                else
                    SerializationOption = MemberSerializationOption.ImplicitIgnore;
            }

        }

        public Version Version { get; private set; }

        public Type Type { get; private set; }

        public IFlexiSerializer Serializer { get; private set; }

        public Types.Description.ITypeDescription TypeDescription { get; private set; }

        public Types.Description.ITypeDescriptor TypeDescriptor { get; private set; }

        readonly MembersSerializationStrategies _originalMembersSerializationStrategies = new MembersSerializationStrategies();
        protected MembersSerializationStrategies OriginalMembersSerializationStrategies
        {
            get { return _originalMembersSerializationStrategies; }
        }

        readonly MembersSerializationStrategies _actualMembersSerializationStrategies = new MembersSerializationStrategies();
        protected MembersSerializationStrategies ActualMembersSerializationStrategies
        {
            get { return _actualMembersSerializationStrategies; }
        }

        public TypeSerializationStrategy(IFlexiSerializer serializer, Type type, Types.Description.ITypeDescriptor typeDescriptor)
        {
            this.Serializer = serializer;
            this.Type = type;
            this.TypeDescriptor = typeDescriptor;

            this.TypeDescription = TypeDescriptor.DescribeType(type);

            Initialize();
        }

        protected void Initialize()
        {
            //# get all source members that can be serialized
            var serializableMembers =
                (from m in TypeDescription.Members
                 where CanSerializeMember(m)
                 select m);

            //# create default strategies for each serializable member

            foreach (var member in TypeDescription.Members)
            {
                var strategy = CreateSerializationStrategyForMember(member);

                bool isSerializable = CanSerializeMember(member);
                OriginalMembersSerializationStrategies.Add(member.Name, new MemberSerializationStrategyInfo(strategy, isSerializable));
                ActualMembersSerializationStrategies.Add(member.Name, new MemberSerializationStrategyInfo(strategy, isSerializable));
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


        public void IgnoreMember(string memberName)
        {
            var strategyInfo = (MemberSerializationStrategyInfo) null;

            if(ActualMembersSerializationStrategies.TryGetValue(memberName, out strategyInfo))
            {
                strategyInfo.SerializationOption = MemberSerializationOption.ExplicitIgnore;
            }
            else
            {
                throw new ArgumentException(memberName);
            }
        }


        public void CopySerializationSetupFrom(ITypeSerializationStrategy other_strategy)
        {
            var other = other_strategy as TypeSerializationStrategy;

            if (other == null)
                throw new ArgumentException("other strategy must be of type TypeSerializationStrategy");

            foreach(var os in other.ActualMembersSerializationStrategies)
            {
                var ts = (MemberSerializationStrategyInfo) null;

                if(this.ActualMembersSerializationStrategies.TryGetValue(os.Key, out ts))
                {
                    ts.SerializationOption = os.Value.SerializationOption;
                }
                else
                {
                    var info = new MemberSerializationStrategyInfo(os.Value.Strategy, os.Value.IsMemberSerializable);
                    info.SerializationOption = os.Value.SerializationOption;

                    this.ActualMembersSerializationStrategies.Add(os.Key, info);
                }
            }
        }
    }
}
