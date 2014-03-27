using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SquaredInfinity.Foundation.Types.Description.Reflection
{
    public class ReflectionBasedTypeDescriptor : ITypeDescriptor
    {
        readonly ConcurrentDictionary<string, ITypeDescription> TypeDescriptionCache = new ConcurrentDictionary<string, ITypeDescription>();

        public ITypeDescription DescribeType(Type type)
        {
            var td = new TypeDescription();

            td.AssemblyQualifiedName = type.AssemblyQualifiedName;
            td.FullName = type.FullName;
            td.Name = type.Name;
            td.Namespace = type.Namespace;

            var ps = type.GetProperties();

            for (int i = 0; i < ps.Length; i++)
            {
                var p = ps[i];

                var md = new TypeMemberDescription();

                var member_type = p.PropertyType;

                var typeDescription = TypeDescriptionCache.GetOrAdd(member_type.AssemblyQualifiedName, (_) => DescribeType(member_type));

                md.AssemblyQualifiedMemberTypeName = typeDescription.AssemblyQualifiedName;
                md.FullMemberTypeName = typeDescription.FullName;
                md.MemberTypeName = typeDescription.Name;

                md.RawName = member_type.Name;
                md.SanitizedName = member_type.Name;

                td.Members.Add(md);
            }

            return td;
        }
    }
}
