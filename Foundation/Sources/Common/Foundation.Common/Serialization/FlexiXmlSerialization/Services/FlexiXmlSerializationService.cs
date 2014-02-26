using SquaredInfinity.Foundation.Serialization.Services;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace SquaredInfinity.Foundation.Serialization.FlexiXmlSerialization.Services
{
    /// <summary>
    /// Allows easy serialization to xml of objects without marking them with serialization attributes.
    /// This approach allows to decoupling of application entities/types from serialization process (types are serialization-agnostic)
    /// </summary>
    public partial class FlexiXmlSerializationService : ISerializationService
    {
        //ILogger Diagnostics = InternalDiagnosticLogger.CreateLoggerForType<FlexiXmlSerializationService>();

        public event EventHandler<ConvertStringToPropertyValueEventArgs> TryConvertStringToPropertyValue;

        public event EventHandler<ResolveElementReferenceEventArgs> TryResolveElementReference;

        /// <summary>
        /// Name of the attribute which will be used to store type information for element
        /// </summary>
        static readonly string AttributeName_Type = "type";

        static readonly IEnumerable<string> ReservedAttributeNames = new string[] 
        {
            AttributeName_Type
        };

        List<string> _typeLookupNamespaces = new List<string>();
        /// <summary>
        /// List of assemly-qualified namespaces where to look for types (if *type* attribute not present on xml element)
        /// </summary>
        public List<string> TypeLookupNamespaces
        {
            get { return _typeLookupNamespaces; }
        }

        Dictionary<string, Type> _knownTypes = new Dictionary<string, Type>();
        /// <summary>
        /// List of known types
        /// </summary>
        public Dictionary<string, Type> KnownTypes
        {
            get { return _knownTypes; }
        }

        public FlexiXmlSerializationService()
        { }

        public Stream Serialize(object obj)
        {
            throw new NotSupportedException();
        }

        public T Deserialize<T>(Stream stream)
        {
            var doc = XDocument.Load(stream, LoadOptions.SetLineInfo);

            return Deserialize<T>(doc);
        }

        public T Deserialize<T>(string xml)
        {
            var doc = XDocument.Parse(xml, LoadOptions.SetLineInfo);

            return Deserialize<T>(doc);
        }

        public T Deserialize<T>(XDocument doc)
        {
            return Deserialize<T>(doc.Root);
        }

        public T Deserialize<T>(XElement el)
        {
            var tType = typeof(T);
            var tName = tType.Name;

            //# add requested type to known types list
            if (!KnownTypes.ContainsKey(tName))
                KnownTypes.Add(tName, tType);

            //# add namespace of requested type to lookup (so it will be used to resolve other types in a future)
            if (!TypeLookupNamespaces.Contains(tType.Namespace))
                TypeLookupNamespaces.Add(tType.Namespace);

            var result = ProcessRootElement(el);

            return (T)result;
        }


        object ProcessRootElement(XElement el)
        {
            return ProcessElement(deserializationRootObject: null, el: el);
        }

        object ProcessElement(object deserializationRootObject, XElement el, Type elementType = null)
        {
            // TYPE RESOLUTION
            //  - if elementType != null => use elementType
            //  - if element contains type="xxx" attribute => use that attribute value
            //  - check known types
            //  - check known lookup namespaces

            var type = elementType;

            if (type == null)
            {
                //# check if element has type attribute
                // e.g. <MyThing type="MyProject.MyThing, MyProjectAssembly" />

                var typeName = el.GetAttributeValue(AttributeName_Type);

                if (typeName != null)
                {
                    type = Type.GetType(
                        typeName,
                        TypeExtensions.LastHopeAssemblyResolver,
                        TypeExtensions.LastHopeTypeResolver,
                        throwOnError: false,
                        ignoreCase: true);
                }
                else
                {
                    //# check if element name represents a known type
                    foreach (var kvp in KnownTypes)
                    {
                        if (kvp.Key == el.Name.LocalName)
                        {
                            type = kvp.Value;
                            break;
                        }
                    }

                    if (type == null)
                    {
                        //# check if element name represents actual type from lookup namespaces
                        foreach (var ns in TypeLookupNamespaces)
                        {
                            type = Type.GetType("{0}.{1}".FormatWith(ns, el.Name.LocalName));

                            if (type != null)
                                break;
                        }
                    }
                }
            }

            if (type == null)
                throw new ApplicationException("Unable to find type for xml element {0}".FormatWith(el.GetNameAndLocationForDebugging()));

            object instance = null;

            if (type.IsInterface || type.IsAbstract && el.Elements().Count() == 1)
            {
                instance = ProcessElement(deserializationRootObject, el.Elements().Single());
                return instance;
            }
            else
            {
                var attributes = el.Attributes().ToArray();

                if (attributes.Length == 1 && attributes[0].Name.LocalName == "ref")
                {
                    instance = ProcessRefElement(deserializationRootObject, el, attributes[0]);
                    return instance;
                }
                else
                {
                    instance = Activator.CreateInstance(type, nonPublic: true);
                }
            }

            if (deserializationRootObject == null)
                deserializationRootObject = instance;

            ProcessAttributes(deserializationRootObject, el, type, instance);
            ProcessPropertyElements(deserializationRootObject, el, type, instance);

            if (type.ImplementsInterface<IList>())
            {
                var list = instance as IList;

                foreach (var child in el.Elements())
                {
                    var childInstance = ProcessElement(deserializationRootObject, child);

                    list.Add(childInstance);
                }
            }
            else
            {
                foreach (var child in el.Elements())
                {
                    var name = child.Name.LocalName;

                    var propertyName = name;

                    //# check if object has property of that name (with setter)
                    var property = type.GetProperty(
                        propertyName,
                        BindingFlags.Instance
                        | BindingFlags.Public
                        | BindingFlags.NonPublic);

                    if (property == null)
                    {
                        //Diagnostics.Log.Warning(() =>
                        //        "Unable to find property {0} on type {1} ({2})"
                        //        .FormatWith(propertyName, type.FullName, child.GetNameAndLocationForDebugging()));

                        continue;
                    }

                    if (property.GetSetMethod() == null)
                    {
                        //Diagnostics.Log.Warning(() =>
                        //        "Skipping readonly property {0} on type {1} ({2})"
                        //        .FormatWith(propertyName, type.FullName, child.GetNameAndLocationForDebugging()));

                        continue;
                    }

                    object value = ProcessElement(deserializationRootObject, child, property.PropertyType);

                    property.SetValue(instance, value, index: null);
                }
            }

            return instance;
        }

        private object ProcessRefElement(object deserializationRootObject, XElement el, XAttribute refAttribute)
        {
            object result = null;

            if (!OnTryResolveElementReference(deserializationRootObject, el.Name.LocalName, refAttribute.Value, out result))
            {
                throw new System.Xml.XmlException(
                    "Unable to find referenced element: <{0} ref=\"{1}\" />"
                    .FormatWith(el.Name.LocalName, refAttribute.Value));
            }

            return result;
        }

        /// <summary>
        /// Processes Property Elements (<Parent.XXX>value</Parent.XXX>)
        /// </summary>
        /// <param name="deserializationRootObject"></param>
        /// <param name="el"></param>
        /// <param name="type"></param>
        /// <param name="instance"></param>
        private void ProcessPropertyElements(object deserializationRootObject, XElement el, Type type, object instance)
        {
            foreach (var child in el.Elements())
            {
                var name = child.Name.LocalName;

                //# check if is property element
                if (!name.StartsWith(el.Name.LocalName))
                {
                    continue;
                }

                var propertyName = name.Substring(name.LastIndexOf('.') + 1);

                //# check if object has property of that name (with setter)
                var property = type.GetProperty(
                    propertyName,
                    BindingFlags.Instance
                    | BindingFlags.Public
                    | BindingFlags.NonPublic);

                if (property == null)
                {
                    //Diagnostics.Log.Warning(() =>
                    //        "Unable to find property {0} on type {1} ({2})"
                    //        .FormatWith(propertyName, type.FullName, child.GetNameAndLocationForDebugging()));

                    continue;
                }

                bool isCollection =
                    typeof(IList).IsAssignableFrom(property.PropertyType);

                if (!isCollection && property.GetSetMethod() == null)
                {
                    //Diagnostics.Log.Warning(() =>
                    //        "Skipping readonly property {0} on type {1} ({2})"
                    //        .FormatWith(propertyName, type.FullName, child.GetNameAndLocationForDebugging()));

                    continue;
                }

                if (isCollection)
                {
                    var collection = property.GetValue(instance, new object[] { }) as IList;

                    //# shouldClear control attibure

                    bool shouldClear = false;

                    var shouldClearAttributeValue =
                        child.GetAttributeValue(
                        XName.Get("{http://schemas.squaredinfinity.com/flexiXmlSerialization}clear"),
                        "false");

                    bool.TryParse(shouldClearAttributeValue, out shouldClear);

                    if (shouldClear)
                        collection.Clear();

                    //# add elements to collection
                    foreach (var childElement in child.Elements())
                    {
                        var collectionItem = ProcessElement(deserializationRootObject, childElement);

                        collection.Add(collectionItem);
                    }

                    //# onInitialzied control attribute

                    var onInitializedAttributeValue =
                        child.GetAttributeValue(
                        XName.Get("{http://schemas.squaredinfinity.com/flexiXmlSerialization}onInitialized"),
                        null);

                    if (!onInitializedAttributeValue.IsNullOrEmpty())
                    {
                        var methodInfo = type.GetMethod(onInitializedAttributeValue);

                        if (methodInfo == null || methodInfo.GetParameters().Length != 0)
                        {
                            //Diagnostics.Log.Warning(() =>
                            //    "onInitialize method should not contain any parameters. Location: {0}"
                            //    .FormatWith(child.GetNameAndLocationForDebugging()));
                        }
                        else
                        {
                            methodInfo.Invoke(instance, null);
                        }
                    }
                }
                else
                {
                    //# set value of a property

                    if (child.Value.IsNullOrEmpty() && child.Elements().Count() == 1)
                    {
                        // no string value present.
                        // value may be expressed as more complex element

                        var value = ProcessElement(deserializationRootObject, child.Elements().Single(), property.PropertyType);
                        property.SetValue(instance, value, index: null);
                    }
                    else
                    {
                        // simple string to (actual) value conversion

                        object value = ConvertToPropertyTypeObject(deserializationRootObject, property, child.Value);
                        property.SetValue(instance, value, index: null);
                    }
                }
            }
        }

        void ProcessAttributes(object deserializationRootObject, XElement el, Type instanceType, object instance)
        {
            foreach (var attrib in el.Attributes())
            {
                if (attrib.Name.LocalName.IsIn(ReservedAttributeNames))
                    continue;

                var attribName = attrib.Name.LocalName;
                string valueAsString = attrib.Value;

                var propertyName = attribName;
                if (attribName.EndsWith("-ref"))
                    propertyName = attribName.Substring(0, attribName.Length - "-ref".Length);

                //# check if object has property of that name (with setter)
                var property = instanceType.GetProperty(
                    propertyName,
                    BindingFlags.Instance
                    | BindingFlags.Public
                    | BindingFlags.NonPublic
                    | BindingFlags.IgnoreCase);

                if (property == null)
                {
                    //Diagnostics.Log.Warning(() =>
                    //           "Unable to map attribute to property {0} on type {1}. Location: {0}"
                    //           .FormatWith(
                    //           attribName,
                    //           instanceType.FullName,
                    //           el.GetNameAndLocationForDebugging()));
                    continue;
                }

                if (property.GetSetMethod() == null)
                    continue;

                //# check if this is a -ref attribute
                if (attribName.EndsWith("-ref"))
                {
                    object result;

                    var referenceType = attribName.Substring(0, attribName.Length - "-ref".Length);

                    if (!OnTryResolveElementReference(deserializationRootObject, referenceType, valueAsString, out result))
                    {
                        throw new System.Xml.XmlException(
                            "Unable to find referenced element: {0}={1}"
                            .FormatWith(attribName, valueAsString));
                    }

                    property.SetValue(instance, result, index: null);
                }
                else
                {
                    var value = ConvertToPropertyTypeObject(deserializationRootObject, property, valueAsString);

                    property.SetValue(instance, value, index: null);
                }
            }
        }

        object ConvertToPropertyTypeObject(object deserializationRootObject, PropertyInfo pi, string valueAsString)
        {
            object result;

            //# try using custom converters to get value
            if (OnTryConvertStringToPropertyValue(deserializationRootObject, pi, valueAsString, out result))
            {
                return result;
            }

            if (pi.PropertyType.IsEnum)
            {
                return ConvertToPropertyTypeObjectFromEnum(pi.PropertyType, valueAsString);
            }
            else
            {
                return Convert.ChangeType(valueAsString, pi.PropertyType);
            }
        }

        bool OnTryResolveElementReference(
            object deserializationRootObject,
            string referenceType,
            string valueAsString,
            out object result)
        {
            var args =
                new ResolveElementReferenceEventArgs(
                    deserializationRootObject,
                    referenceType.ToLower(),
                    valueAsString.ToLower());

            if (TryResolveElementReference.TryHandle(args))
            {
                result = args.Result;
                return true;
            }

            result = null;
            return false;
        }

        bool OnTryConvertStringToPropertyValue(
            object deserializationRootObject,
            PropertyInfo pi,
            string valueAsString,
            out object result)
        {
            var args =
                new ConvertStringToPropertyValueEventArgs(
                    deserializationRootObject,
                    pi.DeclaringType,
                    pi,
                    valueAsString);

            if (TryConvertStringToPropertyValue.TryHandle(args))
            {
                result = args.Result;
                return true;
            }

            result = null;
            return false;
        }

        object ConvertToPropertyTypeObjectFromEnum(Type enumType, string value)
        {
            return Enum.Parse(enumType, value);
        }

    }
}
