using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Controls;
using System.Linq;
using SquaredInfinity.Foundation.Extensions;
using System.Windows;
using SquaredInfinity.Foundation.Presentation.Infrastructure;
using System.Windows.Controls.Primitives;
using System.ComponentModel;
using System.Windows.Media;
using SquaredInfinity.Foundation.Collections;
using System.Collections;
using System.Windows.Input;
using SquaredInfinity.Foundation.Presentation.Controls.Infrastructure;
using System.Threading;

namespace SquaredInfinity.Foundation.Presentation.Controls.AdaptiveSelector
{
    public class AdaptiveSelector : MultiSelector, INotifyPropertyChanged
    {
        #region Input Controller

        public ISelectorBehaviorController InputController
        {
            get { return (ISelectorBehaviorController)GetValue(InputControllerProperty); }
            set { SetValue(InputControllerProperty, value); }
        }

        public static readonly DependencyProperty InputControllerProperty =
            DependencyProperty.Register(
                "InputController",
                typeof(ISelectorBehaviorController),
                typeof(AdaptiveSelector),
                new PropertyMetadata(null));

        #endregion

        #region Selector Identifier

        public object SelectorIdentifier
        {
            get { return (object)GetValue(SelectorIdentifierProperty); }
            set { SetValue(SelectorIdentifierProperty, value); }
        }


        public static readonly DependencyProperty SelectorIdentifierProperty =
            DependencyProperty.Register(
                "SelectorIdentifier",
                typeof(object),
                typeof(AdaptiveSelector),
                new PropertyMetadata(null));

        #endregion

        #region Selection Highlight

        public Color SelectionHighlight
        {
            get { return (Color)GetValue(SelectionHighlightProperty); }
            set { SetValue(SelectionHighlightProperty, value); }
        }

        public static readonly DependencyProperty SelectionHighlightProperty =
            DependencyProperty.Register(
            "SelectionHighlight",
            typeof(Color),
            typeof(AdaptiveSelector),
            new PropertyMetadata(Color.FromArgb(0xff, 0x0F, 0x9D, 0x58)));

        #endregion

        #region Use Nice Increments

        public bool UseNiceIncrements
        {
            get { return (bool)GetValue(UseNiceIncrementsProperty); }
            set { SetValue(UseNiceIncrementsProperty, value); }
        }

        public static readonly DependencyProperty UseNiceIncrementsProperty =
            DependencyProperty.Register(
            "UseNiceIncrements",
            typeof(bool),
            typeof(AdaptiveSelector),
            new PropertyMetadata(true));

        #endregion

        #region Selection Markers

        public ObservableCollectionEx<SelectionMarker> SelectionMarkers
        {
            get { return (ObservableCollectionEx<SelectionMarker>)GetValue(SelectionMarkersProperty); }
            set { SetValue(SelectionMarkersProperty, value); }
        }

        public static readonly DependencyPropertyKey SelectionMarkersPropertyKey =
            DependencyProperty.RegisterReadOnly(
            "SelectionMarkers",
            typeof(ObservableCollectionEx<SelectionMarker>),
            typeof(AdaptiveSelector),
            new PropertyMetadata());

        public static readonly DependencyProperty SelectionMarkersProperty
            = SelectionMarkersPropertyKey.DependencyProperty;

        #endregion

        #region BindableSelectedItems

        public IList BindableSelectedItems
        {
            get { return (IList)GetValue(BindableSelectedItemsProperty); }
            set { SetValue(BindableSelectedItemsProperty, value); }
        }

        public static readonly DependencyProperty BindableSelectedItemsProperty =
            DependencyProperty.Register(
            "BindableSelectedItems",
            typeof(IList),
            typeof(AdaptiveSelector),
            new PropertyMetadata(null, OnBindableSelectedItemsChanged));

        static void OnBindableSelectedItemsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var sel = d as AdaptiveSelector;

            sel.SelectionChanged -= sel_SelectionChanged;
            sel.SelectionChanged += sel_SelectionChanged;

            if (e.NewValue != null)
            {
                var initial_items = new List<object>();
                foreach (var item in (IList)e.NewValue)
                {
                    initial_items.Add(item);
                }

                foreach (var item in initial_items)
                {
                    sel.SelectedItems.Add(item);
                }
            }

            sel.IncrementVersion();
        }

        static void sel_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var sel = sender as AdaptiveSelector;

            if (sel == null)
                return;

