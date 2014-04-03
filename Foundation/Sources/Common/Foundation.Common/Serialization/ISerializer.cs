using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace SquaredInfinity.Foundation.Serialization
{
    public interface ISerializer
    {
        XDocument Serialize(object obj);

        T Deserialize<T>(XDocument xml);
    }
}
