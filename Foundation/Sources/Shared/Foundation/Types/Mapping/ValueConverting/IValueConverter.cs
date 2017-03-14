using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SquaredInfinity.Types.Mapping.ValueConverting
{
    public interface IValueConverter
    {
        Type From { get; }
        Type To { get; }

        object Convert(object source);
    }

    public interface IValueConverter<TFrom, TTo> : IValueConverter
    {
        TTo Convert(TFrom source);
    }
}
