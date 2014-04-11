using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SquaredInfinity.Foundation.TestEntities
{
    [DebuggerDisplay("{Id}")]
    public class LinkedListNode
    {
        public int Id { get; set; }
        public LinkedListNode Previous { get; set; }
        public LinkedListNode Next { get; set; }


        public static LinkedListNode CreateDefaultTestHierarchy()
        {
            var n0 = new LinkedListNode { Id = 0 };
            var n1 = new LinkedListNode { Id = 1 };
            var n2 = new LinkedListNode { Id = 2 };
            var n3 = new LinkedListNode { Id = 3 };

            n0.Next = n1;
            n1.Next = n2;
            n2.Next = n3;
            n3.Next = n0;

            n0.Previous = n3;
            n1.Previous = n0;
            n2.Previous = n1;
            n3.Previous = n2;

            return n3;
        }

        public static void EnsureDefaultTestHierarchyPreserved(LinkedListNode node)
        {
            Assert.AreEqual(3, node.Id);

            Assert.AreEqual(2, node.Previous.Id);
            Assert.AreEqual(1, node.Previous.Previous.Id);
            Assert.AreEqual(0, node.Previous.Previous.Previous.Id);
            Assert.AreEqual(3, node.Previous.Previous.Previous.Previous.Id);

            Assert.AreEqual(0, node.Next.Id);
            Assert.AreEqual(1, node.Next.Next.Id);
            Assert.AreEqual(2, node.Next.Next.Next.Id);
            Assert.AreEqual(3, node.Next.Next.Next.Next.Id);

            Assert.AreSame(node, node.Next.Next.Next.Next);
            Assert.AreSame(node, node.Previous.Previous.Previous.Previous);
        }
    }
}
