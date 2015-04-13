using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SquaredInfinity.Foundation.PropertySystem._Entities
{
    public class TestContainer : ExtendedPropertyContainer
    {
        public string Name { get; set; }

        public static readonly IExtendedPropertyDefinition<int> CountProperty =
            new ExtendedPropertyDefinition<int>(
                "TestContainer",
                "Count",
                () => 69);

        public int Count
        {
            get { return CountProperty.GetActualValue(this); }
            set { CountProperty.SetValue(this, value); }
        }

        public static readonly ExtendedCollectionPropertyDefinition<int> ItemsProperty =
            new ExtendedCollectionPropertyDefinition<int>(
                "TestContainer",
                "Items",
                () => new Collection<int> { 1, 2, 3 });

        public IList<int> Items
        {
            get { return ItemsProperty.GetActualValue(this); }
            //private set {  ItemsProperty.SetValue}
        }

        public TestContainer()
        { }
    }
}
