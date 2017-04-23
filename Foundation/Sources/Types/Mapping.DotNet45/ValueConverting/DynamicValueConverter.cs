using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SquaredInfinity.Types.Mapping.ValueConverting
{
    public class DynamicValueConverter<TFrom, TTo> : ValueConverter<TFrom, TTo>
    {
        Func<TFrom, TTo> Convert_Internal { get; set; }

        public DynamicValueConverter(Func<TFrom, TTo> convert)
        {
            this.Convert_Internal = convert;
        }

        public override TTo Convert(TFrom from)
        {
            return Convert_Internal(from);
        }
    }
}
