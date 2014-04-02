using SquaredInfinity.Foundation.Types.Description;
using SquaredInfinity.Foundation.Types.Description.Reflection;
using SquaredInfinity.Foundation.Types.Mapping;
using SquaredInfinity.Foundation.Types.Mapping.MemberMatching;
using SquaredInfinity.Foundation.Types.Mapping.ValueResolving;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace SquaredInfinity.Foundation.Serialization.Mapping
{
    public class FlexiTypeMapper : TypeMapper
    {
        protected override object CreateClonePrototype(Type type)
        {
            return
                new FlexiMappedInstance(type);
        }

        protected override ITypeMappingStrategy CreateDefaultTypeMappingStrategy(Type sourceType, Type targetType)
        {
            var impl = 
                new FlexiTypeMappingStrategy_Implementation(
                    sourceType,
                    targetType,
                    new ReflectionBasedTypeDescriptor(),
                    new MemberMatchingRuleCollection() { new ExactNameMatchMemberMatchingRule() },
                    valueResolvers: null);

            return new FlexiTypeMappingStrategy(impl);
        }
    }

    class FlexiTypeMappingStrategy_Implementation : TypeMappingStrategy
    {
        public FlexiTypeMappingStrategy_Implementation(
            Type sourceType,
            Type targetType,
            ITypeDescriptor typeDescriptor,
            MemberMatchingRuleCollection memberMatchingRules,
            ValueResolverCollection valueResolvers)
            : 
            base(
            sourceType,
            targetType,
            typeDescriptor,
            memberMatchingRules,
            valueResolvers)
        { }

        protected override IValueResolver GetDefaultValueResolver(IMemberMatch match)
        {
            return new FlexiValueResolver(match);
        }
    }

    class FlexiValueResolver : IValueResolver
    {
        readonly IMemberMatch Match;

        public FlexiValueResolver(IMemberMatch match)
        {
            this.Match = match;
        }

        public object ResolveValue(object source)
        {
            var val = Match.From.GetValue(source);

            if (val is XElement)
            {
                return new FlexiXElementToCDATA(source as XElement);
            }

            return val;
        }
    }

    class FlexiXElementToCDATA
    {
        public XElement Original { get; private set; }

        public FlexiXElementToCDATA(XElement original)
        {
            this.Original = original;
        }

        public static implicit operator XElement(FlexiXElementToCDATA value)
        {
            return value.Original;
        }
    }

    class FlexiTypeMappingStrategy : ITypeMappingStrategy
    {
        readonly ITypeMappingStrategy Internal;

        public FlexiTypeMappingStrategy(ITypeMappingStrategy strategy)
        {
            this.Internal = strategy;
        }

        public ITypeDescription SourceTypeDescription
        {
            get { return Internal.SourceTypeDescription; }
        }

        public ITypeDescription TargetTypeDescription
        {
            get { return new FlexiTypeDescription(Internal.SourceTypeDescription); }
        }

        public bool CloneListElements
        {
            get { return Internal.CloneListElements; }
        }

        public bool TryGetValueResolverForMember(string memberName, out Types.Mapping.ValueResolving.IValueResolver valueResolver)
        {
            return Internal.TryGetValueResolverForMember(memberName, out valueResolver);
        }
    }

    public class FlexiTypeDescriptor : ITypeDescriptor
    {
        readonly ITypeDescription Internal;

        public FlexiTypeDescriptor(ITypeDescription description)
        {
            this.Internal = description;
        }

        public ITypeDescription DescribeType(Type type)
        {
            return new FlexiTypeDescription(Internal);            
        }
    }

    public class FlexiTypeMemberDescription : ITypeMemberDescription
    {
        readonly ITypeMemberDescription Internal;

        public FlexiTypeMemberDescription(ITypeMemberDescription description)
        {
            this.Internal = description;
        }
        public string Name
        {
            get { return Internal.Name; }
        }

        public string AssemblyQualifiedMemberTypeName
        {
            get { return Internal.AssemblyQualifiedMemberTypeName; }
        }

        public string FullMemberTypeName
        {
            get { return Internal.FullMemberTypeName; }
        }

        public string MemberTypeName
        {
            get { return Internal.MemberTypeName; }
        }

        public MemberVisibility Visibility
        {
            get { return Internal.Visibility; }
        }

        public ITypeDescription DeclaringType
        {
            get { return Internal.DeclaringType; }
        }

        public bool CanSetValue
        {
            get { return Internal.CanSetValue; }
        }

        public bool CanGetValue
        {
            get { return Internal.CanGetValue; }
        }

        public object GetValue(object obj)
        {
            throw new NotImplementedException();
        }

        public void SetValue(object obj, object value)
        {
            var fmi = obj as FlexiMappedInstance;

            fmi.Value = value;
        }
    }

    public class FlexiTypeDescription : ITypeDescription
    {
        readonly ITypeDescription Internal;

        public FlexiTypeDescription(ITypeDescription typeDescription)
        {
            this.Internal = typeDescription;

            var members = new List<ITypeMemberDescription>();

            for(int i = 0; i < Internal.Members.Count; i++)
            {
                var md = Internal.Members[i];

                var new_md = new FlexiTypeMemberDescription(md);

                members.Add(new_md);
            }

            this.Members = members;
        }

        public string AssemblyQualifiedName
        {
            get { return Internal.AssemblyQualifiedName; }
        }

        public string FullName
        {
            get { return Internal.FullName; }
        }

        public string Name
        {
            get { return Internal.Name; }
        }

        public string Namespace
        {
            get { return Internal.Namespace; }
        }

        public IList<ITypeMemberDescription> Members { get; private set; }
    }
}
