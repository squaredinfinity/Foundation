using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SquaredInfinity.Foundation.Collections
{
	[TestClass]
    public class CollectionEx__VersionChanged
    {
        [TestMethod]
        public void Add__TriggersVersionChanged()
        {
            var c = new CollectionEx<int>();

            var eventCalled = false;
            var lastVersion = c.Version;

            c.VersionChanged += (s, e) =>
                {
                    eventCalled = true;
                };

            c.Add(13);

            Assert.IsTrue(eventCalled);
            Assert.AreNotEqual(lastVersion, c.Version);
        }

        [TestMethod]
        public void Remove__TriggersVersionChanged()
        {
            var c = new CollectionEx<int>();

            c.Add(13);

            var eventCalled = false;
            var lastVersion = c.Version;

            c.VersionChanged += (s, e) =>
            {
                eventCalled = true;
            };

            c.Remove(13);

            Assert.IsTrue(eventCalled);
            Assert.AreNotEqual(lastVersion, c.Version);
        }

        [TestMethod]
        public void RemoveAt__TriggersVersionChangedEvent()
        {
            var c = new CollectionEx<int>();

            c.Add(13);

            var eventCalled = false;
            var lastVersion = c.Version;

            c.VersionChanged += (s, e) =>
            {
                eventCalled = true;
            };

            c.RemoveAt(0);

            Assert.IsTrue(eventCalled);
            Assert.AreNotEqual(lastVersion, c.Version);
        }

        [TestMethod]
        public void Move__TriggersVersionChangedEvent()
        {
            var c = new CollectionEx<int>();

            c.Add(13);
            c.Add(7);

            var eventCalled = false;
            var lastVersion = c.Version;

            c.VersionChanged += (s, e) =>
            {
                eventCalled = true;
            };

            c.Move(1, 0);

            Assert.IsTrue(eventCalled);
            Assert.AreNotEqual(lastVersion, c.Version);
            Assert.AreEqual(7, c[0]);
            Assert.AreEqual(13, c[1]);
        }

        [TestMethod]
        public void Clear__TriggersVersionChangedEvent()
        {
            var c = new CollectionEx<int>();

            c.Add(13);
            c.Add(7);

            var eventCalled = false;
            var lastVersion = c.Version;

            c.VersionChanged += (s, e) =>
            {
                eventCalled = true;
            };

            c.Clear();

            Assert.IsTrue(eventCalled);
            Assert.AreNotEqual(lastVersion, c.Version);
            Assert.AreEqual(0, c.Count);
        }

        public void Replace__TriggersVersionChangedEvent()
        {
            var c = new CollectionEx<int>();

            c.Add(13);
            c.Add(7);

            var eventCalled = false;
            var lastVersion = c.Version;

            c.VersionChanged += (s, e) =>
            {
                eventCalled = true;
            };

            c.Replace(13, 99);

            Assert.IsTrue(eventCalled);
            Assert.AreNotEqual(lastVersion, c.Version);
            Assert.AreEqual(2, c.Count);
        }

        [TestMethod]
        public void IndexerSet__TriggersVersionChangedEvent()
        {
            var c = new CollectionEx<int>();

            c.Add(13);

            var eventCalled = false;
            var lastVersion = c.Version;

            c.VersionChanged += (s, e) =>
            {
                eventCalled = true;
            };

            c[0] = 7;

            Assert.IsTrue(eventCalled);
            Assert.AreNotEqual(lastVersion, c.Version);
            Assert.AreEqual(7, c[0]);
            Assert.AreEqual(1, c.Count);
        }

        [TestMethod]
        public void IndexerGet__DoesNotTriggerVersionChangedEvent()
        {
            var c = new CollectionEx<int>();

            c.Add(13);

            var eventCalled = false;
            var lastVersion = c.Version;

            c.VersionChanged += (s, e) =>
            {
                eventCalled = true;
            };

            var x = c[0];

            Assert.IsFalse(eventCalled);
            Assert.AreEqual(lastVersion, c.Version);
        }

        [TestMethod]
        public void Reset__TriggersVersionChangedEvent()
        {
            var c = new CollectionEx<int>();

            c.Add(13);

            var eventCalled = false;
            var lastVersion = c.Version;

            c.VersionChanged += (s, e) =>
            {
				// version changed should be triggered only once

                if (eventCalled)
                    Assert.Fail("version changed should be triggered only once during reset");

                eventCalled = true;
            };

            c.Reset(new int[] { 1, 2, 3 });

            Assert.IsTrue(eventCalled);
            Assert.AreNotEqual(lastVersion, c.Version);
            Assert.AreEqual(3, c.Count);
        }

        [TestMethod]
        public void AddRange__TriggersVersionChangedEvent()
        {
            var c = new CollectionEx<int>();

            c.Add(13);

            var eventCalled = false;
            var lastVersion = c.Version;

            c.VersionChanged += (s, e) =>
            {
                // version changed should be triggered only once

                if (eventCalled)
                    Assert.Fail("version changed should be triggered only once during AddRange");

                eventCalled = true;
            };

            c.AddRange(new int[] { 1, 2, 3 });

            Assert.IsTrue(eventCalled);
            Assert.AreNotEqual(lastVersion, c.Version);
            Assert.AreEqual(4, c.Count);
        }

        [TestMethod]
        public void RemoveRange__TriggersVersionChangedEvent()
        {
            var c = new CollectionEx<int>();

            c.Add(13);
            c.Add(7);
            c.Add(0);

            var eventCalled = false;
            var lastVersion = c.Version;

            c.VersionChanged += (s, e) =>
            {
                // version changed should be triggered only once

                if (eventCalled)
                    Assert.Fail("version changed should be triggered only once during RemoveRange");

                eventCalled = true;
            };

            c.RemoveRange(0, 2);

            Assert.IsTrue(eventCalled);
            Assert.AreNotEqual(lastVersion, c.Version);
            Assert.AreEqual(1, c.Count);
        }

        [TestMethod]
        public void Constructor__DoesNotTriggerVersionChangedEvent()
        {
            var c = new CollectionEx<int>(new int[] { 1, 2, 3, 4 });

            Assert.AreEqual(0, c.Version);
            Assert.AreEqual(4, c.Count);
        }
    }
}
