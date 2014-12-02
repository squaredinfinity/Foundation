using SquaredInfinity.Foundation.Serialization.FlexiXml;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace SquaredInfinity.Foundation.Serialization
{
    public interface IXmlSerializer
    {
        XElement Serialize(object obj);
        XElement Serialize(object obj, SerializationOptions options);

        XElement Serialize(object obj, string rootElementName);

        XElement Serialize(object obj, string rootElementName, SerializationOptions options);

        XElement Serialize<T>(IEnumerable<T> items, string rootElementName, Func<T, XElement> getItemElement);

        T Deserialize<T>(string xml);

        T Deserialize<T>(XDocument xml);

        T Deserialize<T>(XElement xml);


        ITypeSerializationStrategy GetTypeSerializationStrategy(Type type);
    }
}
