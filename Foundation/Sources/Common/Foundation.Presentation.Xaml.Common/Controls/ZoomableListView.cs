using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using SquaredInfinity.Foundation.Extensions;

namespace SquaredInfinity.Foundation.Presentation.Controls
{
    public class ZoomableListView : ListView
    {
        #region Zoom

        public event EventHandler<EventArgs> AfterZoomChanged;

        public double Zoom
        {
            get { return (double)GetValue(ZoomProperty); }
            set { SetValue(ZoomProperty, value); }
        }

        public static readonly DependencyProperty ZoomProperty =
            DependencyProperty.Register(
            "Zoom",
            typeof(double),
            typeof(ZoomableListView), 
            new PropertyMetadata(1.0, OnZoomChanged));

        static void OnZoomChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var zlv = d as ZoomableListView;

            zlv.OnAfterZoomChanged();
        }

        void OnAfterZoomChanged()
        {
            if (AfterZoomChanged != null)
                AfterZoomChanged(this, EventArgs.Empty);
        }

        #endregion

        #region Min Zoom

        public double MinZoom
        {
            get { return (double)GetValue(MinZoomProperty); }
            set { SetValue(MinZoomProperty, value); }
        }

        public static readonly DependencyProperty MinZoomProperty =
            DependencyProperty.Register(
            "MinZoom",
            typeof(double),
            typeof(ZoomableListView),
            new PropertyMetadata(.01));

        #endregion

        #region Max Zoom

        public double MaxZoom
        {
            get { return (double)GetValue(MaxZoomProperty); }
            set { SetValue(MaxZoomProperty, value); }
        }

        public static readonly DependencyProperty MaxZoomProperty =
            DependencyProperty.Register(
            "MaxZoom",
            typeof(double),
            typeof(ZoomableListView),
            new PropertyMetadata(10.00));

        #endregion

        #region Mouse Wheel Zoom Step

        public double MouseWheelZoomStep
        {
            get { return (double)GetValue(MouseWheelZoomStepProperty); }
            set { SetValue(MouseWheelZoomStepProperty, value); }
        }

        public static readonly DependencyProperty MouseWheelZoomStepProperty =
            DependencyProperty.Register(
            "MouseWheelZoomStep",
            typeof(double),
            typeof(ZoomableListView), 
            new PropertyMetadata(0.1));

        #endregion


        #region Content Padding

        public Thickness ContentPadding
        {
            get { return (Thickness)GetValue(ContentPaddingProperty); }
            set { SetValue(ContentPaddingProperty, value); }
        }

        public static readonly DependencyProperty ContentPaddingProperty =
            DependencyProperty.Register(
            "ContentPadding", 
            typeof(Thickness), 
            typeof(ZoomableListView),
            new PropertyMetadata(new Thickness()));

        #endregion


        Viewbox PART_ViewBox;
        ScrollViewer PART_ScrollViewer;
        ItemsPresenter ItemsPresenter;
        Panel ItemsPanel;

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            PART_ScrollViewer = this.FindVisualDescendant<ScrollViewer>(descendantName: "PART_ScrollViewer");
            
            // search in content of content controls (view box is most likely inside scroll viewer)
            PART_ViewBox = this.FindVisualDescendant<Viewbox>(descendantName: "PART_ViewBox");

            ItemsPresenter = this.FindVisualDescendant<ItemsPresenter>();

            ItemsPresenter.ApplyTemplate();

            ItemsPanel = ItemsPresenter.FindVisualDescendant<Panel>();
        }

        protected override void OnPreviewMouseWheel(MouseWheelEventArgs e)
        {
            // ctrl -> zoom in/out
            if(Keyboard.Modifiers.HasFlag(ModifierKeys.Control))
            {
                // zoom should try to position control so that point under mouse pointer does not shift
                // point under mouse is an actual offset from top/left corner of items presenter
                var pointUnderMouse = e.GetPosition(ItemsPresenter);

                var oldZoom = Zoom;

                var newZoom = Zoom + (MouseWheelZoomStep * Math.Sign(e.Delta));

                // make sure new zoom is within specified bounds
                newZoom = Math.Min(newZoom, MaxZoom);
                newZoom = Math.Max(newZoom, MinZoom);

                Zoom = newZoom;

                var delta_x = (pointUnderMouse.X / oldZoom) * newZoom - pointUnderMouse.X;
                var delta_y = (pointUnderMouse.Y / oldZoom) * newZoom - pointUnderMouse.Y;

                // calculate new content padding (to guarantee that zooming in/out will preserve width/height ratio)
                var horizontalPadding =
                    Math.Max(0, (ItemsPanel.ActualWidth - (ItemsPanel.ActualWidth * Zoom)) / 2);
                var verticalPadding =
                    Math.Max(0, (ItemsPanel.ActualHeight - (ItemsPanel.ActualHeight * Zoom)) / 2);

                var oldContentPadding = ContentPadding;
                ContentPadding = new Thickness(horizontalPadding, verticalPadding, horizontalPadding, verticalPadding);

                var horizontalPadding_Delta = ContentPadding.Left - oldContentPadding.Left;
                var verticalPadding_Delta = ContentPadding.Top - oldContentPadding.Top;

                // padding deltas will need to be added to horizontal / vertical offsets to preserve zoom about the point


                PART_ScrollViewer.ScrollToHorizontalOffset(PART_ScrollViewer.ContentHorizontalOffset + delta_x + horizontalPadding_Delta);
                PART_ScrollViewer.ScrollToVerticalOffset(PART_ScrollViewer.ContentVerticalOffset + delta_y + verticalPadding_Delta);

                e.Handled = true;
                return;
            }

            // shift -> shift view right/left
            if (Keyboard.Modifiers.HasFlag(ModifierKeys.Shift))
            {
                var delta = (e.Delta * -1); // -1 because wheel-up scrolls left, wheel-down scrolls down

                PART_ScrollViewer.ScrollToHorizontalOffset(PART_ScrollViewer.ContentHorizontalOffset + delta);
                
                e.Handled = true;
                return;
            }

            // nothing -> shift view up/down
            base.OnMouseWheel(e);
        }
    }
}
