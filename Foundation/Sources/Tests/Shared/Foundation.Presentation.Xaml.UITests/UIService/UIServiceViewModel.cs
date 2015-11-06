using SquaredInfinity.Foundation.Presentation.ViewModels;
using SquaredInfinity.Foundation.Presentation.Xaml.Styles.Modern.Windows;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;

namespace SquaredInfinity.Foundation.Presentation.Xaml.UITests.UIService
{
    public class UIServiceViewModel : ViewModel
    {
        string _lastInteractionOutcome = "[NONE]";
        public string LastInteractionOutcome
        {
            get { return _lastInteractionOutcome; }
            set { TrySetThisPropertyValue(ref _lastInteractionOutcome, value); }
        }

        public UIServiceViewModel()
            : base(new DefaultUIService(Application.Current.Dispatcher, () => new DefaultDialogWindow(), () => new DefaultDialogWindow()))
        { }

        public void OpenDialogWindow()
        {
            var view = new _internal.GenericView();
            view.ViewModel.Title = "Dialog Window Title";

            UIService.ShowDialog(view);

            LastInteractionOutcome = view.ViewModel.InteractionOutcome.ToString();            
        }
    }
}
