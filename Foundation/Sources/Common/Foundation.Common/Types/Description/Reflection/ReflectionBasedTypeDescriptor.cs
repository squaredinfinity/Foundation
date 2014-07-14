using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using SquaredInfinity.Foundation.Extensions;

namespace SquaredInfinity.Foundation.Types.Description.Reflection
{
    public class ReflectionBasedTypeDescriptor : ITypeDescriptor
    {
        static readonly Type TYPE_RuntimePropertyInfo;
        static readonly PropertyInfo PROPERTY_BindingFlags;

        static readonly Type TYPE_RuntimeFieldInfo;
        static readonly PropertyInfo FIELD_BindingFlags;

        static ReflectionBasedTypeDescriptor()
        {
            TYPE_RuntimePropertyInfo = typeof(PropertyInfo).Assembly.GetType("System.Reflection.RuntimePropertyInfo");
            PROPERTY_BindingFlags = TYPE_RuntimePropertyInfo.GetProperty("BindingFlags", BindingFlags.NonPublic | BindingFlags.Instance);

            TYPE_RuntimeFieldInfo = typeof(FieldInfo).Assembly.GetType("System.Reflection.RuntimeFieldInfo");
            FIELD_BindingFlags = TYPE_RuntimeFieldInfo.GetProperty("BindingFlags", BindingFlags.NonPublic | BindingFlags.Instance);
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

            var properties =
                (from p in type.GetProperties(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public)
                 where p.GetIndexParameters().IsNullOrEmpty()
                 select p).ToArray();

            var fields = new List<FieldInfo>().ToArray();
            // todo: develop customizable filters which can be used to determine if field should be included or now
            // those should work in later part (down below) after member description is created but before it is added
                //(from f in type.GetFields(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public)
                // where !f.Name.EndsWith("_backingfield", StringComparison.InvariantCultureIgnoreCase)
                // select f).ToArray();
            
            var prototypeMembers = new TypeMemberDescriptionCollection();

            for (int i = 0; i < fields.Length; i++)
            {
                var f = fields[i];

                var md = new ReflectionBasedTypeMemberDescription(f);

                var member_type = f.FieldType;

                var memberTypeDescription = (ITypeDescription)null;

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

                md.Name = f.Name;
                md.SanitizedName = f.Name;

                // todo: handle readonly fields
                md.CanGetValue = true;
                md.CanSetValue = true;

                md.Visibility = GetMemberVisibility(f);

                md.DeclaringType = prototype;

                prototypeMembers.Add(md);
            }

            for (int i = 0; i < properties.Length; i++)
            {
                var p = properties[i];

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

                var explicit_interface_separator_index = p.Name.IndexOf(".");

                if (explicit_interface_separator_index != -1)
                {
                    // this is an explicit interface implementation

                    md.IsExplicitInterfaceImplementation = true;
                    md.Name = p.Name;

                    //var interface_name = p.Name.Substring(0, explicit_interface_separator_index);

                    //var mapp

                    //foreach(var interf in member_type.GetInterfaces())
                    //{
                    //    foreach(var interf_property in interf.GetProperties())
                    //    {
                    //        var isMatch =
                    //            p.Name == interf_property.Name
                    //            && member_type == interf_property.PropertyType
                                
                    //    }
                    //}
                    
                }
                else
                {
                    md.Name = p.Name;
                }

                md.SanitizedName = p.Name;

                md.CanGetValue = p.CanRead;
                md.CanSetValue = p.CanWrite;

                md.Visibility = GetMemberVisibility(p);

                md.DeclaringType = prototype;

                prototypeMembers.Add(md);
            }

            prototype.Members = prototypeMembers;
        }

        MemberVisibility GetMemberVisibility(PropertyInfo pi)
        {
            if (((BindingFlags)PROPERTY_BindingFlags.GetValue(pi, null)).IsFlagSet(BindingFlags.Public))
                return MemberVisibility.Public;

            return MemberVisibility.NonPublic;
        }

        MemberVisibility GetMemberVisibility(FieldInfo fi)
        {
            if (((BindingFlags)FIELD_BindingFlags.GetValue(fi, null)).IsFlagSet(BindingFlags.Public))
                return MemberVisibility.Public;

            return MemberVisibility.NonPublic;
        }
    }
}
