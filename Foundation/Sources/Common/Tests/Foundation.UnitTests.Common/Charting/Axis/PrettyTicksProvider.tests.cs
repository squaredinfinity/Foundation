using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SquaredInfinity.Foundation.Charting.Axis
{
    [TestClass]
    public class PrettyTicksProviderTests
    {
        [TestMethod]
        public void Test()
        {
            var x = 2.123456;

            new PrettyTicksProvider().GetTicks(2.123456789, 2.23456789, 10);
        }
    }
}
