using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SquaredInfinity.Foundation
{
    [TestClass]
    public class TryHandleTests
    {
        class Test
        {
            public event EventHandler<CommandHandlerEventArgs> TryHandleCommand;

            public bool TryHandle()
            {
                var args = new CommandHandlerEventArgs<int>();

                return TryHandleCommand.TryHandle(args);
            }
        }

        [TestMethod]
        public void CommandHasBeenHandled__ReturnsTrue()
        {
            var t = new Test();
            t.TryHandleCommand += (s, e) => { };
            t.TryHandleCommand += (s, e) => { e.Handle(); };
            t.TryHandleCommand += (s, e) => { Assert.Fail("should not be called, previous event handler handles the command."); };

            var r = t.TryHandle();

            Assert.IsTrue(r);
        }

        [TestMethod]
        public void CommandHasNotBeenHandled__ReturnsFalse()
        {
            var t = new Test();
            t.TryHandleCommand += (s, e) => { };
            t.TryHandleCommand += (s, e) => { };

            var r = t.TryHandle();

            Assert.IsFalse(r);
        }
    }
}
