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
        BackgroundRenderingService RS = new BackgroundRenderingService();

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

            RS.RequestRender(this);
        }

        void cc_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action != NotifyCollectionChangedAction.Remove)
            {
                RS.RequestRender(this);
            }
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

    public class BackgroundLoadingListViewItem : ListViewItem
    {
        public bool BackgroundLoadComplete
        {
            get { return (bool)GetValue(BackgroundLoadCompleteProperty); }
            set { SetValue(BackgroundLoadCompleteProperty, value); }
        }

        public static readonly DependencyProperty BackgroundLoadCompleteProperty =
            DependencyProperty.Register("BackgroundLoadComplete", typeof(bool), typeof(BackgroundLoadingListViewItem), new PropertyMetadata(false));




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
            if (!BackgroundLoadComplete)
            {
                if (VisualParent is VirtualizingPanel && VirtualizingPanel.GetIsVirtualizing(this.VisualParent.FindVisualParent<ListView>()))
                {
                    return base.MeasureOverride(constraint);
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
            if (!BackgroundLoadComplete)
            {
                if (VisualParent is VirtualizingPanel && VirtualizingPanel.GetIsVirtualizing(this.VisualParent.FindVisualParent<ListView>()))
                {
                    return base.ArrangeOverride(arrangeBounds);
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
