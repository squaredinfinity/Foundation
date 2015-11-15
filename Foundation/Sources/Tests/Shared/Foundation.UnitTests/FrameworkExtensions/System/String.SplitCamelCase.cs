using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SquaredInfinity.Foundation.Extensions
{
    [TestClass]
    public class String__SplitCamelCase
    {
        [TestMethod]
        public void BUG__001_removes_non_alphanumeric_characters()
        {
            var ss = "BUG-001".SplitCamelCase();

            Assert.AreEqual("BUG-001", ss);
        }
    }
}
