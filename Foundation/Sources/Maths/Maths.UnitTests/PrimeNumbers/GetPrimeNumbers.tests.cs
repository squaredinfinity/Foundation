//using Microsoft.VisualStudio.TestTools.UnitTesting;
//using SquaredInfinity.Maths.PrimeNumbers;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace Maths.UnitTests.PrimeNumbers
//{
//    [TestClass]
//    public class GetPrimeNumbers
//    {
//        [TestMethod]
//        public void Zero_One__Not_PrimeNumbers()
//        {
//            foreach (var imp in PrimeNumbersProvider.Providers)
//            {
//                var pf = imp.GetPrimeNumbers(0).ToArray();
//                Assert.AreEqual(0, pf.Length);

//                pf = imp.GetPrimeNumbers(1).ToArray();
//                Assert.AreEqual(0, pf.Length);
//            }
//        }

//        [TestMethod]
//        public void N_is_prime_number__N_is_included_in_result()
//        {
//            foreach (var imp in PrimeNumbersProvider.Providers)
//            {
//                var pf = imp.GetPrimeNumbers(13).ToArray();
//                Assert.AreEqual(6, pf.Length);
//                Assert.AreEqual(13, pf[5]);
//            }
//        }

//        [TestMethod]
//        public void General_test_cases()
//        {
//            foreach (var imp in PrimeNumbersProvider.Providers)
//            {
//                var pf = imp.GetPrimeNumbers(30).ToArray();

//                var expected = new uint[] { 2, 3, 5, 7, 11, 13, 17, 19, 23, 19 };

//                CollectionAssert.AreEqual(expected, pf);

//            }
//        }
//    }
//}
