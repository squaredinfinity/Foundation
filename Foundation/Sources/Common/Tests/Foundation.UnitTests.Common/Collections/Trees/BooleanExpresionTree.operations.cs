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
            var root = new PredicateConnectiveNode();
            var childPredicate = new TestPredicateNode();
            root.InsertLeft(childPredicate);

            Assert.AreEqual(root.Left, childPredicate);
            Assert.AreEqual(childPredicate.Parent, root);
        }

        [TestMethod]
        public void ConnectiveNode__CanInsertPredicateAsRightChildNode()
        {
            var root = new PredicateConnectiveNode();
            var childPredicate = new TestPredicateNode();
            root.InsertRight(childPredicate);

            Assert.AreEqual(root.Right, childPredicate);
            Assert.AreEqual(childPredicate.Parent, root);
        }

        [TestMethod]
        public void InjectInto__PredicateInsertion_1()
        {
            // start:
            //      (c1)
            //     /    \
            //    (c2)  (p1)
            //   /    \
            //  (p2)  (p3)

            var c1 = new PredicateConnectiveNode();
            var c2 = new PredicateConnectiveNode();
            var p1 = new TestPredicateNode();
            var p2 = new TestPredicateNode();
            var p3 = new TestPredicateNode();

            c1.InsertLeft(c2);
            c1.InsertRight(p1);

            c2.InsertLeft(p2);
            c2.InsertRight(p3);

            var tree = new BooleanExpressionTree();
            tree.ReplaceRootAndSubtree(c1);

            // insert new predicate into p3

            var p4 = new TestPredicateNode();

            tree.InjectInto(p4, p3);

            // end:
            //      (c1)
            //     /    \
            //    (c2)  (p1)
            //   /    \
            //  (p2)  (c3)
            //        /   \
            //     (p3)   (p4)

            Assert.AreSame(tree.Root, c1);
            Assert.AreSame(c1.Left, c2);
            Assert.AreSame(c1.Right, p1);
            Assert.AreSame(c2.Left, p2);

            var c3 = c2.Right;

            Assert.IsNotNull(c3);
            Assert.AreSame(c3.Left, p3);
            Assert.AreSame(c3.Right, p4);

        }

        [TestMethod]
        public void InjectInto__PredicateInsertion_1()
        {
            // start:
            //      (c1)
            //     /    \
            //    (c2)  (p1)
            //   /    \
            //  (p2)  (p3)

            var c1 = new PredicateConnectiveNode();
            var c2 = new PredicateConnectiveNode();
            var p1 = new TestPredicateNode();
            var p2 = new TestPredicateNode();
            var p3 = new TestPredicateNode();

            c1.InsertLeft(c2);
            c1.InsertRight(p1);

            c2.InsertLeft(p2);
            c2.InsertRight(p3);

            var tree = new BooleanExpressionTree();
            tree.ReplaceRootAndSubtree(c1);

            // insert new predicate into p3

            var p4 = new TestPredicateNode();

            tree.InjectInto(p4, p3);

            // end:
            //      (c1)
            //     /    \
            //    (c2)  (p1)
            //   /    \
            //  (p2)  (c3)
            //        /   \
            //     (p3)   (p4)

            Assert.AreSame(tree.Root, c1);
            Assert.AreSame(c1.Left, c2);
            Assert.AreSame(c1.Right, p1);
            Assert.AreSame(c2.Left, p2);

            var c3 = c2.Right;

            Assert.IsNotNull(c3);
            Assert.AreSame(c3.Left, p3);
            Assert.AreSame(c3.Right, p4);

        }
    }

    public class TestPredicateNode : PredicateNode
    {
        public override bool Evaluate(object payload)
        {
            throw new NotImplementedException();
        }
    }
}
