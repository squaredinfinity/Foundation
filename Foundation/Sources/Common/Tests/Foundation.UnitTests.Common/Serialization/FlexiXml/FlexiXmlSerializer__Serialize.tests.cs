using Microsoft.VisualStudio.TestTools.UnitTesting;
using SquaredInfinity.Foundation.Serialization.FlexiXml.TestEntities;
using SquaredInfinity.Foundation.TestEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

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

        [TestMethod]
        public void Bug002__CollectionWithItemsOfTypeString__ItemsAreSerializedAsSeparateCharactersInsteadOfOneString()
        {
            var list = new List<string>()
            {
                "one",
                "two"
            };


            var s = new FlexiXmlSerializer();

            var xml = s.Serialize(list);

            var expected =
@"<List Capacity=""4"">
  <String>one</String>
  <String>two</String>
</List>";

            Assert.AreEqual(expected, xml.ToString());
            
            var list2 = s.Deserialize<List<string>>(xml);

            Assert.AreEqual(2, list2.Count);
            Assert.AreEqual("one", list2[0]);
            Assert.AreEqual("two", list2[1]);

        }

        
    }

    [TestClass]
    public class FlexiXmlSerialize__Serialize
    {
        [TestMethod]
        public void CanFilterListElementsBeforeSerialization()
        {
            List<int> numbers = new List<int> { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };

            var s = new FlexiXml.FlexiXmlSerializer();
            s.GetOrCreateEnumerableTypeSerializationStrategy<List<int>, int>()
                .ElementFilter(x => x % 2 == 0);

            Assert.Fail();
        }

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
        public void Property_With_Null_Value__SerialziedToAttribute()
        {
            var x = new SimpleSerializableType();

            var s = new FlexiXml.FlexiXmlSerializer();

            var xml = s.Serialize(x);


            var a = s.Deserialize<SimpleSerializableType>(xml);

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

        [TestMethod]
        public void CanSelectivlyOptOutMembersFromSerialization()
        {
            var x = new SimpleSerializableType
            {
                GuidProperty = Guid.NewGuid(),
                IntProperty = 13,
                NullableIntProperty = 14,
                StringProperty = "text"
            };

            var s = new FlexiXmlSerializer();

            s.GetOrCreateTypeSerializationStrategy<SimpleSerializableType>()
                .IgnoreMember(_ => _.IntProperty);

            var xml = s.Serialize(x);

            Assert.IsNotNull(xml);
            Assert.IsNull(xml.Attribute("IntProperty"));
        }

        [TestMethod]
        public void MemberIsMultiLineString__SerializedAsCData()
        {
            var x = new SimpleSerializableType();
            x.StringProperty =
@"
this is
multi-line
string";

            var s = new FlexiXmlSerializer();

            var xml = s.Serialize(x);
            
            var x2 = s.Deserialize<SimpleSerializableType>(xml);

            Assert.AreSame(x.StringProperty, x2.StringProperty);
        }
    }
}
