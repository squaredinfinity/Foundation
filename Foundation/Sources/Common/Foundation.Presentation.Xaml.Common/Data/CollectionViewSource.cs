using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Data;

namespace SquaredInfinity.Foundation.Presentation.Data
{
    public class CollectionViewSource : System.Windows.Data.CollectionViewSource
    {
        #region Filter Predicate

        public Predicate<object> FilterPredicate
        {
            get { return (Predicate<object>)GetValue(FilterPredicateProperty); }
            set { SetValue(FilterPredicateProperty, value); }
        }

        public static readonly DependencyProperty FilterPredicateProperty =
            DependencyProperty.Register(
            "FilterPredicate",
            typeof(Predicate<object>),
            typeof(CollectionViewSource), 
            new PropertyMetadata(null));

        #endregion

        public object RefreshTriggerProperty
        {
            get { return (object)GetValue(RefreshTriggerPropertyProperty); }
            set { SetValue(RefreshTriggerPropertyProperty, value); }
        }

        public static readonly DependencyProperty RefreshTriggerPropertyProperty =
            DependencyProperty.Register(
            "RefreshTriggerProperty",
            typeof(object),
            typeof(CollectionViewSource),
            new PropertyMetadata(null, OnRefreshTriggerPropertyChanged));

        static void OnRefreshTriggerPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var cvs = d as CollectionViewSource;

            if (cvs == null)
                return;

            if (cvs.View == null)
                return;

            cvs.View.Refresh();
        }

        #region SortPredicate

        public Func<object, object, int> SortCompareMethod
        {
            get { return (Func<object, object, int>) GetValue(SortCompareMethodProperty); }
            set { SetValue(SortCompareMethodProperty, value); }
        }

        public static readonly DependencyProperty SortCompareMethodProperty =
            DependencyProperty.Register(
            "SortCompareMethod",
            typeof(Func<object, object, int>),
            typeof(CollectionViewSource),
            new PropertyMetadata(null, OnSortCompareMethodChanged));

        static void OnSortCompareMethodChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var cvs = d as CollectionViewSource;

            if (cvs == null)
                return;

            if (cvs.View == null)
                return;

            var lcv = cvs.View as ListCollectionView;
            
            if (lcv == null)
                return;

            if(e.NewValue == null)
            {
                lcv.CustomSort = null;
                return;
            }

            lcv.CustomSort = new DefaultComparer(e.NewValue as Func<object, object, int>);
        }

        #endregion

        public CollectionViewSource()
        {
            this.Filter += CollectionViewSource_Filter;
        }

        protected override void OnSourceChanged(object oldSource, object newSource)
        {
            base.OnSourceChanged(oldSource, newSource);
        }

        void CollectionViewSource_Filter(object sender, FilterEventArgs e)
        {
            if (FilterPredicate == null)
                return;

            e.Accepted = FilterPredicate(e.Item);
        }

        class DefaultComparer : IComparer
        {
            readonly Func<object, object, int> CompareMethod;

            public DefaultComparer(Func<object, object, int> compareMethod)
            {
                this.CompareMethod = compareMethod;
            }

            public int Compare(object x, object y)
            {
                return CompareMethod(x, y);
            }
        }
    }
}
