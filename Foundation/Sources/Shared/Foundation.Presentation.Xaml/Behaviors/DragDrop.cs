using System;
using System.Collections;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using SquaredInfinity.Foundation.Presentation.DragDrop.Utilities;
using System.Windows.Media.Imaging;
using SquaredInfinity.Foundation.Extensions;
using SquaredInfinity.Foundation.Presentation.DragDrop;
using System.Diagnostics;

namespace SquaredInfinity.Foundation.Presentation.Behaviors
{
    public static partial class DragDrop
    {
        public static readonly DataFormat DataFormat = DataFormats.GetDataFormat("SquaredInfinity.Foundation.Presentation.DragDrop");

        static IDragInfo _dragInfo;
        static bool _dragInProgress;
        static object _clickSupressItem;
        static Point _adornerPos;
        static Size _adornerSize;

        public static IDragSource DefaultDragSource { get; set; }

        public static IDropTarget DefaultDropTarget { get; set; }

        static DragDrop()
        {
            DefaultDragSource = new DefaultDragSource();
            DefaultDropTarget = new DefaultDropTarget();
        }

        private static void CreateDragAdorner()
        {
            var template = GetDragAdornerTemplate(_dragInfo.VisualSource);
            var templateSelector = GetDragAdornerTemplateSelector(_dragInfo.VisualSource);

            UIElement adornment = null;

            var useDefaultDragAdorner = GetUseDefaultDragAdorner(_dragInfo.VisualSource);

            if (template == null && templateSelector == null && useDefaultDragAdorner)
            {
                template = new DataTemplate();

                var factory = new FrameworkElementFactory(typeof(Image));

                var bs = _dragInfo.VisualSourceItem.RenderToBitmap(_dragInfo.VisualSourceFlowDirection);
                factory.SetValue(Image.SourceProperty, bs);
                factory.SetValue(RenderOptions.EdgeModeProperty, EdgeMode.Aliased);
                factory.SetValue(RenderOptions.BitmapScalingModeProperty, BitmapScalingMode.HighQuality);
                if (_dragInfo.VisualSourceItem is FrameworkElement)
                {
                    factory.SetValue(FrameworkElement.WidthProperty, ((FrameworkElement)_dragInfo.VisualSourceItem).ActualWidth);
                    factory.SetValue(FrameworkElement.HeightProperty, ((FrameworkElement)_dragInfo.VisualSourceItem).ActualHeight);
                    factory.SetValue(FrameworkElement.HorizontalAlignmentProperty, HorizontalAlignment.Left);
                    factory.SetValue(FrameworkElement.VerticalAlignmentProperty, VerticalAlignment.Top);
                }
                template.VisualTree = factory;
            }

            if (template != null || templateSelector != null)
            {
                if (_dragInfo.Data is IEnumerable && !(_dragInfo.Data is string))
                {
                    if (((IEnumerable)_dragInfo.Data).Cast<object>().Count() <= 10)
                    {
                        var itemsControl = new ItemsControl();
                        itemsControl.ItemsSource = (IEnumerable)_dragInfo.Data;
                        itemsControl.ItemTemplate = template;
                        itemsControl.ItemTemplateSelector = templateSelector;

                        // The ItemsControl doesn't display unless we create a border to contain it.
                        // Not quite sure why this is...
                        var border = new Border();
                        border.Child = itemsControl;
                        adornment = border;
                    }
                }
                else
                {
                    var contentPresenter = new ContentPresenter();
                    contentPresenter.Content = _dragInfo.Data;
                    contentPresenter.ContentTemplate = template;
                    contentPresenter.ContentTemplateSelector = templateSelector;
                    adornment = contentPresenter;
                }
            }

            if (adornment != null)
            {
                if (useDefaultDragAdorner)
                {
                    adornment.Opacity = GetDefaultDragAdornerOpacity(_dragInfo.VisualSource);
                }

                var parentWindow = _dragInfo.VisualSource.FindVisualParent<Window>();
                var rootElement = parentWindow != null ? parentWindow.Content as UIElement : null;
                if (rootElement == null && Application.Current != null && Application.Current.MainWindow != null)
                {
                    rootElement = (UIElement)Application.Current.MainWindow.Content;
                }

                if (rootElement == null)
                {
                    rootElement = _dragInfo.VisualSource.FindVisualParent<UserControl>();
                }

                DragAdorner = new DragAdorner(rootElement, adornment);
            }
        }

