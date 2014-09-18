using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SquaredInfinity.Foundation.Collections.Trees
{
    public class BooleanExpressionTreeTraversingCollection : VirtualizingCollection<IBooleanExpressionTreeNode>
    {
        IBooleanExpressionTreeNode Root;
        IList<IBooleanExpressionTreeNode> Flattened;

        public BooleanExpressionTreeTraversingCollection()
        {
            Flattened = new List<IBooleanExpressionTreeNode>(capacity: 0);
        }

        public void UpdateRoot(IBooleanExpressionTreeNode root, Func<IBooleanExpressionTreeNode, bool> filter = null)
        {
            this.Root = root;

            if (root != null)
            {
                var flattened = root.TraverseTreeInOrder();

                if (filter != null)
                    flattened = flattened.Where(filter);

                Flattened = flattened.ToList();
            }
            else
            {
                Flattened.Clear();
            }
        }

        protected override int GetCount()
        {
            return Flattened.Count;
        }

        protected override void SetCount(int newCount)
        {
            throw new NotSupportedException();
        }

        protected override IBooleanExpressionTreeNode GetItem(int index)
        {
            return Flattened[index];
        }

        public override bool Contains(IBooleanExpressionTreeNode item)
        {
            return Flattened.Contains(item);
        }

        public override int IndexOf(IBooleanExpressionTreeNode item)
        {
            return Flattened.IndexOf(item);
        }

        public override IEnumerator<IBooleanExpressionTreeNode> GetEnumerator()
        {
            for (int i = 0; i < Count; i++)
            {
                yield return GetItem(i);
            }
        }
    }
}
