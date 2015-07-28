using Microsoft.VisualStudio.TestTools.UnitTesting;
using SquaredInfinity.Foundation.Extensions;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SquaredInfinity.Foundation.Collections.Trees
{
    [TestClass]
    public class BooleanExpresionTree__Operations
    {
        //[TestMethod]
        //public void ConnectiveNode__CanInsertPredicateConnectiveAsLeftChildNode()
        //{
        //    var root = new PredicateConnectiveNode();
        //    var childConnective = new PredicateConnectiveNode();
        //    root.InsertLeft(childConnective);

        //    Assert.AreEqual(root.Left, childConnective);
        //    Assert.AreEqual(childConnective.Parent, root);
        //}

        //[TestMethod]
        //public void ConnectiveNode__CanInsertPredicateConnectiveAsRightChildNode()
        //{
        //    var root = new PredicateConnectiveNode();
        //    var childConnective = new PredicateConnectiveNode();
        //    root.InsertRight(childConnective);

        //    Assert.AreEqual(root.Right, childConnective);
        //    Assert.AreEqual(childConnective.Parent, root);
        //}

        //[TestMethod]
        //public void ConnectiveNode__CanInsertPredicateAsLeftChildNode()
        //{
        //    var root = new PredicateConnectiveNode();
        //    var childPredicate = new TestPredicateNode("");
        //    root.InsertLeft(childPredicate);

        //    Assert.AreEqual(root.Left, childPredicate);
        //    Assert.AreEqual(childPredicate.Parent, root);
        //}

        //[TestMethod]
        //public void ConnectiveNode__CanInsertPredicateAsRightChildNode()
        //{
        //    var root = new PredicateConnectiveNode();
        //    var childPredicate = new TestPredicateNode("");
        //    root.InsertRight(childPredicate);

        //    Assert.AreEqual(root.Right, childPredicate);
        //    Assert.AreEqual(childPredicate.Parent, root);
        //}

        [TestMethod]
        public void InjectInto__Insert__FunctionNode_Into_RightFunctionNode()
        {
            // start:
            //      (c1)
            //     /    \
            //    (c2)  (f1)
            //   /    \
            //  (f2)  (f3)

            var c1 = new TestConnectiveNode("c1");
            var c2 = new TestConnectiveNode("c2");
            var f1 = new TestPredicateNode("f1");
            var f2 = new TestPredicateNode("f2");
            var f3 = new TestPredicateNode("f3");

            c1.InsertLeft(c2);
            c1.InsertRight(f1);

            c2.InsertLeft(f2);
            c2.InsertRight(f3);

            var tree = new TestExpressionTree();
            tree.ReplaceRootAndSubtree(c1);

            // insert new predicate into f3

            var f4 = new TestPredicateNode("f4");

            tree.InjectInto(f4, f3);

            // end:
            //      (c1)
            //     /    \
            //    (c2)  (f1)
            //   /    \
            //  (f2)  (c3)
            //        /   \
            //     (f3)   (f4)

            Assert.AreSame(tree.Root, c1);
            Assert.AreSame(c1.Left, c2);
            Assert.AreSame(c1.Right, f1);
            Assert.AreSame(c2.Left, f2);

            var c3 = c2.Right;

            Assert.IsNotNull(c3);
            Assert.AreSame(c3.Left, f3);
            Assert.AreSame(c3.Right, f4);

        }

        [TestMethod]
        public void InjectInto__Insert__FunctionNode_Into_LeftFunctionNode()
        {
            // start:
            //      (c1)
            //     /    \
            //    (c2)  (f1)
            //   /    \
            //  (f2)  (f3)

            var c1 = new TestConnectiveNode("c1");
            var c2 = new TestConnectiveNode("c2");
            var f1 = new TestPredicateNode("f1");
            var f2 = new TestPredicateNode("f2");
            var f3 = new TestPredicateNode("f3");

            c1.InsertLeft(c2);
            c1.InsertRight(f1);

            c2.InsertLeft(f2);
            c2.InsertRight(f3);

            var tree = new TestExpressionTree();
            tree.ReplaceRootAndSubtree(c1);

            // insert new predicate into f2

            var f4 = new TestPredicateNode("f4");

            tree.InjectInto(f4, f2);

            // end:
            //        (c1)
            //       /    \
            //      (c2)  (f1)
            //     /    \
            //    (c3)  (f3)
            //    /   \
            //  (f4) (f2)

            Assert.AreSame(tree.Root, c1);
            Assert.AreSame(c1.Left, c2);
            Assert.AreSame(c1.Right, f1);
            Assert.AreSame(c2.Right, f3);

            var c3 = c2.Left;

            Assert.IsNotNull(c3);
            Assert.AreSame(c3.Left, f4);
            Assert.AreSame(c3.Right, f2);

        }

        [TestMethod]
        public void InjectInto__Move_FunctionNode_Into_LeftFunctionNode()
        {
            // start:
            //      (c1)
            //     /    \
            //    (c2)  (f1)
            //   /    \
            // (f2)  (f3)

            var c1 = new TestConnectiveNode("c1");
            var c2 = new TestConnectiveNode("c2");
            var f1 = new TestPredicateNode("f1");
            var f2 = new TestPredicateNode("f2");
            var f3 = new TestPredicateNode("f3");

            c1.InsertLeft(c2);
            c1.InsertRight(f1);

            c2.InsertLeft(f2);
            c2.InsertRight(f3);

            var tree = new TestExpressionTree();
            tree.ReplaceRootAndSubtree(c1);

            // move f1 to f2
            
            tree.InjectInto(f1, f2);

            // end:
            //    (c2)
            //   /    \
            //  (c3)  (f3)
            //  /   \
            //(f1) (f2)

            //! c1 is removed from the tree
            //! f1 takes a place of f2
            //! f2 is placed on the opposite side to f1

            Assert.AreSame(tree.Root, c2);
            Assert.AreSame(c2.Right, f3);

            var c3 = c2.Left;
            Assert.AreSame(c3.Parent, c2);


            Assert.AreSame(c3.Left, f1);
            Assert.AreSame(c3.Right, f2);
        }

        [TestMethod]
        public void InjectInto__Move_FunctionNode_Into_RightFunctionNode()
        {
            // start:
            //      (c1)
            //     /    \
            //    (c2)  (f1)
            //   /    \
            //  (f2)  (f3)

            var c1 = new TestConnectiveNode("c1");
            var c2 = new TestConnectiveNode("c2");
            var f1 = new TestPredicateNode("f1");
            var f2 = new TestPredicateNode("f2");
            var f3 = new TestPredicateNode("f3");

            c1.InsertLeft(c2);
            c1.InsertRight(f1);

            c2.InsertLeft(f2);
            c2.InsertRight(f3);

            var tree = new TestExpressionTree();
            tree.ReplaceRootAndSubtree(c1);

            // move f1 to f3

            tree.InjectInto(f1, f3);

            // end:
            //    (c2)
            //    /    \
            //  (f2)  (c3)
            //        /   \
            //      (f3) (f1)

            //! c1 is removed from the tree
            //! f1 takes a place of f3
            //! f3 is placed on the opposite side to f1

            Assert.AreSame(tree.Root, c2);
            Assert.AreSame(c2.Left, f2);

            var c3 = c2.Right;
            Assert.AreSame(c3.Parent, c2);


            Assert.AreSame(c3.Left, f3);
            Assert.AreSame(c3.Right, f1);
        }

        [TestMethod]
        public void InjectInto__Move_ConnectiveNode_Into_FunctionNode()
        {
            // start:
            //           (c1)
            //          /    \
            //        (c2)  (f1)
            //       /    \
            //     (c3)   (f2)
            //     /  \
            //  (f3)  (f4)

            var c1 = new TestConnectiveNode("c1");
            var c2 = new TestConnectiveNode("c2");
            var c3 = new TestConnectiveNode("c3");
            var f1 = new TestPredicateNode("f1");
            var f2 = new TestPredicateNode("f2");
            var f3 = new TestPredicateNode("f3");
            var f4 = new TestPredicateNode("f4");

            c1.InsertLeft(c2);
            c1.InsertRight(f1);

            c2.InsertLeft(c3);
            c2.InsertRight(f2);

            c3.InsertLeft(f3);
            c3.InsertRight(f4);

            var tree = new TestExpressionTree();
            tree.ReplaceRootAndSubtree(c1);

            // move c2 to f4

            tree.InjectInto(c2, f4);

            // end:
            //          (c1)
            //         /   \
            //       (c3)   (f1)
            //       /  \
            //     (f3) (c2)
            //          /  \
            //        (f4) (f2)

            Assert.AreSame(tree.Root, c1);
            Assert.AreSame(c1.Left, c3);
            Assert.AreSame(c1.Right, f1);

            Assert.AreSame(c3.Left, f3);
            Assert.AreSame(c3.Right, c2);
            Assert.AreSame(c2.Left, f4);
            Assert.AreSame(c2.Right, f2);
        }

        [TestMethod]
        public void InjectInto__Move_ConnectiveNode_Into_FunctionNode_SourceIsRoot()
        {
            // start:
            //           (c1)
            //          /    \
            //        (c2)  (f1)
            //       /    \
            //     (f2)   (f3)

            var c1 = new TestConnectiveNode("c1");
            var c2 = new TestConnectiveNode("c2");
            var f1 = new TestPredicateNode("f1");
            var f2 = new TestPredicateNode("f2");
            var f3 = new TestPredicateNode("f3");

            c1.InsertLeft(c2);
            c1.InsertRight(f1);

            c2.InsertLeft(f2);
            c2.InsertRight(f3);

            var tree = new TestExpressionTree();
            tree.ReplaceRootAndSubtree(c1);

            // move c1 to f3

            tree.InjectInto(c1, f3);

            // end:
            //          (c2)
            //         /   \
            //       (c1)   (f2)
            //       /  \
            //     (f3) (f1)

            Assert.AreSame(tree.Root, c2);
            Assert.AreSame(c2.Left, f2);
            Assert.AreSame(c2.Right, c1);

            Assert.AreSame(c1.Left, f3);
            Assert.AreSame(c1.Right, f1);
        }

        [TestMethod]
        public void InjectInto__FunctionNode_Move_SameParent__SwapsPlaces_LetftRight()
        {
            // start:
            //      (c1)
            //     /    \
            //   (p1)  (p2)

            var c1 = new TestConnectiveNode("c1");
            var p1 = new TestPredicateNode("p1");
            var p2 = new TestPredicateNode("p2");

            c1.InsertLeft(p1);
            c1.InsertRight(p2);

            var tree = new TestExpressionTree();
            tree.ReplaceRootAndSubtree(c1);

            // inject p1 into p2
            
            tree.InjectInto(p1, p2);

            // end:
            //      (c1)
            //     /    \
            //   (p2)  (p1)

            Assert.AreSame(tree.Root, c1);
            Assert.AreSame(c1.Left, p2);
            Assert.AreSame(c1.Right, p1);
        }

        [TestMethod]
        public void InjectInto__FunctionNode_Move_SameParent__SwapsPlaces_RightLeft()
        {
            // start:
            //      (c1)
            //     /    \
            //   (p1)  (p2)

            var c1 = new TestConnectiveNode("c1");
            var p1 = new TestPredicateNode("p1");
            var p2 = new TestPredicateNode("p2");

            c1.InsertLeft(p1);
            c1.InsertRight(p2);

            var tree = new TestExpressionTree();
            tree.ReplaceRootAndSubtree(c1);

            // inject p2 into p1

            tree.InjectInto(p2, p2);

            // end:
            //      (c1)
            //     /    \
            //   (p2)  (p1)

            Assert.AreSame(tree.Root, c1);
            Assert.AreSame(c1.Left, p2);
            Assert.AreSame(c1.Right, p1);
        }

        [TestMethod]
        public void InjectInto__Move_ConnectiveNode_Into_ItsLeftChild()
        {
            // start:
            //      (c1)
            //     /    \
            //   (p1)  (p2)

            var c1 = new TestConnectiveNode("c1");
            var p1 = new TestPredicateNode("p1");
            var p2 = new TestPredicateNode("p2");

            c1.InsertLeft(p1);
            c1.InsertRight(p2);

            var tree = new TestExpressionTree();
            tree.ReplaceRootAndSubtree(c1);

            // inject c1 into p1

            tree.InjectInto(c1, p1);

            // end: (no changes)
            //      (c1)
            //     /    \
            //   (p1)  (p2)

            Assert.AreSame(tree.Root, c1);
            Assert.AreSame(c1.Left, p1);
            Assert.AreSame(c1.Right, p2);
        }

        [TestMethod]
        public void InjectInto__Move_ConnectiveNode_Into_ItsRightChild()
        {
            // start:
            //      (c1)
            //     /    \
            //   (p1)  (p2)

            var c1 = new TestConnectiveNode("c1");
            var p1 = new TestPredicateNode("p1");
            var p2 = new TestPredicateNode("p2");

            c1.InsertLeft(p1);
            c1.InsertRight(p2);

            var tree = new TestExpressionTree();
            tree.ReplaceRootAndSubtree(c1);

            // inject c1 into p2

            tree.InjectInto(c1, p1);

            // end: (no changes)
            //      (c1)
            //     /    \
            //   (p1)  (p2)

            Assert.AreSame(tree.Root, c1);
            Assert.AreSame(c1.Left, p1);
            Assert.AreSame(c1.Right, p2);
        }
    }

    public class TestOperator : IBinaryOperator
    {
        public bool Evaluate(object payload, IExpressionTreeNode left, IExpressionTreeNode right)
        {
            throw new NotImplementedException();
        }

        public IBinaryOperator DeepClone()
        {
            return this.DeepClone<TestOperator>();
        }

        public bool Equals(IBinaryOperator other)
        {
            return base.Equals(other);
        }
    }

    public class TestExpressionTree : ExpressionTree
    {
        public override IPredicateConnectiveNode GetDefaultConnectiveNode()
        {
            return new TestConnectiveNode("[auto]") { Operator = new TestOperator() };
        }
    }

    [DebuggerDisplay("{DebuggerDisplay}")]
    public class TestPredicateNode : PredicateNode
    {
        public string Name { get; set; }

        public string DebuggerDisplay
        {
            get { return Name; }
        }

        public TestPredicateNode(string name)
        {
            this.Name = name;
        }

        public override bool Evaluate(object payload)
        {
            throw new NotImplementedException();
        }
    }

    [DebuggerDisplay("{DebuggerDisplay}")]
    public class TestConnectiveNode : PredicateConnectiveNode
    {
        public string Name { get; set; }

        public string DebuggerDisplay
        {
            get { return Name; }
        }

        public TestConnectiveNode(string name)
        {
            this.Name = name;
            this.Operator = new TestOperator();
        }
    }
}
