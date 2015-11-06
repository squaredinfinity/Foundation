using SquaredInfinity.Foundation.Presentation.Behaviors;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace SquaredInfinity.Foundation.Presentation.Windows
{
    public class DefaultDialogWindow : ModernWindow
    {
        ContentPresenter PART_ContentPresenter;

        public DefaultDialogWindow()
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
                var minWidth = DialogHost.GetMinWidth(fe);
                var minHeight = DialogHost.GetMinHeight(fe);

                if (minHeight != null)
                {
                    MinHeight = minHeight.Value;
                }

                if (minWidth != null)
                {
                    MinWidth = minWidth.Value;
                }
            }

            SizeToContent = SizeToContent.Manual;

            this.LayoutUpdated -= DefaultDialogWindow_LayoutUpdated;
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            PART_ContentPresenter = Template.FindName("PART_content_presenter", this) as ContentPresenter;
        }
    }
}
