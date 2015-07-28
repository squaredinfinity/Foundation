using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SquaredInfinity.Foundation.Serialization.FlexiXml.TestEntities
{
    public class TypeWithCollectionProperty
    {
        List<int> _items = new List<int>();

        public List<int> Items
        {
            get { return _items; }
            set { _items = value; }
        }
    }
}
