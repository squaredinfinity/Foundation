using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SquaredInfinity.Collections.Trees
{
    public class ExpressionTreeTraversingCollection : VirtualizingCollection<IExpressionTreeNode>
    {
        IExpressionTreeNode Root;
        IList<IExpressionTreeNode> Flattened;

        public ExpressionTreeTraversingCollection()
        {
            Flattened = new List<IExpressionTreeNode>(capacity: 0);
        }

        public void UpdateRoot(IExpressionTreeNode root, Func<IExpressionTreeNode, bool> filter = null)
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

        protected override IExpressionTreeNode GetItem(int index)
        {
            return Flattened[index];
        }

        public override bool Contains(IExpressionTreeNode item)
        {
            return Flattened.Contains(item);
        }

        public override int IndexOf(IExpressionTreeNode item)
        {
            return Flattened.IndexOf(item);
        }

        public override IEnumerator<IExpressionTreeNode> GetEnumerator()
        {
            for (int i = 0; i < Count; i++)
            {
                yield return GetItem(i);
            }
        }
    }
}
