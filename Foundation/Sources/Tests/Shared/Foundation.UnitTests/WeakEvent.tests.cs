using Microsoft.VisualStudio.TestTools.UnitTesting;
using SquaredInfinity.Foundation.TestEntities;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SquaredInfinity.Foundation.Extensions;

namespace SquaredInfinity.Foundation
{
    [TestClass]
    public class WeakEventTests
    {
        [TestMethod]
        public void RemoveHandlerCalledWhenDisposed()
        {
            TypeWithEvent t = new TypeWithEvent();

            int subscribe_count = 0;
            int add_count = 0;
            int remove_count = 0;

            var subscription = 
                t.CreateWeakEventHandler().ForEvent<EventHandler<TypeWithEvent.RandomNumberEventArgs>, TypeWithEvent.RandomNumberEventArgs>(
                (s,h) => 
                    {
                        add_count++;
                        s.AfterRandomNumberGenerated += h;
                    },
                (s, h) => 
                    {
                        remove_count++;
                        s.AfterRandomNumberGenerated -= h;
                    })
                .Subscribe((s, args) =>
                {
                    subscribe_count++;
                });

            t.GenerateNumber();
            t.GenerateNumber();
            t.GenerateNumber();


            subscription.Dispose();

            GC.Collect(GC.MaxGeneration, GCCollectionMode.Forced);
            GC.WaitForPendingFinalizers();

            Assert.AreEqual(3, subscribe_count);
            Assert.AreEqual(1, add_count);
            Assert.AreEqual(1, remove_count);

            t.GenerateNumber();

            Assert.AreEqual(3, subscribe_count);
            Assert.AreEqual(1, add_count);
            Assert.AreEqual(1, remove_count);
        }
    }
}
