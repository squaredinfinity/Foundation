using Microsoft.VisualStudio.TestTools.UnitTesting;
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
    }
}
