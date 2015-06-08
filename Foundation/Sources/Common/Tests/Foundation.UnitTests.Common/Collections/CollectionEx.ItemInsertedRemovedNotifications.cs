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
                this.OnBeforeItemInserted_Callback(index, item);
            }

            protected override void OnAfterItemInserted(int index, string item)
            {
                base.OnAfterItemInserted(index, item);
                this.OnAfterItemInserted_Callback(index, item);
            }

            protected override void OnBeforeItemRemoved(int index, string item)
            {
                base.OnBeforeItemRemoved(index, item);
                this.OnBeforeItemRemoved_Callback(index, item);
            }

            protected override void OnAfterItemRemoved(int index, string item)
            {
                base.OnAfterItemRemoved(index, item);
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

            col.OnBeforeItemInserted_Callback = (ix, obj) => callback_states.Notified_before_inserted = true;
            col.OnAfterItemInserted_Callback = (ix, obj) => callback_states.Notified_after_inserted = true;

            col.Add("one");

            Assert.IsTrue(callback_states.Notified_before_inserted);
            Assert.IsTrue(callback_states.Notified_after_inserted);
        }
    }
}
