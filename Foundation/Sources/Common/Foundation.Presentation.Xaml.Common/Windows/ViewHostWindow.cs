using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace SquaredInfinity.Foundation.Presentation.Windows
{
    public partial class ViewHostWindow : Window
    {
        ContentPresenter PART_ContentPresenter;

        #region Hosted View

        public FrameworkElement HostedView
        {
            get { return (FrameworkElement)GetValue(HostedViewProperty); }
            set { SetValue(HostedViewProperty, value); }
        }

        public static readonly DependencyProperty HostedViewProperty =
            DependencyProperty.Register(
            "HostedView", 
            typeof(FrameworkElement), 
            typeof(ViewHostWindow), 
            new PropertyMetadata(null));
       
        #endregion

        public ViewHostWindow()
        {
            SetValue(Window.MaxHeightProperty, System.Windows.SystemParameters.PrimaryScreenHeight * .75);
            SetValue(Window.MaxWidthProperty, System.Windows.SystemParameters.PrimaryScreenHeight * .75);

            this.LayoutUpdated += DefaultDialogWindow_LayoutUpdated;
        }

        void DefaultDialogWindow_LayoutUpdated(object sender, EventArgs e)
        {
            if (PART_ContentPresenter == null || PART_ContentPresenter.Content == null)
                return;

            var fe = PART_ContentPresenter.Content as FrameworkElement;

            if (fe != null)
            {
                fe.ClearValue(FrameworkElement.WidthProperty);
                fe.ClearValue(FrameworkElement.HeightProperty);
            }

            SizeToContent = SizeToContent.Manual;

            ClearValue(FrameworkElement.HeightProperty);
            ClearValue(FrameworkElement.WidthProperty);
            ClearValue(FrameworkElement.MaxHeightProperty);
            ClearValue(FrameworkElement.MaxWidthProperty);

            this.LayoutUpdated -= DefaultDialogWindow_LayoutUpdated;
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            PART_ContentPresenter = FindName("PART_content_presenter") as ContentPresenter;
        }
    }
}
