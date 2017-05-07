using Microsoft.VisualStudio.TestTools.UnitTesting;
using SquaredInfinity.Clipboard;
using SquaredInfinity.Clipboard.Windows;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace Clipboard.Windows.UnitTests
{
    [TestClass]
    public class Clipboard
    {
        [TestMethod]
        public void MyTestMethod()
        {
            var cs = new WindowsClipboardService();

            var builder = cs.CreateClipboardBuilder();

            builder.SetHtml("<b>lol</b>");
            builder.SetText("lol");
            builder.CopyToClipboard();

            cs.Execute(builder);

            Assert.IsTrue(true);

            
        }
    }
}