        private static void CreateEffectAdorner(DropInfo dropInfo)
        {
            var template = GetEffectAdornerTemplate(dropInfo.VisualTarget, dropInfo.AllowedEffects, dropInfo.DestinationText);

            if (template != null)
            {
                var rootElement = (UIElement)Application.Current.MainWindow.Content;
                UIElement adornment = null;

                var contentPresenter = new ContentPresenter();
                contentPresenter.Content = _dragInfo.Data;
                contentPresenter.ContentTemplate = template;

                adornment = contentPresenter;

                EffectAdorner = new DragAdorner(rootElement, adornment);
            }
        }

        private static DataTemplate CreateDefaultEffectDataTemplate(UIElement target, BitmapImage effectIcon, string effectText, string destinationText)
        {
            if (!GetUseDefaultEffectDataTemplate(target))
            {
                return null;
            }

            // Add effect text
            var effectTextBlockFactory = new FrameworkElementFactory(typeof(TextBlock));
            effectTextBlockFactory.SetValue(TextBlock.TextProperty, effectText);
            effectTextBlockFactory.SetValue(TextBlock.FontSizeProperty, 11.0);
            effectTextBlockFactory.SetValue(TextBlock.ForegroundProperty, Brushes.Blue);

            // Add destination text
            var destinationTextBlockFactory = new FrameworkElementFactory(typeof(TextBlock));
            destinationTextBlockFactory.SetValue(TextBlock.TextProperty, destinationText);
            destinationTextBlockFactory.SetValue(TextBlock.FontSizeProperty, 11.0);
            destinationTextBlockFactory.SetValue(TextBlock.ForegroundProperty, Brushes.DarkBlue);
            destinationTextBlockFactory.SetValue(TextBlock.MarginProperty, new Thickness(3, 0, 0, 0));
            destinationTextBlockFactory.SetValue(TextBlock.FontWeightProperty, FontWeights.DemiBold);

            // Create containing panel
            var stackPanelFactory = new FrameworkElementFactory(typeof(StackPanel));
            stackPanelFactory.SetValue(StackPanel.OrientationProperty, Orientation.Horizontal);
            stackPanelFactory.SetValue(FrameworkElement.MarginProperty, new Thickness(2.0));
            //stackPanelFactory.AppendChild(imageFactory);
            stackPanelFactory.AppendChild(effectTextBlockFactory);
            stackPanelFactory.AppendChild(destinationTextBlockFactory);

            // Add border
            var borderFactory = new FrameworkElementFactory(typeof(Border));
            var stopCollection = new GradientStopCollection {
                                                        new GradientStop(Colors.White, 0.0),
                                                        new GradientStop(Colors.AliceBlue, 1.0)
                                                      };
            var gradientBrush = new LinearGradientBrush(stopCollection)
            {
                StartPoint = new Point(0, 0),
                EndPoint = new Point(0, 1)
            };
            borderFactory.SetValue(Panel.BackgroundProperty, gradientBrush);
            borderFactory.SetValue(Border.BorderBrushProperty, Brushes.DimGray);
            borderFactory.SetValue(Border.CornerRadiusProperty, new CornerRadius(3.0));
            borderFactory.SetValue(Border.BorderThicknessProperty, new Thickness(1.0));
            borderFactory.AppendChild(stackPanelFactory);

            // Finally add content to template
            var template = new DataTemplate();
            template.VisualTree = borderFactory;
            return template;
        }

        static DataTemplate GetEffectAdornerTemplate(UIElement target, DragDropEffects effect, string destinationText)
        {
            switch (effect)
            {
                case DragDropEffects.All:
                    return null;
                case DragDropEffects.Copy:
                    return GetEffectCopyAdornerTemplate(target, destinationText);
                case DragDropEffects.Move:
                    return GetEffectMoveAdornerTemplate(target, destinationText);
                case DragDropEffects.None:
                    return GetEffectNoneAdornerTemplate(target);
                default:
                    return null;
            }
        }

