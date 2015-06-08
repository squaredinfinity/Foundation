using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SquaredInfinity.Foundation.Collections
{
	[TestClass]
    public partial class CollectionExTests__ItemInsertedRemovedNotifications
    {
        class CallbackCollection : CollectionEx<string>
        {
            public Action<int, string> OnBeforeItemInserted_Callback { get; set; }
            public Action<int, string> OnAfterItemInserted_Callback { get; set; }


            public Action<int, string> OnBeforeItemRemoved_Callback { get; set; }
            public Action<int, string> OnAfterItemRemoved_Callback { get; set; }


            protected override void OnBeforeItemInserted(int index, string item)
            {
                base.OnBeforeItemInserted(index, item);
                if(OnBeforeItemInserted_Callback != null)
                    this.OnBeforeItemInserted_Callback(index, item);
            }

            protected override void OnAfterItemInserted(int index, string item)
            {
                base.OnAfterItemInserted(index, item);
                if(OnAfterItemInserted_Callback != null)
                    this.OnAfterItemInserted_Callback(index, item);
            }

            protected override void OnBeforeItemRemoved(int index, string item)
            {
                base.OnBeforeItemRemoved(index, item);
                if(OnBeforeItemRemoved_Callback != null)
                    this.OnBeforeItemRemoved_Callback(index, item);
            }

            protected override void OnAfterItemRemoved(int index, string item)
            {
                base.OnAfterItemRemoved(index, item);
                if(OnAfterItemRemoved_Callback != null)
                    this.OnAfterItemRemoved_Callback(index, item);
            }
        }

        class CallbacksStates
        {
            public bool Notified_before_inserted = false;
            public bool Notified_after_inserted = false;
            public bool Notified_before_removed = false;
            public bool Notified_after_removed = false;
        }

        [TestMethod]
        public void AddItem__OnItemInsertedRaised()
        {
            var col = new CallbackCollection();

            var callback_states = new CallbacksStates();

            col.OnBeforeItemInserted_Callback = (ix, obj) =>
                {
                    callback_states.Notified_before_inserted = true;
                    Assert.AreEqual(0, ix);
                    Assert.AreEqual("one", obj);
                };
            col.OnAfterItemInserted_Callback = (ix, obj) =>
                {
                    callback_states.Notified_after_inserted = true;
                    Assert.AreEqual(0, ix);
                    Assert.AreEqual("one", obj);
                };

            col.Add("one");

            Assert.IsTrue(callback_states.Notified_before_inserted);
            Assert.IsTrue(callback_states.Notified_after_inserted);
        }


        [TestMethod]
        public void AddRange__OnItemInsertedRaised()
        {
            var col = new CallbackCollection();

            var callback_states = new CallbacksStates();

            List<string> before_items = new List<string>();
            List<int> before_indexes = new List<int>();

            List<string> after_items = new List<string>();
            List<int> after_indexes = new List<int>();


            col.OnBeforeItemInserted_Callback = (ix, obj) =>
            {
                callback_states.Notified_before_inserted = true;
                before_indexes.Add(ix);
                before_items.Add(obj);
            };
            col.OnAfterItemInserted_Callback = (ix, obj) =>
            {
                callback_states.Notified_after_inserted = true;
                after_indexes.Add(ix);
                after_items.Add(obj);
            };

            col.AddRange(new string[] { "one", "two", "three"});

            Assert.IsTrue(callback_states.Notified_before_inserted);
            Assert.IsTrue(callback_states.Notified_after_inserted);

            CollectionAssert.AreEquivalent(new string[] { "one", "two", "three" }, before_items);
            CollectionAssert.AreEquivalent(new int[] { 0, 1, 2 }, before_indexes);

            CollectionAssert.AreEquivalent(new string[] { "one", "two", "three" }, after_items);
            CollectionAssert.AreEquivalent(new int[] { 0, 1, 2 }, after_indexes);
        }

        [TestMethod]
        public void RemoveItem__OnItemRemovedRaised()
        {
            var col = new CallbackCollection();

            var callback_states = new CallbacksStates();

            col.Add("one");

            col.OnBeforeItemRemoved_Callback = (ix, obj) =>
                {
                    callback_states.Notified_before_removed = true;
                    Assert.AreEqual(0, ix);
                    Assert.AreEqual("one", obj);
                };

            col.OnAfterItemRemoved_Callback = (ix, obj) =>
                {
                    callback_states.Notified_after_removed = true;
                    Assert.AreEqual(0, ix);
                    Assert.AreEqual("one", obj);
                };

            col.RemoveAt(0);

            Assert.IsTrue(callback_states.Notified_before_removed);
            Assert.IsTrue(callback_states.Notified_after_removed);
        }

        [TestMethod]
        public void RemoveRange__OnItemRemovedRaised()
        {
            var col = new CallbackCollection();

            var callback_states = new CallbacksStates();

            col.Add("one");
            col.Add("two");
            col.Add("three");
            col.Add("four");
            col.Add("five");


            List<string> before_removedItems = new List<string>();
            List<int> before_removedIndexes = new List<int>();

            List<string> after_removedItems = new List<string>();
            List<int> after_removedIndexes = new List<int>();

            col.OnBeforeItemRemoved_Callback = (ix, obj) =>
            {
                callback_states.Notified_before_removed = true;

                before_removedIndexes.Add(ix);
                before_removedItems.Add(obj);                
            };

            col.OnAfterItemRemoved_Callback = (ix, obj) =>
            {
                callback_states.Notified_after_removed = true;
                
                after_removedIndexes.Add(ix);
                after_removedItems.Add(obj);                
            };

            col.RemoveRange(1, 3);

            Assert.IsTrue(callback_states.Notified_before_removed);
            Assert.IsTrue(callback_states.Notified_after_removed);

            CollectionAssert.AreEquivalent(new string[] { "two", "three", "four" }, before_removedItems);
            CollectionAssert.AreEquivalent(new int[] { 1, 2, 3 }, before_removedIndexes);

            CollectionAssert.AreEquivalent(new string[] { "two", "three", "four" }, after_removedItems);
            CollectionAssert.AreEquivalent(new int[] { 1, 2, 3 }, after_removedIndexes);
        }


        [TestMethod]
        public void Reset__InsertedAndRemovedEventsRaised()
        {
            var col = new CallbackCollection();

            var callback_states = new CallbacksStates();

            col.Add("one");
            col.Add("two");
            col.Add("three");


            List<string> before_removedItems = new List<string>();
            List<int> before_removedIndexes = new List<int>();

            List<string> after_removedItems = new List<string>();
            List<int> after_removedIndexes = new List<int>();


            List<string> before_insertedItems = new List<string>();
            List<int> before_insertedIndexes = new List<int>();

            List<string> after_insertedItems = new List<string>();
            List<int> after_insertedIndexes = new List<int>();

            col.OnBeforeItemRemoved_Callback = (ix, obj) =>
            {
                callback_states.Notified_before_removed = true;

                before_removedIndexes.Add(ix);
                before_removedItems.Add(obj);
            };

            col.OnAfterItemRemoved_Callback = (ix, obj) =>
            {
                callback_states.Notified_after_removed = true;

                after_removedIndexes.Add(ix);
                after_removedItems.Add(obj);
            };

            col.OnBeforeItemInserted_Callback = (ix, obj) =>
            {
                callback_states.Notified_before_inserted = true;
                before_insertedIndexes.Add(ix);
                before_insertedItems.Add(obj);
            };
            col.OnAfterItemInserted_Callback = (ix, obj) =>
            {
                callback_states.Notified_after_inserted = true;
                after_insertedIndexes.Add(ix);
                after_insertedItems.Add(obj);
            };

            col.Reset(new[] { "four", "five", "six" });

            Assert.IsTrue(callback_states.Notified_before_removed);
            Assert.IsTrue(callback_states.Notified_after_removed);
            Assert.IsTrue(callback_states.Notified_before_inserted);
            Assert.IsTrue(callback_states.Notified_after_inserted);

            CollectionAssert.AreEquivalent(new string[] { "one", "two", "three" }, before_removedItems);
            CollectionAssert.AreEquivalent(new int[] { 0, 1, 2 }, before_removedIndexes);

            CollectionAssert.AreEquivalent(new string[] { "one", "two", "three" }, after_removedItems);
            CollectionAssert.AreEquivalent(new int[] { 0, 1, 2 }, after_removedIndexes);


            CollectionAssert.AreEquivalent(new string[] { "four", "five", "six" }, before_insertedItems);
            CollectionAssert.AreEquivalent(new int[] { 0, 1, 2 }, before_insertedIndexes);

            CollectionAssert.AreEquivalent(new string[] { "four", "five", "six" }, after_insertedItems);
            CollectionAssert.AreEquivalent(new int[] { 0, 1, 2 }, after_insertedIndexes);
        }
    }
}
