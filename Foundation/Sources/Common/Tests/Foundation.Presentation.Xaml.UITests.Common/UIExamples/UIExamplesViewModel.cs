using SquaredInfinity.Foundation.Presentation.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Foundation.Presentation.Xaml.UITests.UIExamples
{
    public class UIExamplesViewModel : ViewModel
    {
        FontWeight? _selectedFontWeight = null;
        public FontWeight? SelectedFontWeight
        {
            get { return _selectedFontWeight; }
            set { TrySetThisPropertyValue(ref _selectedFontWeight, value); }
        }

        List<FontWeight> _availableFontWeights;
        public List<FontWeight> AvailableFontWeights
        {
            get { return _availableFontWeights; }
        }

        public UIExamplesViewModel()
        {
            _availableFontWeights = new List<FontWeight>()
            {
                FontWeights.Normal,
                FontWeights.Bold
            };
        }

    }
}
