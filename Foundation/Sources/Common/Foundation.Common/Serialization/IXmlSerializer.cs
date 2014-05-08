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

        XElement Serialize<T>(IEnumerable<T> items, string rootElementName, Func<T, XElement> getItemElement);

        T Deserialize<T>(XDocument xml);

        T Deserialize<T>(XElement xml);
    }
}
