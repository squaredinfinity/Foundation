using Microsoft.VisualStudio.TestTools.UnitTesting;
using SquaredInfinity.Foundation.Serialization.FlexiXml.TestEntities;
using SquaredInfinity.Foundation.TestEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using SquaredInfinity.Foundation.Extensions;
using System.Diagnostics;
using System.IO;

namespace SquaredInfinity.Foundation.Serialization.FlexiXml
{
    [TestClass]
    public class FlexiXmlSerialize__Deserialize
    {
        [TestMethod]
        public void XmlRootNameCanBeAnything()
        {
            var xml =
                Assembly.GetExecutingAssembly()
                .GetManifestResourceStream(@"SquaredInfinity.Foundation.Serialization.FlexiXml.Resources.Deserialization.XmlRootNameCanBeAnything.xml")
                .ReadToEnd();

            var s = new FlexiXmlSerializer();

            var root = s.Deserialize<Root>(xml);

            Assert.IsNotNull(root);

            Assert.AreEqual("string value 1", root.StringProperty);
        }

        [TestMethod]
        public void AttributesCanBeUsedToMapPropertiesWithValuesConvertibleToString()
        {
            var xml =
                Assembly.GetExecutingAssembly()
                .GetManifestResourceStream(@"SquaredInfinity.Foundation.Serialization.FlexiXml.Resources.Deserialization.AttributesCanBeUsedToMapPropertiesWithValuesConvertibleToString.xml")
                .ReadToEnd();

            var s = new FlexiXmlSerializer();

            var root = s.Deserialize<Root>(xml);

            Assert.IsNotNull(root);

            Assert.AreEqual("string value 1", root.StringProperty);
            Assert.AreEqual(13, root.IntProperty);
            Assert.AreEqual(14, root.NullableIntProperty);
            Assert.AreEqual(new Guid("9763C96B-5AC4-4ACA-AE4E-5D71EDDBE0DD"), root.GuidProperty);
        }

        [TestMethod]
        public void AttachedElementsCanBeUsedToMapProperties()
        {
            var xml =
                Assembly.GetExecutingAssembly()
                .GetManifestResourceStream(@"SquaredInfinity.Foundation.Serialization.FlexiXml.Resources.Deserialization.AttachedElementsCanBeUsedToMapProperties.xml")
                .ReadToEnd();

            var s = new FlexiXmlSerializer();

            var root = s.Deserialize<Root>(xml);

            Assert.IsNotNull(root);

            Assert.AreEqual("string value 1", root.StringProperty);
            Assert.AreEqual(13, root.IntProperty);
            Assert.AreEqual(14, root.NullableIntProperty);
            Assert.AreEqual(new Guid("9763C96B-5AC4-4ACA-AE4E-5D71EDDBE0DD"), root.GuidProperty);
        }

        [TestMethod]
        public void CanDeserializeCollections()
        {
            var xml =
                Assembly.GetExecutingAssembly()
                .GetManifestResourceStream(@"SquaredInfinity.Foundation.Serialization.FlexiXml.Resources.Deserialization.collection_tests.xml")
                .ReadToEnd();

            var s = new FlexiXmlSerializer();

            var root = s.Deserialize<Root>(xml);

            Assert.IsNotNull(root);

            Assert.AreEqual(13, (root.CollectionReadOnlyPropertyCreatedInConstructor as CollectionItemCollection).IntProperty);
            Assert.AreEqual("string value 1", (root.CollectionReadOnlyPropertyCreatedInConstructor as CollectionItemCollection).StringProperty);
            Assert.AreEqual(3, root.CollectionReadOnlyPropertyCreatedInConstructor.Count);
            Assert.AreEqual(14, root.CollectionReadOnlyPropertyCreatedInConstructor[0].IntProperty);
            Assert.AreEqual(15, root.CollectionReadOnlyPropertyCreatedInConstructor[1].IntProperty);
            Assert.AreEqual(16, root.CollectionReadOnlyPropertyCreatedInConstructor[2].IntProperty);

            Assert.AreEqual(23, (root.Collection as CollectionItemCollection).IntProperty);
            Assert.AreEqual("string value 2", (root.Collection as CollectionItemCollection).StringProperty);
            Assert.AreEqual(3, root.Collection.Count);
            Assert.AreEqual(24, root.Collection[0].IntProperty);
            Assert.AreEqual(25, root.Collection[1].IntProperty);
            Assert.AreEqual(26, root.Collection[2].IntProperty);

            Assert.AreEqual(33, root.NonGenericCollection.IntProperty);
            Assert.AreEqual("string value 3", root.NonGenericCollection.StringProperty);
            Assert.AreEqual(3, root.NonGenericCollection.Count);
            Assert.AreEqual(34, root.NonGenericCollection[0].IntProperty);
            Assert.AreEqual(35, root.NonGenericCollection[1].IntProperty);
            Assert.AreEqual(36, root.NonGenericCollection[2].IntProperty);
        }

