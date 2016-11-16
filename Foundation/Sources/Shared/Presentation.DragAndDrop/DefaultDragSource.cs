using System.Linq;
using System.Windows;
using SquaredInfinity.Foundation.Presentation.DragDrop.Utilities;

namespace SquaredInfinity.Foundation.Presentation.DragDrop
{
    public class DefaultDragSource : IDragSource
    {
        public virtual void StartDrag(IDragInfo dragInfo)
        {
            var itemCount = dragInfo.SourceItems.Cast<object>().Count();

            if (itemCount == 1)
            {
                dragInfo.Data = dragInfo.SourceItems.Cast<object>().First();
            }
            else if (itemCount > 1)
            {
                dragInfo.Data = dragInfo.SourceItems;
            }

            if (dragInfo.Data == null)
                dragInfo.AllowedEffects = DragDropEffects.None;
            else
                dragInfo.AllowedEffects = DragDropEffects.Copy | DragDropEffects.Move;
        }

        public bool CanStartDrag(IDragInfo dragInfo)
        {
            return true;
        }

        public virtual void Dropped(IDropInfo dropInfo)
        {
            // nothing to do here
        }

        public virtual void DragCancelled()
        {
            // nothing to do here
        }
    }
}