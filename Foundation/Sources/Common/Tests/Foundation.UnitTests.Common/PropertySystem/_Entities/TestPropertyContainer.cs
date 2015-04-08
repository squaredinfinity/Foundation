using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SquaredInfinity.Foundation.PropertySystem._Entities
{
    public class TestPropertyContainer : ExtendedPropertyContainer
    {
        public string Name { get; set; }

        public IExtendedProperty<string> ToolTip { get; private set; }

        public IExtendedProperty<int> Count { get; private set; }

        public CollectionExtendedProperty<int> Items { get; private set; }

        public TestPropertyContainer()
        {
            ToolTip = ExtendedProperties.RegisterProperty<string>("ToolTip", () => "[UNSET]");
            Count = ExtendedProperties.RegisterProperty<int>("Count", () => 69);
            Items = ExtendedProperties.RegisterCollectionProperty("Items", () => new Collection<int>());
        }
    }
}
