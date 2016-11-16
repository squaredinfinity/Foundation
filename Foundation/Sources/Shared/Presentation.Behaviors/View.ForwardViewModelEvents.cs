using SquaredInfinity.Foundation.Presentation.Views;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;

namespace SquaredInfinity.Foundation.Presentation.Behaviors
{
    public static partial class ForwardViewModelEvents
    {
        #region Target

        public static void SetTarget(View element, View value)
        {
            element.SetValue(TargetProperty, value);
        }

        public static View GetTarget(View element)
        {
            return (View)element.GetValue(TargetProperty);
        }

        public static readonly DependencyProperty TargetProperty =
            DependencyProperty.RegisterAttached(
            "Target",
            typeof(View),
            typeof(ForwardViewModelEvents),
            new PropertyMetadata(null, OnTargetChanged));

        static void OnTargetChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            View source = d as View;

            source.ViewModelEvent -= Source_ViewModelEvent;
            source.ViewModelEvent += Source_ViewModelEvent;
        }

        static void Source_ViewModelEvent(object sender, ViewModelEventRoutedEventArgs e)
        {
            CreateAndRaiseNewEventArgs(sender, e);
        }

        static void CreateAndRaiseNewEventArgs(object sender, ViewModelEventRoutedEventArgs e)
        {
            var v = sender as View;

            var target = GetTarget(v);

            var new_e2 = new ViewModelEventRoutedEventArgs(e.ViewModelEventArgs, e.RoutedEvent, e.Source);

            target.RaiseEvent(new_e2);

            if (new_e2.Handled)
                e.Handled = true;
        }

        #endregion
    }
}
