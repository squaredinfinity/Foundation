﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnitTestProject1.Types.Mapping.TestEntities;

namespace SquaredInfinity.Foundation.Types.Mapping
{
    [TestClass]
    public class TypeMapper__DeepClone
    {
        [TestMethod]
        public void CanCloneAnObject()
        {
            var st = new SimpleType();
            st.EnumProperty = DayOfWeek.Friday;
            st.IntegerProperty = 13;
            st.StringProperty = "some string";

            var tm = new TypeMapper();

            var clone = tm.DeepClone<SimpleType>(st);

            Assert.AreEqual(st.EnumProperty, clone.EnumProperty);
            Assert.AreEqual(st.IntegerProperty, clone.IntegerProperty);
            Assert.AreEqual(st.StringProperty, clone.StringProperty);
        }

        [TestMethod]
        public void CanCloneToSubType()
        {
            var st = new SimpleType2();
            st.EnumProperty = DayOfWeek.Friday;
            st.IntegerProperty = 13;
            st.StringProperty = "some string";
            st.AnotherProperty = 1;

            var tm = new TypeMapper();

            // clone SimpleType2 to its subtype SimpleType
            var clone = tm.DeepClone<SimpleType>(st);

            Assert.AreEqual(st.EnumProperty, clone.EnumProperty);
            Assert.AreEqual(st.IntegerProperty, clone.IntegerProperty);
            Assert.AreEqual(st.StringProperty, clone.StringProperty);

            Assert.IsFalse(clone is SimpleType2);
            Assert.IsTrue(clone is SimpleType);        
        }


        [TestMethod]
        public void CanCloneToDifferentType()
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

            var clone = tm.DeepClone<LinkedListNode>(n);

            Assert.AreEqual(13, clone.Id);
            Assert.AreSame(n, n.Next);
            Assert.AreSame(n, n.Previous);
        }


        [TestMethod]
        public void HandlesCircularReferences__ObjectReferencesSelfIndirectly()
        {
            var n0 = new LinkedListNode { Id = 0 };
            var n1 = new LinkedListNode { Id = 1 };
            var n2 = new LinkedListNode { Id = 2 };
            var n3 = new LinkedListNode { Id = 3 };

            n0.Next = n1;
            n1.Next = n2;
            n2.Next = n3;
            n3.Next = n0;

            n0.Previous = n3;
            n1.Previous = n0;
            n2.Previous = n1;
            n3.Previous = n2;

            var tm = new TypeMapper();

            var clone = tm.DeepClone<LinkedListNode>(n3);

            Assert.AreEqual(3, clone.Id);

            Assert.AreEqual(2, clone.Previous.Id);
            Assert.AreEqual(1, clone.Previous.Previous.Id);
            Assert.AreEqual(0, clone.Previous.Previous.Previous.Id);
            Assert.AreEqual(3, clone.Previous.Previous.Previous.Previous.Id);

            Assert.AreEqual(0, clone.Next.Id);
            Assert.AreEqual(1, clone.Next.Next.Id);
            Assert.AreEqual(2, clone.Next.Next.Next.Id);
            Assert.AreEqual(3, clone.Next.Next.Next.Next.Id);

            Assert.AreSame(clone, clone.Next.Next.Next.Next);
            Assert.AreSame(clone, clone.Previous.Previous.Previous.Previous);
        }
    }
}
