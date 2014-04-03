﻿using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace SquaredInfinity.Foundation.Types.Description.Reflection
{
    public class ReflectionBasedTypeDescriptor : ITypeDescriptor
    {
        static readonly Type TYPE_RuntimeMemberInfo;
        static readonly PropertyInfo PROPERTY_BindingFlags;

        static ReflectionBasedTypeDescriptor()
        {
            TYPE_RuntimeMemberInfo = typeof(PropertyInfo).Assembly.GetType("System.Reflection.RuntimePropertyInfo");
            PROPERTY_BindingFlags = TYPE_RuntimeMemberInfo.GetProperty("BindingFlags", BindingFlags.NonPublic | BindingFlags.Instance);
        }

        readonly ConcurrentDictionary<string, ITypeDescription> TypeDescriptionCache = new ConcurrentDictionary<string, ITypeDescription>();

        public ITypeDescription DescribeType(Type type)
        {
            bool isNew = false;

            var memberTypeDescription =
                TypeDescriptionCache.GetOrAdd(
                type.AssemblyQualifiedName,
                (_) =>
                {
                    isNew = true;
                    return new TypeDescription();
                });

            if (isNew)
                DescribeTypeInternal(type, (TypeDescription)memberTypeDescription);

            return memberTypeDescription;
        }

        void DescribeTypeInternal(Type type, TypeDescription prototype)
        {
            prototype.AssemblyQualifiedName = type.AssemblyQualifiedName;
            prototype.FullName = type.FullName;
            prototype.Name = type.Name;
            prototype.Namespace = type.Namespace;

            var ps = type.GetProperties();

            for (int i = 0; i < ps.Length; i++)
            {
                var p = ps[i];

                var md = new ReflectionBasedTypeMemberDescription(p);

                var member_type = p.PropertyType;

                var memberTypeDescription = (ITypeDescription) null;

                if (member_type == type)
                {
                    memberTypeDescription = prototype;
                }
                else
                {
                    memberTypeDescription = DescribeType(member_type);
                }

                md.AssemblyQualifiedMemberTypeName = memberTypeDescription.AssemblyQualifiedName;
                md.FullMemberTypeName = memberTypeDescription.FullName;
                md.MemberTypeName = memberTypeDescription.Name;

                md.Name = p.Name;
                md.SanitizedName = p.Name;

                md.CanGetValue = p.CanRead;
                md.CanSetValue = p.CanWrite;

                md.Visibility = GetMemberVisibility(p);

                md.DeclaringType = prototype;

                prototype.Members.Add(md);
            }
        }

        MemberVisibility GetMemberVisibility(PropertyInfo pi)
        {
            if ((BindingFlags)PROPERTY_BindingFlags.GetValue(pi, null) == BindingFlags.Public)
                return MemberVisibility.Public;

            return MemberVisibility.NonPublic;
        }
    }
}