        [TestMethod]
        public void Bug001__ChildElementNamesWrong()
        {
            // read-only collection has a member DefaultItem
            // it should be serialized as:
            //
            // <root>
            //  <root.items>
            //      <items.defaultitem>..
            //
            // instead is serialzied as:
            //
            // <root>
            //  <root.items>
            //      <root.items.defaultitem>

            var root = new Root();
            (root.CollectionReadOnlyPropertyCreatedInConstructor as CollectionItemCollection).StringProperty = "string value 1";
            (root.CollectionReadOnlyPropertyCreatedInConstructor as CollectionItemCollection).DefaultItem = 
                new CollectionItem() { IntProperty = 13, StringProperty = "07" };
            
            var s = new FlexiXmlSerializer();

            var xml = s.Serialize(root);

            Assert.IsNotNull(xml);

            // there should be no elements with more than 1 '.' in the name

            foreach(var el in xml.Descendants())
            {
                Assert.IsTrue(el.Name.LocalName.Count(c => c == '.') <= 1);
            }
        }

        [TestMethod]
        public void CanDeserializeSerializedTypes()
        {
            var n = LinkedListNode.CreateDefaultTestHierarchy();

            var s = new FlexiXml.FlexiXmlSerializer();

            var xml = s.Serialize(n);

            Assert.IsNotNull(xml);

            var n1 = s.Deserialize<LinkedListNode>(xml);

            Assert.IsNotNull(n1);
            
            LinkedListNode.EnsureDefaultTestHierarchyPreserved(n1);
        }

        [TestMethod]
        public void TypeWithCollectionReadOnlyProperty__CanDeserialize()
        {
            var o = new TypeWithCollectionReadOnlyProperty();
            o.Items.Add(7);
            o.Items.Add(13);

            var s = new FlexiXml.FlexiXmlSerializer();

            var xml = s.Serialize(o);

            var o2 = s.Deserialize<TypeWithCollectionReadOnlyProperty>(xml);

            Assert.IsNotNull(o2);
            Assert.IsNotNull(o2.Items);
            Assert.AreEqual(2, o2.Items.Count);

            Assert.AreEqual(7, o2.Items[0]);
            Assert.AreEqual(13, o2.Items[1]);
        }


        [TestMethod]
        public void TypeWithNonGenericCollectionReadOnlyProperty__CanDeserialize()
        {
            var o = new TypeWithNonGenericCollectionReadOnlyProperty();
            o.Items.Add(7);
            o.Items.Add(13);

            var s = new FlexiXml.FlexiXmlSerializer();

            var xml = s.Serialize(o);

            var o2 = s.Deserialize<TypeWithNonGenericCollectionReadOnlyProperty>(xml);

            Assert.IsNotNull(o2);
            Assert.IsNotNull(o2.Items);
            Assert.AreEqual(2, o2.Items.Count);

            Assert.AreEqual(7, o2.Items[0]);
            Assert.AreEqual(13, o2.Items[1]);
        }

        [TestMethod]
        public void TypeWithCollectionProperty__CanDeserialize()
        {
            var o = new TypeWithCollectionProperty();
            o.Items.Add(7);
            o.Items.Add(13);

            var s = new FlexiXml.FlexiXmlSerializer();

            var xml = s.Serialize(o);

            var o2 = s.Deserialize<TypeWithCollectionProperty>(xml);

            Assert.IsNotNull(o2);
            Assert.IsNotNull(o2.Items);
            Assert.AreEqual(2, o2.Items.Count);

            Assert.AreEqual(7, o2.Items[0]);
            Assert.AreEqual(13, o2.Items[1]);
        }

        [TestMethod]
        public void TypeWithNonGenericCollectionProperty__CanDeserialize()
        {
            var o = new TypeWithNonGenericCollectionProperty();
            o.Items.Add(7);
            o.Items.Add(13);

            var s = new FlexiXml.FlexiXmlSerializer();

            var xml = s.Serialize(o);

            var o2 = s.Deserialize<TypeWithNonGenericCollectionProperty>(xml);

            Assert.IsNotNull(o2);
            Assert.IsNotNull(o2.Items);
            Assert.AreEqual(2, o2.Items.Count);

            Assert.AreEqual(7, o2.Items[0]);
            Assert.AreEqual(13, o2.Items[1]);
        }

        [TestMethod]
        public void TypeWithInterfaceProperty__CanDeserialize()
        {
            var o = new InterfaceTypeProperty();
            o.MyDispsableProperty = new InterfaceTypeProperty();

            var s = new FlexiXmlSerializer();

            var xml = s.Serialize(o);

            var o2 = s.Deserialize<InterfaceTypeProperty>(xml);

            Assert.IsNotNull(o2);

            Assert.IsNotNull(o2.MyDispsableProperty);

            Assert.AreNotSame(o2, o2.MyDispsableProperty);
        }
    }
}
