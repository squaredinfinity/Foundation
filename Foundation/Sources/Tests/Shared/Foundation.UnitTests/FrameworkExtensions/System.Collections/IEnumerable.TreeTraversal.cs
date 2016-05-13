using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SquaredInfinity.Foundation.Extensions;
using SquaredInfinity.Foundation.Maths.Graphs.Trees;

namespace SquaredInfinity.Foundation.Extensions
{

    // NOTE:    TreeTraversal methods do not have to be implemented as extension methods (they can be instance methods on type itself)
    //          This here is only to show that TreeTraversal can be used with external types which we cannot extend.
    public static class TreeNodeExtensions
    {
        public static IEnumerable<IEnumerableExtensions__TreeTraversal.TreeNode> TreeTraversal(
            this IEnumerableExtensions__TreeTraversal.TreeNode root)
        {
            return root.TreeTraversal(TreeTraversalMode.Default);
        }

        public static IEnumerable<IEnumerableExtensions__TreeTraversal.TreeNode> TreeTraversal(
            this IEnumerableExtensions__TreeTraversal.TreeNode root,
            TreeTraversalMode traversalMode)
        {
            foreach (var c in root.TreeTraversal(traversalMode, GetChildren))
                yield return c;
        }

        static IEnumerable<IEnumerableExtensions__TreeTraversal.TreeNode> GetChildren(
            IEnumerableExtensions__TreeTraversal.TreeNode root)
        {
            if (root == null)
                yield break;

            foreach (var c in root.Children)
                yield return c;
        }
    }

    [TestClass]
    public class IEnumerableExtensions__TreeTraversal
    {
        static void ValidateDefaultTestTreeTraversalResult(
                IReadOnlyList<ITreeNode> nodes,
                TreeTraversalMode traversalMode = TreeTraversalMode.Default)
        {
            Assert.AreEqual(13, nodes.Count);

            if (traversalMode == TreeTraversalMode.BreadthFirst)
            {
                Assert.AreEqual("1", nodes[0].Id);
                Assert.AreEqual("1.1", nodes[1].Id);
                Assert.AreEqual("1.2", nodes[2].Id);
                Assert.AreEqual("1.3", nodes[3].Id);
                Assert.AreEqual("1.1.1", nodes[4].Id);
                Assert.AreEqual("1.1.2", nodes[5].Id);
                Assert.AreEqual("1.1.3", nodes[6].Id);
                Assert.AreEqual("1.2.1", nodes[7].Id);
                Assert.AreEqual("1.2.2", nodes[8].Id);
                Assert.AreEqual("1.2.3", nodes[9].Id);
                Assert.AreEqual("1.3.1", nodes[10].Id);
                Assert.AreEqual("1.3.2", nodes[11].Id);
                Assert.AreEqual("1.3.3", nodes[12].Id);
            }
            else if (traversalMode == TreeTraversalMode.DepthFirst)
            {
                Assert.AreEqual("1", nodes[0].Id);
                Assert.AreEqual("1.1", nodes[1].Id);
                Assert.AreEqual("1.1.1", nodes[2].Id);
                Assert.AreEqual("1.1.2", nodes[3].Id);
                Assert.AreEqual("1.1.3", nodes[4].Id);
                Assert.AreEqual("1.2", nodes[5].Id);
                Assert.AreEqual("1.2.1", nodes[6].Id);
                Assert.AreEqual("1.2.2", nodes[7].Id);
                Assert.AreEqual("1.2.3", nodes[8].Id);
                Assert.AreEqual("1.3", nodes[9].Id);
                Assert.AreEqual("1.3.1", nodes[10].Id);
                Assert.AreEqual("1.3.2", nodes[11].Id);
                Assert.AreEqual("1.3.3", nodes[12].Id);
            }
            else if (traversalMode == TreeTraversalMode.BottomUp)
            {
                Assert.AreEqual("1.1.1", nodes[0].Id);
                Assert.AreEqual("1.1.2", nodes[1].Id);
                Assert.AreEqual("1.1.3", nodes[2].Id);
                Assert.AreEqual("1.2.1", nodes[3].Id);
                Assert.AreEqual("1.2.2", nodes[4].Id);
                Assert.AreEqual("1.2.3", nodes[5].Id);
                Assert.AreEqual("1.3.1", nodes[6].Id);
                Assert.AreEqual("1.3.2", nodes[7].Id);
                Assert.AreEqual("1.3.3", nodes[8].Id);
                Assert.AreEqual("1.1", nodes[9].Id);
                Assert.AreEqual("1.2", nodes[10].Id);
                Assert.AreEqual("1.3", nodes[11].Id);
                Assert.AreEqual("1", nodes[12].Id);
            }
            else
            {
                Assert.Fail("cannot validate traversal mode");
            }

        }

