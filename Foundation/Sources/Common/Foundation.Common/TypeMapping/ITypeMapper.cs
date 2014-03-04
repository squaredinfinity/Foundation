using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SquaredInfinity.Foundation.TypeMapping
{
    public interface ITypeMapper
    {
        void Map<TFrom, TTo>(TFrom from, TTo to);
    }
}