            var bound_collection = sel.BindableSelectedItems;

            var compatible_types = bound_collection.GetCompatibleItemsTypes();

            bool is_adaptive_item_compatible =
                compatible_types.Any(x => x.IsTypeEquivalentTo(typeof(IAdaptiveSelectorItem), treatBaseTypesAndinterfacesAsEquivalent: true, treatNullableAsEquivalent: true));

            if (bound_collection != null)
            {
                var buc = bound_collection as IBulkUpdatesCollection;
                var bulk_update = (IDisposable)null;
                if (buc != null)
                    bulk_update = buc.BeginBulkUpdate() as IBulkUpdate;

                using (bulk_update)
                {
                    // Remove bound but no longer selected
                    for (int i = bound_collection.Count - 1; i >= 0; i--)
                    {
                        var item = bound_collection[i];

                        bool item_is_selected = false;

                        for (int sel_ix = 0; sel_ix < sel.SelectedItems.Count; sel_ix++)
                        {
                            var sel_item = sel.SelectedItems[sel_ix];

                            var sel_ai = sel_item as IAdaptiveSelectorItem;

                            if (is_adaptive_item_compatible)
                            {
                                throw new NotImplementedException("Bound collection does not yet support IDaptiveSeclectorItems");
                            }
                            else
                            {
                                if (sel_ai != null)
                                {
                                    if (sel.ItemEqualityComparer.Equals(sel_ai, item))
                                    {
                                        item_is_selected = true;
                                        break;
                                    }
                                }
                                else
                                {
                                    if (sel.ItemEqualityComparer.Equals(sel_item, item))
                                    {
                                        item_is_selected = true;
                                        break;
                                    }
                                }
                            }
                        }

                        if (!item_is_selected)
                            bound_collection.RemoveAt(i);
                    }

                    // Add selected but not yet bound
                    foreach (var item in sel.SelectedItems)
                    {
                        if (is_adaptive_item_compatible)
                        {
                            if (!bound_collection.Contains(item))
                            {
                                bound_collection.Add(item);
                            }
                        }
                        else
                        {
                            var ai = item as IAdaptiveSelectorItem;

                            if (ai != null)
                            {
                                if (!bound_collection.Contains(ai.Item))
                                {
                                    bound_collection.Add(ai.Item);
                                }
                            }
                            else
                            {
                                if (!bound_collection.Contains(item))
                                {
                                    bound_collection.Add(item);
                                }
                            }
                        }
                    }
                }



                //var buc = bound_collection as IBulkUpdatesCollection;

                //var bulk_update = (IDisposable)null;

                //if (buc != null)
                //    bulk_update = buc.BeginBulkUpdate() as IBulkUpdate;

                //using (bulk_update)
                //{
                //    bound_collection.Clear();

                //    foreach (var item in sel.SelectedItems)
                //    {
                //        bound_collection.Add(item);
                //    }

                //    //bulk_update.Dispose();

                //    //bulk_update.Dispose();
                //}

            }
        }

        #endregion

        #region Selection Marker Visibility

        public Visibility? SelectionMarkerVisibility
        {
            get { return (Visibility?)GetValue(SelectionMarkerVisibilityProperty); }
            set { SetValue(SelectionMarkerVisibilityProperty, value); }
        }

        public static readonly DependencyProperty SelectionMarkerVisibilityProperty =
            DependencyProperty.Register(
                "SelectionMarkerVisibility",
                typeof(Visibility?),
                typeof(AdaptiveSelector),
                new PropertyMetadata(null));


        #endregion


        IEqualityComparer<object> _itemEqualityComparer = EqualityComparer<object>.Default;

        /// <summary>
        /// Equality comparer used to check equality of items in this selector.
        /// </summary>
        public IEqualityComparer<object> ItemEqualityComparer
        {
            get { return _itemEqualityComparer; }
            set
            {
                if (value == null)
                    throw new ArgumentNullException();

                _itemEqualityComparer = value;
            }
        }

        public virtual Visibility GetSelectionMarkersVisibility()
        {
            if (SelectionMarkerVisibility != null)
                return SelectionMarkerVisibility.Value;

            // 0 or 1 item, don't show markers
            if (Items.Count < 2)
            {
                return Visibility.Collapsed;
            }

            // not all items selected, show markers
            if (SelectedItems.Count < Items.Count)
            {
                return Visibility.Visible;
            }
            else // all items selected, don't show markers
            {
                return Visibility.Collapsed;
            }
        }

        public void InitializeSelectionMarkerThumb(Thumb thumb)
        {
            // set initial offset
            var selection_marker = thumb.DataContext as SelectionMarker;

            var offset = GetOffset(selection_marker.Item);

            var cp = thumb.FindVisualParent<ContentPresenter>();

            Canvas.SetLeft(cp, offset);

            BeginThumbMonitoring(thumb);
        }

        public void BeginThumbMonitoring(Thumb thumb)
        {
            thumb.DragDelta -= thumb_DragDelta;
            thumb.DragDelta += thumb_DragDelta;

            thumb.DragCompleted -= thumb_DragCompleted;
            thumb.DragCompleted += thumb_DragCompleted;
        }

        void thumb_DragCompleted(object sender, DragCompletedEventArgs e)
        {
            var t = sender as Thumb;

            var cp = t.FindVisualParent<ContentPresenter>();

            var selection_marker = t.DataContext as SelectionMarker;

            if (selection_marker == null)
                return;

            var offset = GetOffset(selection_marker.Item);

            Canvas.SetLeft(cp, offset);

            selection_marker.IsDragging = false;

            if (selection_marker.Item == null)
                SelectionMarkers.Remove(selection_marker);
        }

        void thumb_DragDelta(object sender, DragDeltaEventArgs e)
        {
            var t = sender as Thumb;

            var cp = t.FindVisualParent<ContentPresenter>();

            var selection_marker = t.DataContext as SelectionMarker;

            if (selection_marker == null)
                return;

            selection_marker.IsDragging = true;

            var width = GetWidth(selection_marker.Item);

            var full_width_offset = width - t.ActualWidth;

            var old_left = Canvas.GetLeft(cp).Clamp(0 - full_width_offset, ActualWidth - full_width_offset);

            var new_left = (old_left + e.HorizontalChange).Clamp(0 - full_width_offset, ActualWidth - full_width_offset);

            Canvas.SetLeft(cp, new_left);

            // change selection

            var offset_including_marker_point = new_left + (cp.ActualWidth / 2);

            var new_selected_item = FindItemByOffset(offset_including_marker_point);

            if (!object.Equals(selection_marker.Item, new_selected_item))
            {
                if (SelectedItems.Contains(new_selected_item))
                {
                    // items under the marker is already selected
                    SelectedItems.Remove(selection_marker.Item);
                    selection_marker.Item = null;
                }
                else
                {
                    var old_selected_item = selection_marker.Item;
                    selection_marker.Item = new_selected_item;

                    base.BeginUpdateSelectedItems();

                    SelectedItems.Add(new_selected_item);
                    SelectedItems.Remove(old_selected_item);

                    base.EndUpdateSelectedItems();
                }
            }
        }

        public void StopThumbMonitoring(Thumb thumb)
        {

        }

        public Color GetItemBackground(object item)
        {
            var adaptive_item = item as IAdaptiveSelectorItem;

            if (adaptive_item == null)
            {
                return SelectionHighlight;
            }

            var background = adaptive_item.GetBackgroundColor();

            if (background == null)
                return SelectionHighlight;

            if (SelectedItems.Contains(item))
            {
                return background.Value.ChangeLighthness(1.1);
            }
            else
            {
                return background.Value;
            }
        }

        ItemsPresenter ItemsPresenter;

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            ItemsPresenter = this.FindVisualDescendant<ItemsPresenter>();
        }

        public void ToggleSelection(object item)
        {

            if (SelectedItems.Contains(item))
                SelectedItems.Remove(item);
            else
                SelectedItems.Add(item);
        }

        public AdaptiveSelector()
        {
            SetValue(SelectionMarkersPropertyKey, new ObservableCollectionEx<SelectionMarker>());

            this.SelectionChanged += (s, e) => IncrementVersion();
            this.SelectionChanged += AdaptiveSelector_SelectionChanged;
            this.MouseWheel += MultiSelectionSlider_MouseWheel;

        }



        protected override void OnItemsChanged(System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            base.OnItemsChanged(e);

            UpdateSelectedItemFromBindableSelectedItems();
        }

        void UpdateSelectedItemFromBindableSelectedItems()
        {
            if (Items == null || BindableSelectedItems == null)
            {
                return;
            }

            if (Items.Count == 0 || BindableSelectedItems.Count == 0)
                return;

            foreach (var item in BindableSelectedItems)
                SelectedItems.Add(item);

            IncrementVersion();
        }

        void AdaptiveSelector_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            foreach (var item in e.RemovedItems)
            {
                var to_remove =
                    (from sm in SelectionMarkers
                     where !sm.IsDragging && object.Equals(sm.Item, item)
                     select sm).ToArray();

                foreach (var td in to_remove)
                    SelectionMarkers.Remove(td);
            }

            foreach (var item in e.AddedItems)
            {
                var existing_marker =
                    (from sm in SelectionMarkers
                     where object.Equals(sm.Item, item)
                     select sm).FirstOrDefault();

                if (existing_marker != null)
                    continue;

                var new_marker = new SelectionMarker() { Item = item };
                SelectionMarkers.Add(new_marker);

                var adaptive_item = item as IAdaptiveSelectorItem;

                if (adaptive_item != null && adaptive_item.Group != null)
                {
                    // disable others within same group
                    var other_with_same_group =
                        (from si in this.SelectedItems.OfType<IAdaptiveSelectorItem>()
                             // same group, different item
                         where object.Equals(si.Group, adaptive_item.Group) && !ItemEqualityComparer.Equals(si, adaptive_item)
                         select si).ToList();


                    foreach (var rem in other_with_same_group)
                    {
                        SelectedItems.Remove(rem);
                    }
                }
            }

            // make sure only one item from single-selection groups is selected at the time
        }

        void MultiSelectionSlider_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (e.Delta > 0)
                IncreaseNumberOfSelectedItems();
            else
                DecreaseNumberOfSelectedItems();
        }

        public void IncreaseNumberOfSelectedItems()
        {
            var new_selected_count = SelectedItems.Count + 1;

            if (UseNiceIncrements)
            {
                if (new_selected_count > Items.Count / 2)
                {
                    new_selected_count = Items.Count;
                }
                else if (new_selected_count != 0)
                {
                    var even_count = MathExtensions.FindEvenNumber(Items.Count, ForwardBackwardDirection.Backward);

                    new_selected_count = MathExtensions.FindDivisorWithoutReminder(even_count, new_selected_count, ForwardBackwardDirection.Forward);
                }

                if (Items.Count % 2 == 1 && Items.Count > new_selected_count)
                    new_selected_count = MathExtensions.FindOddNumber(new_selected_count, ForwardBackwardDirection.Forward);
            }

            if (new_selected_count > Items.Count)
                return;

            SelectItems(new_selected_count);
        }

        public void DecreaseNumberOfSelectedItems()
        {
            var new_selected_count = SelectedItems.Count - 1;

            if (Items.Count % 2 == 1 && new_selected_count > 0)
                new_selected_count--;

            if (UseNiceIncrements)
            {
                if (new_selected_count > Items.Count / 2)
                {
                    new_selected_count = Items.Count / 2;
                }
                else if (new_selected_count != 0)
                {
                    var even_count = MathExtensions.FindEvenNumber(Items.Count, ForwardBackwardDirection.Backward);

                    new_selected_count = MathExtensions.FindDivisorWithoutReminder(even_count, new_selected_count, ForwardBackwardDirection.Backward);

                    if (new_selected_count != 0 && Items.Count % 2 == 1 && Items.Count > new_selected_count)
                        new_selected_count = MathExtensions.FindOddNumber(new_selected_count, ForwardBackwardDirection.Forward);
                }
            }

            if (new_selected_count < 0)
                return;

            SelectItems(new_selected_count);
        }

        void SelectItems(int count)
        {
            SelectedItems.Clear();

            if (Items.Count == 0)
                return;

            if (count == 0)
                return;

            if (count == 1)
            {
                var mid = Items.Count / 2;
                SelectedItems.Add(Items[mid]);
                return;
            }

            SelectItems(0, Items.Count, count);
        }

        void SelectItems(int ix_from, int all_count, int selected_count)
        {
            var r = 0;

            var x = Math.DivRem(all_count - 1, selected_count - 1, out r);

            if (r == 0)
            {
                for (int i = ix_from; i < all_count; i += x)
                {
                    SelectedItems.Add(Items[i]);
                }
            }
            else
            {
                var selected_count_left = selected_count / 2;
                var selected_count_right = selected_count - selected_count_left;

                if (selected_count % 2 != 0)
                    selected_count_right--;


                var r_left = r / 2;
                var r_right = r_left;

                var ix_mid = ix_from + (all_count / 2);

                // move from left
                for (int i = ix_from; i < ix_mid && selected_count_left > 0; i += x)
                {
                    SelectedItems.Add(Items[i]);
                    selected_count_left--;

                    //  if(r_left > 0)
                    if (selected_count_left < r_left)
                    {
                        i += 1;
                        r_left--;
                    }
                }

                // move from right
                for (int i = all_count - 1; i > ix_mid && selected_count_right > 0; i -= x)
                {
                    SelectedItems.Add(Items[i]);
                    selected_count_right--;

                    //if (r_right > 0)
                    if (selected_count_right < r_right)
                    {
                        i -= 1;
                        r_right--;
                    }
                }

                if (selected_count % 2 != 0)
                {
                    SelectedItems.Add(Items[ix_mid]);
                }
            }
        }

        #region Version

        [field: NonSerialized]
        public event EventHandler VersionChanged;

        protected void IncrementVersion()
        {
            var newVersion = Interlocked.Increment(ref _version);

            OnVersionChanged();
        }

        protected void OnVersionChanged()
        {
            if (VersionChanged != null)
                VersionChanged(this, EventArgs.Empty);

            RaisePropertyChanged("Version");
        }

        long _version;
        public long Version
        {
            get { return _version; }
        }

        #endregion

        protected override System.Windows.DependencyObject GetContainerForItemOverride()
        {
            return new MultiSelectionSliderContentPresenter();
        }

        public Thickness GetBorder(object item)
        {
            if (Items.IndexOf(item) == Items.Count - 1)
                return new Thickness(0.0, 0.0, 0.0, 0.0);
            else
                return new Thickness(0.0, 0.0, 1.0, 0.0);
        }

        object FindItemByOffset(double offset)
        {
            if (Items.Count == 0)
                return null;

            if (offset >= ActualWidth)
                return Items[Items.Count - 1];

            if (offset <= 0.0)
                return Items[0];

            long reminder = 0;

            var item_width = Math.DivRem((long)ActualWidth, (long)Items.Count, out reminder);

            double search_offset = 0.0;

            for (int i = 0; i < Items.Count; i++)
            {
                var reminder_modifier = 0.0;

                if (i < reminder)
                    reminder_modifier = 1.0;

                if (offset.IsGreaterThanOrClose(search_offset) && offset.IsLessThanOrClose(search_offset + item_width + reminder_modifier))
                    return Items[i];

                search_offset += item_width + reminder_modifier;
            }

            return null;
        }

        public double GetOffset(object item)
        {
            var item_ix = Items.IndexOf(item);

            long reminder = 0;

            var new_item_width = Math.DivRem((long)ActualWidth, (long)Items.Count, out reminder);

            double offset = 0.0;

            for (int i = 0; i < item_ix; i++)
            {
                if (i < reminder)
                    offset += 1.0;

                offset += new_item_width;
            }

            var thumb_width = GetThumbWidth(item);

            return offset + thumb_width / 4;
        }

        public double GetThumbWidth(object item)
        {
            var item_width = GetWidth(item);

            return item_width * .667;
        }

        public double GetWidth(object item)
        {
            long reminder = 0;

            var total_width = ActualWidth;

            var new_item_width = Math.DivRem((long)total_width, (long)Items.Count, out reminder);

            // keep adding 1 to width to items from the left so that eventually there's no extra reminder left and cells still look similar in size
            if (Items.IndexOf(item) < reminder)
            {
                return new_item_width + 1;
            }
            else
            {
                return new_item_width;
            }
        }

        protected override Size ArrangeOverride(Size arrangeBounds)
        {
            var x = base.ArrangeOverride(arrangeBounds);

            return x;
        }

        #region Property Changed

        public event PropertyChangedEventHandler PropertyChanged;

        void RaisePropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion

        public IEnumerable<IUserAction> GetAvailableActions(object node)
        {
            List<IUserAction> actions = new List<IUserAction>();

            actions.Add(new UserAction("lol", "", (x) =>
            {
                MessageBox.Show("(. Y .)");
                //var report_run = x.GetParameterValue<IReportRun>("node", () => null);

                //report_run.ParentReport.ReportRuns.RemoveRun(report_run);
            }));

            return actions;
        }

        public void ExecuteAction(Dictionary<string, object> parameters)
        {
            var action = (IUserAction)parameters["action"];

            action.Execute(parameters);
        }

    }
}
