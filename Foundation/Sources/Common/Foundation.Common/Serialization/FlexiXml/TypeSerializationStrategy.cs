using SquaredInfinity.Foundation.Types.Description;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace SquaredInfinity.Foundation.Serialization.FlexiXml
{
    public interface ISerializationResolver
    {
        XObject Resolve(ISerializationResolverContext cx);
    }

    public abstract class SerializationResolver : ISerializationResolver
    {
        public abstract XObject Resolve(ISerializationResolverContext cx);
    }

    public class TypeMemberSerializationResolver : SerializationResolver
    {
        ITypeMemberDescription TypeMemberDescription { get; set; }

        public TypeMemberSerializationResolver(ITypeMemberDescription typeMemberDescription)
        {
            this.TypeMemberDescription = typeMemberDescription;
        }

        public override XObject Resolve(ISerializationResolverContext cx)
        {
            // todo: how to handle nulls?
            if (cx.CurrentInstance == null)
                return null;

            var val = TypeMemberDescription.GetValue(cx.CurrentInstance);

            var val_as_string = (string)null;

            if (TryConvertToStringIfTypeSupports(cx.CurrentInstance, out val_as_string))
            {
                // member value supports converting to and from string
                //
                // => create attribute which will store member value

                // todo:    what if conversion to string produces text which isn't valid for attribute value?
                //          should it be stored in CDATA element instead?
                //          or perhaps escaped? or configurable?
                var attributeName = TypeMemberDescription.Name;

                var result = new XAttribute(attributeName, val_as_string);

                return result;
            }
            else
            {
                if (!TypeMemberDescription.CanSetValue && val is IList)
                {
                    // cannot set value of a member 
                    // (so the type does not matter, it will probably be initialized to correct type by object itself)
                    // and member is a list
                    //
                    // => create wrapper element for the list
                    // => serialize elements of the list

                    // list wrapper is an attached element with name: <parent_element_name.member_name>
                    // list wrapper for read-only property contains elements directly (as its children)

                    var wrapperElementName = cx.CurrentElement.Name + "." + TypeMemberDescription.Name;

                    var xe = cx.Serialize(val, wrapperElementName);

                    return xe;
                }
                else
                {
                    // member can be set (so we do care about the actual type of its value)
                    //
                    // => create a wrapper element for the member
                    // => create child element (child of the wrapper) which contains serialized data

                    // member wrapper is an attached element with name <parent_element_name.member_name>

                    var wrapperElementName = cx.CurrentElement.Name + "." + TypeMemberDescription.Name;

                    var wrapper = new XElement(wrapperElementName);

                    var xeName = cx.ConstructRootRelementForType(val.GetType());
                    
                    var el = cx.Serialize(val, xeName);

                    wrapper.Add(el);

                    return wrapper;
                }
            }
        }

        bool TryConvertToStringIfTypeSupports(object obj, out string result)
        {
            var typeConverter = System.ComponentModel.TypeDescriptor.GetConverter(obj);

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
    }

    public class TypeSerializationStrategy : ITypeSerializationStrategy
    {
        public Version Version { get; set; }

        public Type Type { get; set; }

        public Types.Description.ITypeDescription TypeDescription { get; set; }

        public Types.Description.ITypeDescriptor TypeDescriptor { get; set; }

        public TypeSerializationStrategy(Type type, Types.Description.ITypeDescriptor typeDescriptor)
        {
            this.Type = type;
            this.TypeDescriptor = typeDescriptor;

            this.TypeDescription = TypeDescriptor.DescribeType(type);

            Initialize();
        }

        void Initialize()
        {
            //# get all source members that can be serialized
            var serializableMembers =
                (from m in TypeDescription.Members
                 where
                    m.Visibility == MemberVisibility.Public // process public members only
                    &&
                    !m.IsExplicitInterfaceImplementation // do not process explicit interface implemntations (for now, may need to change this behavior later, main problem is how to support this?)
                    &&
                    m.CanGetValue // we must be able to get a value of a member to serialize it
                 select m);

            //# create default resolvers for each serializable member

            foreach (var m in serializableMembers)
            {
                var resolver = new TypeMemberSerializationResolver(m);
            }
        }

        public XElement Serialize(object instance, ITypeSerializationContext cx)
        {
            // get name 

            // process all resolvers

            //foreach (var m in serializableMembers)
            //{
            //    var val = m.GetValue(source);

            //    if (val == null) // todo: may need to specify a custom way of handling this
            //        continue;

            //    var val_as_string = (string)null;

            //    //if (m.CanSetValue)
            //    {
            //        if (TryConvertToStringIfTypeSupports(val, out val_as_string))
            //        {
            //            var attributeName = m.Name;

            //            target.Add(new XAttribute(attributeName, val_as_string)); // todo: do mapping if needed, + conversion
            //            continue;
            //        }
            //        else
            //        {
            //            {
            //                if (!m.CanSetValue && val is IList)
            //                {
            //                    var el = new XElement(target.Name + "." + m.Name);
            //                    SerializeInternal(el, val, options, cx);
            //                    target.Add(el);
            //                }
            //                else
            //                {
            //                    var wrapper = new XElement(target.Name + "." + m.Name);

            //                    var elName = ConstructRootElementForType(val.GetType());
            //                    var el = new XElement(elName);

            //                    SerializeInternal(el, val, options, cx);

            //                    wrapper.Add(el);

            //                    target.Add(wrapper);
            //                }
            //            }
            //        }
            //    }
            //    //else
            //    {

            //    }
            //}


            return new XElement("lol");
        }

        public object Deserialize(XElement element)
        {
            throw new NotImplementedException();
        }
    }

    public class TypeSerializationStrategy<T> : TypeSerializationStrategy, ITypeSerializationStrategy<T>
    {
        public TypeSerializationStrategy(Types.Description.ITypeDescriptor typeDescriptor)
            : base(typeof(T), typeDescriptor)
        {

        }

        public ITypeSerializationStrategy<T> IgnoreAllMembers()
        {
            return this;
        }


        public ITypeSerializationStrategy<T> IgnoreMember(System.Linq.Expressions.Expression<Func<object>> memberExpression)
        {
            return this;
        }
        
        public ITypeSerializationStrategy<T> ResolveReferenceWith<TypeToResolve>(Action<ReferenceResolutionContext<T, TypeToResolve>> resolveReference)
        {
            return this;
        }


        public ITypeSerializationStrategy<T> IgnoreMember(System.Linq.Expressions.Expression<Func<T>> memberExpression)
        {
            return this;
        }

        public ITypeSerializationStrategy<T> SerializeMember(System.Linq.Expressions.Expression<Func<T>> memberExpression)
        {
            return this;
        }
    }
}
