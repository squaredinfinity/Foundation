﻿using SquaredInfinity.Foundation.Presentation.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Foundation.Presentation.Xaml.UITests.MarkupExtensions
{
    public class MarkupExtensionsViewModel : ViewModel
    {
        int _numberOne = 1;
        public int NumberOne
        {
            get { return _numberOne; }
            set { TrySetThisPropertyValue(ref _numberOne, value); }
        }

        int _numberTwo = 1;
        public int NumberTwo
        {
            get { return _numberTwo; }
            set { TrySetThisPropertyValue(ref _numberTwo, value); }
        }

        public string Sum()
        {
            return (NumberOne + NumberTwo).ToString();
        }
    }
}
