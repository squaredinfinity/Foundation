using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SquaredInfinity.Foundation.Serialization.FlexiXml.TestEntities
{
    public class TypeWithNonGenericCollectionProperty
    {
        NonGenericCollection _items = new NonGenericCollection();

        public NonGenericCollection Items
        {
            get { return _items; }
            set { _items = value; }
        }
    }
}
