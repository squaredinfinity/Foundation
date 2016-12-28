using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SquaredInfinity.Foundation.Maths.Parsing;

namespace Maths.Parsing.UnitTests
{
    [TestClass]
    public class ColonOperator__CreateArray
    {
        [TestMethod]
        public void j_k()
        {
            var op = new ColonOperator();
            var a = op.CreateArray(-10, 10);

            var v = -10;

            for(int i = 0; i < a.Length; i++)
            {
                Assert.AreEqual(v++, a[i]);
            }
        }
    }
}
