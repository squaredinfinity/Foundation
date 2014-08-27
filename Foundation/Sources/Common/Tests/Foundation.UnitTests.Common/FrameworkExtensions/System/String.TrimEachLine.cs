using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SquaredInfinity.Foundation.Extensions
{
    [TestClass]
    public class String__TrimEachLine
    {
        [TestMethod]
        public void NonEmptyMultilineString__TrimsEachLine_RemovesEmptyLines()
        {
            var input =
                "  \t  "
                + Environment.NewLine +
                " abc "
                + Environment.NewLine +
                " 123"
                + Environment.NewLine +
                "  \t  "
                + Environment.NewLine +
                "\tone two three\t";

            var result = input.TrimEachLine();

            var expected =
                "abc"
                + Environment.NewLine +
                "123"
                + Environment.NewLine +
                "one two three";

            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void NonEmptyMultilineString_RemoveEmptyLinesSetToFale__TrimsEachLine_KeepsEmptyLines()
        {
            var input =
                "  \t  " 
                + Environment.NewLine +
                " abc "
                + Environment.NewLine +
                " 123"
                + Environment.NewLine +
                "  \t  "
                + Environment.NewLine +
                "\tone two three\t";

            var result = input.TrimEachLine(removeEmptyLines:false);

            var expected =
                Environment.NewLine +
                "abc"
                + Environment.NewLine +
                "123"
                + Environment.NewLine
                + Environment.NewLine +
                "one two three";

            Assert.AreEqual(expected, result);
        }
    }
}
