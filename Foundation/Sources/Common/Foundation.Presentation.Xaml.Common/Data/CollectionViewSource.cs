using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Data;

namespace SquaredInfinity.Foundation.Presentation.Data
{
    public class CollectionViewSource : System.Windows.Data.CollectionViewSource
    {
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

            cvs.View.Refresh();
        }
      


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
    }
}
