using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using SquaredInfinity.Foundation.Extensions;
using System.Diagnostics;
using System.Windows.Media;
using System.Windows;
using System.Collections.Specialized;

namespace WPF
{
    public class BackgroundLoadingListView : ListView
    {
        public  BackgroundRenderingService RS = new BackgroundRenderingService();

        public BackgroundLoadingListView()
        {
            RS.AfterItemRendered += RS_AfterItemRendered;
            RS.Start();
        }

        protected override void OnItemsSourceChanged(System.Collections.IEnumerable oldValue, System.Collections.IEnumerable newValue)
        {
            base.OnItemsSourceChanged(oldValue, newValue);

            var cc_old = oldValue as INotifyCollectionChanged;

            if (cc_old != null)
            {
                cc_old.CollectionChanged -= cc_CollectionChanged;
            }

            var cc_new = newValue as INotifyCollectionChanged;

            if (cc_new != null)
            {
                cc_new.CollectionChanged += cc_CollectionChanged;
            }

            int priority = IsVisible ? 1 : 2;

            RS.RequestRender(priority, this);
        }

        void cc_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            Trace.WriteLine("collection changed : " + e.Action.ToString());

            if (e.Action != NotifyCollectionChangedAction.Remove)
            {
                int priority = IsVisible ? 1 : 2;

                RS.RequestRender(priority, this);
            }
        }

        protected override Size MeasureOverride(Size constraint)
        {
            return base.MeasureOverride(constraint);
        }

        protected override Size ArrangeOverride(Size arrangeBounds)
        {
            return base.ArrangeOverride(arrangeBounds);
        }

        void RS_AfterItemRendered(object sender, BackgroundRenderingService.AfterItemRenderedEventArgs e)
        {
            Trace.WriteLine("rendered, {0} in queue.".FormatWith(RS.RemainingQueueSize));
        }

        protected override System.Windows.DependencyObject GetContainerForItemOverride()
        {
            return new BackgroundLoadingListViewItem();
        }
    }

    public interface ISupportsBackgroundRendering
    {
        bool BackgroundRenderingComplete { get; set; }
        bool ScheduledForBackgroundRendering { get; set; }
    }

    public class BackgroundLoadingListViewItem : ListViewItem, ISupportsBackgroundRendering
    {
        public bool BackgroundRenderingComplete { get; set; }
        public bool ScheduledForBackgroundRendering { get; set; }





        public Size SizeWhileBackgroundLoading
        {
            get { return (Size)GetValue(SizeWhileBackgroundLoadingProperty); }
            set { SetValue(SizeWhileBackgroundLoadingProperty, value); }
        }

        public static readonly DependencyProperty SizeWhileBackgroundLoadingProperty =
            DependencyProperty.Register("SizeWhileBackgroundLoading", typeof(Size), typeof(BackgroundLoadingListViewItem), new PropertyMetadata(new Size(0, 20)));

        

        public BackgroundLoadingListViewItem()
        {   
            
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
        }

        protected override System.Windows.Size MeasureOverride(System.Windows.Size constraint)
        {
            if (!BackgroundRenderingComplete)
            {
                var parentListView = this.VisualParent.FindVisualParent<BackgroundLoadingListView>();

                if (VisualParent is VirtualizingPanel && VirtualizingPanel.GetIsVirtualizing(parentListView))
                {
                    return base.MeasureOverride(constraint);
                }

                if (!ScheduledForBackgroundRendering)
                {
                    int priority = parentListView.IsVisible ? 1 : 2;
                    parentListView.RS.RequestRender(priority, this);
                }

                if (!constraint.IsInfinite())
                    return constraint;

                return SizeWhileBackgroundLoading;
            }
            else
            {
                return base.MeasureOverride(constraint);
            }
        }

        protected override System.Windows.Size ArrangeOverride(System.Windows.Size arrangeBounds)
        {
            if (!BackgroundRenderingComplete)
            {
                var parentListView = this.VisualParent.FindVisualParent<BackgroundLoadingListView>();

                if (VisualParent is VirtualizingPanel && VirtualizingPanel.GetIsVirtualizing(parentListView))
                {
                    return base.ArrangeOverride(arrangeBounds);
                }

                if (!ScheduledForBackgroundRendering)
                {
                    int priority = parentListView.IsVisible ? 1 : 2;
                    parentListView.RS.RequestRender(priority, this);
                }

                return arrangeBounds;
            }
            else
            {
                return base.ArrangeOverride(arrangeBounds);
            }
        }
    }
}
