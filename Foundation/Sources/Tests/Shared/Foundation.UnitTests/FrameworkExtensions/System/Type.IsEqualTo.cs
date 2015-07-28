using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SquaredInfinity.Foundation.Extensions
{
    [TestClass]
    public class Type__IsEqualTo
    {
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Null__ThrowsArgumentNullException()
        {
            var t = typeof(Type__IsEqualTo);

            t.IsTypeEquivalentTo(null);
        }

        [TestMethod]
        public void ExactlyTheSameType__ReturnsTrue()
        {
            var t = typeof(Type__IsEqualTo);

            var r = t.IsTypeEquivalentTo(typeof(Type__IsEqualTo));

            Assert.IsTrue(r);
        }

        [TestMethod]
        public void NullableOfTheSameType_TreatNullableAsEquivalentIsTrue__ReturnsTrue()
        {
            var t = typeof(int);

            var r = t.IsTypeEquivalentTo(typeof(Nullable<int>), treatNullableAsEquivalent: true);

            Assert.IsTrue(r);
        }

        [TestMethod]
        public void NullableOfTheSameType_TreatNullableAsEquivalentIsFalse__ReturnsFalse()
        {
            var t = typeof(int);

            var r = t.IsTypeEquivalentTo(typeof(Nullable<int>), treatNullableAsEquivalent: false);

            Assert.IsFalse(r);
        }

        [TestMethod]
        public void NullableOfTheSameType_DefaultTreatNullable__ReturnsFalse()
        {
            var t = typeof(int);

            var r = t.IsTypeEquivalentTo(typeof(Nullable<int>));

            Assert.IsFalse(r);
        }
    }
}
