using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SquaredInfinity.Foundation.Extensions
{
    [TestClass]
    public class ToTitleCaseTests
    {
        [TestMethod]
        public void ConvertsStringToTitleCaseUsingCurrnetUICuture()
        {
            var txt = "wAr aNd pEaCe";

            var expected = CultureInfo.CurrentUICulture.TextInfo.ToTitleCase(txt);
            var result = txt.ToTitleCase();

            Assert.AreEqual(expected, result);
        }
    }
}
