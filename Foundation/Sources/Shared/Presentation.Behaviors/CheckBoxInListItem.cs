using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using SquaredInfinity.Extensions;
using System.Windows.Controls.Primitives;

namespace SquaredInfinity.Presentation.Behaviors
{
    public class CheckBoxInListItem
    {
        #region InterceptKeySelectionEvents

        public static void SetInterceptKeySelectionEvents(Control element, bool value)
        {
            element.SetValue(InterceptKeySelectionEventsProperty, value);
        }

        public static bool GetInterceptKeySelectionEvents(Control element)
        {
            return (bool)element.GetValue(InterceptKeySelectionEventsProperty);
        }

        public static readonly DependencyProperty InterceptKeySelectionEventsProperty =
            DependencyProperty.RegisterAttached(
            "InterceptKeySelectionEvents",
            typeof(bool),
            typeof(CheckBoxInListItem),
            new PropertyMetadata(false, OnInterceptKeySelectionEventsChanged));

        static void OnInterceptKeySelectionEventsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var target = d as CheckBox;

            var parentSelector = target.FindVisualParent<Selector>();

            var eventSubscriptions = GetEventSubscriptions(d);

            foreach (var ev in eventSubscriptions)
                ev.Dispose();

            eventSubscriptions.Clear();

            var previewKeyDownSubscription =
                parentSelector
                .CreateWeakEventHandler()
                .ForEvent<KeyEventHandler, KeyEventArgs>(
                (s, h) => s.PreviewKeyDown += h,
                (s, h) => s.PreviewKeyDown -= h)
                .Subscribe((_s, _e) => 
                    {
                        var selector = _s as Selector;

                        // ListBox handles Space itself and will only raise PreviewKeyDow (i.e. KeyDown will not be raised)
                        // so handle it here
                        if (_s is ListBox)
                        {
                            if (_e.Key == Key.Space)
                                ToggleCheckedStateIfItemSelected(selector, target);
                        }
                    });

            var keyDownSubscription =
                parentSelector
                .CreateWeakEventHandler()
                .ForEvent<KeyEventHandler, KeyEventArgs>(
                (s, h) => s.KeyDown += h,
                (s, h) => s.KeyDown -= h)
                .Subscribe((_s, _e) => 
                    {
                        var selector = _s as Selector;

                        if (_e.Key == Key.Space)
                            ToggleCheckedStateIfItemSelected(selector, target);
                    });

            eventSubscriptions.Add(previewKeyDownSubscription);
            eventSubscriptions.Add(keyDownSubscription);
        }

        private static void ToggleCheckState(CheckBox target)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region EventSubscriptions

        static void SetEventSubscriptions(DependencyObject element, List<IDisposable> value)
        {
            element.SetValue(EventSubscriptionsProperty, value);
        }

        static List<IDisposable> GetEventSubscriptions(DependencyObject element)
        {
             var result = (List<IDisposable>)element.GetValue(EventSubscriptionsProperty);

             if (result == null)
                 result = new List<IDisposable>();

             SetEventSubscriptions(element, result);

             return result;
        }

        static readonly DependencyProperty EventSubscriptionsProperty =
            DependencyProperty.RegisterAttached(
            "EventSubscriptions",
            typeof(List<IDisposable>),
            typeof(CheckBoxInListItem),
            new PropertyMetadata(null));

        #endregion

        static void ToggleCheckedStateIfItemSelected(Selector selector, CheckBox target)
        {
            var selectedItems = selector.GetSelectedItems();

            var targetIsChildOfSelectedItem = false;

            foreach (var item in selectedItems)
            {
                var listItem = selector.ItemContainerGenerator.ContainerFromItem(item);

                if (listItem == null)
                {
                    continue;
                }

                if (target.IsVisualChildOf(listItem))
                {
                    targetIsChildOfSelectedItem = true;
                    break;
                }
            }

            if (!targetIsChildOfSelectedItem)
                return;

            target.IsChecked = !target.IsChecked;
        }
    }
}
