using Microsoft.VisualStudio.TestTools.UnitTesting;
using SquaredInfinity.Foundation.TestEntities;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SquaredInfinity.Foundation
{
    [TestClass]
    public class WeakEventTests
    {
        [TestMethod]
        public void RemoveHandlerCalledWhenDisposed()
        {
            TypeWithEvent t = new TypeWithEvent();

            var subscription = 
                t.CreateWeakEventHandler().ForEvent<EventHandler<TypeWithEvent.RandomNumberEventArgs>, TypeWithEvent.RandomNumberEventArgs>(
                (s,h) => 
                    {
                        // todo: assert called only once
                        s.AfterRandomNumberGenerated += h;
                    },
                (s, h) => 
                    {
                        // todo: assert called only once
                        s.AfterRandomNumberGenerated -= h;
                    })
                .Subscribe((s, args) =>
                {
                    // todo: assert called 3 times
                    Trace.WriteLine(":)");
                });

            t.GenerateNumber();
            t.GenerateNumber();
            t.GenerateNumber();


            subscription.Dispose();
        }
    }
}
