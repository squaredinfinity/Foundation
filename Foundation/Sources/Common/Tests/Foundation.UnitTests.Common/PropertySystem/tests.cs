using Microsoft.VisualStudio.TestTools.UnitTesting;
using SquaredInfinity.Foundation.PropertySystem._Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SquaredInfinity.Foundation.PropertySystem
{
    [TestClass]
    public class CollectionExtendedPropertyTests
    {
        [TestMethod]
        public void UsesDefaultValue()
        {
            var tc = new TestContainer();

            Assert.AreEqual(3, tc.Items.Count);
            Assert.AreEqual(1, tc.Items[0]);
            Assert.AreEqual(2, tc.Items[1]);
            Assert.AreEqual(3, tc.Items[2]);
        }

        [TestMethod]
        public void SetValue__CanSetCollectionItemsUsing__Replace()
        {
            var tc_parent = new TestContainer() { Name = "Parent" };
            tc_parent.Items.Clear();
            tc_parent.Items.Add(4);
            tc_parent.Items.Add(5);
            tc_parent.Items.Add(6);

            //tc_parent.Items.IsValueSet = true;

            var tc = new TestContainer() { Name = "Child" };
            tc.Parent = tc_parent;

            var items = new List<int> { 7, 8, 9 };

            // NOTE: IsValueSet and InheritenceMode must be set *before* collection is modified
            TestContainer.ItemsProperty.SetValue(tc, items);
            TestContainer.ItemsProperty.SetInheritanceMode(tc, CollectionInheritanceMode.Replace);

            Assert.AreEqual(3, tc.Items.Count);
            Assert.AreEqual(7, tc.Items[0]);
            Assert.AreEqual(8, tc.Items[1]);
            Assert.AreEqual(9, tc.Items[2]);
        }

        [TestMethod]
        public void CanSetCollectionItems__Replace()
        {
            var tc_parent = new TestContainer() { Name = "Parent" };
            tc_parent.Items.Clear();
            tc_parent.Items.Add(4);
            tc_parent.Items.Add(5);
            tc_parent.Items.Add(6);

            //tc_parent.Items.IsValueSet = true;

            var tc = new TestContainer() { Name = "Child" };
            tc.Parent = tc_parent;

            // NOTE:    IsValueSet and InheritenceMode must be set *before* collection is modified
            //          Alternatively overload of SetValue can be used
            TestContainer.ItemsProperty.SetValue(tc);
            TestContainer.ItemsProperty.SetInheritanceMode(tc, CollectionInheritanceMode.Replace);

            tc.Items.Clear();
            tc.Items.Add(7);
            tc.Items.Add(8);
            tc.Items.Add(9);

            Assert.AreEqual(3, tc.Items.Count);
            Assert.AreEqual(7, tc.Items[0]);
            Assert.AreEqual(8, tc.Items[1]);
            Assert.AreEqual(9, tc.Items[2]);
        }

        [TestMethod]
        public void SetValue__CanSetCollectionItemsUsing__Merge()
        {
            var tc_parent = new TestContainer() { Name = "Parent" };
            TestContainer.ItemsProperty.SetValue(tc_parent);
            tc_parent.Items.Clear();
            tc_parent.Items.Add(4);
            tc_parent.Items.Add(5);
            tc_parent.Items.Add(6);

            var tc = new TestContainer() { Name = "Child" };
            tc.Parent = tc_parent;

            var items = new List<int> { 7, 8, 9 };

            // NOTE: IsValueSet and InheritenceMode must be set *before* collection is modified
            TestContainer.ItemsProperty.SetValue(tc, items);
            TestContainer.ItemsProperty.SetInheritanceMode(tc, CollectionInheritanceMode.Merge);

            Assert.AreEqual(6, tc.Items.Count);
            Assert.AreEqual(4, tc.Items[0]);
            Assert.AreEqual(5, tc.Items[1]);
            Assert.AreEqual(6, tc.Items[2]);
            Assert.AreEqual(7, tc.Items[3]);
            Assert.AreEqual(8, tc.Items[4]);
            Assert.AreEqual(9, tc.Items[5]);
        }
    }

    [TestClass]
    public class ExtendedPropertyTests
    {
        [TestMethod]
        public void DefaultValue_SubsequentAccessReturnsSameInstance()
        {
            var tc = new TestContainer();
            var i1 = tc.Items;
            var i2 = tc.Items;

            Assert.AreSame(i1, i2);
        }

        [TestMethod]
        public void SettingValue_MarksValueAsSet()
        {
            var tc = new TestContainer();

            Assert.IsFalse(TestContainer.CountProperty.GetIsValueSet(tc));

            tc.Count = 13;

            Assert.AreEqual(13, tc.Count);

            Assert.IsTrue(TestContainer.CountProperty.GetIsValueSet(tc));
        }

        [TestMethod]
        public void ExtendedPropertyCanBeRegistered()
        {
            var tc = new TestContainer();

            var pd = new ExtendedPropertyDefinition<int>("Grid", "Row", () => 0);

            var p = tc.ExtendedProperties.GetOrAddProperty<int>(pd);
            
            tc.ExtendedProperties["Grid.Row"].Value = 3;

            Assert.AreEqual(3, tc.ExtendedProperties["Grid.Row"].ActualValue);
        }

        [TestMethod]
        public void ValueOfExtendedPropertyCanBeInherited()
        {
            // leaf
            var tc = new TestContainer { Name = "1" };

            // parent
            var parentTC = new TestContainer { Name = "Parent" };
            tc.Parent = parentTC;

            // grand parent
            var grandParentTC = new TestContainer { Name = "Grand Parent" };
            parentTC.Parent = grandParentTC;

            tc.Count = 1;
            parentTC.Count = 13;
            grandParentTC.Count = 69;

            TestContainer.CountProperty.UnsetValue(tc);
            TestContainer.CountProperty.UnsetValue(parentTC);

            Assert.AreEqual(69, tc.Count);
            Assert.AreEqual(69, parentTC.Count);
            Assert.AreEqual(69, grandParentTC.Count);

            TestContainer.CountProperty.UnsetValue(tc);
            TestContainer.CountProperty.SetValue(parentTC);

            Assert.AreEqual(13, tc.Count);
            Assert.AreEqual(13, parentTC.Count);
            Assert.AreEqual(69, grandParentTC.Count);

            TestContainer.CountProperty.SetValue(tc);
            TestContainer.CountProperty.UnsetValue(parentTC);

            Assert.AreEqual(1, tc.Count);
            Assert.AreEqual(69, parentTC.Count);
            Assert.AreEqual(69, grandParentTC.Count);

            TestContainer.CountProperty.SetValue(tc);
            TestContainer.CountProperty.SetValue(parentTC);

            Assert.AreEqual(1, tc.Count);
            Assert.AreEqual(13, parentTC.Count);
            Assert.AreEqual(69, grandParentTC.Count);

        }


        [TestMethod]
        public void DefaultValueIsSetAndReturnedAsActualValueIfNoInheritance()
        {
            var tc = new TestContainer();

            Assert.AreEqual(69, tc.Count);
        }

        #region No Parent

        [TestMethod]
        public void ValueIsNotSet_And_NoParent__TryGetInheritedPropertyValueReturnsFalse()
        {
            var tc = new TestContainer();

            var val = (object)null;

            var res = tc.TryGetInheritedPropertyValue(TestContainer.CountProperty, out val);

            Assert.IsNull(val);
            Assert.IsFalse(res);
        }

        [TestMethod]
        public void ValueIsSet_And_NoParent__TryGetInheritedPropertyValueReturnsFalse()
        {
            var tc = new TestContainer();

            tc.Count = 13;

            var val = (object)null;
            var res = tc.TryGetInheritedPropertyValue(TestContainer.CountProperty, out val);

            Assert.IsNull(val);
            Assert.IsFalse(res);
        }

        [TestMethod]
        public void ValueIsNotSet_And_NoParent__TryGetActualPropertyValueReturnsTrue_And_DefaultValue()
        {
            var tc = new TestContainer();

            var val = (object)null;
            var res = tc.TryGetActualPropertyValue(TestContainer.CountProperty, out val);

            Assert.AreEqual(69, val);
            Assert.IsTrue(res);
        }

        [TestMethod]
        public void ValueIsSet_And_NoParent__TryGetActualPropertyValueReturnsTrue()
        {
            var tc = new TestContainer();

            tc.Count = 13;

            var val = (object)null;
            var res = tc.TryGetActualPropertyValue(TestContainer.CountProperty, out val);

            Assert.AreEqual(13, val);
            Assert.IsTrue(res);
        }

        #endregion

        #region Has Parent

        [TestMethod]
        public void ValueIsNotSet_And_HasParent__TryGetInheritedPropertyValueReturnsFalse()
        {
            var tc = new TestContainer();

            var val = (object)null;
            var res = tc.TryGetInheritedPropertyValue(TestContainer.CountProperty, out val);

            Assert.IsNull(val);
            Assert.IsFalse(res);
        }

        [TestMethod]
        public void ValueIsSet_And_HasParent__TryGetInheritedPropertyValueReturnsFalse()
        {
            var tc = new TestContainer();

            tc.Count = 13;

            var val = (object)null;
            var res = tc.TryGetInheritedPropertyValue(TestContainer.CountProperty, out val);

            Assert.IsNull(val);
            Assert.IsFalse(res);
        }

        [TestMethod]
        public void ValueIsNotSet_And_HasParent__TryGetActualPropertyValueReturnsTrue_And_DefaultValue()
        {
            var tc = new TestContainer();

            var val = (object)null;
            var res = tc.TryGetActualPropertyValue(TestContainer.CountProperty, out val);

            Assert.AreEqual(69, val);
            Assert.IsTrue(res);
        }

        [TestMethod]
        public void ValueIsSet_And_HasParent__TryGetActualPropertyValueReturnsTrue()
        {
            var tc = new TestContainer();

            tc.Count = 13;

            var val = (object)null;
            var res = tc.TryGetActualPropertyValue(TestContainer.CountProperty, out val);

            Assert.AreEqual(13, val);
            Assert.IsTrue(res);
        }

        #endregion
    }
}