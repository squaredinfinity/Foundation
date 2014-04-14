﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using SquaredInfinity.Foundation.TestEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnitTestProject1.Types.Mapping.TestEntities;

namespace SquaredInfinity.Foundation.Types.Mapping
{
    [TestClass]
    public class TypeMapper__Map
    {
        [TestMethod]
        public void CanMapAnObject()
        {
            var st = new SimpleType();
            st.EnumProperty = DayOfWeek.Friday;
            st.IntegerProperty = 13;
            st.StringProperty = "some string";

            var tm = new TypeMapper();

            var clone = tm.Map<SimpleType_>(st);

            Assert.AreEqual(st.EnumProperty, clone.EnumProperty);
            Assert.AreEqual(st.IntegerProperty, clone.IntegerProperty);
            Assert.AreEqual(st.StringProperty, clone.StringProperty);
        }

        [TestMethod]
        public void CanMapToSubType()
        {
            var st = new SimpleType2();
            st.EnumProperty = DayOfWeek.Friday;
            st.IntegerProperty = 13;
            st.StringProperty = "some string";
            st.AnotherProperty = 1;

            var tm = new TypeMapper();

            // clone SimpleType2 to its subtype SimpleType
            var clone = tm.Map<SimpleType>(st);
            
            Assert.AreEqual(st.EnumProperty, clone.EnumProperty);
            Assert.AreEqual(st.IntegerProperty, clone.IntegerProperty);
            Assert.AreEqual(st.StringProperty, clone.StringProperty);

            Assert.IsFalse(clone is SimpleType2);
            Assert.IsTrue(clone is SimpleType);        
        }


        [TestMethod]
        public void CanMapToDifferentType()
        {
            var st = new SimpleType();
            st.EnumProperty = DayOfWeek.Friday;
            st.IntegerProperty = 13;
            st.StringProperty = "some string";

            var tm = new TypeMapper();

            SimpleType_ st2 = new SimpleType_();

            tm.Map<SimpleType_>(st, st2);

            Assert.AreEqual(st.EnumProperty, st2.EnumProperty);
            Assert.AreEqual(st.IntegerProperty, st2.IntegerProperty);
            Assert.AreEqual(st.StringProperty, st2.StringProperty);
        }


        [TestMethod]
        public void HandlesCircularReferences__ObjectReferencesSelfDirectly()
        {
            var n = new LinkedListNode { Id = 13 };
            n.Next = n;
            n.Previous = n;

            var tm = new TypeMapper();

            var clone = tm.Map<LinkedListNode>(n);

            Assert.AreEqual(13, clone.Id);
            Assert.AreSame(n, n.Next);
            Assert.AreSame(n, n.Previous);
        }


        [TestMethod]
        public void HandlesCircularReferences__ObjectReferencesSelfIndirectly()
        {
            var n = LinkedListNode.CreateDefaultTestHierarchy();

            var tm = new TypeMapper();

            var clone = tm.Map<LinkedListNode>(n);

            LinkedListNode.EnsureDefaultTestHierarchyPreserved(clone);
        }

        [TestMethod]
        public void IgnoreNulls__DoesNotMapMembersWithNullValue()
        {
            var st = new SimpleType();
            st.EnumProperty = DayOfWeek.Friday;
            st.IntegerProperty = 13;
            st.StringProperty = null;

            var tm = new TypeMapper();

            SimpleType st2 = new SimpleType();
            st2.EnumProperty = DayOfWeek.Thursday;
            st2.IntegerProperty = 14;
            st2.StringProperty = "this should remain what it is";

            tm.Map(st, st2, new MappingOptions { IgnoreNulls = true });

            Assert.AreEqual(st.EnumProperty, st2.EnumProperty);
            Assert.AreEqual(st.IntegerProperty, st2.IntegerProperty);
            Assert.AreEqual("this should remain what it is", st2.StringProperty);
        }

        [TestMethod]
        public void CanConvertNullableWithValueToNonNullable()
        {
            var np = new NullablesTests.HasNullableProperty();
            np.Property = 13;

            var tm = new TypeMapper();

            var p = tm.Map<NullablesTests.HasNonNullableProperty>(np);

            Assert.AreEqual(13, p.Property);
        }

        public class MapTestCollectionItem
        {
            public int Id { get; set; }
        }

        public class MapTestCollection : List<MapTestCollectionItem>
        {

        }

        public class MapTestCollectionOwner
        {
            public MapTestCollection Collection { get; set; }
        }

