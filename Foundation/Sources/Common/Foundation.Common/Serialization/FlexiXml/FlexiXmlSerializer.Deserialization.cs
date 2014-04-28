using SquaredInfinity.Foundation.Types.Mapping;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using SquaredInfinity.Foundation.Extensions;
using System.Collections.Concurrent;
using SquaredInfinity.Foundation.Types.Description.Reflection;
using SquaredInfinity.Foundation.Types.Description;
using System.Threading;
using System.ComponentModel;
using System.Collections;

namespace SquaredInfinity.Foundation.Serialization.FlexiXml
{
    public partial class FlexiXmlSerializer
    {
        public T Deserialize<T>(XDocument xml)
        {
            return Deserialize<T>(xml.Root);
        }

        public T Deserialize<T>(XElement xml)
        {
            var cx = new DeserializationContext();

            var type = typeof(T);

            var root = DeserializeInternal(type, xml, new ReflectionBasedTypeDescriptor(), new SerializationOptions(), cx);

            return (T)root;
        }

        object DeserializeInternal(Type targetType, XElement xml, ITypeDescriptor typeDescriptor, SerializationOptions options, DeserializationContext cx)
        {
            var typeDescription = typeDescriptor.DescribeType(targetType);

            object target = (object)null;

            // check serialization control attributes first

            var id_ref_attrib = xml.Attribute(UniqueIdReferenceAttributeName);

            if(id_ref_attrib != null)
            {
                var id = Int64.Parse(id_ref_attrib.Value);

                if (cx.Objects_InstanceIdTracker.TryGetValue(new InstanceId(id), out target))
                {
                    return target;
                }
                else
                {
                    // todo: log, this should always resolve unless serialization xml is corrupted
                    return null;
                }
            }


            if(!targetType.IsValueType && !TryCreateInstace(targetType, new CreateInstanceContext(), out target))
            {
                //todo: log error
                return null;
            }            

            var id_attrib = xml.Attribute(UniqueIdAttributeName);

            if(id_attrib != null)
            {
                var id = Int64.Parse(id_attrib.Value);

                cx.Objects_InstanceIdTracker.AddOrUpdate(new InstanceId(id), target);
            }

            var value = (object)null;

            if (xml.Value != null && TryConvertFromStringIfTypeSupports(xml.Value, targetType, out value))
            {
                return value;
            }

            foreach (var attribute in xml.Attributes())
            {
                var member =
                    (from f in typeDescription.Members
                     where f.Name == attribute.Name
                     && f.CanSetValue 
                     && f.CanGetValue
                     select f).FirstOrDefault();

                if (member == null)
                    continue;

                var memberType = Type.GetType(member.AssemblyQualifiedMemberTypeName);

                value = (object)null;

                if (TryConvertFromStringIfTypeSupports(attribute.Value, memberType, out value))
                {
                    member.SetValue(target, value);
                }
            }

            foreach (var el in xml.Elements())
            {
                var member =
                    (from m in typeDescription.Members
                     let memberType = Type.GetType(m.AssemblyQualifiedMemberTypeName)
                     where m.Name == el.Name.LocalName
                        &&
                        (
                            (m.CanGetValue && m.CanSetValue) // can set and get value
                            ||
                            (m.CanGetValue && memberType.ImplementsInterface<IEnumerable>()) // cannot set value, but it is a collection
                        )
                     select m).FirstOrDefault();

                if (member == null)
                    continue;

                var mrType = Type.GetType(member.AssemblyQualifiedMemberTypeName);

                if (!member.CanSetValue && mrType.ImplementsInterface<IList>())
                {
                    var targetList = member.GetValue(target) as IList;

                    if(targetList == null)
                        return null;

                    var targetListItemTypes = targetList.GetItemsTypes();

                    foreach (var itemXml in el.Elements())
                    {
                        // todo: check if type attribute has been set, if not, try to deduce item name from element name
                        
                        if(false) // todo: if type attribute exists
                        {
                            // check type attribute
                        }
                        else
                        {
                            var itemElementName = itemXml.Name.LocalName;

                            // try to find type of that name in all assemblies
                            // the type should be compatible with the

                            var itemType = 
                                TypeExtensions.ResolveType(
                                itemElementName, 
                                ignoreCase: true,
                                baseTypes:targetListItemTypes);

                            if(itemType == null)
                            {
                                // todo: log
                                continue;
                            }

                            var item = DeserializeInternal(itemType, itemXml, typeDescriptor, options, cx);

                            if (item == null)
                                continue;

                            targetList.Add(item);
                        }
                    }
                }
                else
                {

                    if (!el.Value.IsNullOrEmpty())
                    {
                        throw new NotImplementedException();
                        // not supported at the moment
                        // for element to have value, mapped member would need to be convertable to string
                        // but members convertable to string are serialized to attributes by default
                    }
                    else
                    {
                        // actual serialization is in child element

                        var contentEl = el.Elements().FirstOrDefault();

                        if (contentEl != null)
                        {
                            var typeAttribute = contentEl.Attribute(TypeAttributeName);

                            var elementType = (Type) null;

                            if (typeAttribute != null)
                            {
                                elementType =
                                    TypeExtensions.ResolveType(
                                    typeAttribute.Value, 
                                    ignoreCase:true);
                            }
                            else
                            {
                                elementType =
                                        TypeExtensions.ResolveType(
                                        contentEl.Name.LocalName,
                                        ignoreCase: true,
                                        baseTypes: new List<Type> { mrType });
                            }


                            if (elementType == null)
                            {
                                // type could not be resolver
                                // todo: log
                                continue;
                            }

                            value = DeserializeInternal(elementType, contentEl, typeDescriptor, options, cx);

                            member.SetValue(target, value);
                        }
                    }
                }
            }

            return target;
        }

        protected virtual Type ResolveType(string fullOrPartialTypeName, IReadOnlyList<Type> baseTypes, SerializationOptions options)
        {
            var t =
                TypeExtensions.ResolveType(
                fullOrPartialTypeName,
                ignoreCase: true,
                baseTypes: baseTypes);

            if(t == null)
            {

            }

            return null;
        }
    }
}
