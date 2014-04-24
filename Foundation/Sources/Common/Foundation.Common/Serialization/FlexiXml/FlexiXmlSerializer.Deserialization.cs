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

            target = Activator.CreateInstance(targetType, nonPublic: true);

            var id_attrib = xml.Attribute(UniqueIdAttributeName);

            if(id_attrib != null)
            {
                var id = Int64.Parse(id_attrib.Value);

                cx.Objects_InstanceIdTracker.AddOrUpdate(new InstanceId(id), target);
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

                var value = (object)null;

                if (TryConvertFromStringIfTypeSupports(attribute.Value, memberType, out value))
                {
                    member.SetValue(target, value);
                }
            }

            foreach (var el in xml.Elements())
            {
                var member =
                    (from f in typeDescription.Members
                     where f.Name == el.Name
                     && f.CanSetValue
                     && f.CanGetValue
                     select f).FirstOrDefault();

                var memberType = Type.GetType(member.AssemblyQualifiedMemberTypeName);

                var value = DeserializeInternal(memberType, el, typeDescriptor, options, cx);

                member.SetValue(target, value);
            }

            return target;
        }
    }
}
