using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SquaredInfinity.Foundation.Collections.Trees
{
    [TestClass]
    public class BooleanExpresionTree__Operations
    {
        [TestMethod]
        public void ConnectiveNode__CanInsertPredicateConnectiveAsLeftChildNode()
        {
            var root = new PredicateConnectiveNode();
            var childConnective = new PredicateConnectiveNode();
            root.InsertLeft(childConnective);

            Assert.AreEqual(root.Left, childConnective);
            Assert.AreEqual(childConnective.Parent, root);
        }

        [TestMethod]
        public void ConnectiveNode__CanInsertPredicateConnectiveAsRightChildNode()
        {
            var root = new PredicateConnectiveNode();
            var childConnective = new PredicateConnectiveNode();
            root.InsertRight(childConnective);

            Assert.AreEqual(root.Right, childConnective);
            Assert.AreEqual(childConnective.Parent, root);
        }

        [TestMethod]
        public void ConnectiveNode__CanInsertPredicateAsLeftChildNode()
        {
            //var root = new PredicateConnectiveNode();
            //var childPredicate = new PredicateNode();
            //root.InsertLeft(childPredicate);

            //Assert.AreEqual(root.Left, childPredicate);
            //Assert.AreEqual(childPredicate.Parent, root);
        }

        [TestMethod]
        public void ConnectiveNode__CanInsertPredicateAsRightChildNode()
        {
            //var root = new PredicateConnectiveNode();
            //var childPredicate = new PredicateNode();
            //root.InsertRight(childPredicate);

            //Assert.AreEqual(root.Right, childPredicate);
            //Assert.AreEqual(childPredicate.Parent, root);
        }


    }
}