        public interface ITreeNode
        {
            string Id { get; }
        }

        public class EnumerableTreeNode : List<EnumerableTreeNode>, ITreeNode
        {
            public string Id { get; set; }

            public static EnumerableTreeNode GetDefaultTestTree()
            {
                var root = new EnumerableTreeNode { Id = "1" };

                for(int i = 1; i <= 3; i++)
                {
                    var c1 = new EnumerableTreeNode { Id = "1." + i.ToString() };

                    for(int ii = 1; ii <= 3; ii++)
                    {
                        var c2 = new EnumerableTreeNode { Id = "1." + i.ToString() + "." + ii.ToString() };

                        c1.Add(c2);
                    }

                    root.Add(c1);
                }

                return root;
            }
        }

        public class TreeNode : ITreeNode
        {
            public string Id { get; set; }

            public List<TreeNode> Children = new List<TreeNode>();

            public static TreeNode GetDefaultTestTree()
            {
                var root = new TreeNode { Id = "1" };

                for (int i = 1; i <= 3; i++)
                {
                    var c1 = new TreeNode { Id = "1." + i.ToString() };

                    for (int ii = 1; ii <= 3; ii++)
                    {
                        var c2 = new TreeNode { Id = "1." + i.ToString() + "." + ii.ToString() };

                        c1.Children.Add(c2);
                    }

                    root.Children.Add(c1);
                }

                return root;
            }
        }

        [TestMethod]
        public void TreeTraversal__TraversingEnumerableReturnsAllItsElements()
        {
            var list = new List<int> { 1, 2, 3 };

            var r1 = list.TreeTraversal().ToArray();

            Assert.AreEqual(3, r1.Length);
            Assert.AreEqual(1, r1[0]);
            Assert.AreEqual(2, r1[1]);
            Assert.AreEqual(3, r1[2]);

            var array = new int[] { 1, 2, 3 };

            var r2 = array.TreeTraversal().ToArray();

            Assert.AreEqual(3, r2.Length);
            Assert.AreEqual(1, r2[0]);
            Assert.AreEqual(2, r2[1]);
            Assert.AreEqual(3, r2[2]);
        }

        [TestMethod]
        public void TreeTraversal__ByDefaultCanTraverseEnumerableTreeNodes()
        {
            var t = EnumerableTreeNode.GetDefaultTestTree();

            var items = t.TreeTraversal(TreeTraversalMode.BreadthFirst).ToArray();
            ValidateDefaultTestTreeTraversalResult(items, TreeTraversalMode.BreadthFirst);

            items = t.TreeTraversal(TreeTraversalMode.DepthFirst).ToArray();
            ValidateDefaultTestTreeTraversalResult(items, TreeTraversalMode.DepthFirst);

            items = t.TreeTraversal(TreeTraversalMode.BottomUp).ToArray();
            ValidateDefaultTestTreeTraversalResult(items, TreeTraversalMode.BottomUp);
        }

        [TestMethod]
        public void TreeTraversal__CanTraverseEnumerableTreeNodes()
        {
            var t = TreeNode.GetDefaultTestTree();

            var items = t.TreeTraversal(TreeTraversalMode.BreadthFirst).ToArray();
            ValidateDefaultTestTreeTraversalResult(items, TreeTraversalMode.BreadthFirst);

            items = t.TreeTraversal(TreeTraversalMode.DepthFirst).ToArray();
            ValidateDefaultTestTreeTraversalResult(items, TreeTraversalMode.DepthFirst);

            items = t.TreeTraversal(TreeTraversalMode.BottomUp).ToArray();
            ValidateDefaultTestTreeTraversalResult(items, TreeTraversalMode.BottomUp);
        }
    }
}
