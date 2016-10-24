using Microsoft.VisualStudio.TestTools.UnitTesting;
using SquaredInfinity.Foundation.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SquaredInfinity.Foundation.Maths.Parsing
{
    [TestClass]
    public class ColonOperatorTests
    {
        [TestMethod]
        public void CanParseColonOperatorExpression()
        {
            var x = 2.2 + 0.2;

            x.IsCloseTo(2.4);

            var cop = new ColonOperator();

            var ar = cop.CreateArray(0.2, 0.2, 15000);

            Assert.AreEqual(15000.0 / 0.2, ar.Length + 1);

            var check = 0.2;

            for(int i = 0; i < ar.Length; i++)
            {
                    Assert.IsTrue(check.IsCloseTo(ar[i]));
                check += 0.2;
                check = check.RoundToMaxSupportedSignificantFigures();
            }
        }
    }
}