        static void Scroll(DependencyObject o, DragEventArgs e)
        {
            var scrollViewer = o.FindVisualDescendant<ScrollViewer>();

            if (scrollViewer != null)
            {
                var position = e.GetPosition(scrollViewer);
                var scrollMargin = Math.Min(scrollViewer.FontSize * 2, scrollViewer.ActualHeight / 2);

                if (position.X >= scrollViewer.ActualWidth - scrollMargin &&
                    scrollViewer.HorizontalOffset < scrollViewer.ExtentWidth - scrollViewer.ViewportWidth)
                {
                    scrollViewer.LineRight();
                }
                else if (position.X < scrollMargin && scrollViewer.HorizontalOffset > 0)
                {
                    scrollViewer.LineLeft();
                }
                else if (position.Y >= scrollViewer.ActualHeight - scrollMargin &&
                         scrollViewer.VerticalOffset < scrollViewer.ExtentHeight - scrollViewer.ViewportHeight)
                {
                    scrollViewer.LineDown();
                }
                else if (position.Y < scrollMargin && scrollViewer.VerticalOffset > 0)
                {
                    scrollViewer.LineUp();
                }
            }
        }

        /// <summary>
        /// Finds a Drag Source
        /// </summary>
        /// <param name="dragInfo"></param>
        /// <param name="sender"></param>
        /// <returns></returns>
        static IDragSource GetDragSource(IDragInfo dragInfo, UIElement sender)
        {
            IDragSource dragSource = null;
            
            // check if defined on Visual Source
            if(dragInfo == null)
            {
                if(dragInfo.VisualSource != null)
                    dragSource = GetDragSource(dragInfo.VisualSource);
            }
            
            // check if implemented by a sender
            if (dragSource == null && sender != null)
            {
                dragSource = GetDragSource(sender);
            }

            if (dragSource == null)
                return DefaultDragSource;
            else
                return dragSource;
        }

        private static void DragSource_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            // Ignore the click if clickCount != 1 or the user has clicked on a scrollbar.
            var elementPosition = e.GetPosition((IInputElement)sender);
            if (e.ClickCount != 1
                || HitTestUtilities.HitTest4Type<RangeBase>(sender, elementPosition) // scrollbar
                || HitTestUtilities.HitTest4Type<TextBoxBase>(sender, elementPosition)
                || HitTestUtilities.HitTest4Type<PasswordBox>(sender, elementPosition)
                || HitTestUtilities.HitTest4Type<ComboBox>(sender, elementPosition)
                || HitTestUtilities.HitTest4GridViewColumnHeader(sender, elementPosition)
                || HitTestUtilities.HitTest4DataGridTypes(sender, elementPosition)
                || HitTestUtilities.IsNotPartOfSender(sender, e) // TODO: this needs to be double checked when drag from non-itemscontrol will be enabled
                || GetDragSourceIgnore((UIElement)sender))
            {
                _dragInfo = null;
                return;
            }

            _dragInfo = DragInfo.CreateFromEvent(sender, e);

            if (_dragInfo == null)
                return;

            var dragHandler = GetDragSource(_dragInfo, sender as UIElement);
            if (!dragHandler.CanStartDrag(_dragInfo))
            {
                _dragInfo = null;
                return;
            }

            // If the sender is a list box that allows multiple selections, ensure that clicking on an 
            // already selected item does not change the selection, otherwise dragging multiple items 
            // is made impossible.
            var itemsControl = sender as ItemsControl;

            if (_dragInfo.VisualSourceItem != null && itemsControl != null && itemsControl.CanSelectMultipleItems())
            {
                var selectedItems = itemsControl.GetSelectedItems().Cast<object>().ToList();

                if (selectedItems.Count > 1 && selectedItems.Contains(_dragInfo.SourceItem))
                {
                    _clickSupressItem = _dragInfo.SourceItem;
                    e.Handled = true;
                }
            }
        }

        private static void DragSource_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            // If we prevented the control's default selection handling in DragSource_PreviewMouseLeftButtonDown
            // by setting 'e.Handled = true' and a drag was not initiated, manually set the selection here.
            var itemsControl = sender as ItemsControl;

            if (itemsControl != null && _dragInfo != null && _clickSupressItem == _dragInfo.SourceItem)
            {
                if ((Keyboard.Modifiers & ModifierKeys.Control) != 0)
                {
                    itemsControl.SetItemSelected(_dragInfo.SourceItem, false);
                }
                else
                {
                    itemsControl.SetSelectedItem(_dragInfo.SourceItem);
                }
            }

