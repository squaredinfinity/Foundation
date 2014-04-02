using SquaredInfinity.Foundation.Types.Description;
using SquaredInfinity.Foundation.Types.Mapping;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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
            return base.CreateDefaultTypeMappingStrategy(sourceType, targetType);
        }
    }

    public class FlexiTypeMappingStrategy : ITypeMappingStrategy
    {
        readonly ITypeMappingStrategy InternalStrategy;

        public FlexiTypeMappingStrategy(ITypeMappingStrategy strategy)
        {
            this.InternalStrategy = strategy;
        }

        public ITypeDescription SourceTypeDescription
        {
            get { return InternalStrategy.SourceTypeDescription; }
        }

        public ITypeDescription TargetTypeDescription
        {
            // todo: this should be a custom implementation of ITypeDescriptor just for Flexi Serializer
            get { return InternalStrategy.SourceTypeDescription; }
        }

        public bool CloneListElements
        {
            get { return InternalStrategy.CloneListElements; }
        }

        public bool TryGetValueResolverForMember(string memberName, out Types.Mapping.ValueResolving.IValueResolver valueResolver)
        {
            return InternalStrategy.TryGetValueResolverForMember(memberName, out valueResolver);
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
            throw new NotImplementedException();
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
