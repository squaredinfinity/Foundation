using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SquaredInfinity.Foundation
{
    public static class IServiceProviderExtensions
    {
        public static TServiceOut GetService<TServiceIn, TServiceOut>(this IServiceProvider serviceProvider)
        {
            return (TServiceOut)serviceProvider.GetService(typeof(TServiceIn));
        }

        public static TService GetService<TService>(this IServiceProvider serviceProvider)
        {
            return (TService)serviceProvider.GetService(typeof(TService));
        }
    }
}
