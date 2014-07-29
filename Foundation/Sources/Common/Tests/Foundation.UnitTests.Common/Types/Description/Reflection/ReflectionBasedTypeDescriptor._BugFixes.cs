using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SquaredInfinity.Foundation.Types.Description.Reflection
{
    [TestClass]
    public partial class ReflectionBasedTypeDescriptor__BugFixes
    {
        #region BUG 001

        [TestMethod]
        [Description("stack overflow when type being described is used is used as a property in another type")]
        public void Bug001__StackOverflowIssue()
        {
            var td = new ReflectionBasedTypeDescriptor();

            var description = td.DescribeType(typeof(Bug001_Class));

            Assert.AreEqual(1, description.Members.Count);
        }

        public class Bug001_Class
        {
            public class InternalClass
            {
                // having property of type Bug001_Class will cause stack overflow issue
                public Bug001_Class Proeprty { get; set; }
            }

            public InternalClass Property2 { get; set; }
        }

        #endregion

    }
}
