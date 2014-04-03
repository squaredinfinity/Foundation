using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SquaredInfinity.Foundation.Extensions
{
    [TestClass]
    public class IsEqualToTests
    {
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Null__ThrowsArgumentNullException()
        {
            var t = typeof(IsEqualToTests);

            t.IsTypeEquivalentTo(null);
        }

        [TestMethod]
        public void ExactlyTheSameType__ReturnsTrue()
        {
            var t = typeof(IsEqualToTests);

            var r = t.IsTypeEquivalentTo(typeof(IsEqualToTests));

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
