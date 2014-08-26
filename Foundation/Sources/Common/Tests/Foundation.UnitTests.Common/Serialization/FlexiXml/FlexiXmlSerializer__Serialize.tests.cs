using Microsoft.VisualStudio.TestTools.UnitTesting;
using SquaredInfinity.Foundation.Serialization.FlexiXml.TestEntities;
using SquaredInfinity.Foundation.TestEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SquaredInfinity.Foundation.Serialization.FlexiXml
{
    public class x
    {
        public double[,] Data { get; set; }
    }

    [TestClass]
    public class FlexiXmleSerializer__BugFixes
    {
        [TestMethod]
        public void Bug001__ReadOnlyNonEnumerableMembersShouldNotBeSerialized() // this is now supported
        {
            // read-only non-enumerable members should not be serialized by default
            // as they would be impossible to deserialize

            var list = new Bug001(new Bug001_Item[] { new Bug001_Item { Number = 1 }, new Bug001_Item { Number = 2 } });

            var s = new FlexiXmlSerializer();

            var xml = s.Serialize(list);

            var list_2 = s.Deserialize<Bug001>(xml);

            //var containsFirstItemElement = 
            //    (from e in xml.Descendants()
            //     where e.Name.LocalName.ToLower().Contains("firstitem")
            //     select e).Any();

            //Assert.IsFalse(containsFirstItemElement);
        }

        public class Bug001_Item
        {
            public int Number { get; set; }
        }

        public class Bug001 : List<Bug001_Item>
        {
            public Bug001_Item FirstItem { get; private set; }

            public Bug001() { }

            public Bug001(Bug001_Item[] items)
            {
                this.AddRange(items);

                FirstItem = items.First();
            }
        }
    }

    [TestClass]
    public class FlexiXmlSerialize__Serialize
    {
        [TestMethod]
        public void SerializeMultidimensionalArray()
        {
            var array = new double[2, 2];

            array[0, 0] = 0.0;
            array[0, 1] = 0.1;
            array[1, 0] = 1.0;
            array[1, 1] = 1.1;

            var x = new x();
            x.Data = array;
            

            var s = new FlexiXml.FlexiXmlSerializer();

            var xml = s.Serialize(x);
        }

        [TestMethod]
        public void ByDefault__IfPropertyCanBeConvertedToString__SerializeToAttribute()
        {
            var n = new SimpleSerializableType();
            n.IntProperty = 13;
            n.NullableIntProperty = 14;
            n.StringProperty = "string value";
            n.GuidProperty = new Guid("9763C96B-5AC4-4ACA-AE4E-5D71EDDBE0DD");

            var s = new FlexiXml.FlexiXmlSerializer();

            var xml = s.Serialize(n);


        }

        [TestMethod]
        public void CanSerializeTypes()
        {
            var n = LinkedListNode.CreateDefaultTestHierarchy();

            var s = new FlexiXml.FlexiXmlSerializer();

            var xml = s.Serialize(n);

            Assert.IsNotNull(xml);
        }

        [TestMethod]
        public void CanSpecifyCustomTypeInitialization()
        {
            var n = LinkedListNode.CreateDefaultTestHierarchy();

            var s = new FlexiXml.FlexiXmlSerializer();

            var xml = s.Serialize(n);

            bool wasCustomInstanceCalled = false;

            s.CustomCreateInstanceWith = (type, cx) =>
                {
                    wasCustomInstanceCalled = true;
                    return Activator.CreateInstance(type);
                };

            var result = s.Deserialize<LinkedListNode>(xml);

            Assert.IsNotNull(xml);
            Assert.IsNotNull(result);
            Assert.IsTrue(wasCustomInstanceCalled);
        }

        [TestMethod]
        public void TypeWithCollectionReadOnlyProperty__CanSerialize()
        {
            var o = new TypeWithCollectionReadOnlyProperty();
            o.Items.Add(7);
            o.Items.Add(13);

            var s = new FlexiXml.FlexiXmlSerializer();

            var xml = s.Serialize(o);

            Assert.IsNotNull(xml);

            Assert.IsNotNull(xml.Element("TypeWithCollectionReadOnlyProperty.Items"));
            Assert.IsTrue(
                xml.Element("TypeWithCollectionReadOnlyProperty.Items").Descendants().Count() == 3, 
                "Not all collection items have been serialized");
        }

        [TestMethod]
        public void CollectionWithNullableValueTypeItems__CanSerialize()
        {
            var list = new List<int?>();
            list.Add(null);
            list.Add(1);
            list.Add(2);
            list.Add(null);
            list.Add(4);
            list.Add(null);

            var s = new FlexiXml.FlexiXmlSerializer();

            var xml = s.Serialize(list);

            Assert.IsNotNull(xml);

            var deserialized = s.Deserialize<List<int?>>(xml);

            Assert.AreEqual(6, deserialized.Count);
            Assert.IsNull(deserialized[0]);
            Assert.IsNull(deserialized[3]);
            Assert.IsNull(deserialized[5]);

            Assert.AreEqual(1, deserialized[1]);
            Assert.AreEqual(2, deserialized[2]);
            Assert.AreEqual(4, deserialized[4]);
        }
    }
}