            if (_dragInfo != null)
            {
                _dragInfo = null;
            }

            _clickSupressItem = null;
        }

        private static void DragSource_PreviewMouseMove(object sender, MouseEventArgs e)
        {
            if (_dragInfo != null && !_dragInProgress)
            {
                var dragStart = _dragInfo.DragStartPosition;
                var position = e.GetPosition((IInputElement)sender);

                if (Math.Abs(position.X - dragStart.X) > SystemParameters.MinimumHorizontalDragDistance ||
                    Math.Abs(position.Y - dragStart.Y) > SystemParameters.MinimumVerticalDragDistance)
                {
                    var dragHandler = GetDragSource(_dragInfo, sender as UIElement);
                    if (dragHandler.CanStartDrag(_dragInfo))
                    {
                        dragHandler.StartDrag(_dragInfo);

                        if (_dragInfo.AllowedEffects != DragDropEffects.None && _dragInfo.Data != null)
                        {
                            var data = _dragInfo.DataObject;

                            if (data == null)
                            {
                                data = new DataObject(DataFormat.Name, _dragInfo.Data);
                            }
                            else
                            {
                                data.SetData(DataFormat.Name, _dragInfo.Data);
                            }

                            try
                            {
                                _dragInProgress = true;
                                var result = System.Windows.DragDrop.DoDragDrop(_dragInfo.VisualSource, data, _dragInfo.AllowedEffects);
                                if (result == DragDropEffects.None)
                                    dragHandler.DragCancelled();
                            }
                            finally
                            {
                                _dragInProgress = false;
                            }

                            _dragInfo = null;
                        }
                    }
                }
            }
        }

        private static void DragSource_QueryContinueDrag(object sender, QueryContinueDragEventArgs e)
        {
            if (e.Action == DragAction.Cancel || e.EscapePressed)
            {
                DragAdorner = null;
                EffectAdorner = null;
                DropTargetAdorner = null;
            }
        }

        private static void DropTarget_PreviewDragEnter(object sender, DragEventArgs e)
        {
            DropTarget_PreviewDragOver(sender, e);

            Mouse.OverrideCursor = Cursors.Arrow;
        }

        private static void DropTarget_PreviewDragLeave(object sender, DragEventArgs e)
        {
            DragAdorner = null;
            EffectAdorner = null;
            DropTargetAdorner = null;

            Mouse.OverrideCursor = null;
        }

        private static void DropTarget_PreviewDragOver(object sender, DragEventArgs e)
        {
            var elementPosition = e.GetPosition((IInputElement)sender);
            if (HitTestUtilities.HitTest4Type<ScrollBar>(sender, elementPosition)
                || HitTestUtilities.HitTest4GridViewColumnHeader(sender, elementPosition)
                || HitTestUtilities.HitTest4DataGridTypesOnDragOver(sender, elementPosition))
            {
                e.Effects = DragDropEffects.None;
                e.Handled = true;
                return;
            }

            var dropInfo = new DropInfo(sender, e, _dragInfo);
            var dropHandler = GetDropHandler((UIElement)sender);

            if (dropHandler == null)
            {
                // sender does not have DropHandler attached property set.
                // use default drop target in that case

                dropHandler = DefaultDropTarget;
            }
            var itemsControl = dropInfo.VisualTarget;

            dropHandler.DragOver(dropInfo);

            if (DragAdorner == null && _dragInfo != null)
            {
                CreateDragAdorner();
            }

            if (DragAdorner != null)
            {
                var tempAdornerPos = e.GetPosition(DragAdorner.AdornedElement);

                if (tempAdornerPos.X > 0 && tempAdornerPos.Y > 0)
                {
                    _adornerPos = tempAdornerPos;
                }

                if (DragAdorner.RenderSize.Width > 0 && DragAdorner.RenderSize.Height > 0)
                {
                    _adornerSize = DragAdorner.RenderSize;
                }

                if (_dragInfo != null)
                {
                    // move the adorner
                    var offsetX = _adornerSize.Width * -GetDragMouseAnchorPoint(_dragInfo.VisualSource).X;
                    var offsetY = _adornerSize.Height * -GetDragMouseAnchorPoint(_dragInfo.VisualSource).Y;
                    _adornerPos.Offset(offsetX, offsetY);
                    var maxAdornerPosX = DragAdorner.AdornedElement.RenderSize.Width;
                    var adornerPosRightX = (_adornerPos.X + _adornerSize.Width);
                    if (adornerPosRightX > maxAdornerPosX)
                    {
                        _adornerPos.Offset(-adornerPosRightX + maxAdornerPosX, 0);
                    }
                }

                DragAdorner.MousePosition = _adornerPos;
                DragAdorner.InvalidateVisual();
            }

            // If the target is an ItemsControl then update the drop target adorner.
            if (itemsControl != null)
            {
                // Display the adorner in the control's ItemsPresenter. If there is no 
                // ItemsPresenter provided by the style, try getting hold of a
                // ScrollContentPresenter and using that.
                var adornedElement =
                  (UIElement)itemsControl.FindVisualDescendant<ItemsPresenter>() ??
                  (UIElement)itemsControl.FindVisualDescendant<ScrollContentPresenter>();

                if (adornedElement != null)
                {
                    if (dropInfo.DropTargetAdorner == null)
                    {
                        DropTargetAdorner = null;
                    }
                    else if (!dropInfo.DropTargetAdorner.IsInstanceOfType(DropTargetAdorner))
                    {
                        DropTargetAdorner = DropTargetAdorner.Create(dropInfo.DropTargetAdorner, adornedElement);
                    }

                    if (DropTargetAdorner != null)
                    {
                        DropTargetAdorner.DropInfo = dropInfo;
                        DropTargetAdorner.InvalidateVisual();
                    }
                }
            }

            // Set the drag effect adorner if there is one
            if (EffectAdorner == null && _dragInfo != null)
            {
                CreateEffectAdorner(dropInfo);
            }

            if (EffectAdorner != null)
            {
                var adornerPos = e.GetPosition(EffectAdorner.AdornedElement);
                adornerPos.Offset(20, 20);
                EffectAdorner.MousePosition = adornerPos;
                EffectAdorner.InvalidateVisual();
            }

            e.Effects = dropInfo.AllowedEffects;
            e.Handled = true;

            if(e.Effects != DragDropEffects.None)
                Scroll((DependencyObject)dropInfo.VisualTarget, e);
        }

        private static void DropTarget_PreviewDrop(object sender, DragEventArgs e)
        {
            var local_dragInfo = _dragInfo;

            if (local_dragInfo == null)
            {
                //NOTE: internal drag info may be null if drag operation did not originate from control using this DragDrop implementation
                //      (e.g. outside of wpf app)
                //      In that case convert DragEventArgs to DragInfo representation and then use it

                local_dragInfo = DragInfo.CreateFromEvent(sender, e);
            }

            var dropInfo = new DropInfo(sender, e, local_dragInfo);
            var dropHandler = GetDropHandler((UIElement)sender) ?? DefaultDropTarget;
            var dragHandler = GetDragSource(local_dragInfo, sender as UIElement);

            DragAdorner = null;
            EffectAdorner = null;
            DropTargetAdorner = null;

            dropHandler.Drop(dropInfo);
            dragHandler.Dropped(dropInfo);

            Mouse.OverrideCursor = null;
            e.Handled = true;
        }

        static void DropTarget_GiveFeedback(object sender, GiveFeedbackEventArgs e)
        {
            if (EffectAdorner != null)
            {
                e.UseDefaultCursors = false;
                e.Handled = true;
            }
            else
            {
                e.UseDefaultCursors = true;
                e.Handled = true;
            }
        }


        static DragAdorner _dragAdorner;
        static DragAdorner DragAdorner
        {
            get { return _dragAdorner; }
            set
            {
                if (_dragAdorner != null)
                    _dragAdorner.Detatch();

                _dragAdorner = value;
            }
        }


        static DragAdorner _effectAdorner;
        static DragAdorner EffectAdorner
        {
            get { return _effectAdorner; }
            set
            {
                if (_effectAdorner != null)
                    _effectAdorner.Detatch();

                _effectAdorner = value;
            }
        }


        static DropTargetAdorner _dropTargetAdorner;
        static DropTargetAdorner DropTargetAdorner
        {
            get { return _dropTargetAdorner; }
            set
            {
                if (_dropTargetAdorner != null)
                    _dropTargetAdorner.Detatch();

                _dropTargetAdorner = value;
            }
        }
    }
}