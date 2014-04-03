using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SquaredInfinity.Foundation.Types.Description.Reflection
{
    [TestClass]
    public class ReflectionBasedTypeDescriptor__DescribeType
    {
        [TestMethod]
        public void TypeHasPropertyOfTheSameType()
        {
            var td = new ReflectionBasedTypeDescriptor();

            var description = td.DescribeType(typeof(TestEntities.TypeHasPropertyOfTheSameType));

            Assert.AreEqual(1, description.Members.Count);
        }
    }
}