        [TestMethod]
        public void PreservesExistingCollectionsAndCollectionItems_ButMapsTheirProperties()
        {
            var source = new MapTestCollectionOwner();
            
            var sourceCollection = new MapTestCollection();
            source.Collection = sourceCollection;

            var sourceCollectionItem = new MapTestCollectionItem { Id = 13 };
            source.Collection.Add(sourceCollectionItem);

            var target = new MapTestCollectionOwner();

            var targetCollection = new MapTestCollection();
            target.Collection = targetCollection;

            var targetCollectionItem = new MapTestCollectionItem { Id = 7 };
            target.Collection.Add(targetCollectionItem);

            TypeMapper mapper = new TypeMapper();

            mapper.Map(source, target);

            Assert.AreSame(sourceCollection, source.Collection);
            Assert.AreSame(sourceCollectionItem, source.Collection.Single());

            Assert.AreSame(targetCollection, target.Collection);
            Assert.AreSame(targetCollectionItem, target.Collection.Single());

            Assert.AreEqual(13, target.Collection.Single().Id);

            Assert.AreEqual(1, target.Collection.Count);
        }

        [TestMethod]
        public void PreservesExistingCollectionsAndCollectionItems_ButMapsTheirProperties__TrimsRemainingOldItems()
        {
            var source = new MapTestCollectionOwner();

            var sourceCollection = new MapTestCollection();
            source.Collection = sourceCollection;

            var sourceCollectionItem = new MapTestCollectionItem { Id = 13 };
            source.Collection.Add(sourceCollectionItem);

            var target = new MapTestCollectionOwner();

            var targetCollection = new MapTestCollection();
            target.Collection = targetCollection;

            var targetCollectionItem_1 = new MapTestCollectionItem { Id = 7 };
            var targetCollectionItem_2 = new MapTestCollectionItem { Id = 21 };
            target.Collection.Add(targetCollectionItem_1);
            target.Collection.Add(targetCollectionItem_2);

            TypeMapper mapper = new TypeMapper();

            mapper.Map(source, target);

            Assert.AreSame(sourceCollection, source.Collection);
            Assert.AreSame(sourceCollectionItem, source.Collection.Single());

            Assert.AreSame(targetCollection, target.Collection);
            Assert.AreSame(targetCollectionItem_1, target.Collection.Single());

            Assert.AreEqual(13, target.Collection.Single().Id);

            Assert.AreEqual(1, target.Collection.Count);
        }

        [TestMethod]
        public void PreservesExistingCollectionsAndCollectionItems_ButMapsTheirProperties__AddsAdditionalNewItemsWhenNeeded()
        {
            var source = new MapTestCollectionOwner();

            var sourceCollection = new MapTestCollection();
            source.Collection = sourceCollection;

            var sourceCollectionItem_1 = new MapTestCollectionItem { Id = 13 };
            var sourceCollectionItem_2 = new MapTestCollectionItem { Id = 21 };
            source.Collection.Add(sourceCollectionItem_1);
            source.Collection.Add(sourceCollectionItem_2);

            var target = new MapTestCollectionOwner();

            var targetCollection = new MapTestCollection();
            target.Collection = targetCollection;

            var targetCollectionItem_1 = new MapTestCollectionItem { Id = 7 };
            target.Collection.Add(targetCollectionItem_1);

            TypeMapper mapper = new TypeMapper();

            mapper.Map(source, target);

            Assert.AreSame(sourceCollection, source.Collection);
            Assert.AreSame(sourceCollectionItem_1, source.Collection.First());
            Assert.AreSame(sourceCollectionItem_2, source.Collection.Skip(1).First());

            Assert.AreSame(targetCollection, target.Collection);
            
            Assert.AreSame(targetCollectionItem_1, target.Collection.First());
            Assert.AreEqual(13, target.Collection.First().Id);

            Assert.AreNotSame(sourceCollectionItem_2, target.Collection.Skip(1).First());
            Assert.AreEqual(21, target.Collection.Skip(1).First().Id);

            Assert.AreEqual(2, target.Collection.Count);
        }

        [TestMethod]
        public void CanOverrideDefaultMapping()
        {
            var s = new SimpleType { IntegerProperty = 13, StringProperty = "Thirteen" };
            var t = new SimpleType { IntegerProperty = 7, StringProperty = "Seven" };

            var tm = new TypeMapper();

            tm.GetOrCreateTypeMappingStrategy<SimpleType, SimpleType>()
            .IgnoreAll()
            .MapMember(x => s.IntegerProperty, x => x.IntegerProperty + 10);

            tm.Map(s, t);

            Assert.AreEqual(13, s.IntegerProperty);
            Assert.AreEqual("Thirteen", s.StringProperty);

            Assert.AreEqual(23, t.IntegerProperty);
            Assert.AreEqual("Seven", t.StringProperty);
        }
    }
}