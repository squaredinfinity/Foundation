using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SquaredInfinity.Tagging;
using Newtonsoft.Json;

namespace Tagging.UnitTests
{
    [TestClass]
    public class TagCollection__Serialization__JSON
    {
        [TestMethod]
        public void no_value_specified__adds_marker_value()
        {
            var tc = new TagCollection();

            tc.Add("isSet");
            tc.Add("single", 13);
            tc.AddOrUpdate("multiple", 1);
            tc.AddOrUpdate("multiple", 2);
            tc.AddOrUpdate("multiple", 3);

            var s = JsonConvert.SerializeObject(tc);

            var tc2 = JsonConvert.DeserializeObject<TagCollection>(s);

            Assert.IsTrue(tc2.Contains("isSet"));
            var z = tc2["single"];
            var z2 = tc2["multiple"];
        }
    }
}
