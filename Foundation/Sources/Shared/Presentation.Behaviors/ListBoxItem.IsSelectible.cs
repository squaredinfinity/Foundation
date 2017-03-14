using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;

namespace SquaredInfinity.Presentation.Behaviors
{
    public class ListItem
    {
        #region IsSelectible

        public static readonly DependencyProperty IsSelectibleProperty = 
            DependencyProperty.RegisterAttached(
            "IsSelectible", 
            typeof(bool),
            typeof(ListItem), 
            new PropertyMetadata(true));

        public static void SetIsSelectible(ListBoxItem element, bool value)
        {
            element.SetValue(IsSelectibleProperty, value);
        }

        public static bool GetIsSelectible(ListBoxItem element)
        {
            return (bool)element.GetValue(IsSelectibleProperty);
        }

        #endregion

        #region GivesHoverFeedback

        public static readonly DependencyProperty GivesHoverFeedbackProperty = 
            DependencyProperty.RegisterAttached(
            "GivesHoverFeedback",
            typeof(bool),
            typeof(ListItem), 
            new PropertyMetadata(true));

        public static void SetGivesHoverFeedback(ListBoxItem element, bool value)
        {
            element.SetValue(GivesHoverFeedbackProperty, value);
        }

        public static bool GetGivesHoverFeedback(ListBoxItem element)
        {
            return (bool)element.GetValue(GivesHoverFeedbackProperty);
        }

        #endregion
    }
}
