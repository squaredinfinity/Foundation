using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using SquaredInfinity.Foundation.Extensions;
using System.Collections;

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

                    // create uninitialized version of type description (a prototype)
                    // and finish initialization later
                    // this is to avoid recursive initialization if some property somewhere deep inside type hierarchy is of this type

                    if (type.ImplementsInterface(typeof(IEnumerable)))
                    {
                        return new ReflectionBasedEnumerableTypeDescription();
                    }
                    else
                    {
                        return new ReflectionBasedTypeDescription();
                    }
                });

            if(isNew)
                memberTypeDescription = DescribeTypeInternal(type, memberTypeDescription as ReflectionBasedTypeDescription);

            return memberTypeDescription;
        }

        protected virtual ITypeDescription DescribeTypeInternal(Type type, ReflectionBasedTypeDescription typeDescription)
        {
            if (type.ImplementsInterface(typeof(IEnumerable)))
            {
                var enumerableTypeDescription = typeDescription as ReflectionBasedEnumerableTypeDescription;

                typeDescription = enumerableTypeDescription;

                var compatibleItemTypes = type.GetCompatibleItemTypes();

                foreach(var t in compatibleItemTypes)
                    enumerableTypeDescription.AddCompatibleItemType(t);

                // find min concrete type which could be used as an item
                enumerableTypeDescription.DefaultConcreteItemType =
                    (from t in compatibleItemTypes
                     where !t.IsInterface && !t.IsAbstract
                     select t).FirstOrDefault();

                var capacityProperty = type.GetProperty("Capacity");

                if (capacityProperty != null && capacityProperty.CanWrite)
                {
                    enumerableTypeDescription.CanSetCapacity = true;
                    enumerableTypeDescription.CapacityPropertyInfo = capacityProperty;
                }
            }

            typeDescription.AssemblyQualifiedName = type.AssemblyQualifiedName;
            typeDescription.FullName = type.FullName;
            typeDescription.Name = type.Name;
            typeDescription.Namespace = type.Namespace;
            typeDescription.Type = type;

            typeDescription.IsValueType = type.IsValueType;
            
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
            
            // assume that all members are immutable
            // change to false if any field or property is of immutable type
            bool areAllMembersImmutable = true;

            var prototypeMembers = new TypeMemberDescriptionCollection();

            for (int i = 0; i < fields.Length; i++)
            {
                var f = fields[i];

                var md = new ReflectionBasedTypeMemberDescription(f);

                var member_type = f.FieldType;

                if(!member_type.IsValueType)
                    areAllMembersImmutable = false;

                var memberTypeDescription = (ITypeDescription)null;

                if (member_type == type)
                {
                    memberTypeDescription = typeDescription;
                }
                else
                {
                    memberTypeDescription = DescribeType(member_type);
                }

                md.AssemblyQualifiedMemberTypeName = memberTypeDescription.AssemblyQualifiedName;
                md.MemberType = memberTypeDescription;

                md.Name = f.Name;
                md.SanitizedName = f.Name;

                // todo: handle readonly fields
                md.CanGetValue = true;
                md.CanSetValue = true;

                md.Visibility = GetMemberVisibility(f);

                md.DeclaringType = typeDescription;

                prototypeMembers.Add(md);
            }

            for (int i = 0; i < properties.Length; i++)
            {
                var p = properties[i];

                var md = new ReflectionBasedTypeMemberDescription(p);

                var member_type = p.PropertyType;

                if(!member_type.IsValueType)
                    areAllMembersImmutable = false;

                var memberTypeDescription = (ITypeDescription) null;

                if (member_type == type)
                {
                    memberTypeDescription = typeDescription;
                }
                else
                {
                    memberTypeDescription = DescribeType(member_type);
                }

                md.MemberType = memberTypeDescription;

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

                md.DeclaringType = typeDescription;

                prototypeMembers.Add(md);
            }

            typeDescription.Members = prototypeMembers;

            typeDescription.AreAllMembersImmutable = areAllMembersImmutable;

            return typeDescription;
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
