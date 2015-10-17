using SquaredInfinity.Foundation.Serialization.FlexiXml;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace SquaredInfinity.Foundation.Serialization.FlexiXml
{
    public interface IFlexiXmlSerializer : IFlexiSerializer, ISerializer
    {
        XElement Serialize(object obj);
        XElement Serialize(object obj, FlexiXmlSerializationOptions options);

        XElement Serialize(object obj, string rootElementName);

        XElement Serialize(object obj, string rootElementName, FlexiXmlSerializationOptions options);

        XElement Serialize<T>(IEnumerable<T> items, string rootElementName, Func<T, XElement> getItemElement);

        T Deserialize<T>(string xml);

        T Deserialize<T>(XDocument xml);

        T Deserialize<T>(XElement xml);
    }
}
