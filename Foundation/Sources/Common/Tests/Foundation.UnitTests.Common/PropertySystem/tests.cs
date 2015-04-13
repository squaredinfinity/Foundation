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
    public class PropertySystemTests
    {
        [TestMethod]
        public void ExtendedPropertyCanBeRegistered()
        {
            var tc = new TestPropertyContainer();

            tc.ExtendedProperties.RegisterProperty<int>("Grid.Row");

            tc.Count.Value++;

            tc.ExtendedProperties["Grid.Row"].Value = 3;

            // default of Count is 69, + 1 == 70
            Assert.AreEqual(70, tc.Count.ActualValue);

            Assert.AreEqual(3, tc.ExtendedProperties["Grid.Row"].ActualValue);
        }

        [TestMethod]
        public void ValueOfExtendedPropertyCanBeInherited()
        {
            // leaf
            var tc = new TestPropertyContainer { Name = "1" };

            // parent
            var parentTC = new TestPropertyContainer { Name = "Parent" };
            tc.Parent = parentTC;

            // grand parent
            var grandParentTC = new TestPropertyContainer { Name = "Parent" };
            parentTC.Parent = grandParentTC;

            tc.Count.Value = 1;
            parentTC.Count.Value = 13;
            grandParentTC.Count.Value = 69;

            tc.Count.IsValueSet = false;
            parentTC.Count.IsValueSet = false;
            Assert.AreEqual(69, tc.Count.ActualValue);
            Assert.AreEqual(69, parentTC.Count.ActualValue);
            Assert.AreEqual(69, grandParentTC.Count.ActualValue);

            tc.Count.IsValueSet = false;
            parentTC.Count.IsValueSet = true;
            Assert.AreEqual(13, tc.Count.ActualValue);
            Assert.AreEqual(13, parentTC.Count.ActualValue);
            Assert.AreEqual(69, grandParentTC.Count.ActualValue);


            tc.Count.IsValueSet = true;
            parentTC.Count.IsValueSet = false;
            Assert.AreEqual(1, tc.Count.ActualValue);
            Assert.AreEqual(69, parentTC.Count.ActualValue);
            Assert.AreEqual(69, grandParentTC.Count.ActualValue);


            tc.Count.IsValueSet = true;
            parentTC.Count.IsValueSet = true;
            Assert.AreEqual(1, tc.Count.ActualValue);
            Assert.AreEqual(13, parentTC.Count.ActualValue);
            Assert.AreEqual(69, grandParentTC.Count.ActualValue);
        }


        [TestMethod]
        public void DefaultValueIsSetAndReturnedAsActualValueIfNoInheritance()
        {
            var tc = new TestPropertyContainer();

            Assert.AreEqual("[UNSET]", tc.ToolTip.ActualValue);
        }

        [TestMethod]
        public void DefaultValueIsSetAndReturnedAsValue()
        {
            var tc = new TestPropertyContainer();

            Assert.AreEqual("[UNSET]", tc.ToolTip.Value);
        }

        [TestMethod]
        public void SettingValue_MarksValueAsSet()
        {
            var tc = new TestPropertyContainer();

            Assert.IsFalse(tc.Count.IsValueSet);

            tc.Count.Value = 13;

            Assert.AreEqual(13, tc.Count.Value);
            Assert.IsTrue(tc.Count.IsValueSet);
        }

        #region No Parent

        [TestMethod]
        public void ValueIsNotSet_And_NoParent__TryGetInheritedPropertyValueReturnsFalse()
        {
            var tc = new TestPropertyContainer();

            var val = (object)null;
            var res = tc.TryGetInheritedPropertyValue(tc.Count.UniqueName, out val);

            Assert.IsNull(val);
            Assert.IsFalse(res);
        }

        [TestMethod]
        public void ValueIsSet_And_NoParent__TryGetInheritedPropertyValueReturnsFalse()
        {
            var tc = new TestPropertyContainer();

            tc.Count.Value = 13;

            var val = (object)null;
            var res = tc.TryGetInheritedPropertyValue(tc.Count.UniqueName, out val);

            Assert.IsNull(val);
            Assert.IsFalse(res);
        }

        [TestMethod]
        public void ValueIsNotSet_And_NoParent__TryGetActualPropertyValueReturnsTrue_And_DefaultValue()
        {
            var tc = new TestPropertyContainer();

            var val = (object)null;
            var res = tc.TryGetActualPropertyValue(tc.Count.UniqueName, out val);

            Assert.AreEqual(69, val);
            Assert.IsTrue(res);
        }

        [TestMethod]
        public void ValueIsSet_And_NoParent__TryGetActualPropertyValueReturnsTrue()
        {
            var tc = new TestPropertyContainer();

            tc.Count.Value = 13;

            var val = (object)null;
            var res = tc.TryGetActualPropertyValue(tc.Count.UniqueName, out val);

            Assert.AreEqual(13, val);
            Assert.IsTrue(res);
        }

        #endregion

        #region Has Parent

        [TestMethod]
        public void ValueIsNotSet_And_HasParent__TryGetInheritedPropertyValueReturnsFalse()
        {
            var tc = new TestPropertyContainer();

            var val = (object)null;
            var res = tc.TryGetInheritedPropertyValue(tc.Count.UniqueName, out val);

            Assert.IsNull(val);
            Assert.IsFalse(res);
        }

        [TestMethod]
        public void ValueIsSet_And_HasParent__TryGetInheritedPropertyValueReturnsFalse()
        {
            var tc = new TestPropertyContainer();

            tc.Count.Value = 13;

            var val = (object)null;
            var res = tc.TryGetInheritedPropertyValue(tc.Count.UniqueName, out val);

            Assert.IsNull(val);
            Assert.IsFalse(res);
        }

        [TestMethod]
        public void ValueIsNotSet_And_HasParent__TryGetActualPropertyValueReturnsTrue_And_DefaultValue()
        {
            var tc = new TestPropertyContainer();

            var val = (object)null;
            var res = tc.TryGetActualPropertyValue(tc.Count.UniqueName, out val);

            Assert.AreEqual(69, val);
            Assert.IsTrue(res);
        }

        [TestMethod]
        public void ValueIsSet_And_HasParent__TryGetActualPropertyValueReturnsTrue()
        {
            var tc = new TestPropertyContainer();

            tc.Count.Value = 13;

            var val = (object)null;
            var res = tc.TryGetActualPropertyValue(tc.Count.UniqueName, out val);

            Assert.AreEqual(13, val);
            Assert.IsTrue(res);
        }

        #endregion

        #region Collection Property

        [TestMethod]
        public void CanReplaceInheritedCollectionItems()
        {
            var tc_parent = new TestPropertyContainer() { Name = "Parent" };
            tc_parent.Items.Value.Add(1);
            tc_parent.Items.Value.Add(2);
            tc_parent.Items.Value.Add(3);
            tc_parent.Items.IsValueSet = true;

            var tc = new TestPropertyContainer() { Name = "Child" };
            tc.Parent = tc_parent;
            tc.Items.Value.Add(4);
            tc.Items.Value.Add(5);
            tc.Items.Value.Add(6);
            tc.Items.IsValueSet = true;
            tc.Items.InheritanceMode = CollectionInheritanceMode.Replace;

            var items = tc.Items.ActualValue;

            Assert.AreEqual(3, items.Count);
            Assert.AreEqual(4, items[0]);
            Assert.AreEqual(5, items[1]);
            Assert.AreEqual(6, items[2]);
        }


        [TestMethod]
        public void CanMergeInheritedCollectionItems()
        {
            var tc_parent = new TestPropertyContainer() { Name = "Parent" };
            tc_parent.Items.Value.Add(1);
            tc_parent.Items.Value.Add(2);
            tc_parent.Items.Value.Add(3);
            tc_parent.Items.IsValueSet = true;

            var tc = new TestPropertyContainer() { Name = "Child" };
            tc.Parent = tc_parent;
            tc.Items.Value.Add(4);
            tc.Items.Value.Add(5);
            tc.Items.Value.Add(6);
            tc.Items.IsValueSet = true;
            tc.Items.InheritanceMode = CollectionInheritanceMode.Merge;

            var items = tc.Items.ActualValue;

            Assert.AreEqual(6, items.Count);
            Assert.AreEqual(1, items[0]);
            Assert.AreEqual(2, items[1]);
            Assert.AreEqual(3, items[2]);
            Assert.AreEqual(4, items[3]);
            Assert.AreEqual(5, items[4]);
            Assert.AreEqual(6, items[5]);
        }

        #endregion

    }
}


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

    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void ExtendedPropertyCanBeRegistered()
        {
            var tc = new TestContainer();

            //tc.ExtendedProperties.RegisterProperty<int>("Grid.Row");

            //tc.Count.Value++;

            //tc.ExtendedProperties["Grid.Row"].Value = 3;

            //// default of Count is 69, + 1 == 70
            //Assert.AreEqual(70, tc.Count.ActualValue);

            //Assert.AreEqual(3, tc.ExtendedProperties["Grid.Row"].ActualValue);

            Assert.Fail();
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
    }

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
        public void SettingValue_MarksValueAsSet()
        {
            var tc = new TestContainer();

            Assert.IsFalse(TestContainer.CountProperty.GetIsValueSet(tc));
            
            tc.Count = 13;

            Assert.AreEqual(13, tc.Count);

            Assert.IsTrue(TestContainer.CountProperty.GetIsValueSet(tc));
        }

        #region No Parent

        [TestMethod]
        public void ValueIsNotSet_And_NoParent__TryGetInheritedPropertyValueReturnsFalse()
        {
            var tc = new TestContainer();

            var val = (object) null;
            
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
